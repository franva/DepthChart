using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NFLPlayers.Helpers;
using NFLPlayers.Interfaces;
using NFLPlayers.Models;
using System.Collections.Generic;

namespace NFLPlayers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepthChartController : ControllerBase
    {
        private readonly IDepthChartService _depthChartService;
        private readonly IMemoryCache _cache;

        public DepthChartController(IDepthChartService depthChartService, IMemoryCache cache)
        {
            _depthChartService = depthChartService;
            _cache = cache;
        }

        [HttpPost("addPlayer")]
        public IActionResult AddPlayerToDepthChart([FromBody] AddPlayerRequest request)
        {
            try
            {
                var playerWithExtraInfo = ControllerHelper.CreatePlayer(request);

                _depthChartService.AddPlayerToDepthChart(playerWithExtraInfo.Player.SportId, playerWithExtraInfo.Player.TeamId, playerWithExtraInfo.Position!, playerWithExtraInfo!.Player!, playerWithExtraInfo.PositionDepth);
                return Ok();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("removePlayer")]
        public IActionResult RemovePlayerFromDepthChart([FromBody] PlayerRequest request)
        {
            try
            {
                var depthChat = _depthChartService.GetFullDepthChart(request.SportId, request.TeamId);
                var player = depthChat[request.Position!]?.FirstOrDefault(p => p.Number == request.PlayerNumber);
                if (player != null)
                {
                    var removedPlayer = _depthChartService.RemovePlayerFromDepthChart(request.SportId, request.TeamId, request.Position!, player!);
                    return removedPlayer != null ? Ok(removedPlayer) : NotFound();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{sportId:int}/{teamId:int}/getBackups/{position}/{playerNumber:int}")]
        public IActionResult GetBackups(int sportId, int teamId, string position, int playerNumber)
        {
            try
            {
                var cacheKey = $"Backups-{sportId}-{teamId}-{position}";
                if(!_cache.TryGetValue(cacheKey, out List<Player> backups))
                {
                    var fullDepthChart = _depthChartService.GetFullDepthChart(sportId, teamId);
                    if (fullDepthChart == null || !fullDepthChart.ContainsKey(position) || fullDepthChart[position].Count == 0)
                    {
                        return NotFound();
                    }

                    var player = fullDepthChart[position].FirstOrDefault(p => p.Number == playerNumber);
                    if (player == null)
                    {
                        return NotFound();
                    }

                    backups = _depthChartService.GetBackups(sportId, teamId, position, player);
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                    _cache.Set(cacheKey, backups, cacheEntryOptions);
                }

                return Ok(backups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{sportId:int}/{teamId:int}/fullDepthChart")]
        public IActionResult GetFullDepthChart(int sportId, int teamId)
        {
            try
            {
                var cacheKey = $"fullDepthChart_{sportId}_{teamId}";

                if (!_cache.TryGetValue(cacheKey, out Dictionary<string, List<Player>> fullDepthChart))
                {
                    fullDepthChart = _depthChartService.GetFullDepthChart(sportId, teamId);
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                    _cache.Set(cacheKey, fullDepthChart, cacheEntryOptions);
                }

                return Ok(fullDepthChart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("seedData")]
        public IActionResult SeedData()
        {
            _depthChartService.SeedData();
            return Ok("Depth chart seeded successfully.");
        }

    }

}
