using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Instances
{
    private static WebGLClientUI _webGLClientUI;
    private static OSCManager _oscManager;
    private static MyMessageBroker _myMessageBroker;

    public static WebGLClientUI WebGLClientUI {
        get
        {
            if (_webGLClientUI == null)
                _webGLClientUI = Object.FindObjectOfType<WebGLClientUI>();

            if (_webGLClientUI == null)
                throw new System.Exception($"Couldn't find WebGLClientUI!");
            
            return _webGLClientUI;
        }
    }
    
    public static OSCManager OSCManager {
        get
        {
            if (_oscManager == null)
                _oscManager = Object.FindObjectOfType<OSCManager>();

            if (_oscManager == null)
                throw new System.Exception($"Couldn't find OSCManager!");
            
            return _oscManager;
        }
    }
    
    public static MyMessageBroker MyMessageBroker {
        get
        {
            if (_myMessageBroker == null)
                _myMessageBroker = Object.FindObjectOfType<MyMessageBroker>();

            if (_myMessageBroker == null)
                throw new System.Exception($"Couldn't find MyMessageBroker!");
            
            return _myMessageBroker;
        }
    }
}
