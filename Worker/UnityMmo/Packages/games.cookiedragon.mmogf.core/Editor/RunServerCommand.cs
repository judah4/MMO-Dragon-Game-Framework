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
            var commnadLineArguments = CommandLineArgs();
            UnityEngine.Debug.Log("Starting Server Locally." + commnadLineArguments);

            var path = Path.GetDirectoryName(Application.dataPath);
            var programPath = Path.Combine(path, "Server/MmogfMainServer.exe");
            var log = await InvokeProcessStartAsync(programPath, commnadLineArguments);
            UnityEngine.Debug.Log(log);
        }

        public static string CommandLineArgs()
        {
            var sb = new StringBuilder();
            //sb.Append("-i "); sb.Append("MmogfMessages");
            //sb.Append(" -o "); sb.Append("Scripts/Generated");

            return sb.ToString();
        }

        const string InstallName = "messagepack.generator";

        public static async Task<bool> IsInstalledMpc()
        {
            var list = await InvokeProcessStartAsync("dotnet", "tool list -g");
            if (list.Contains(InstallName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<string> InstallMpc()
        {
            return await InvokeProcessStartAsync("dotnet", "tool install --global " + InstallName);
        }

        public static async Task<(bool found, string version)> FindDotnetAsync()
        {
            try
            {
                var version = await InvokeProcessStartAsync("dotnet", "--version");
                return (true, version);
            }
            catch
            {
                return (false, null);
            }
        }

        public static Task<string> InvokeProcessStartAsync(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                //WorkingDirectory = Application.dataPath,
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
