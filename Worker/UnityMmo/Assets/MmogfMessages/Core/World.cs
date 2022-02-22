using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    public struct World
    {

        [MessagePackObject]
        public struct CreateEntity : ICommand
        {
            [Key(0)]
            public string EntityType { get; set; }
            [Key(1)]
            public Position Position { get; set; }
            [Key(2)]
            public Rotation Rotation { get; set; }
            [Key(3)]
            public Dictionary<int, byte[]> Components { get; set; }

            public CreateEntity(string entityType, Position position, Rotation rotation, Dictionary<int, byte[]> components)
            {
                EntityType = entityType;
                Position = position;
                Rotation = rotation;
                Components = components;
            }


        }
    }
}