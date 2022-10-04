#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MessagePack.Unity.Editor
{
    internal class RunServerCommand : EditorWindow
    {


        [MenuItem("DragonGF/Run Server (Local)", priority = 50)]
        public static async Task DoRun()
        {
            var commandLineArguments = CommandLineArgs();
            UnityEngine.Debug.Log("Starting Server Locally." + commandLineArguments);

            var path = Path.GetDirectoryName(Application.dataPath);
            var workingPath = Path.Combine(path, "Server");
            var programPath = Path.Combine(workingPath, "MmogfMainServer.exe");
            var log = await InvokeProcessStartAsync(programPath, commandLineArguments, workingPath);
            UnityEngine.Debug.Log(log);
        }

        public static string CommandLineArgs()
        {
            var sb = new StringBuilder();
            //sb.Append("-i "); sb.Append("MmogfMessages");
            //sb.Append(" -o "); sb.Append("Scripts/Generated");

            return sb.ToString();
        }

        public static Task<string> InvokeProcessStartAsync(string fileName, string arguments, string workingPath)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingPath,
            };

            Process p;
            try
            {
                p = Process.Start(psi);
            }
            catch (Exception ex)
            {
                return Task.FromException<string>(ex);
            }

            var tcs = new TaskCompletionSource<string>();
            p.EnableRaisingEvents = true;
            p.Exited += (object sender, System.EventArgs e) =>
            {
                var data = p.StandardOutput.ReadToEnd();
                p.Dispose();
                p = null;

                tcs.TrySetResult(data);
            };

            return tcs.Task;
        }
    }
}

#endif
