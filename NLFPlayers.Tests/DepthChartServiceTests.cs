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
            // Arrange
            var player = new Player { Number = 1, Name = "Test Player" };

            // Act
            _service.AddPlayerToDepthChart("QB", player, null);

            // Assert
            var depthChart = _service.GetFullDepthChart();
            Assert.That(depthChart["QB"].Contains(player), Is.True, "Player should be in QB position.");

            // Also check whether this player is the last one in the list
            Assert.That(depthChart["QB"].Last(), Is.EqualTo(player), "Player should be the last one in the list.");
        }

        [Test]
        public void AddPlayerToDepthChart_AddsPlayerCorrectly()
        {
            var player = new Player { Number = 1, Name = "Test Player" };
            _service.AddPlayerToDepthChart("QB", player, 0);

            var depthChart = _service.GetFullDepthChart();
            Assert.That(depthChart["QB"].Contains(player), Is.True);
        }

        [Test]
        public void AddPlayerToDepthChart_AddsSamePlayerToDifferentPosition()
        {
            var player = new Player { Number = 1, Name = "Test Player" };
            _service.AddPlayerToDepthChart("QB", player, 0);
            _service.AddPlayerToDepthChart("RB", player, 0);

            var depthChart = _service.GetFullDepthChart();
            Assert.That(depthChart["QB"].Contains(player), Is.True);
            Assert.That(depthChart["RB"].Contains(player), Is.True);
        }

        [Test]
        public void AddPlayerToDepthChart_ThrowsWhenDuplicateNumber()
        {
            var position = "QB";
            var player1 = new Player { Number = 1, Name = "Test Player" };
            _service.AddPlayerToDepthChart(position, player1, 0);
            var player2 = new Player { Number = 1, Name = "Another Player" };

            var ex = Assert.Throws<InvalidOperationException>(() => _service.AddPlayerToDepthChart(position, player2, 0));
            Assert.That(ex.Message, Is.EqualTo($"A player with number 1 already exists in the {position} position."));
        }

        [Test]
        public void AddPlayerToDepthChart_ThrowsIfPositionDepthOutOfBounds()
        {
            var player = new Player { Number = 3, Name = "Out Player" };
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.AddPlayerToDepthChart("QB", player, -1));
        }

        [Test]
        public void AddPlayerToDepthChart_InsertsPlayerInMiddle()
        {
            var player1 = new Player { Number = 1, Name = "Player 1" };
            var player2 = new Player { Number = 2, Name = "Player 2" };
            var player3 = new Player { Number = 3, Name = "Player 3" };
            _service.AddPlayerToDepthChart("QB", player1, 0);
            _service.AddPlayerToDepthChart("QB", player2, 1);
            _service.AddPlayerToDepthChart("QB", player3, 1);

            var depthChart = _service.GetFullDepthChart();
            Assert.That(depthChart["QB"][1], Is.EqualTo(player3));
            Assert.That(depthChart["QB"][2], Is.EqualTo(player2));
        }

        // --------------------------Tests for RemovePlayerFromDepthChart--------------------------
        [Test]
        public void RemovePlayerFromDepthChart_RemovesPlayerSuccessfully()
        {
            var player = new Player { Number = 4, Name = "Remove Player" };
            _service.AddPlayerToDepthChart("QB", player, 0);
            var removedPlayer = _service.RemovePlayerFromDepthChart("QB", player);

            Assert.That(removedPlayer, Is.EqualTo(player));
            Assert.That(_service.GetFullDepthChart()["QB"].Contains(player), Is.False);
        }

        [Test]
        public void RemovePlayerFromDepthChart_ReturnsNullIfPlayerNotFound()
        {
            var player = new Player { Number = 5, Name = "Non-Existent" };
            var result = _service.RemovePlayerFromDepthChart("QB", player);

            Assert.That(result, Is.Null);
        }

        // --------------------------Tests for GetBackups--------------------------
        [Test]
        public void GetBackups_ReturnsCorrectBackups()
        {
            var player1 = new Player { Number = 6, Name = "Starter" };
            var player2 = new Player { Number = 7, Name = "Backup" };
            _service.AddPlayerToDepthChart("QB", player1, 0);
            _service.AddPlayerToDepthChart("QB", player2, 1);

            var backups = _service.GetBackups("QB", player1);
            Assert.That(backups.Contains(player2), Is.True);
            Assert.That(backups.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetBackups_ReturnsEmptyIfNoBackups()
        {
            var player = new Player { Number = 8, Name = "Lone Player" };
            _service.AddPlayerToDepthChart("QB", player, 0);

            var backups = _service.GetBackups("QB", player);
            Assert.That(backups, Is.Empty);
        }

        // --------------------------Tests for GetFullDepthChart--------------------------
        [Test]
        public void GetFullDepthChart_ReturnsCompleteDepthChart()
        {
            var player1 = new Player { Number = 9, Name = "Player One" };
            var player2 = new Player { Number = 10, Name = "Player Two" };
            _service.AddPlayerToDepthChart("QB", player1, 0);
            _service.AddPlayerToDepthChart("RB", player2, 0);

            var depthChart = _service.GetFullDepthChart();
            Assert.That(depthChart.Keys.Count, Is.EqualTo(2));
            Assert.That(depthChart["QB"].First(), Is.EqualTo(player1));
            Assert.That(depthChart["RB"].First(), Is.EqualTo(player2));
        }

        [Test]
        public void Full_test()
        {
            var TomBrady = new Player(12, "Tom Brady");
            var BlaineGabbert = new Player(11, "Blaine Gabbert");
            var KyleTrask = new Player(2, "Kyle Trask");
            var MikeEvans = new Player(13, "Mike Evans");
            var JaelonDarden = new Player(1, "Jaelon Darden");
            var ScottMiller = new Player(10, "Scott Miller");
            _service.AddPlayerToDepthChart("QB", TomBrady, 0);
            _service.AddPlayerToDepthChart("QB", BlaineGabbert, 1);
            _service.AddPlayerToDepthChart("QB", KyleTrask, 2);
            _service.AddPlayerToDepthChart("LWR", MikeEvans, 0);
            _service.AddPlayerToDepthChart("LWR", JaelonDarden, 1);
            _service.AddPlayerToDepthChart("LWR", ScottMiller, 2);
            
            var backups = _service.GetBackups("QB", TomBrady);
            /* Output */
            // #11 � Blaine Gabbert
            // #2 � Kyle Trask
            //Assert.That(depthChart, Is.Empty);
            Assert.That(_service.GetBackups("QB", TomBrady).Count, Is.EqualTo(2));
            // assert the backups are in the correct order and contain the correct players
            Assert.That(backups[0], Is.EqualTo(BlaineGabbert));
            Assert.That(backups[1], Is.EqualTo(KyleTrask));
            

            backups = _service.GetBackups("QB", JaelonDarden);
            /* Output */
            // Empty
            Assert.That(backups, Is.Empty);
            Assert.That(backups.Count, Is.EqualTo(0));

            backups = _service.GetBackups("QB", MikeEvans);
            /* Output */
            // <NO LIST>
            Assert.That(backups, Is.Empty);

            backups = _service.GetBackups("QB", BlaineGabbert);
            /* Output */
            // #2 - Kyle Trask
            Assert.That(backups.Count, Is.EqualTo(1));
            Assert.That(backups[0], Is.EqualTo(KyleTrask));

            backups = _service.GetBackups("QB", KyleTrask);
            /* Output */
            // <NO LIST>
            Assert.That(backups, Is.Empty);

            var depthChartTemp = _service.GetFullDepthChart();
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
            
            var removedPlayer = _service.RemovePlayerFromDepthChart("WR", MikeEvans);
            /* Output */
            // #13 MikeEvans
            Assert.That(removedPlayer, Is.EqualTo(null));

            removedPlayer = _service.RemovePlayerFromDepthChart("LWR", MikeEvans);
            Assert.That(removedPlayer, Is.EqualTo(MikeEvans));
            Assert.That(_service.GetFullDepthChart()["LWR"].Contains(MikeEvans), Is.False);

            depthChartTemp = _service.GetFullDepthChart();
            /* Output */
            // QB � (#12, Tom Brady), (#11, Blaine Gabbert), (#2, Kyle Trask)
            // LWR - (#1, Jaelon Darden), (#10, Scott Miller)
            Assert.That(depthChartTemp["QB"][0], Is.EqualTo(TomBrady));
            Assert.That(depthChartTemp["QB"][1], Is.EqualTo(BlaineGabbert));
            Assert.That(depthChartTemp["QB"][2], Is.EqualTo(KyleTrask));
            Assert.That(depthChartTemp["LWR"][0], Is.EqualTo(JaelonDarden));
            Assert.That(depthChartTemp["LWR"][1], Is.EqualTo(ScottMiller));
        }

    }
}