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
    public bool localDefaults;
    public bool localServer;
    public bool localWebGLClient;
    public bool localOSCClient;

    [SerializeField] private Multipass multipass;
    [SerializeField] private Tugboat tugboat;
    [SerializeField] private Bayou bayou;

    [SerializeField] private ushort clientBayouPort = 443;
    [SerializeField] private string localClientTugboatPort = "localhost";
    public ushort serverBayouPort = 7777;
    public string playflowToken = "ec27ab23758bce61f2e92807d0a3f2d4";

    private void Awake()
    {
        if (localDefaults)
        {
            Debug.Log($"Local default LOCAL connection");
            
            if(ParrelSync.ClonesManager.IsClone() == false)
            {
                localServer = true;
                
                StartLocalServer();
            }
            else
            {
                string cloneArgument = ParrelSync.ClonesManager.GetArgument();
                
                if(cloneArgument == "clone 0")
                    localOSCClient = true;
                else
                    localWebGLClient = true;
                
                StartLocalClient();
            }
            return;
        }
        if (localServer || localWebGLClient || localOSCClient)
        {
            if (localServer) StartLocalServer();
            if (localWebGLClient) StartLocalClient();
            if (localOSCClient) StartLocalClient();
            
            Debug.Log($"Start LOCAL connection, local server ({localServer}) or local webGLClient ({localWebGLClient} or local OSCClient ({localOSCClient})");
            
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
                Debug.Log(ConnectionTypeHolder.IP);
                // FindObjectOfType<TextMeshProUGUI>().text = ConnectionTypeHolder.ActiveServer;
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
    } }
