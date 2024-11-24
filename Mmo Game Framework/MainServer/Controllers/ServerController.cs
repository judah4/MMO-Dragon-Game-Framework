﻿using Microsoft.AspNetCore.Mvc;
using MmoGameFramework.Models;
using System.Collections.Generic;

namespace MmoGameFramework.Controllers;

[ApiController]
[Route("api/server/")]
public class ServerController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetServerStatus()
    {
        //var server = Program.GetServer();

        // Create a Connections List
        var connections = new List<ConnectionStatusModel>();
        //foreach (var connection in server._connections)
        //{
        //    var connectionStatusModel = new ConnectionStatusModel
        //    {
        //        Id = connection.Key.Id,
        //        Worker = connection.Value.WorkerId.Id,
        //        Position = connection.Value.InterestPosition
        //    };
        //
        //    connections.Add(connectionStatusModel);
        //}

        var entities = new List<EntityModel>();
        //foreach (var entity in server.Entities.Entities.Values)
        //{
        //    entities.Add(EntityWebFactory.Convert(entity));
        //}

        // Populate Model
        var model = new ServerStatusModel
        {
            Type = "Not implemented",
            Active = false,
            Connections = connections,
            Entities = entities
        };


        return Ok(model);
    }
}