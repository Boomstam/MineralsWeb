using System;
using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildTypeManager : MonoBehaviour
{
    public bool forceBuildType;
    [ShowIf("forceBuildType")] public BuildType buildTypeToForce;
    
    [SerializeField] private Camera cameraPrefab;
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private PerformanceManager performanceManagerPrefab;
    [SerializeField] private OSCManager oscManagerPrefab;
    [SerializeField] private AudioManager audioManager;
    
    [SerializeField] private MyMessageBroker myMessageBrokerPrefab;
    [SerializeField] private NetworkConnectionManager networkConnectionManagerPrefab;
    [SerializeField] private NetworkOSCManager networkOscManagerPrefab;
    [SerializeField] private NetworkedMonitor networkedMonitorPrefab;
    [SerializeField] private NetworkedVoting networkedVotingPrefab;
    [SerializeField] private NetworkedAppState networkedAppStatePrefab;
    
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private WebGLClientUI webGLClientUI;
    [SerializeField] private OSCClientUI oscClientUI;
    [SerializeField] private MonitorUI monitorUICanvas;
    
    private bool hasConnected;
    
    void Update()
    {
        if (hasConnected == false && (InstanceFinder.IsServer == false || InstanceFinder.ServerManager.Started))
        {
            hasConnected = true;
            
            OnNetworkStarted();
        }
    }
    
    private void OnNetworkStarted()
    {
        Debug.Log($"Instantiate Prefabs, build type {Instances.BuildType}");
        
        if (Instances.BuildType == BuildType.Server)
        {
            NetworkObject networkConnectionManger = Instantiate(networkConnectionManagerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkConnectionManger);
            NetworkObject myMessageBroker = Instantiate(myMessageBrokerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(myMessageBroker);
            NetworkObject performanceManager = Instantiate(performanceManagerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(performanceManager);
            NetworkObject networkOscManager = Instantiate(networkOscManagerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkOscManager);
            NetworkObject networkedMonitor = Instantiate(networkedMonitorPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkedMonitor);
            NetworkObject networkedVoting = Instantiate(networkedVotingPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkedVoting);
            NetworkObject networkedAppState = Instantiate(networkedAppStatePrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkedAppState);
        }
        else
        {
            Instantiate(cameraPrefab);
            Instantiate(eventSystemPrefab);
        }

        if (Instances.BuildType == BuildType.Score)
        {
            Instantiate(scoreUI);
        }
        if (Instances.BuildType == BuildType.Voting)
        {
            Instantiate(webGLClientUI);
            Instantiate(audioManager);
        }
        if (Instances.BuildType == BuildType.OSCClient)
        {
            Instantiate(oscManagerPrefab);
            Instantiate(oscClientUI);
        }
        if (Instances.BuildType == BuildType.Monitor)
        {
            Instantiate(monitorUICanvas);
        }
    }
}