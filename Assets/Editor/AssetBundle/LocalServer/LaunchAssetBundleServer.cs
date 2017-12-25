using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace AssetBundles
{
	internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
	{

		[SerializeField]
		int m_ServerPID = 0;
        
        public static void CheckAndDoRunning()
        {
            bool needRunning = AssetBundleConfig.IsSimulateMode;
            bool isRunning = IsRunning();
            if (needRunning != isRunning)
            {
                if (needRunning)
                {
                    Run();
                    UnityEngine.Debug.Log("Local assetbundle server run!");
                }
                else
                {
                    KillRunningAssetBundleServer();
                    UnityEngine.Debug.Log("Local assetbundle server stop!");
                }
            }
        }
        
		static bool IsRunning ()
		{
			if (instance.m_ServerPID == 0)
				return false;

			var process = Process.GetProcessById (instance.m_ServerPID);
			if (process == null)
				return false;

			return !process.HasExited;
		}
        
		static void KillRunningAssetBundleServer ()
		{
			try
			{
				if (instance.m_ServerPID == 0)
					return;

				var lastProcess = Process.GetProcessById (instance.m_ServerPID);
				lastProcess.Kill();
				instance.m_ServerPID = 0;
			}
			catch
			{
			}
		}

		static void Run ()
		{
			KillRunningAssetBundleServer();
            
			AssetBundleUtility.WriteAssetBundleServerURL();

			string args = string.Format("\"{0}\" {1}", AssetBundleConfig.LocalSvrAppWorkPath, Process.GetCurrentProcess().Id);
            ProcessStartInfo startInfo = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), 
                "4.0", AssetBundleConfig.LocalSvrAppPath, args , true);
            startInfo.WorkingDirectory = AssetBundleConfig.LocalSvrAppWorkPath;
			startInfo.UseShellExecute = false;
			Process launchProcess = Process.Start(startInfo);
			if (launchProcess == null || launchProcess.HasExited == true || launchProcess.Id == 0)
			{
				UnityEngine.Debug.LogError ("Unable Start AssetBundleServer process!");
			}
			else
			{
				instance.m_ServerPID = launchProcess.Id;
			}
		}
	}
}