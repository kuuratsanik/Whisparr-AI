using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;

namespace MatchdBaseline;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static int Main(string[] args)
    {
        var corpusPath = args.ElementAtOrDefault(0)
            ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "corpus", "seed.jsonl"));
        var outPath = args.ElementAtOrDefault(1)
            ?? Path.GetFullPath(Path.Combine(Path.GetDirectoryName(corpusPath)!, "..", "results", "baseline.json"));

        if (!File.Exists(corpusPath))
        {
            Console.Error.WriteLine($"corpus not found: {corpusPath}");
            return 2;
        }

        var rows = File.ReadLines(corpusPath)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => JsonSerializer.Deserialize<CorpusRow>(l, JsonOpts)!)
            .ToList();

        var cases = new List<CaseResult>();
        foreach (var row in rows)
        {
            cases.Add(Score(row));
        }

        var summary = BuildSummary(cases);
        var report = new BaselineReport
        {
            GeneratedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
            ParserApi = "NzbDrone.Core.Parser.Parser.ParseMovieTitle (eros)",
            CorpusPath = corpusPath,
            CorpusSha256 = Sha256File(corpusPath),
            CorpusRows = rows.Count,
            Summary = summary,
            Cases = cases
        };

        Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
        File.WriteAllText(outPath, JsonSerializer.Serialize(report, JsonOpts) + "\n");

        Console.WriteLine($"wrote {outPath}");
        Console.WriteLine(
            $"scene kind P={summary.SceneKindPrecision:F3} R={summary.SceneKindRecall:F3} | " +
            $"studioAcc={summary.StudioAccuracy:F3} dateAcc={summary.DateAccuracy:F3} titleAcc={summary.TitleAccuracy:F3} | " +
            $"rejectOk={summary.RejectOkRate:F3} prefixStrip={summary.PrefixStripAccuracy:F3}");

        return 0;
    }

    private static CaseResult Score(CorpusRow row)
    {
        ParsedMovieInfo? parsed = null;
        string? error = null;
        try
        {
            parsed = Parser.ParseMovieTitle(row.ReleaseTitle);
        }
        catch (Exception ex)
        {
            error = ex.GetType().Name + ": " + ex.Message;
        }

        var expect = row.Expect ?? new Expect();
        var kind = expect.Kind ?? "reject";
        var predictedKind = PredictKind(parsed);
        var field = new FieldScores();

        field.Kind = predictedKind == kind;

        if (kind == "scene")
        {
            field.Studio = ExpectBlank(expect.Studio)
                ? true
                : Eq(parsed?.StudioTitle, expect.Studio);
            field.Date = ExpectBlank(expect.Date)
                ? true
                : Eq(parsed?.ReleaseDate, expect.Date);
            field.PrimaryTitle = ExpectBlank(expect.PrimaryTitle)
                ? true
                : TitleOverlapOk(TitleHaystack(parsed), expect.PrimaryTitle!, 0.6);
            field.Performers = ExpectBlankList(expect.Performers)
                ? true
                : PerformersOk(parsed, expect.Performers!);
        }
        else if (kind == "movie")
        {
            field.Studio = true;
            field.Date = true;
            field.PrimaryTitle = ExpectBlank(expect.PrimaryTitle)
                ? true
                : Eq(parsed?.PrimaryMovieTitle, expect.PrimaryTitle)
                  || TitleOverlapOk(TitleHaystack(parsed), expect.PrimaryTitle!, 0.6);
            field.Performers = true;
        }
        else // reject
        {
            // Pass when parser returns null or a non-scene parse (no studio/date invention).
            field.Kind = parsed == null || !parsed.IsScene;
            field.Studio = parsed == null || string.IsNullOrWhiteSpace(parsed.StudioTitle) || !parsed.IsScene;
            field.Date = parsed == null || string.IsNullOrWhiteSpace(parsed.ReleaseDate) || !parsed.IsScene;
            field.PrimaryTitle = true;
            field.Performers = true;
        }

        var prefixCase = IsCategoryPrefixed(row.ReleaseTitle);
        var prefixOk = !prefixCase || field.Kind && (
            kind != "movie" || field.PrimaryTitle);

        var pass = field.Kind && field.Studio && field.Date && field.PrimaryTitle && field.Performers;

        return new CaseResult
        {
            Id = row.Id,
            ReleaseTitle = row.ReleaseTitle,
            ExpectKind = kind,
            PredictedKind = predictedKind,
            Pass = pass,
            CategoryPrefixed = prefixCase,
            PrefixStripOk = prefixOk,
            Fields = field,
            Parsed = parsed == null
                ? null
                : new ParsedSnapshot
                {
                    IsScene = parsed.IsScene,
                    StudioTitle = parsed.StudioTitle,
                    ReleaseDate = parsed.ReleaseDate,
                    FirstPerformer = parsed.FirstPerformer,
                    PrimaryMovieTitle = parsed.PrimaryMovieTitle,
                    ReleaseTokens = parsed.ReleaseTokens,
                    Code = parsed.Code,
                    Year = parsed.Year
                },
            Error = error
        };
    }

    private static string PredictKind(ParsedMovieInfo? parsed)
    {
        if (parsed == null)
        {
            return "reject";
        }

        if (parsed.IsScene)
        {
            return "scene";
        }

        if (!string.IsNullOrWhiteSpace(parsed.PrimaryMovieTitle) || parsed.Year > 0)
        {
            return "movie";
        }

        return "reject";
    }

    private static BaselineSummary BuildSummary(List<CaseResult> cases)
    {
        var sceneExpect = cases.Where(c => c.ExpectKind == "scene").ToList();
        var scenePred = cases.Where(c => c.PredictedKind == "scene").ToList();
        var tp = cases.Count(c => c.ExpectKind == "scene" && c.PredictedKind == "scene");
        var precision = scenePred.Count == 0 ? 0.0 : (double)tp / scenePred.Count;
        var recall = sceneExpect.Count == 0 ? 0.0 : (double)tp / sceneExpect.Count;

        var studioAcc = Mean(sceneExpect.Select(c => c.Fields.Studio));
        var dateAcc = Mean(sceneExpect.Select(c => c.Fields.Date));
        var titleAcc = Mean(cases.Where(c => c.ExpectKind is "scene" or "movie").Select(c => c.Fields.PrimaryTitle));
        var reject = cases.Where(c => c.ExpectKind == "reject").ToList();
        var rejectOk = Mean(reject.Select(c => c.Fields.Kind));
        var prefix = cases.Where(c => c.CategoryPrefixed).ToList();
        var prefixAcc = Mean(prefix.Select(c => c.PrefixStripOk));

        return new BaselineSummary
        {
            Passed = cases.Count(c => c.Pass),
            Failed = cases.Count(c => !c.Pass),
            SceneKindPrecision = precision,
            SceneKindRecall = recall,
            StudioAccuracy = studioAcc,
            DateAccuracy = dateAcc,
            TitleAccuracy = titleAcc,
            RejectOkRate = rejectOk,
            PrefixStripAccuracy = prefixAcc,
            FailedIds = cases.Where(c => !c.Pass).Select(c => c.Id).ToList()
        };
    }

    private static double Mean(IEnumerable<bool> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 1.0 : list.Count(v => v) / (double)list.Count;
    }

    private static string TitleHaystack(ParsedMovieInfo? parsed)
    {
        if (parsed == null)
        {
            return string.Empty;
        }

        var tokens = parsed.ReleaseTokens ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(tokens))
        {
            tokens = Parser.NormalizeEpisodeTitle(tokens);
        }

        return string.Join(' ', new[]
        {
            parsed.PrimaryMovieTitle,
            parsed.FirstPerformer,
            tokens
        }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private static bool TitleOverlapOk(string haystack, string expected, double min)
    {
        var a = Tokens(haystack);
        var b = Tokens(expected);
        if (b.Count == 0)
        {
            return true;
        }

        if (a.Count == 0)
        {
            return false;
        }

        var hit = b.Count(t => a.Contains(t));
        return hit / (double)b.Count >= min;
    }

    private static bool PerformersOk(ParsedMovieInfo? parsed, List<string> performers)
    {
        var hay = TitleHaystack(parsed).ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(parsed?.FirstPerformer))
        {
            hay += " " + parsed!.FirstPerformer.ToLowerInvariant();
        }

        // All performer tokens must appear somewhere in haystack (lenient baseline).
        return performers.All(p => Tokens(p).All(t => hay.Contains(t, StringComparison.Ordinal)));
    }

    private static HashSet<string> Tokens(string text)
    {
        return text
            .ToLowerInvariant()
            .Split(new[] { ' ', '.', '_', '-', ':', '[', ']', '(', ')', '/' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 1)
            .Where(t => t is not ("mp4" or "mkv" or "avi" or "1080p" or "720p" or "2160p" or "web" or "dl" or "bluray" or "x264" or "x265" or "h264" or "aac" or "repack"))
            .ToHashSet();
    }

    private static bool Eq(string? a, string? b) =>
        string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool ExpectBlank(string? s) => string.IsNullOrWhiteSpace(s);
    private static bool ExpectBlankList(List<string>? list) => list == null || list.Count == 0;

    private static bool IsCategoryPrefixed(string title) =>
        System.Text.RegularExpressions.Regex.IsMatch(
            title,
            @"^(?:GAY|XXX|PORN|ADULT|STRAIGHT|TRANS|LESBIAN|BISEXUAL|HENTAI)\s*:\s*",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private static string Sha256File(string path)
    {
        using var stream = File.OpenRead(path);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class CorpusRow
    {
        public string Id { get; set; } = "";
        public string ReleaseTitle { get; set; } = "";
        public Expect? Expect { get; set; }
        public string? Notes { get; set; }
        public string? Source { get; set; }
    }

    private sealed class Expect
    {
        public string? Kind { get; set; }
        public string? Studio { get; set; }
        public string? Date { get; set; }
        public string? PrimaryTitle { get; set; }
        public List<string>? Performers { get; set; }
    }

    private sealed class BaselineReport
    {
        public string GeneratedAtUtc { get; set; } = "";
        public string ParserApi { get; set; } = "";
        public string CorpusPath { get; set; } = "";
        public string CorpusSha256 { get; set; } = "";
        public int CorpusRows { get; set; }
        public BaselineSummary Summary { get; set; } = new();
        public List<CaseResult> Cases { get; set; } = new();
    }

    private sealed class BaselineSummary
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public double SceneKindPrecision { get; set; }
        public double SceneKindRecall { get; set; }
        public double StudioAccuracy { get; set; }
        public double DateAccuracy { get; set; }
        public double TitleAccuracy { get; set; }
        public double RejectOkRate { get; set; }
        public double PrefixStripAccuracy { get; set; }
        public List<string> FailedIds { get; set; } = new();
    }

    private sealed class CaseResult
    {
        public string Id { get; set; } = "";
        public string ReleaseTitle { get; set; } = "";
        public string ExpectKind { get; set; } = "";
        public string PredictedKind { get; set; } = "";
        public bool Pass { get; set; }
        public bool CategoryPrefixed { get; set; }
        public bool PrefixStripOk { get; set; }
        public FieldScores Fields { get; set; } = new();
        public ParsedSnapshot? Parsed { get; set; }
        public string? Error { get; set; }
    }

    private sealed class FieldScores
    {
        public bool Kind { get; set; }
        public bool Studio { get; set; }
        public bool Date { get; set; }
        public bool PrimaryTitle { get; set; }
        public bool Performers { get; set; }
    }

    private sealed class ParsedSnapshot
    {
        public bool IsScene { get; set; }
        public string? StudioTitle { get; set; }
        public string? ReleaseDate { get; set; }
        public string? FirstPerformer { get; set; }
        public string? PrimaryMovieTitle { get; set; }
        public string? ReleaseTokens { get; set; }
        public string? Code { get; set; }
        public int Year { get; set; }
    }
}
