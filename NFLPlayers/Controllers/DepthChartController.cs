using Microsoft.AspNetCore.Mvc;
using NFLPlayers.Helpers;
using NFLPlayers.Interfaces;
using System.Text.Json;

namespace NFLPlayers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepthChartController : ControllerBase
    {
        private readonly IDepthChartService _depthChartService;

        public DepthChartController(IDepthChartService depthChartService)
        {
            _depthChartService = depthChartService;
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
        public IActionResult GetBackups(int sportId, int teamId, string position, [FromQuery] int playerNumber)
        {
            try
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

                var backups = _depthChartService.GetBackups(sportId, teamId, position, player);
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
                var fullDepthChart = _depthChartService.GetFullDepthChart(sportId, teamId);
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
