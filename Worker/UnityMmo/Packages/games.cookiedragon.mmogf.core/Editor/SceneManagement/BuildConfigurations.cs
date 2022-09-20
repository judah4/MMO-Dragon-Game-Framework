#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
#if ADDRESSABLES
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MessagePack.Unity.Editor
{
    [CreateAssetMenu(fileName = "BuildConfigurations", menuName = "DragonGF/Scenes/Build Configurations")]
    public class BuildConfigurations : ScriptableObject
    {
        public List<BuildConfiguration> ClientBuildConfigurations = new List<BuildConfiguration>();
        public List<BuildConfiguration> ServerBuildConfigurations = new List<BuildConfiguration>();

        public BuildConfiguration DevClientBuildConfiguration;
        public BuildConfiguration LoadTestingClientBuildConfiguration;

        static BuildConfigurations GetConfigurations()
        {
            var buildConfigurations = AssetDatabase.LoadAssetAtPath<BuildConfigurations>("Assets/Config/BuildConfigurations.asset");
            if (buildConfigurations == null)
            {
                Debug.LogError("Could not find build configurations!");
            }

            return buildConfigurations;
        }

        [MenuItem("DragonGF/Build/Open File Explorer", priority = 100)]
        public static void OpenFileExplorer()
        {
            var projectPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(projectPath, "build/Dragongf");
            var file = Directory.EnumerateFiles(path).FirstOrDefault();
            if (!string.IsNullOrEmpty(file))
                EditorUtility.RevealInFinder(Path.Combine(path, file));
            else
            {
                var directory = Directory.EnumerateDirectories(path).FirstOrDefault();
                if (!string.IsNullOrEmpty(directory))
                    EditorUtility.RevealInFinder(Path.Combine(path, directory));
                else
                    EditorUtility.RevealInFinder(path);
            }
        }

        [MenuItem("DragonGF/Build/Build Dev Client", priority = 120)]
        public static void BuildDevClient()
        {
            Debug.Log("Building Development Client");

            //grab build
            var buildConfigurations = GetConfigurations();
            if (buildConfigurations == null)
                return;

            buildConfigurations.DevClientBuildConfiguration.Build();
        }

        [MenuItem("DragonGF/Build/Build Load Testing Client", priority = 120)]
        public static void BuildLoadTestingClient()
        {
            Debug.Log("Building Loading Testing Client");

            //grab build
            var buildConfigurations = GetConfigurations();
            if (buildConfigurations == null)
                return;

            buildConfigurations.LoadTestingClientBuildConfiguration.Build();

            buildConfigurations.DevClientBuildConfiguration.SwitchActiveBuildTarget();
        }

        [MenuItem("DragonGF/Build/Build Clients", priority = 140)]
        public static void BuildClients()
        {
            Debug.Log("Building Clients");

            //grab build
            var buildConfigurations = GetConfigurations();
            if (buildConfigurations == null)
                return;

            foreach(var buildConfig in buildConfigurations.ClientBuildConfigurations)
            {
                buildConfig.Build();
            }
            buildConfigurations.DevClientBuildConfiguration.SwitchActiveBuildTarget();

        }

        [MenuItem("DragonGF/Build/Build Server", priority = 140)]
        public static void BuildServers()
        {
            Debug.Log("Building Servers");

            //grab build
            var buildConfigurations = GetConfigurations();
            if (buildConfigurations == null)
                return;

            foreach (var buildConfig in buildConfigurations.ServerBuildConfigurations)
            {
                buildConfig.Build();
            }
            buildConfigurations.DevClientBuildConfiguration.SwitchActiveBuildTarget();

        }

        [MenuItem("DragonGF/Build/Build Production", priority = 160)]
        public static void BuildProduction()
        {
            Debug.Log("Building Server and Clients");

            //grab build
            var buildConfigurations = GetConfigurations();
            if (buildConfigurations == null)
                return;

            foreach (var buildConfig in buildConfigurations.ClientBuildConfigurations)
            {
                buildConfig.Build();
            }
            foreach (var buildConfig in buildConfigurations.ServerBuildConfigurations)
            {
                buildConfig.Build();
            }

            buildConfigurations.DevClientBuildConfiguration.SwitchActiveBuildTarget();

        }

#if ADDRESSABLES

        [MenuItem("DragonGF/Build/Build Addressables")]
        public static bool BuildAddressables()
        {
            //https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildPlayerContent.html
            var settings = GetAddressableSettings();

            //settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);

            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        static AddressableAssetSettings GetAddressableSettings()
        {
            var buildConfigurations = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/Config/AddressableAssetSettings.asset");
            if (buildConfigurations == null)
            {
                Debug.LogError("Could not find Addressable Configurations!");
            }

            return buildConfigurations;
        }

#endif

    }
}

#endif
