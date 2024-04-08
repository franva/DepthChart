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
        public IActionResult AddPlayerToDepthChart([FromBody] JsonElement request)
        {
            try
            {
                var playerWithExtraInfo = ControllerHelper.CreatePlayer(request);

                _depthChartService.AddPlayerToDepthChart(playerWithExtraInfo.Position!, playerWithExtraInfo!.Player!, playerWithExtraInfo.PositionDepth);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("removePlayer")]
        public IActionResult RemovePlayerFromDepthChart([FromBody] dynamic request)
        {
            try
            {
                var playerWithExtraInfo = ControllerHelper.CreatePlayer(request);
                var removedPlayer = _depthChartService.RemovePlayerFromDepthChart(playerWithExtraInfo.Position!, playerWithExtraInfo.Player!);
                return removedPlayer != null ? Ok(removedPlayer) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("getBackups")]
        public IActionResult GetBackups([FromQuery] string position, [FromQuery] int playerNumber)
        {
            try
            {
                var fullDepthChart = _depthChartService.GetFullDepthChart();
                if (fullDepthChart == null || !fullDepthChart.ContainsKey(position) || fullDepthChart[position].Count == 0)
                {
                    return NotFound();
                }

                var player = fullDepthChart[position].FirstOrDefault(p => p.Number == playerNumber);
                if (player == null)
                {
                    return NotFound();
                }

                var backups = _depthChartService.GetBackups(position, player);
                return Ok(backups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("fullDepthChart")]
        public IActionResult GetFullDepthChart()
        {
            try
            {
                var fullDepthChart = _depthChartService.GetFullDepthChart();
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
