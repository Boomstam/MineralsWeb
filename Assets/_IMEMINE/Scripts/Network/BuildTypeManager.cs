using System;
using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildTypeManager : MonoBehaviour
{
    public bool isScoreApp;
    
    [SerializeField] private Camera cameraPrefab;
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private NetworkConnectionManager networkConnectionManagerPrefab;
    [SerializeField] private MyMessageBroker myMessageBrokerPrefab;
    [SerializeField] private PerformanceManager performanceManagerPrefab;
    [SerializeField] private NetworkOSCManager networkOscManagerPrefab;
    [SerializeField] private OSCManager oscManagerPrefab;
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private WebGLClientUI webGLClientUI;
    [SerializeField] private OSCClientUI oscClientUI;
    [SerializeField] private MonitorUI monitorUICanvas;
    [SerializeField] private AudioManager audioManager;
    
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
            NetworkObject networkOSCManager = Instantiate(networkOscManagerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(networkOSCManager);
        }
        else
        {
            Instantiate(cameraPrefab);
            Instantiate(eventSystemPrefab);
        }

        if (Instances.BuildType == BuildType.Monitor)
        {
            if (isScoreApp)
            {
                Instantiate(scoreUI);
            }
            else
            {
                Instantiate(monitorUICanvas);  
            }
        }
        if (Instances.BuildType == BuildType.WebGLClient)
        {
            if (isScoreApp)
            {
                Instantiate(scoreUI);
            }
            else
            {
                Instantiate(webGLClientUI);
                Instantiate(audioManager);   
            }
        }
        if (Instances.BuildType == BuildType.OSCClient)
        {
            Instantiate(oscManagerPrefab);
            Instantiate(oscClientUI);
        }
    }
}
