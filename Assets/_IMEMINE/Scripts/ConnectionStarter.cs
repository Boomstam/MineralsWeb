using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using UnityEditor;
using UnityEngine;
 
public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private ConnectionType connectionType;
    [SerializeField] private Multipass multipass;
    
    private void Awake()
    {
        if (connectionType == ConnectionType.Host)
        {
            multipass.StartConnection(true);
        }
        else 
        {
            if(connectionType == ConnectionType.TugboatClient)
                multipass.SetClientTransport<Tugboat>();
            else if(connectionType == ConnectionType.BayouClient)
                multipass.SetClientTransport<Bayou>();
            
            multipass.ClientTransport.StartConnection(false);
        }
    }
 
    private void Start()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }
 
    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
 
#if UNITY_EDITOR
        if (args.ConnectionState == LocalConnectionState.Stopping)
            EditorApplication.isPlaying = false;
#endif
    }
    
    public enum ConnectionType
    {
        Host,
        TugboatClient,
        BayouClient,
    }
}
