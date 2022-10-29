using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MmoGameFramework.Models;

public class ServerStatusModel
{
    public String Type { get; set; }
    public Boolean Active { get; set; }
    public List<EntityModel> Entities { get; set; }
    public List<ConnectionStatusModel> Connections { get; set; }
}