using Microsoft.AspNetCore.Mvc;
using MapAPI.Helpers;
using MapAPI.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace MapAPI.Controllers
{
    [Route("api/map")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private static Graph _graph; // Static variable to hold the graph in memory
        private readonly string _validApiKey;

        // Constructor to initialize valid API key from configuration
        public MapController(IConfiguration configuration)
        {
            _validApiKey = configuration["ApiKeys:FS_ReadWrite"]; // API key to access SetMap
            if (_graph == null)
            {
                _graph = new Graph();
            }
        }

        // Middleware for API key validation
        private bool IsValidApiKey(string apiKey)
        {
            // If the API key is missing or empty, return false
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            return apiKey == _validApiKey; // Check if the provided key matches the valid API key
        }

        // SetMap endpoint
        [HttpPost("SetMap")]
        public IActionResult SetMap([FromHeader] string apiKey, [FromBody] Graph newGraph)
        {
            // Check if API key is missing or invalid
            if (string.IsNullOrEmpty(apiKey) || !IsValidApiKey(apiKey))
            {
                return Unauthorized("API key is missing or invalid.");
            }

            // Check if graph data is provided
            if (newGraph == null || !newGraph.Nodes.Any())
            {
                return BadRequest("Graph data is required.");
            }

            _graph = newGraph;

            // Log the graph for debugging purposes
            Console.WriteLine("Graph set: " + JsonConvert.SerializeObject(_graph));

            return Ok("Graph stored successfully.");
        }

        // GetMap endpoint
        [HttpGet("GetMap")]
        public IActionResult GetMap()
        {
            // Check if the graph is set
            if (_graph == null || !_graph.Nodes.Any())
            {
                return NotFound("Graph not found.");
            }

            return Ok(_graph);
        }

        // ShortestRoute endpoint
        [HttpGet("ShortestRoute")]
        public IActionResult ShortestRoute([FromQuery] string from, [FromQuery] string to, [FromHeader] string apiKey)
        {
            // Check if API key is missing or invalid
            if (string.IsNullOrEmpty(apiKey) || !IsValidApiKey(apiKey))
            {
                return Unauthorized("API key is missing or invalid.");
            }

            // Check if 'from' and 'to' parameters are provided
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return BadRequest("Both 'from' and 'to' parameters are required.");
            }

            // Check if the nodes exist in the graph
            if (!_graph.Nodes.ContainsKey(from) || !_graph.Nodes.ContainsKey(to))
            {
                return BadRequest($"Unknown node names: '{from}' or '{to}' do not exist in the graph.");
            }

            // If the graph is not set, return an error
            if (_graph.Nodes.Count == 0)
            {
                return BadRequest("Map has not been set. Please set the graph data first.");
            }

            try
            {
                // Call the ShortestRouteCalculator to get the path
                var path = ShortestRouteCalculator.FindShortestRoute(_graph, from, to);
                return Ok(new { Path = path });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // ShortestDistance endpoint
        [HttpGet("ShortestDistance")]
        public IActionResult ShortestDistance([FromQuery] string from, [FromQuery] string to, [FromHeader] string apiKey)
        {
            // Check if API key is missing or invalid
            if (string.IsNullOrEmpty(apiKey) || !IsValidApiKey(apiKey))
            {
                return Unauthorized("API key is missing or invalid.");
            }

            // Check if 'from' and 'to' parameters are provided
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return BadRequest("Both 'from' and 'to' parameters are required.");
            }

            // Check if the nodes exist in the graph
            if (!_graph.Nodes.ContainsKey(from) || !_graph.Nodes.ContainsKey(to))
            {
                return BadRequest($"Unknown node names: '{from}' or '{to}' do not exist in the graph.");
            }

            // If the graph is not set, return an error
            if (_graph.Nodes.Count == 0)
            {
                return BadRequest("Map has not been set. Please set the graph data first.");
            }

            try
            {
                // Call the ShortestDistanceCalculator to get the distance
                var distance = ShortestDistanceCalculator.FindShortestDistance(_graph, from, to);
                return Ok(new { Distance = distance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
