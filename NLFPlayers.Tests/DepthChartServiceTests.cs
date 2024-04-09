using NFLPlayers.Interfaces;
using NFLPlayers.Models;
using NFLPlayers.Services;

namespace NLFPlayers.Tests
{
    [TestFixture]
    public class DepthChartServiceTests
    {
        private IDepthChartService _service;

        [SetUp]
        public void Setup()
        {
            _service = new DepthChartService();
        }
        
        [Test]
        public void AddPlayerToDepthChart_AddsPlayerToEndIfNoPositionDepthGiven()
        {
            var player = new Player { Number = 1, Name = "New Player", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player, null); // No depth given, should add to end

            var result = _service.GetFullDepthChart(1, 101)["QB"];
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Last(), Is.EqualTo(player)); // Player should be last
        }

        [Test]
        public void AddPlayerToDepthChart_AddsPlayerCorrectly()
        {
            var player = new Player { Number = 1, Name = "New Player", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player, 0); // Add to specific position

            var result = _service.GetFullDepthChart(1, 101)["QB"];
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First(), Is.EqualTo(player)); // Player should be at the specified depth
        }

        [Test]
        public void AddPlayerToDepthChart_AddsSamePlayerToDifferentPosition()
        {
            var player = new Player { Number = 1, Name = "New Player", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player, 0);
            _service.AddPlayerToDepthChart(1, 101, "WR", player, 0); // Add same player to different position

            var qbResult = _service.GetFullDepthChart(1, 101)["QB"];
            var wrResult = _service.GetFullDepthChart(1, 101)["WR"];
            Assert.That(qbResult.First(), Is.EqualTo(player));
            Assert.That(wrResult.First(), Is.EqualTo(player)); // Should be present in both positions
        }

        [Test]
        public void AddPlayerToDepthChart_ThrowsWhenDuplicateNumber()
        {
            var player1 = new Player { Number = 12, Name = "Tom Brady", SportId = 1, TeamId = 101 };
            var player2 = new Player { Number = 12, Name = "Tom Brady Clone", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player1, 0);

            var ex = Assert.Throws<InvalidOperationException>(() => _service.AddPlayerToDepthChart(1, 101, "QB", player2, 1));
            Assert.That(ex.Message, Is.EqualTo("A player with number 12 already exists in the QB position."));
        }

        [Test]
        public void AddPlayerToDepthChart_ThrowsIfPositionDepthOutOfBounds()
        {
            var player = new Player { Number = 18, Name = "New Player", SportId = 1, TeamId = 101 };
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.AddPlayerToDepthChart(1, 101, "QB", player, 5)); // Assuming no players are at this depth yet
        }

        [Test]
        public void AddPlayerToDepthChart_InsertsPlayerInMiddle()
        {
            var player1 = new Player { Number = 10, Name = "Starter QB", SportId = 1, TeamId = 101 };
            var player2 = new Player { Number = 11, Name = "Backup QB", SportId = 1, TeamId = 101 };
            var player3 = new Player { Number = 12, Name = "New QB", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player1, 0);
            _service.AddPlayerToDepthChart(1, 101, "QB", player2, 1);
            _service.AddPlayerToDepthChart(1, 101, "QB", player3, 1); // Inserting in the middle

            var result = _service.GetFullDepthChart(1, 101)["QB"];
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[1], Is.EqualTo(player3));
            Assert.That(result[2], Is.EqualTo(player2));
        }

        // --------------------------Tests for RemovePlayerFromDepthChart--------------------------
        [Test]
        public void RemovePlayerFromDepthChart_RemovesPlayerSuccessfully()
        {
            var player = new Player { Number = 1, Name = "New Player", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player, 0);
            var result = _service.RemovePlayerFromDepthChart(1, 101, "QB", player);

            Assert.That(result, Is.Not.Null);
            Assert.That(_service.GetFullDepthChart(1, 101)["QB"], Is.Empty);
        }

        [Test]
        public void RemovePlayerFromDepthChart_ReturnsNullIfPlayerNotFound()
        {
            var player = new Player { Number = 1, Name = "New Player", SportId = 1, TeamId = 101 };
            var result = _service.RemovePlayerFromDepthChart(1, 101, "QB", player);

            Assert.That(result, Is.Null);
        }

        // --------------------------Tests for GetBackups--------------------------
        [Test]
        public void GetBackups_ReturnsCorrectBackups()
        {
            var player1 = new Player { Number = 1, Name = "Starting QB", SportId = 1, TeamId = 101 };
            var player2 = new Player { Number = 2, Name = "Backup QB", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player1, 0);
            _service.AddPlayerToDepthChart(1, 101, "QB", player2, 1);

            var backups = _service.GetBackups(1, 101, "QB", player1);
            Assert.That(backups.Count, Is.EqualTo(1));
            Assert.That(backups.First(), Is.EqualTo(player2));
        }

        [Test]
        public void GetBackups_ReturnsEmptyIfNoBackups()
        {
            var player = new Player { Number = 1, Name = "Only QB", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player, 0);

            var backups = _service.GetBackups(1, 101, "QB", player);
            Assert.That(backups, Is.Empty);
        }

        [Test]
        public void GetFullDepthChart_ReturnsCompleteDepthChart()
        {
            var player1 = new Player { Number = 1, Name = "QB1", SportId = 1, TeamId = 101 };
            var player2 = new Player { Number = 2, Name = "RB1", SportId = 1, TeamId = 101 };
            _service.AddPlayerToDepthChart(1, 101, "QB", player1, 0);
            _service.AddPlayerToDepthChart(1, 101, "RB", player2, 0);

            var result = _service.GetFullDepthChart(1, 101);
            Assert.That(result.Keys.Count, Is.EqualTo(2)); // QB and RB
            Assert.That(result["QB"].First(), Is.EqualTo(player1));
            Assert.That(result["RB"].First(), Is.EqualTo(player2));
        }

        //// --------------------------Tests for GetFullDepthChart--------------------------

        [Test]
        public void Full_test()
        {
            int nflId = 1, tigersId = 101;
            var TomBrady = new Player(12, "Tom Brady", nflId, tigersId);
            var BlaineGabbert = new Player(11, "Blaine Gabbert", nflId, tigersId);
            var KyleTrask = new Player(2, "Kyle Trask", nflId, tigersId);
            var MikeEvans = new Player(13, "Mike Evans", nflId, tigersId);
            var JaelonDarden = new Player(1, "Jaelon Darden", nflId, tigersId);
            var ScottMiller = new Player(10, "Scott Miller", nflId, tigersId);

            // Setup initial depth chart
            _service.AddPlayerToDepthChart(nflId, tigersId, "QB", TomBrady, 0);
            _service.AddPlayerToDepthChart(nflId, tigersId, "QB", BlaineGabbert, 1);
            _service.AddPlayerToDepthChart(nflId, tigersId, "QB", KyleTrask, 2);
            _service.AddPlayerToDepthChart(nflId, tigersId, "LWR", MikeEvans, 0);
            _service.AddPlayerToDepthChart(nflId, tigersId, "LWR", JaelonDarden, 1);
            _service.AddPlayerToDepthChart(nflId, tigersId, "LWR", ScottMiller, 2);

            var backups = _service.GetBackups(nflId, tigersId, "QB", TomBrady);
            /* Output */
            // #11 � Blaine Gabbert
            // #2 � Kyle Trask
            //Assert.That(depthChart, Is.Empty);
            Assert.That(_service.GetBackups(nflId, tigersId, "QB", TomBrady).Count, Is.EqualTo(2));
            // assert the backups are in the correct order and contain the correct players
            Assert.That(backups[0], Is.EqualTo(BlaineGabbert));
            Assert.That(backups[1], Is.EqualTo(KyleTrask));


            backups = _service.GetBackups(nflId, tigersId, "QB", JaelonDarden);
            /* Output */
            // Empty
            Assert.That(backups, Is.Empty);
            Assert.That(backups.Count, Is.EqualTo(0));

            backups = _service.GetBackups(nflId, tigersId, "QB", MikeEvans);
            /* Output */
            // <NO LIST>
            Assert.That(backups, Is.Empty);

            backups = _service.GetBackups(nflId, tigersId, "QB", BlaineGabbert);
            /* Output */
            // #2 - Kyle Trask
            Assert.That(backups.Count, Is.EqualTo(1));
            Assert.That(backups[0], Is.EqualTo(KyleTrask));

            backups = _service.GetBackups(nflId, tigersId, "QB", KyleTrask);
            /* Output */
            // <NO LIST>
            Assert.That(backups, Is.Empty);

            var depthChartTemp = _service.GetFullDepthChart(nflId, tigersId);
            /* Output */
            // QB " (#12, Tom Brady), (#11, Blaine Gabbert), (#2, Kyle Trask)
            // LWR " (#13, Mike Evans), (#1, Jaelon Darden), (#10, Scott Miller)
            // Assert the depth chart is in the correct order and contains the correct players
            Assert.That(depthChartTemp["QB"][0], Is.EqualTo(TomBrady));
            Assert.That(depthChartTemp["QB"][1], Is.EqualTo(BlaineGabbert));
            Assert.That(depthChartTemp["QB"][2], Is.EqualTo(KyleTrask));
            Assert.That(depthChartTemp["LWR"][0], Is.EqualTo(MikeEvans));
            Assert.That(depthChartTemp["LWR"][1], Is.EqualTo(JaelonDarden));
            Assert.That(depthChartTemp["LWR"][2], Is.EqualTo(ScottMiller));

            var removedPlayer = _service.RemovePlayerFromDepthChart(nflId, tigersId, "WR", MikeEvans);
            /* Output */
            Assert.That(removedPlayer, Is.EqualTo(null));

            removedPlayer = _service.RemovePlayerFromDepthChart(nflId, tigersId, "LWR", MikeEvans);
            Assert.That(removedPlayer, Is.EqualTo(MikeEvans));
            Assert.That(_service.GetFullDepthChart(nflId, tigersId)["LWR"].Contains(MikeEvans), Is.False);

            depthChartTemp = _service.GetFullDepthChart(nflId, tigersId);
            /* Output */
            // QB (#12, Tom Brady), (#11, Blaine Gabbert), (#2, Kyle Trask)
            // LWR - (#1, Jaelon Darden), (#10, Scott Miller)
            Assert.That(depthChartTemp["QB"][0], Is.EqualTo(TomBrady));
            Assert.That(depthChartTemp["QB"][1], Is.EqualTo(BlaineGabbert));
            Assert.That(depthChartTemp["QB"][2], Is.EqualTo(KyleTrask));
            Assert.That(depthChartTemp["LWR"][0], Is.EqualTo(JaelonDarden));
            Assert.That(depthChartTemp["LWR"][1], Is.EqualTo(ScottMiller));
        }

    }
}