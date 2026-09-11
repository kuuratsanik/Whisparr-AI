using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.ParserTests
{
    [TestFixture]
    public class SceneMatchingFixture : CoreTest
    {
        // MVP-D fixture pack — Studio.Date.Performer.Title (#1218)
        // and category-prefixed titles (#1257).

        [TestCase("Studio.2025-08-10.Performer.Name.Scene.Title.mp4", "Studio", "2025-08-10")]
        [TestCase("Studio.2025.08.10.Performer.Name.Scene.Title.mp4", "Studio", "2025-08-10")]
        [TestCase("Blacked.2024-01-15.Jane.Doe.Deep.Desires.1080p.mp4", "Blacked", "2024-01-15")]
        [TestCase("Vixen.24.03.22.Performer.One.And.Two.Title.Here.mp4", "Vixen", "2024-03-22")]
        public void should_parse_studio_date_performer_title(string title, string studio, string releaseDate)
        {
            var parsed = Parser.Parser.ParseMovieTitle(title);

            parsed.Should().NotBeNull();
            parsed.IsScene.Should().BeTrue();
            parsed.StudioTitle.Should().Be(studio);
            parsed.ReleaseDate.Should().Be(releaseDate);
            parsed.ReleaseTokens.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase("GAY: To the Last Man: The Gathering Storm (2009.xvid)", "To the Last Man: The Gathering Storm")]
        [TestCase("XXX: Example Movie Title (2020)", "Example Movie Title")]
        [TestCase("ADULT: Another.Sample.Movie.2018.1080p.BluRay.x264", "Another Sample Movie")]
        public void should_strip_category_prefix_from_movie_title(string releaseTitle, string expectedPrimaryTitle)
        {
            var parsed = Parser.Parser.ParseMovieTitle(releaseTitle);

            parsed.Should().NotBeNull();
            parsed.PrimaryMovieTitle.Should().Be(expectedPrimaryTitle);
        }
    }
}
