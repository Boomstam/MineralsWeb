using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Tugboat;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.IO.Compression;

/// <summary>
/// Setup:
/// 1. Set up PlayFlow cloud and copy token into this file
/// 2. Select the right platform in the build settings.
/// 3. You should now be able to use this script!
/// 
/// Warning: Make sure the object holding the ConnectionStarter and the Bayou and Tugboat components is not a prefab.
/// There is a weird issue where the relevant values are seemingly changed in the editor but revert
/// to the prefab value on build or play. 
/// </summary>
public class BuildAutomation : Editor
{
    private static ConnectionStarter connectionStarter => FindObjectOfType<ConnectionStarter>();
    private static Bayou bayou => FindObjectOfType<Bayou>();
    private static Tugboat tugboat => FindObjectOfType<Tugboat>();
    private static BuildType buildType => CurrentBuildType();
    private static ConnectionStarter.ConnectionType connectionTypeFromBuildType => CurrentConnectionType();
    
    [MenuItem("Minerals/Build")]
    private static void Build()
    {
        if (connectionStarter.localClient || connectionStarter.localServer)
        {
            Debug.LogError($"Can't build, local client: {connectionStarter.localClient} or local server: {connectionStarter.localServer}");
            return;
        }
        
        Debug.Log($"Start build {buildType} with connection: {connectionTypeFromBuildType}, client address: {bayou.GetClientAddress()}" +
                  $" and token: {connectionStarter.playflowToken}");
        
        SetConnectionType(connectionTypeFromBuildType);
        
        if(ConnectionTypeHolder.ConnectionType == connectionTypeFromBuildType)
            Observable.Timer(TimeSpan.FromSeconds(0.69f)).Subscribe(_ => DoBuild());
    }

