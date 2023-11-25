using System;
using System.Collections;
using System.Collections.Generic;
using extOSC;
using extOSC.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestOSC : MonoBehaviour
{
    [SerializeField] private SimpleMessageTransmitter simpleMessageTransmitter;
    [SerializeField] private string address = "/example/2";
    // [SerializeField] private string message = "Hi, globe!";
    
    
    [Button]
    public void SendOSCMessage()
    {
        var message = new OSCMessage(address);
        message.AddValue(OSCValue.String("Hi, globe!"));
        
        simpleMessageTransmitter.Transmitter.Send(message);
    }
}
