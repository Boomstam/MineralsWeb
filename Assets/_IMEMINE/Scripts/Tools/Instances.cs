using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Instances
{
    public static BuildType BuildType
    {
        get
        {
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.Host)
                return BuildType.Server;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.BayouClient)
                return BuildType.WebGLClient;
            if(Application.isEditor)
                return BuildType.NonWebGLClient;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.TugboatClient)
                return BuildType.OSCClient;
            throw new Exception($"Couldn't find build type for connection {ConnectionTypeHolder.ConnectionType}");
        }
    }
    private static WebGLClientUI _webGLClientUI;
    private static OSCClientUI _oscClientUI;
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
    
    public static OSCClientUI OSCClientUI {
        get
        {
            if (_oscClientUI == null)
                _oscClientUI = Object.FindObjectOfType<OSCClientUI>();

            if (_oscClientUI == null)
                throw new System.Exception($"Couldn't find OSCClientUI!");
            
            return _oscClientUI;
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
