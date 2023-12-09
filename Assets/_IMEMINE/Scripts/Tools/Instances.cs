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
            if (LocalServer)
                return BuildType.Server;
            if (LocalWebGLClient)
                return BuildType.WebGLClient;
            if (LocalOSCClient)
                return BuildType.OSCClient;
            
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.Host)
                return BuildType.Server;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.BayouClient)
                return BuildType.WebGLClient;
            if(Application.isEditor)
                return BuildType.WebGLClient;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.TugboatClient)
                return BuildType.OSCClient;
            throw new Exception($"Couldn't find build type for connection {ConnectionTypeHolder.ConnectionType}");
        }
    }

    public static bool LocalServer => ConnectionStarter.localServer;
    public static bool LocalWebGLClient => ConnectionStarter.localWebGLClient;
    public static bool LocalOSCClient => ConnectionStarter.localOSCClient;
    
    private static ConnectionStarter _connectionStarter;
    private static WebGLClientUI _webGLClientUI;
    private static OSCClientUI _oscClientUI;
    private static MonitorUI _monitorUI;
    private static PerformanceManager _performanceManager;
    private static OSCManager _oscManager;
    private static MyMessageBroker _myMessageBroker;

    public static ConnectionStarter ConnectionStarter {
        get
        {
            if (_connectionStarter == null)
                _connectionStarter = Object.FindObjectOfType<ConnectionStarter>();

            if (_connectionStarter == null)
                throw new System.Exception($"Couldn't find ConnectionStarter!");
            
            return _connectionStarter;
        }
    }
    
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
    
    public static MonitorUI MonitorUI {
        get
        {
            if (_monitorUI == null)
                _monitorUI = Object.FindObjectOfType<MonitorUI>();

            if (_monitorUI == null)
                throw new System.Exception($"Couldn't find MonitorUI!");
            
            return _monitorUI;
        }
    }
    
    public static PerformanceManager PerformanceManager {
        get
        {
            if (_performanceManager == null)
                _performanceManager = Object.FindObjectOfType<PerformanceManager>();

            if (_performanceManager == null)
                throw new System.Exception($"Couldn't find PerformanceManager!");
            
            return _performanceManager;
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
