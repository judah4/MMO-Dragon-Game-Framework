using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MmoGameFramework.Models;

public class ServerStatusModel
{
    public String type { get; set; }
    public Boolean active { get; set; }
    public ConcurrentDictionary<int, Entity> entities { get; set; }
    public List<ConnectionStatusModel> connections { get; set; }
}