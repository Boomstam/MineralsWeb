using System;
using System.Collections;
using System.Reflection;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;

public class ConnectionStarter : MonoBehaviour
{
    public bool runLocally;
    [SerializeField] private Multipass multipass;
    [SerializeField] private Tugboat tugboat;
    [SerializeField] private Bayou bayou;
    [SerializeField] private float reconnectionInterval;

    [SerializeField] private ushort clientBayouPort = 443;
    [SerializeField] private string localClientTugboatPort = "localhost";
    public ushort serverBayouPort = 7777;
    public string playflowToken = "ec27ab23758bce61f2e92807d0a3f2d4";

    private float lastReconnectionTry;
    
    private void Awake()
    {
        if (runLocally)
        {
            Debug.Log($"Local default LOCAL connection");
            
            if(Instances.BuildType == BuildType.Server)
            {
                StartLocalServer();
            }
            else
            {
                StartLocalClient();
            }
            return;
        }

        StartConnection();
    }

    private void Update()
    {
        if (runLocally)
            return;
        
        if(InstanceFinder.ClientManager.Started)
            return;
        
        if (Time.time - lastReconnectionTry > reconnectionInterval)
        {
            StartConnection();
            
            lastReconnectionTry = Time.time;
        }
    }

    [Button]
    private void StartConnection()
    {
        ConnectionType connectionType = ConnectionTypeHolder.ConnectionType;
        Debug.Log($"Start connection with connection type {connectionType}");

        if (connectionType == ConnectionType.Host)
        {
            bayou.SetPort(serverBayouPort);
            bayou.SetUseWSS(false);
            multipass.StartConnection(true);
        }
        else 
        {
            if(connectionType == ConnectionType.TugboatClient)
            {
                Debug.Log(ConnectionTypeHolder.IP);
                // tugboat.SetClientAddress(ConnectionTypeHolder.ActiveServer);
                tugboat.SetClientAddress(ConnectionTypeHolder.IP);
                multipass.SetClientTransport<Tugboat>();
            }
            else if(connectionType == ConnectionType.BayouClient)
            {
                Debug.Log($"Bayou client");
                bayou.SetPort(clientBayouPort);
                bayou.SetUseWSS(true);
                bayou.SetClientAddress(ConnectionTypeHolder.ActiveServer);
                
                Debug.Log($"bayou port: {bayou.GetPort()}");
                Debug.Log($"bayou client address: {bayou.GetClientAddress()}");
                multipass.SetClientTransport<Bayou>();
            }
            
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => { multipass.ClientTransport.StartConnection(false); });
        }
    }
    
    [Button]
    private void StopConnection()
    {
        InstanceFinder.ClientManager.StopConnection();

        StartCoroutine(TryReconnecting());
    }

    private IEnumerator TryReconnecting()
    {
        if (InstanceFinder.ClientManager.Started == false)
        {
            StartConnection();

            yield return new WaitForSeconds(reconnectionInterval);
        }
    }
    
    [Button]
    private void StartLocalServer()
    {
        multipass.SetClientTransport<Tugboat>();
        multipass.StartConnection(true);
    }
    
    [Button]
    private void StartLocalClient()
    {
        multipass.SetClientTransport<Tugboat>();
        multipass.SetClientAddress(localClientTugboatPort);
        multipass.StartConnection(false);
    }
}
