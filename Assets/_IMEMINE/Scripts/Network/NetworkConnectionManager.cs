using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public class NetworkConnectionManager : NetworkBehaviour
{
    private Dictionary<int, BuildType> connections = new Dictionary<int, BuildType>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        AddOnServer(Instances.BuildType);
    }

    [Button, ServerRpc(RequireOwnership = false)]
    public void ResetConnections()
    {
        connections = new Dictionary<int, BuildType>();

        foreach (NetworkConnection connection in ServerManager.Clients.Values)
        {
            AddOnServerAfterReset(connection);
        }
    }

    [TargetRpc]
    private void AddOnServerAfterReset(NetworkConnection conn)
    {
        AddOnServer(Instances.BuildType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddOnServer(BuildType buildType, NetworkConnection connection = null)
    {
        connections.Add(connection.ClientId, buildType);
        
        UpdateConnections();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        RemoveFromServer();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemoveFromServer(NetworkConnection connection = null)
    {
        if(connections.ContainsKey(connection.ClientId))
            connections.Remove(connection.ClientId);
        
        UpdateConnections();
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        connections = new Dictionary<int, BuildType>();
    }

    [Server]
    private void UpdateConnections()
    {
        int numOSCConnections = connections.Values.Count(buildType => buildType == BuildType.OSCClient);
        SendOSCConnectionsToMonitor(numOSCConnections);
        
        int numClientConnections = connections.Values.Count(buildType => buildType == BuildType.Voting);
        SendClientConnectionsToMonitor(numClientConnections);
    }

    [ObserversRpc]
    private void SendOSCConnectionsToMonitor(int oscConnections)
    {
        if(Instances.BuildType != BuildType.Monitor)
            return;
        
        Instances.MonitorUI.SetOSCConnections(oscConnections);
    }
    
    [ObserversRpc]
    private void SendClientConnectionsToMonitor(int clientConnections)
    {
        if(Instances.BuildType != BuildType.Monitor)
            return;
        
        Instances.MonitorUI.SetClientConnections(clientConnections);
    }
    
    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        
        if (IsServer == false)
        {
            FindObjectsOfType<UIWithConnection>().ForEach(uiWithConnection => uiWithConnection.SetConnection(false));
            
            Debug.Log($"Connection lost");
        }
    }
}
