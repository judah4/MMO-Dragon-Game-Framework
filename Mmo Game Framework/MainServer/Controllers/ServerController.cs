using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MmoGameFramework.Models;
using WebCommon.Factory;

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

        var entities = new List<EntityModel>();
        foreach (var entity in server.Entities.Entities.Values)
        {
            entities.Add(EntityWebFactory.Convert(entity));
        }
        
        // Populate Model
        var model = new ServerStatusModel
        {
            Type = server.WorkerType,
            Active = server.Active,
            Connections = connections,
            Entities = entities
        };

        
        return Ok(model);
    }
}