    private static void DoBuild()
    {
        if(buildType == BuildType.Server)
        {
            PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();

            SetUpPlayFlowServer(playFlowDeployWindow);

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => BuildServer(playFlowDeployWindow));
        }
        else
        {
            if (string.IsNullOrEmpty(ConnectionTypeHolder.ActiveServer))
            {
                RefreshAndSetActiveServer();
                Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
                return;
            }
            if(buildType == BuildType.WebGLClient)
            {
                bayou.SetClientAddress(ConnectionTypeHolder.ActiveServer);
                
                Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => BuildWEBGLClient());
            } 
            else
            {
                tugboat.SetClientAddress(ConnectionTypeHolder.ActiveServer);
                
                // if(buildType == BuildType.OSCClient)
                //     Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => BuildOSCClient());
            }
        }
    }

    #region Build Clients

    private static void BuildWEBGLClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_IMEMINE/Scenes/Main.unity" };
        buildPlayerOptions.locationPathName = "C:\\Users\\menno\\MineralsWeb\\Builds\\WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        if (summary.result == BuildResult.Failed)
            Debug.Log("Build failed");
    }
    
    private static void BuildOSCClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_IMEMINE/Scenes/Main.unity" };
        buildPlayerOptions.locationPathName = "C:\\Users\\menno\\MineralsWeb\\Builds\\Windows\\Windows";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        if (summary.result == BuildResult.Failed)
            Debug.Log("Build failed");
    }

    #endregion
    

    private static void SetUpPlayFlowServer(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        FieldInfo tokenFieldInfo = playFlowDeployWindow.GetType()
            .GetField("tokenField", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField tokenTextField = (TextField)tokenFieldInfo.GetValue(playFlowDeployWindow);

        tokenTextField.value =  connectionStarter.playflowToken;

        FieldInfo locationFieldInfo = playFlowDeployWindow.GetType()
            .GetField("location", BindingFlags.NonPublic | BindingFlags.Instance);
        DropdownField locationDropdown = (DropdownField)locationFieldInfo.GetValue(playFlowDeployWindow);

        locationDropdown.index = 4;

        FieldInfo instanceTypeFieldInfo = playFlowDeployWindow.GetType()
            .GetField("instanceType", BindingFlags.NonPublic | BindingFlags.Instance);
        DropdownField instanceTypeDropdown = (DropdownField)instanceTypeFieldInfo.GetValue(playFlowDeployWindow);

        instanceTypeDropdown.index = 0;

        FieldInfo enableSSLFieldInfo = playFlowDeployWindow.GetType()
            .GetField("enableSSL", BindingFlags.NonPublic | BindingFlags.Instance);
        Toggle enableSSLToggle = (Toggle)enableSSLFieldInfo.GetValue(playFlowDeployWindow);

        enableSSLToggle.value = true;

        FieldInfo sslValueFieldInfo = playFlowDeployWindow.GetType()
            .GetField("sslValue", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField sslValueTextField = (TextField)sslValueFieldInfo.GetValue(playFlowDeployWindow);

        sslValueTextField.value = connectionStarter.serverBayouPort.ToString();

        MethodInfo onUploadMethodInfo = playFlowDeployWindow.GetType()
            .GetMethod("OnUploadPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onUploadMethodInfo.Invoke(playFlowDeployWindow, null);
    }

    private static async void BuildServer(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        string response = await PlayFlowAPI.Get_Upload_Version(connectionStarter.playflowToken);
        
        MethodInfo onStartMethodInfo = playFlowDeployWindow.GetType().GetMethod("OnStartPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onStartMethodInfo.Invoke(playFlowDeployWindow, null);
        
        RefreshAndSetActiveServer();
    }
    
    [MenuItem("Minerals/RefreshAndSetActiveServer")]
    private static void RefreshAndSetActiveServer()
    {
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();
        
        MethodInfo onRefreshMethodInfo = playFlowDeployWindow.GetType().GetMethod("OnGetStatusPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onRefreshMethodInfo.Invoke(playFlowDeployWindow, null);
        
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => SetActiveServer());
    }

    [MenuItem("Minerals/SetActiveServer")]
    private static void SetActiveServer()
    {
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();

        string logs = GetPlayFlowLog(playFlowDeployWindow);
        string activeServer = ExtractJSONProperty(logs, "server_url");
        
        Debug.Log($"Active server: {activeServer}, in holder: {ConnectionTypeHolder.ConnectionType}");
        if(ConnectionTypeHolder.ActiveServer != activeServer)
        {
            ConnectionTypeHolder.ActiveServer = activeServer;
            CreateConnectionTypeHolder(ConnectionTypeHolder.ConnectionType);
        }

        Debug.Log($"Active server set to {ConnectionTypeHolder.ActiveServer}");
        bayou.SetClientAddress(ConnectionTypeHolder.ActiveServer);
        tugboat.SetClientAddress(ConnectionTypeHolder.ActiveServer);
    }
    
    [MenuItem("Minerals/PlayEditorAsClient")]
    private static void SetUpForEditorAndEnterPlaymode()
    {
        if (string.IsNullOrEmpty(ConnectionTypeHolder.ActiveServer))
        {
            SetActiveServer();
            Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
            return;
        }
        SetUpForEditor();
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => { EditorApplication.EnterPlaymode(); });
    }
    
    [MenuItem("Minerals/SetUpForEditor")]
    private static void SetUpForEditor()
    {
        SetConnectionType(ConnectionStarter.ConnectionType.TugboatClient);
        
        Debug.Log($"Set up for editor with activeServer: {ConnectionTypeHolder.ActiveServer}");
        
        if (string.IsNullOrEmpty(ConnectionTypeHolder.ActiveServer))
        {
            SetActiveServer();
            Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
            return;
        }
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            tugboat.SetClientAddress(ConnectionTypeHolder.ActiveServer);
            Debug.Log($"Set tugboat client to activeServer: {ConnectionTypeHolder.ActiveServer}");
        });
    }
    
    private static string GetPlayFlowLog(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        FieldInfo logsFieldInfo = playFlowDeployWindow.GetType().GetField("logs", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField logsTextField = (TextField)logsFieldInfo.GetValue(playFlowDeployWindow);

        return logsTextField.value;
    }
    
    private static void SetConnectionType(ConnectionStarter.ConnectionType connType)
    {
        if(ConnectionTypeHolder.ConnectionType != connType)
            CreateConnectionTypeHolder(connType);
    }

    private static void CreateConnectionTypeHolder(ConnectionStarter.ConnectionType connType)
    {
        Debug.Log($"Create connection type holder");
        if (string.IsNullOrEmpty(ConnectionTypeHolder.ActiveServer))
        {
            Debug.LogError($"Active server null or empty");
            return;
        }

        string connectionTypeString = connType.ToString();
        
        string scriptFileName = "ConnectionTypeHolder.cs";
        string scriptContent = @"
using UnityEngine;

public static class ConnectionTypeHolder
{
    // This script is generated by BuildAutomation.cs.
";
        scriptContent += $"public static ConnectionStarter.ConnectionType ConnectionType = ConnectionStarter.ConnectionType.{connectionTypeString};";
        scriptContent += $"public static string ActiveServer = \"{ConnectionTypeHolder.ActiveServer}\";";
        scriptContent += "}";

        string assetsPath = Application.dataPath + $"/_IMEMINE/Generated";

        string scriptFilePath = Path.Combine(assetsPath, scriptFileName);

        File.WriteAllText(scriptFilePath, scriptContent);
        
        UnityEditor.AssetDatabase.Refresh();
    }
    
    private static string ExtractJSONProperty(string json, string property)
    {
        string separator = $"{property}\":\"";
        string[] splitLogs = json.Split(separator);
        return splitLogs[1].Split("\"")[0];
    }

    private static BuildType CurrentBuildType()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        if(buildTarget is BuildTarget.StandaloneLinux64)
            return BuildType.Server;
        else if (buildTarget is BuildTarget.WebGL)
            return BuildType.WebGLClient;
        else if (buildTarget is BuildTarget.StandaloneWindows64)
            return BuildType.OSCClient;
        else
            return BuildType.NonWebGLClient;
    }
    
    private static ConnectionStarter.ConnectionType CurrentConnectionType()
    {
        if (buildType == BuildType.Server)
            return ConnectionStarter.ConnectionType.Host;
        else if (buildType == BuildType.WebGLClient)
            return ConnectionStarter.ConnectionType.BayouClient;
        else
            return ConnectionStarter.ConnectionType.TugboatClient;
    }

    public enum BuildType
    {
        Server,
        WebGLClient,
        NonWebGLClient,
        OSCClient,
    }
}
