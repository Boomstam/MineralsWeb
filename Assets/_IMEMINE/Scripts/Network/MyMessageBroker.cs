using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class MyMessageBroker : NetworkBehaviour
{
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        
        Debug.Log($"On start network on MyMessageBroker, isServer: {InstanceFinder.IsServer}");
        
        if(Instances.BuildType == BuildType.WebGLClient)
        {
            Instances.WebGLClientUI.SetConnection(true);
            Instances.WebGLClientUI.oscMessage.Subscribe(message => SendMessageToBuildType(BuildType.OSCClient, message));
        }
        if(Instances.BuildType == BuildType.OSCClient)
            Instances.OSCClientUI.SetConnection(true);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SendMessageToBuildType(BuildType targetBuildType, string message)
    {
        SendMessageToBuildTypeClients(targetBuildType, message);
    }

    [ObserversRpc]
    private void SendMessageToBuildTypeClients(BuildType targetBuildType, string message)
    {
        if(Instances.BuildType == targetBuildType)
            OnBuildTypeMessageReceived(message);
    }

    private void OnBuildTypeMessageReceived(string message)
    {
        if(Instances.BuildType == BuildType.OSCClient)
        {
            Instances.OSCClientUI.SetMessage(message);
            Instances.OSCManager.SendOSCMessage("/channel/1", message);
        }
    }
}
