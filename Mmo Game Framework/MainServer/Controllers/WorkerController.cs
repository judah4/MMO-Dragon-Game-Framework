using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MmoGameFramework.Models;
namespace MmoGameFramework.Controllers;

[ApiController]
[Route("api/worker/")]
public class WorkerController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetWorkerStatus()
    {
        var worker = Program.GetWorker();
        
        // Create a Connections List
        var connections = new List<ConnectionStatusModel>();
        foreach (var connection in worker._connections)
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
            type = worker.WorkerType,
            active = worker.Active,
            connections = connections,
            entities = worker.Entities.Entities
        };

        
        return Ok(model);
    }
}