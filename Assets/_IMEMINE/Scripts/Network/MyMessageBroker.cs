using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

public class MyMessageBroker : NetworkBehaviour
{
    // TODO: Keep a list of which client is what build type, so messages don't get sent to everyone first before they're filtered
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        
        Debug.Log($"On start network on MyMessageBroker, isServer: {InstanceFinder.IsServer}");
        
        FindObjectsOfType<UIWithConnection>().ForEach(uiWithConnection => uiWithConnection.SetConnection(true));
        
        if(Instances.BuildType == BuildType.Voting)
        {
            // if(Instances.BuildType == false)
            //     Instances.WebGLClientUI.oscMessage.Subscribe(message => SendMessageToBuildType(BuildType.OSCClient, message));
        }
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
        
        if (Instances.BuildType == BuildType.Score)
        {
            // Debug.Log($"Received message: {message}");
            string[] subStrings = message.Split($" ");
            
            if(subStrings[0] == $"GoToMeasure")
                Instances.ScoreManager.GoToMeasure(int.Parse(subStrings[1]));
            else if(subStrings[0] == $"HighlightChoice")
                Instances.ScoreManager.HighlightChoice((ChoiceType)int.Parse(subStrings[1]));
        }
    }
}
