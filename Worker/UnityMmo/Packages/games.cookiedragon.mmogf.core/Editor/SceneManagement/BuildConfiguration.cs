#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MessagePack.Unity.Editor
{
    [CreateAssetMenu(fileName = "Build Configuration", menuName = "DragonGF/Scenes/Build Configuration")]
    public class BuildConfiguration : ScriptableObject
    {
        public BuildTarget BuildTarget = BuildTarget.StandaloneWindows64;
        public BuildOptions BuildOptions = BuildOptions.None;
        public StandaloneBuildSubtarget StandaloneBuildSubtarget = StandaloneBuildSubtarget.Player;
        public string[] extraScriptDefines = new string[0];

        public List<BuildSceneSet> Scenes;

        public void SwitchActiveBuildTarget()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                targetGroup: BuildPipeline.GetBuildTargetGroup(platform: BuildTarget), target: BuildTarget);
            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget;
        }

        public void Build()
        {
            SwitchActiveBuildTarget();

            var scenePaths = new List<string>();

            foreach(var sceneSet in Scenes)
            {
                foreach(var scene in sceneSet.Scenes)
                {
                    var path = AssetDatabase.GetAssetOrScenePath(scene);
                    scenePaths.Add(path);
                }

            }

            var extension = "exe";
            switch(BuildTarget)
            {
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.LinuxHeadlessSimulation:
                    extension = "x86_64";
                    break;
                case BuildTarget.StandaloneOSX:
                    extension = "app";
                    break;
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenePaths.ToArray();
            buildPlayerOptions.locationPathName = $"build/Dragongf/{BuildTarget}/{Application.productName}.{extension}";
            buildPlayerOptions.target = BuildTarget;
            buildPlayerOptions.options = BuildOptions;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget;
            buildPlayerOptions.extraScriptingDefines = extraScriptDefines;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build {BuildTarget} {StandaloneBuildSubtarget} succeeded: " + Mathf.Round(summary.totalSize/1000f/1000f) + " mb.");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed");
            }
        }

    }
}

#endif
