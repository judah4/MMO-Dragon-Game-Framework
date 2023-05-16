using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{

    public static class ComponentMappings
    {

        static Dictionary<int, System.Type> _components = null;
        static Dictionary<int, System.Type> _commands = null;
        static Dictionary<int, System.Type> _events = null;

        public static void Init(Dictionary<int, System.Type> components, Dictionary<int, System.Type> commands, Dictionary<int, System.Type> events)
        {
            _components = components;
            _commands = commands;
            _events = events;
        }

        public static System.Type GetComponentType(int componentId)
        {
            if(_components == null)
                return null;

            System.Type type;
            if(_components.TryGetValue(componentId, out type))
                return type;

            Debug.LogError($"Component {componentId} not mapped!");
            return null;
        }
        public static System.Type GetCommandType(int commandId)
        {
            if (_commands == null)
                return null;

            System.Type type;
            if (_commands.TryGetValue(commandId, out type))
                return type;

            Debug.LogError($"Command {commandId} not mapped!");
            return null;
        }
        public static System.Type GetEventType(int eventId)
        {
            if (_events == null)
                return null;

            System.Type type;
            if (_events.TryGetValue(eventId, out type))
                return type;

            Debug.LogError($"Component {eventId} not mapped!");
            return null;
        }


    }
}
