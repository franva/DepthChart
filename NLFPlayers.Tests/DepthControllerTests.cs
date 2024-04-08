using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NFLPlayers.Controllers;
using NFLPlayers.Interfaces;
using NFLPlayers.Models;
using NSubstitute;

namespace NLFPlayers.Tests
{
    [TestFixture]
    public class DepthChartControllerTests
    {
        private DepthChartController _controller;
        private IDepthChartService _depthChartService;

        [SetUp]
        public void SetUp()
        {
            _depthChartService = Substitute.For<IDepthChartService>();
            _controller = new DepthChartController(_depthChartService);
        }

        [Test]
        public void AddPlayerToDepthChart_ReturnsOk_WhenPlayerIsAdded()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"number\": 12, \"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;
            var player = JsonConvert.DeserializeObject<Player>(playerJson.ToString());

            // Act
            var result = _controller.AddPlayerToDepthChart(playerJson) as OkResult; // Pass JSON element to the method

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            _depthChartService.Received().AddPlayerToDepthChart(Arg.Is("QB"), Arg.Is(player)!, Arg.Any<int?>());
        }


        [Test]
        public void AddPlayerToDepthChart_ReturnsBadRequest_WhenPlayerDataIsIncomplete()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;

            // Act
            var result = _controller.AddPlayerToDepthChart(playerJson) as BadRequestObjectResult;

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddPlayerToDepthChart_ReturnsBadRequest_WhenPlayerNumberIsDuplicated()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"number\": 12, \"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;
            _depthChartService
                .When(x => x.AddPlayerToDepthChart(Arg.Any<string>(), Arg.Is<Player>(p => p.Number == 12), Arg.Any<int?>()))
                .Throw(new InvalidOperationException("Duplicate player number."));

