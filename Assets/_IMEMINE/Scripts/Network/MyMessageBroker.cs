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
        
        if(ConnectionTypeHolder.ConnectionType == ConnectionStarter.ConnectionType.BayouClient)
            Instances.WebGLClientUI.SetConnection(true);
    }
    
    
}
