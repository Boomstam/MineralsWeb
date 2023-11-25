using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using FishNet.Transporting.Bayou;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
 
public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private Multipass multipass;
    // [SerializeField] private Tugboat tugboat;
    [SerializeField] private Bayou bayou;
    
    [SerializeField] private ushort clientBayouPort = 443;
    public ushort serverBayouPort = 7777;
    public string playflowToken = "1317dd7cadb3232d22e7eb710c4c85f7";
    
    private void Awake()
    {
        // BuildTypeSO buildTypeSo = Resources.Load<BuildTypeSO>("BuildTypeSO");
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
                multipass.SetClientTransport<Tugboat>();
            }
            else if(connectionType == ConnectionType.BayouClient)
            {
                Debug.Log($"Bayou client");
                multipass.SetClientTransport<Bayou>();
                
                bayou.SetPort(clientBayouPort);
                bayou.SetUseWSS(true);
            }
            
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
        BayouClient,
        TugboatClient,
    }
}