            // Act
            var result = _controller.AddPlayerToDepthChart(playerJson) as BadRequestObjectResult;

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Duplicate player number."));
        }

        // unit test for adding the same player to a different position
        [Test]
        public void AddPlayerToDepthChart_ReturnsOk_WhenPlayerIsAddedToDifferentPosition()
        {
            // Arrange 1
            var tempDict = new Dictionary<string, List<Player>>();

            _depthChartService.When(x => x.AddPlayerToDepthChart(Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>()))
                              .Do(x => 
                              {
                                  var position = x.Arg<string>();
                                  if (!tempDict.ContainsKey(position))
                                  {
                                      tempDict[position] = new List<Player>();
                                  }

                                  tempDict[position].Add(x.Arg<Player>());
                              } );

            _depthChartService.GetFullDepthChart().Returns(tempDict);

            var playerJson = JsonDocument.Parse("{\"number\": 12, \"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;
            var player = JsonConvert.DeserializeObject<Player>(playerJson.ToString());


            // Act 1
            var result = _controller.AddPlayerToDepthChart(playerJson) as OkResult;
            
            // Assert 1
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            _depthChartService.Received().AddPlayerToDepthChart(Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>());
            Assert.That(_depthChartService.GetFullDepthChart()["QB"].Contains(player!));


            // Arrange 2
            playerJson = JsonDocument.Parse("{\"number\": 12, \"name\": \"Tom Brady\", \"position\": \"RB\"}").RootElement;
            player = JsonConvert.DeserializeObject<Player>(playerJson.ToString());


            // Act 2
            result = _controller.AddPlayerToDepthChart(playerJson) as OkResult;
            
            // Assert 2
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            _depthChartService.Received().AddPlayerToDepthChart(Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>());
            // check if the player was added to the new position
            Assert.That(_depthChartService.GetFullDepthChart()["RB"].Contains(player!));
            _depthChartService.Received().AddPlayerToDepthChart(Arg.Is("RB"), Arg.Is(player)!, Arg.Any<int?>());
        }

        // -----------------------RemovePlayerFromDepthChart Tests-----------------------
        [Test]
        public void RemovePlayerFromDepthChart_ReturnsOk_WhenPlayerIsRemovedSuccessfully()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"number\": 12, \"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;
            var playerToRemove = new Player { Number = 12, Name = "Tom Brady" };
            _depthChartService.RemovePlayerFromDepthChart(Arg.Any<string>(), Arg.Any<Player>()).Returns(playerToRemove);

            // Act
            var result = _controller.RemovePlayerFromDepthChart(playerJson) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(playerToRemove));
            _depthChartService.Received().RemovePlayerFromDepthChart(Arg.Any<string>(), Arg.Any<Player>());
        }

        [Test]
        public void RemovePlayerFromDepthChart_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"number\": 99, \"name\": \"Non Existent Player\", \"position\": \"QB\"}").RootElement;
            _depthChartService.RemovePlayerFromDepthChart(Arg.Any<string>(), Arg.Any<Player>()).Returns(null as Player);

            // Act
            var result = _controller.RemovePlayerFromDepthChart(playerJson) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        // Example of a failed test case: Invalid player data (e.g., missing number)
        [Test]
        public void RemovePlayerFromDepthChart_ReturnsBadRequest_WhenPlayerDataIsInvalid()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"name\": \"Tom Brady\", \"position\": \"QB\"}").RootElement;

            // Act
            var result = _controller.RemovePlayerFromDepthChart(playerJson) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        // Example of an edge test case: Removing a player that was never added
        [Test]
        public void RemovePlayerFromDepthChart_ReturnsBadRequest_WhenPlayerWasNeverAdded()
        {
            // Arrange
            var playerJson = JsonDocument.Parse("{\"number\": 100, \"name\": \"Ghost Player\", \"position\": \"QB\"}").RootElement;
            _depthChartService.RemovePlayerFromDepthChart(Arg.Any<string>(), Arg.Is<Player>(p => p.Number == 100)).Returns(null as Player);

            // Act
            var result = _controller.RemovePlayerFromDepthChart(playerJson) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        // ------------------- GetBackups Tests -------------------
        [Test]
        public void GetBackups_ReturnsListOfBackups_WhenValidRequest()
        {
            // Arrange
            var position = "QB";
            var playerNumber = 666;

            var fullDepthChart = PrepareData();
            var backupsList = fullDepthChart.GetRange(1, 2);

            _depthChartService.GetFullDepthChart().Returns(new Dictionary<string, List<Player>> { { position, fullDepthChart } });
            _depthChartService.GetBackups(position, Arg.Any<Player>()).Returns(backupsList);
            // Act
            var result = _controller.GetBackups(position, playerNumber) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(backupsList));
        }

        [Test]
        public void GetBackups_ReturnsEmptyList_WhenPlayerHasNoBackups()
        {
            // Arrange
            var position = "QB";
            var playerNumber = 2; // Assuming this player is last on the depth chart
            var fullDepthChart = PrepareData();
            _depthChartService.GetFullDepthChart().Returns(new Dictionary<string, List<Player>> { { position, fullDepthChart } });
            _depthChartService.GetBackups(position, Arg.Any<Player>()).Returns(new List<Player>());

            // Act
            var result = _controller.GetBackups(position, playerNumber) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public void GetBackups_ReturnsNotFound_WhenPositionIsInvalid()
        {
            // Arrange
            var position = "InvalidPosition";
            var playerNumber = 12;

            // Act
            var result = _controller.GetBackups(position, playerNumber) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void GetBackups_ReturnsNotFound_WhenPlayerNumberIsInvalid()
        {
            // Arrange
            var position = "QB";
            var playerNumber = 99; // Assuming this player number does not exist

            // Act
            var result = _controller.GetBackups(position, playerNumber) as NotFoundResult;

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo(404));
        }

        // --------------------- GetFullDepthChart Tests -------------------
        [Test]
        public void GetFullDepthChart_ReturnsCompleteDepthChart_WhenCalled()
        {
            // Arrange
            var fullDepthChart = new Dictionary<string, List<Player>>
            {
                ["QB"] = new List<Player>
            {
                new Player { Number = 12, Name = "Tom Brady" },
                new Player { Number = 11, Name = "Blaine Gabbert" }
            },
                ["RB"] = new List<Player>
            {
                new Player { Number = 28, Name = "Leonard Fournette" },
                new Player { Number = 27, Name = "Ronald Jones" }
            }
            };

            _depthChartService.GetFullDepthChart().Returns(fullDepthChart);

            // Act
            var result = _controller.GetFullDepthChart() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(fullDepthChart));
        }

        // Example of an edge test case: Empty depth chart
        [Test]
        public void GetFullDepthChart_ReturnsEmptyDepthChart_WhenNoPlayersAreListed()
        {
            // Arrange
            var emptyDepthChart = new Dictionary<string, List<Player>>();
            _depthChartService.GetFullDepthChart().Returns(emptyDepthChart);

            // Act
            var result = _controller.GetFullDepthChart() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(emptyDepthChart));
        }

        // Example of a failure case: Service throws an exception (simulating a service error)
        [Test]
        public void GetFullDepthChart_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _depthChartService.When(x => x.GetFullDepthChart()).Do(x => throw new System.Exception("Internal service error"));

            // Act
            var result = _controller.GetFullDepthChart() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal service error"));
        }

        private List<Player> PrepareData()
        {
            var mainPlayer = new Player { Number = 666, Name = "Main Player" };
            var gabbert = new Player { Number = 11, Name = "Gabbert, Blaine" };
            var kyle = new Player { Number = 2, Name = "Trask, Kyle" };

            var fullDepthChart = new List<Player>
                        {
                            mainPlayer,
                            gabbert,
                            kyle
                        };


            return fullDepthChart;
        }

    }
}
