using System;
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
    public bool localServer;
    public bool localClient;

    [SerializeField] private Multipass multipass;
    [SerializeField] private Tugboat tugboat;
    [SerializeField] private Bayou bayou;

    [SerializeField] private ushort clientBayouPort = 443;
    public ushort serverBayouPort = 7777;
    public string playflowToken = "1317dd7cadb3232d22e7eb710c4c85f7";
    
    private void Awake()
    {
        if (localServer || localClient)
        {
            if (localServer) StartLocalServer();
            if (localClient) StartLocalClient();
            
            Debug.Log($"Return, Local server ({localServer}) or local client ({localClient})");
            
            return;
        }
        
        ConnectionType connectionType = ConnectionTypeHolder.ConnectionType;
        Debug.Log($"Awake with connection type {connectionType}");

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
                Debug.Log(ConnectionTypeHolder.ActiveServer);
                // FindObjectOfType<TextMeshProUGUI>().text = ConnectionTypeHolder.ActiveServer;
                tugboat.SetClientAddress(ConnectionTypeHolder.ActiveServer);
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
    private void StartLocalServer()
    {
        multipass.SetClientTransport<Tugboat>();
        multipass.StartConnection(true);
    }
    
    [Button]
    private void StartLocalClient()
    {
        multipass.SetClientTransport<Tugboat>();
        multipass.StartConnection(false);
    }
    
    public enum ConnectionType
    {
        Host,
        BayouClient,
        TugboatClient,
    }
}
