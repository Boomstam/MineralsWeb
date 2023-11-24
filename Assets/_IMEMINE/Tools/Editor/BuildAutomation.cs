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
    private const ushort clientBayouPort = 443;
    private const ushort serverBayouPort = 7777;
    private const string playflowToken = "1317dd7cadb3232d22e7eb710c4c85f7";

    private static string clientAddress;
    private static string activeServer;

    private static Bayou bayou => FindObjectOfType<Bayou>();
    private static Tugboat tugboat => FindObjectOfType<Tugboat>();
    private static ConnectionStarter connectionStarter => FindObjectOfType<ConnectionStarter>();
    private static BuildType buildType => CurrentBuildType();
    private static ConnectionStarter.ConnectionType connectionType => CurrentConnectionType();

    private static IDisposable waitForServerConnection;
    
    [MenuItem("Minerals/Build")]
    private static void Build()
    {
        Debug.Log($"Start build {buildType} with connection {connectionType} and token {playflowToken}");
        SetConnectionType(connectionType);
        
        Observable.Timer(TimeSpan.FromSeconds(0.69f)).Subscribe(_ => DoBuild());
    }

    private static void DoBuild()
    {
            
        if(buildType == BuildType.Server)
        {
            PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();
            
            activeServer = null;
            
            bayou.SetPort(serverBayouPort);
            bayou.SetUseWSS(false);
            
            SetUpPlayFlowServer(playFlowDeployWindow);

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => BuildServer(playFlowDeployWindow));
        }
        else
        {
            if (string.IsNullOrEmpty(activeServer))
            {
                RefreshAndSetActiveServer();
                Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
                return;
            }
            if(buildType == BuildType.WebGLClient)
            {
                bayou.SetPort(clientBayouPort);
                bayou.SetUseWSS(true);
            
                bayou.SetClientAddress(activeServer);
                
                Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => BuildWEBGLClient());
            } 
            else
            {
                tugboat.SetClientAddress(activeServer);
            }
        }
    }

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
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static void SetUpPlayFlowServer(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        FieldInfo tokenFieldInfo = playFlowDeployWindow.GetType()
            .GetField("tokenField", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField tokenTextField = (TextField)tokenFieldInfo.GetValue(playFlowDeployWindow);

        tokenTextField.value = playflowToken;

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

        sslValueTextField.value = serverBayouPort.ToString();

        MethodInfo onUploadMethodInfo = playFlowDeployWindow.GetType()
            .GetMethod("OnUploadPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onUploadMethodInfo.Invoke(playFlowDeployWindow, null);
    }

    private static async void BuildServer(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        string response = await PlayFlowAPI.Get_Upload_Version(playflowToken);
        
        MethodInfo onStartMethodInfo = playFlowDeployWindow.GetType().GetMethod("OnStartPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onStartMethodInfo.Invoke(playFlowDeployWindow, null);
        
        RefreshAndSetActiveServer();
        
        Observable.Timer(TimeSpan.FromSeconds(10f)).Subscribe(_ => SetupServerConnection(playFlowDeployWindow));
    }
    
    [MenuItem("Minerals/RefreshAndSetActiveServer")]
    private static void RefreshAndSetActiveServer()
    {
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();
        
        MethodInfo onRefreshMethodInfo = playFlowDeployWindow.GetType().GetMethod("OnGetStatusPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onRefreshMethodInfo.Invoke(playFlowDeployWindow, null);
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => SetActiveServer());
    }

    private static void SetupServerConnection(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        waitForServerConnection?.Dispose();
        waitForServerConnection = Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ => { ServerConnectionCheck(playFlowDeployWindow); });
    }
    
    [MenuItem("Minerals/SetActiveServer")]
    private static void SetActiveServer()
    {
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();

        string logs = GetPlayFlowLog(playFlowDeployWindow);
        activeServer = ExtractJSONProperty(logs, "server_url");
        
        // FieldInfo activeServerFieldInfo = playFlowDeployWindow.GetType().GetField("activeServersField", BindingFlags.NonPublic | BindingFlags.Instance);
        // DropdownField activeServerDropdown = (DropdownField)activeServerFieldInfo.GetValue(playFlowDeployWindow);

        // activeServer = activeServerDropdown.value.Split(" -> (SSL)")[0];

        Debug.Log($"Active server set to {activeServer}");
    }

    private static void ServerConnectionCheck(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        MethodInfo onRefreshMethodInfo = playFlowDeployWindow.GetType().GetMethod("OnRefreshPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        onRefreshMethodInfo.Invoke(playFlowDeployWindow, null);
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => DoCheck(playFlowDeployWindow));
    }

    private static void DoCheck(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        string logs = GetPlayFlowLog(playFlowDeployWindow);
        
        string status = ExtractJSONProperty(logs, "status");
        
        Debug.Log($"status: {status}");
        
        if(status == "running")
            OnRunning();
    }

    private static void OnRunning()
    {
        Cancel();
        Debug.Log($"<color=green>RUNNING!</color>");
        EditorApplication.Beep();
        Observable.Timer(TimeSpan.FromSeconds(0.69f)).Subscribe(_ => EditorApplication.Beep());
        Observable.Timer(TimeSpan.FromSeconds(0.69f * 2f)).Subscribe(_ => EditorApplication.Beep());
    }

    private static string GetPlayFlowLog(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        FieldInfo logsFieldInfo = playFlowDeployWindow.GetType().GetField("logs", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField logsTextField = (TextField)logsFieldInfo.GetValue(playFlowDeployWindow);

        return logsTextField.value;
    }

    [MenuItem("Minerals/TryDeserialize")]
    private static void TryDeserialize()
    {
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();
        
        string logs = GetPlayFlowLog(playFlowDeployWindow);

        string status = ExtractJSONProperty(logs, "status");
        string serverUrl = ExtractJSONProperty(logs, "server_url");

        Debug.Log(status);
        Debug.Log(serverUrl);
    }

    private static string ExtractJSONProperty(string json, string property)
    {
        string separator = $"{property}\":\"";
        string[] splitLogs = json.Split(separator);
        return splitLogs[1].Split("\"")[0];
    }

    [MenuItem("Minerals/PlayEditorAsClient")]
    private static void SetUpForEditorAndEnterPlaymode()
    {
        if (string.IsNullOrEmpty(activeServer))
        {
            SetActiveServer();
            Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
            return;
        }
        SetUpForEditor();
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            EditorApplication.EnterPlaymode();
        });
    }

    [MenuItem("Minerals/SetUpForEditor")]
    private static void SetUpForEditor()
    {
        SetConnectionType(ConnectionStarter.ConnectionType.TugboatClient);
        
        Debug.Log($"Set up for editor with activeServer: {activeServer}");
        
        if (string.IsNullOrEmpty(activeServer))
        {
            SetActiveServer();
            Debug.Log($"<color=red>Active server not filled in! Try again when server is refreshed in 3 seconds.</color>");
                
            return;
        }
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            tugboat.SetClientAddress(activeServer);
            Debug.Log($"Set tugboat client to activeServer: {activeServer}");
        });
    }
    
    [MenuItem("Minerals/Cancel")]
    private static void Cancel()
    {
        Debug.Log($"<color=orange>Connection check routine cancelled</color>");
        waitForServerConnection?.Dispose();
        waitForServerConnection = null;
    }

    private static void SetConnectionType(ConnectionStarter.ConnectionType connectionType)
    {
        // PlayerPrefs.SetInt(ConnectionStarter.ConnectionTypePlayerPrefsKey, (int)connectionType);
        BuildTypeSO buildTypeSo = Resources.Load<BuildTypeSO>("BuildTypeSO");
        buildTypeSo.ConnectionType = connectionType;
        
        AssetDatabase.SaveAssets();
        Debug.Log($"Set connection type to {buildTypeSo.ConnectionType}");

        // connectionStarter.connectionType = connectionType;
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
