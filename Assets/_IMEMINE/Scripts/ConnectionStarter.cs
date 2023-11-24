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
    private void Awake()
    {
        BuildTypeSO buildTypeSo = Resources.Load<BuildTypeSO>("BuildTypeSO");
        ConnectionType connectionType = buildTypeSo.ConnectionType;
        
        if (connectionType == ConnectionType.Host)
        {
            multipass.StartConnection(true);
        }
        else 
        {
            if(connectionType == ConnectionType.TugboatClient)
                multipass.SetClientTransport<Tugboat>();
            else if(connectionType == ConnectionType.BayouClient)
            {
                Debug.Log($"Bayou client");
                multipass.SetClientTransport<Bayou>();
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
        TugboatClient,
        BayouClient,
    }
}
