using System;
using System.Collections;
using System.Collections.Generic;
using extOSC;
using extOSC.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

public class OSCManager : MonoBehaviour
{
    [SerializeField] private SimpleMessageTransmitter simpleMessageTransmitter;
    [SerializeField] private string testAddress = "/example/2";
    [SerializeField] private string testMessage = "Hi, globe!";
    
    [Button]
    public void SendTestMessage()
    {
        var oscMessage = new OSCMessage(testAddress);
        oscMessage.AddValue(OSCValue.String(testMessage));
        
        simpleMessageTransmitter.Transmitter.Send(oscMessage);
    }
    
    [Button]
    public void SendOSCMessage(string address, string message)
    {
        var oscMessage = new OSCMessage(address);
        oscMessage.AddValue(OSCValue.String(message));
        
        simpleMessageTransmitter.Transmitter.Send(oscMessage);
    }
}
