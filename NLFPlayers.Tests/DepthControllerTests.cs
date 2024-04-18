using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private IMemoryCache _cache;

        [SetUp]
        public void SetUp()
        {
            _depthChartService = Substitute.For<IDepthChartService>();
            _cache = Substitute.For<IMemoryCache>();
            _controller = new DepthChartController(_depthChartService, _cache);
        }

        [TearDownAttribute]
        public void OneTimeTearDown()
        {
            _cache.Dispose();
        }

        [Test]
        public void AddPlayerToDepthChart_ReturnsOk_WhenPlayerIsAdded()
        {
            // Arrange
            var request = new AddPlayerRequest
            {
                Position = "QB",
                Player = new Player { SportId = 1, TeamId = 1, Number = 10, Name = "New Player" },
                PositionDepth = 1
            };

            // Act
            var result = _controller.AddPlayerToDepthChart(request) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            _depthChartService.Received().AddPlayerToDepthChart(request.Player.SportId, request.Player.TeamId, request.Position, Arg.Any<Player>(), request.PositionDepth);
        }


        [Test]
        public void AddPlayerToDepthChart_ReturnsBadRequest_WhenPlayerDataIsIncomplete()
        {
            // Arrange
            var request = new AddPlayerRequest
            {
                Position = "QB",
                Player = new Player { Number = 10, Name = "New Player" },
                PositionDepth = 1
            };
            // Act
            var result = _controller.AddPlayerToDepthChart(request) as BadRequestObjectResult;

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddPlayerToDepthChart_ReturnsBadRequest_WhenPlayerNumberIsDuplicated()
        {
            var request = new AddPlayerRequest
        {
            Position = "QB",
            Player = new Player { SportId = 1,TeamId = 1,Number = 12, Name = "Tom Brady" },
            PositionDepth = 1
        };

        // Simulate the service throwing an exception for duplicate player number
        _depthChartService.When(x => x.AddPlayerToDepthChart(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>()))
                          .Throw(new InvalidOperationException("A player with this number already exists in the position."));

        var result = _controller.AddPlayerToDepthChart(request) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.Value, Is.EqualTo("A player with this number already exists in the position."));
        }

        [Test]
        public void AddPlayerToDepthChart_ReturnsOk_WhenPlayerIsAddedToDifferentPosition()
        {
            var tempDict = new Dictionary<(int, int, string), List<Player>>();

            var request = new AddPlayerRequest
            {
                Position = "WR",
                Player = new Player { SportId = 1, TeamId = 1,Number = 12, Name = "Tom Brady" },
                PositionDepth = 0
            };

            // Simulate successful addition
            _depthChartService.When(x => x.AddPlayerToDepthChart(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>()))
                            .Do(x =>
                                    {
                                        var key = (x.ArgAt<int>(0), x.ArgAt<int>(1), x.Arg<string>());
                                        if (!tempDict.ContainsKey(key))
                                        {
                                            tempDict[key] = new List<Player>();
                                        }
                                        tempDict[key].Add(x.Arg<Player>());
                                    });

            _depthChartService.When(x => x.GetFullDepthChart(Arg.Any<int>(), Arg.Any<int>()))
                            .Do(x => tempDict.Where(k => k.Key.Item1 == x.ArgAt<int>(0) && k.Key.Item2 == x.ArgAt<int>(1))
                                            .ToDictionary(d => d.Key.Item3, d => d.Value));

            var result = _controller.AddPlayerToDepthChart(request) as OkResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            _depthChartService.Received().AddPlayerToDepthChart(request.Player.SportId, request.Player.TeamId, request.Position, request.Player, request.PositionDepth);
        }

        // -----------------------RemovePlayerFromDepthChart Tests-----------------------
        [Test]
        public void RemovePlayerFromDepthChart_ReturnsOk_WhenPlayerIsRemovedSuccessfully()
        {
           // Arrange
           var tempDict = new Dictionary<(int, int, string), List<Player>>();

           var removeRequest = new PlayerRequest
            {
                SportId = 1,
                TeamId = 1,
                Position = "QB",
                PlayerNumber = 12
            };

            var addRequest = new AddPlayerRequest
            {
                Position = "QB",
                Player = new Player { SportId = 1, TeamId = 1,Number = 12, Name = "Tom Brady" },
                PositionDepth = 0
            };

            _depthChartService.When(x => x.AddPlayerToDepthChart(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>()))
                            .Do(x =>
                                    {
                                        var key = (x.ArgAt<int>(0), x.ArgAt<int>(1), x.Arg<string>());
                                        if (!tempDict.ContainsKey(key))
                                        {
                                            tempDict[key] = new List<Player>();
                                        }
                                        tempDict[key].Add(x.Arg<Player>());
                                    });

            _depthChartService.GetFullDepthChart(Arg.Any<int>(), Arg.Any<int>())
                              .Returns(x => tempDict.Where(k => k.Key.Item1 == x.ArgAt<int>(0) && k.Key.Item2 == x.ArgAt<int>(1))
                                                    .ToDictionary(d => d.Key.Item3, d => d.Value));

            _controller.AddPlayerToDepthChart(addRequest);
           var playerToRemove = new Player { Number = removeRequest.PlayerNumber, Name = "Tom Brady" };
           _depthChartService.RemovePlayerFromDepthChart(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Player>()).Returns(playerToRemove);

           // Act
           var result = _controller.RemovePlayerFromDepthChart(removeRequest) as OkObjectResult;

           // Assert
           Assert.That(result, Is.Not.Null);
           Assert.That(result.StatusCode, Is.EqualTo(200));
           Assert.That(result.Value, Is.EqualTo(playerToRemove));
           _depthChartService.Received().RemovePlayerFromDepthChart(removeRequest.SportId, removeRequest.TeamId, removeRequest.Position, Arg.Is<Player>(p => p.Number == playerToRemove.Number && p.Name == playerToRemove.Name));
        }

        [Test]
        public void RemovePlayerFromDepthChart_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            var removeRequest = new PlayerRequest
            {
                SportId = 1,
                TeamId = 1,
                Position = "QB",
                PlayerNumber = 99 // Non-existent player
            };

            var tempDict = new Dictionary<(int, int, string), List<Player>>();

            var addRequest = new AddPlayerRequest
            {
                Position = "QB",
                Player = new Player { SportId = 1, TeamId = 1,Number = 12, Name = "Tom Brady" },
                PositionDepth = 0
            };

            _depthChartService.When(x => x.AddPlayerToDepthChart(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Player>(), Arg.Any<int?>()))
                            .Do(x =>
                                    {
                                        var key = (x.ArgAt<int>(0), x.ArgAt<int>(1), x.Arg<string>());
                                        if (!tempDict.ContainsKey(key))
                                        {
                                            tempDict[key] = new List<Player>();
                                        }
                                        tempDict[key].Add(x.Arg<Player>());
                                    });

            _depthChartService.GetFullDepthChart(Arg.Any<int>(), Arg.Any<int>())
                              .Returns(x => tempDict.Where(k => k.Key.Item1 == x.ArgAt<int>(0) && k.Key.Item2 == x.ArgAt<int>(1))
                                                    .ToDictionary(d => d.Key.Item3, d => d.Value));

            _controller.AddPlayerToDepthChart(addRequest);
            _depthChartService.RemovePlayerFromDepthChart(removeRequest.SportId, removeRequest.TeamId, removeRequest.Position, Arg.Any<Player>()).Returns(null as Player);

            var result = _controller.RemovePlayerFromDepthChart(removeRequest) as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }


        // ------------------- GetBackups Tests -------------------
        [Test]
        public void GetBackups_ReturnsListOfBackups_WhenValidRequest()
        {
            var request = new PlayerRequest
            {
                SportId = 1,
                TeamId = 1,
                Position = "QB",
                PlayerNumber = 12
            };
            var player1 = new Player { Number = 12, Name = "Tom Brady" };
            var player2 = new Player { Number = 11, Name = "Blaine Gabbert" };
            var backups = new List<Player>
            {
                player2
            };
            var fullDepthChartForNFL = new Dictionary<string, List<Player>>();
            fullDepthChartForNFL.Add("QB", new List<Player> { player1, player2 });
            _depthChartService.GetBackups(request.SportId, request.TeamId, request.Position, player1).Returns(backups);
            _depthChartService.GetFullDepthChart(Arg.Any<int>(), Arg.Any<int>())
                              .Returns(fullDepthChartForNFL);

            var result = _controller.GetBackups(request.SportId, request.TeamId, request.Position, request.PlayerNumber) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EquivalentTo(backups));
        }


        // --------------------- GetFullDepthChart Tests -------------------
        [Test]
        public void GetFullDepthChart_ReturnsCompleteDepthChart_WhenCalled()
        {
            var key = (1,1);
            var fullChart = PrepareData();

            var fullDepthChartForNFL = fullChart.Where( kvp => kvp.Key.Item1 == key.Item1 && kvp.Key.Item2 == key.Item2)
                                                .ToDictionary(k => k.Key.Item3, v => v.Value);

            _depthChartService.GetFullDepthChart(Arg.Any<int>(), Arg.Any<int>())
                              .Returns(fullDepthChartForNFL);

            var result = _controller.GetFullDepthChart(1, 1) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(fullDepthChartForNFL));
        }

        private Dictionary<(int, int, string), List<Player>> PrepareData()
        {
            var tempDict = new Dictionary<(int, int, string), List<Player>>();
            var fullDepthChartForNFL = new Dictionary<string, List<Player>>();

            var mainPlayer = new Player { Number = 666, Name = "Main Player" };
            var gabbert = new Player { Number = 11, Name = "Gabbert, Blaine" };
            var kyle = new Player { Number = 2, Name = "Trask, Kyle" };

            fullDepthChartForNFL.Add("QB", new List<Player> { mainPlayer, gabbert, kyle });
            tempDict.Add((1, 1, "QB"), new List<Player> { mainPlayer, gabbert, kyle });

            return tempDict;
        }

        [Test]
        public void GetBackups_ReturnsDataFromCache_WhenDataIsCached()
        {
            // Arrange
            var sportId = 1;
            var teamId = 1;
            var position = "QB";
            var playerNumber = 0;
            var cacheKey = $"Backups-{sportId}-{teamId}-{position}-{playerNumber}";
            var cachedData = new List<Player> { new Player { Number = 10, Name = "Cached Player", TeamId = teamId, SportId = sportId } };
            _cache.TryGetValue(cacheKey, out _).ReturnsForAnyArgs(x =>
            {
                x[1] = cachedData;
                return true;
            });

            // Act
            var result = _controller.GetBackups(sportId, teamId, position, playerNumber) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(cachedData));
        }

    }
}
