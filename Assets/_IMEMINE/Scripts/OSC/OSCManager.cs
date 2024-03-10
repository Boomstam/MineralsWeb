using OscJack;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class OSCManager : MonoBehaviour
{
    [SerializeField] private string testAddress = "/example/2";
    [SerializeField] private string testMessage = "Hi, globe!";
    
    public static string IPAddress;
    public static int Port;
    
    public const string IPPlayerPrefsKey = "IP Address";
    public const string PortPlayerPrefsKey = "Port";
    
    private OscClient oscClient;
    
    private string lastIP;
    private int lastPort;
    
    private void InitOSCClientIfNeeded()
    {
        if(oscClient == null || lastIP != IPAddress || Port != lastPort)
        {
            oscClient = new OscClient(IPAddress, Port);
            
            lastIP = IPAddress;
            lastPort = Port;
            
            PlayerPrefs.SetString(IPPlayerPrefsKey, IPAddress);
            PlayerPrefs.SetInt(PortPlayerPrefsKey, Port);
            
            PlayerPrefs.Save();
        }
    }
    
    [Button]
    public void SendTestMessage()
    {
        InitOSCClientIfNeeded();
        
        oscClient.Send(testAddress, testMessage);
    }
    
    [Button]
    public void SendOSCMessage(string address, string message)
    {
        InitOSCClientIfNeeded();
        
        oscClient.Send(address, message);
        
        /*var oscMessage = new OSCMessage(address);
        oscMessage.AddValue(OSCValue.String(message));
        
        simpleMessageTransmitter.Transmitter.Send(oscMessage);*/
    }
}
