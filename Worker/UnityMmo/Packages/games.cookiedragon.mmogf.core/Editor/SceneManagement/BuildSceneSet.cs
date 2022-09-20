#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MessagePack.Unity.Editor
{
    [CreateAssetMenu(fileName = "BuildSceneSet", menuName = "DragonGF/Scenes/Scene Set")]
    public class BuildSceneSet : ScriptableObject
    {
        public List<SceneAsset> Scenes;

    }
}

#endif
