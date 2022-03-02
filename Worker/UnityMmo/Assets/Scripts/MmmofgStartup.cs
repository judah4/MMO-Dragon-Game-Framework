using MessagePack;
using MessagePack.Resolvers;
using Mmogf.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class MmogfStartup
    {
        static bool serializerRegistered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register(
                    MessagePack.Resolvers.MmogfCoreResolver.Instance,
                    MessagePack.Resolvers.GeneratedResolver.Instance,
                    MessagePack.Resolvers.StandardResolver.Instance
                );

                var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

                MessagePackSerializer.DefaultOptions = option;
                serializerRegistered = true;

                LoadEntityComponentTypesList();
            }
        }

        static void LoadEntityComponentTypesList()
        {
            Dictionary<int, System.Type> types = new Dictionary<int, System.Type>();
            //map components to ids
            //yay reflection!
            var type = typeof(IEntityComponent);
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int cnt = 0; cnt < assemblies.Length; cnt++)
            {
                var assembly = assemblies[cnt];
                var typesList = assembly.GetTypes();
                for(int i = 0; i < typesList.Length; i++)
                {
                    var t = typesList[i];
                    if(type.IsAssignableFrom(t) && t.IsInterface == false)
                    {
                        var component = (IEntityComponent)System.Activator.CreateInstance(t);
                        Debug.Log($"{component.GetComponentId()}, {t.Name}");
                        types.Add(component.GetComponentId(), t);
                    }
                }
            }

            ComponentMappings.Init(types);
        }

    //#if UNITY_EDITOR


    //    [UnityEditor.InitializeOnLoadMethod]
    //    static void EditorInitialize()
    //    {
    //        Initialize();
    //    }

    //#endif
    }
}