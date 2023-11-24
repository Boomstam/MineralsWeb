using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Tugboat;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildAutomation : Editor
{ 
    private static BuildType buildType => CurrentBuildType();
    private static ConnectionStarter.ConnectionType connectionType => CurrentConnectionType();
    
    private const ushort clientBayouPort = 443;
    private const ushort serverBayouPort = 7777;

    private const string playflowToken = "1317dd7cadb3232d22e7eb710c4c85f7";

    private static IDisposable waitForServerConnection;
    
    [MenuItem("BuildAutomation/Build")]
    private static void Build()
    {
        Debug.Log($"Build {buildType} with connection {connectionType}");

        Bayou bayou = FindObjectOfType<Bayou>();
        ConnectionStarter connectionStarter = FindObjectOfType<ConnectionStarter>();

        connectionStarter.connectionType = connectionType;
        
        PlayFlowCloudDeploy playFlowDeployWindow = EditorWindow.GetWindow<PlayFlowCloudDeploy>();

        // playFlowDeployWindow.get_status().ToObservable()
        //     .Subscribe(_ => Debug.Log($"Done {_}"));

        FieldInfo tokenFieldInfo = playFlowDeployWindow.GetType().GetField("tokenField", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField tokenTextField = (TextField)tokenFieldInfo.GetValue(playFlowDeployWindow);

        tokenTextField.value = playflowToken;
        
        FieldInfo locationFieldInfo = playFlowDeployWindow.GetType().GetField("location", BindingFlags.NonPublic | BindingFlags.Instance);
        DropdownField locationDropdown = (DropdownField)locationFieldInfo.GetValue(playFlowDeployWindow);

        locationDropdown.index = 4;
        
        FieldInfo instanceTypeFieldInfo = playFlowDeployWindow.GetType().GetField("instanceType", BindingFlags.NonPublic | BindingFlags.Instance);
        DropdownField instanceTypeDropdown = (DropdownField)instanceTypeFieldInfo.GetValue(playFlowDeployWindow);

        instanceTypeDropdown.index = 0;
        
        FieldInfo enableSSLFieldInfo = playFlowDeployWindow.GetType().GetField("enableSSL", BindingFlags.NonPublic | BindingFlags.Instance);
        Toggle enableSSLToggle = (Toggle)enableSSLFieldInfo.GetValue(playFlowDeployWindow);

        enableSSLToggle.value = true;

        FieldInfo sslValueFieldInfo = playFlowDeployWindow.GetType().GetField("sslValue", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField sslValueTextField = (TextField)sslValueFieldInfo.GetValue(playFlowDeployWindow);

        if(buildType == BuildType.WebGLClient)
        {
            bayou.SetPort(clientBayouPort);
            bayou.SetUseWSS(true);
            
            sslValueTextField.value = clientBayouPort.ToString();
        }
        else if(buildType == BuildType.Server)
        {
            bayou.SetPort(serverBayouPort);
            bayou.SetUseWSS(false);
            
            sslValueTextField.value = serverBayouPort.ToString();
        }
        // waitForServerConnection?.Dispose();
        // waitForServerConnection = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(_ => { LogMessage(); });

        Do();
        
        // Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => { Debug.Log("test " + GetPlayFlowLogs(playFlowDeployWindow)); });
    }

    private static async void Do()
    {
        Debug.Log($"Start do");
        
        string response = await PlayFlowAPI.Get_Upload_Version(playflowToken);
        
        Debug.Log($"response: {response}");
    }

    private static string GetPlayFlowLogs(PlayFlowCloudDeploy playFlowDeployWindow)
    {
        FieldInfo logsFieldInfo = playFlowDeployWindow.GetType().GetField("logs", BindingFlags.NonPublic | BindingFlags.Instance);
        TextField logsTextField = (TextField)logsFieldInfo.GetValue(playFlowDeployWindow);

        return logsTextField.value;
    }
    
    private static void LogMessage()
    {
        Debug.Log($"Debug");
    }

    [MenuItem("BuildAutomation/Cancel")]
    private static void Cancel()
    {
        waitForServerConnection?.Dispose();
        waitForServerConnection = null;
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
