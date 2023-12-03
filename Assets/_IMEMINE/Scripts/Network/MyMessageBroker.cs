using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class MyMessageBroker : NetworkBehaviour
{
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        
        Debug.Log($"On start network, isServer: {InstanceFinder.IsServer}");
        
        if(Instances.BuildType == BuildType.WebGLClient)
            Instances.WebGLClientUI.SetConnection(true);
        if(Instances.BuildType == BuildType.OSCClient)
            Instances.OSCClientUI.SetConnection(true);
    }

    [ServerRpc]
    public void SendMessage(BuildType targetBuildType, string message)
    {
        SendToClients(targetBuildType, message);
    }

    [ObserversRpc]
    private void SendToClients(BuildType targetBuildType, string message)
    {
        if(Instances.BuildType == targetBuildType)
            OnMessageReceived(message);
    }

    private void OnMessageReceived(string message)
    {
        if(Instances.BuildType == BuildType.OSCClient)
        {
            Instances.OSCClientUI.SetMessage(message);
            Instances.OSCManager.SendOSCMessage("/channel/1", message);
        }
    }
}
