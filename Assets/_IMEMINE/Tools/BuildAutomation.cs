using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Tugboat;
using Sirenix.OdinInspector;
using UniRx;
using UnityEditor;
using UnityEngine;

public class BuildAutomation : MonoBehaviour
{
    [InfoBox("Set the build target to Dedicated Server/Linux for the server, WebGL for BayouClient, " +
             "Android or iOS for nonWebGL client, and Windows for OSC client.")]
    [ShowInInspector, ReadOnly] private BuildType buildType => CurrentBuildType();
    [ShowInInspector, ReadOnly] private ConnectionStarter.ConnectionType connectionType => CurrentConnectionType();

    [SerializeField] private Bayou bayou;
    [SerializeField] private ConnectionStarter connectionStarter;
    [SerializeField] private ushort clientBayouPort;
    [SerializeField] private ushort serverBayouPort;
    
    [ShowInInspector, ReadOnly] private string onDraw => OnDraw();

    private IDisposable waitForServerConnection;
    
    [Button]
    private void Build()
    {
        Debug.Log($"Build {buildType} with connection {connectionType}");

        connectionStarter.connectionType = connectionType;

        if(buildType == BuildType.WebGLClient)
        {
            bayou.SetPort(clientBayouPort);
            bayou.SetUseWSS(true);
        }
        else if(buildType == BuildType.Server)
        {
            bayou.SetPort(serverBayouPort);
            bayou.SetUseWSS(false);
        }
        waitForServerConnection = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(_ => { LogMessage(); }).AddTo(this);

    }
    
    
    private void LogMessage()
    {
        Debug.Log($"Debug");
    }

    [Button]
    private void StopWaitingForServer()
    {
        waitForServerConnection.Dispose();
    }

    private BuildType CurrentBuildType()
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
    
    private ConnectionStarter.ConnectionType CurrentConnectionType()
    {
        if (buildType == BuildType.Server)
            return ConnectionStarter.ConnectionType.Host;
        else if (buildType == BuildType.WebGLClient)
            return ConnectionStarter.ConnectionType.BayouClient;
        else
            return ConnectionStarter.ConnectionType.TugboatClient;
    }

    private string OnDraw()
    {
        // Debug.Log($"OnDrawBuildAutomation");

        return "OnDraw Function";
    }

    public enum BuildType
    {
        Server,
        WebGLClient,
        NonWebGLClient,
        OSCClient,
    }
}
