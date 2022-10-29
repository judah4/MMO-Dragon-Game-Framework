using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MmoGameFramework.Models;

namespace MmoGameFramework.Controllers;

[ApiController]
[Route("api/server/")]
public class ServerController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetServerStatus()
    {
        var server = Program.GetServer();
        
        // Create a Connections List
        var connections = new List<ConnectionStatusModel>();
        foreach (var connection in server._connections)
        {
            var connectionStatusModel = new ConnectionStatusModel
            {
                Id = connection.Key,
                Worker = connection.Value.WorkerId,
                Position = connection.Value.InterestPosition
            };
            
            connections.Add(connectionStatusModel);
        }
        
        // Populate Model
        var model = new ServerStatusModel
        {
            type = server.WorkerType,
            active = server.Active,
            connections = connections,
            entities = server.Entities.Entities
        };

        
        return Ok(model);
    }
}