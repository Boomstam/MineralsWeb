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
#if UNITY_EDITOR
            if (LocalServer)
                return BuildType.Server;
            if (LocalMonitor)
                return BuildType.Monitor;
            if (LocalOSCClient)
                return BuildType.OSCClient;
            if (LocalWebGLClient)
                return BuildType.Voting;
#endif
            if (BuildTypeManager.forceBuildType)
                return BuildTypeManager.buildTypeToForce;
            
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.Host)
                return BuildType.Server;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.TugboatClient)
                return BuildType.OSCClient;
            if (ConnectionTypeHolder.ConnectionType == ConnectionType.BayouClient)
                return BuildType.Voting;
            if(Application.isEditor)
                return BuildType.Monitor;
            
            throw new Exception($"Couldn't find build type for connection {ConnectionTypeHolder.ConnectionType}");
        }
    }

#if UNITY_EDITOR
    private static bool LocalServer => ConnectionStarter.runLocally && (ParrelSync.ClonesManager.IsClone() == false);
    private static bool LocalMonitor => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() 
                                                                    && ParrelSync.ClonesManager.GetArgument() == "clone 0";
    private static bool LocalOSCClient => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() 
                                                                      && ParrelSync.ClonesManager.GetArgument() == "clone 1";
    private static bool LocalWebGLClient => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() &&
                                           (ParrelSync.ClonesManager.GetArgument() != "clone 0" && ParrelSync.ClonesManager.GetArgument() != "clone 1");
#endif
    
    private static ConnectionStarter _connectionStarter;
    private static WebGLClientUI _webGLClientUI;
    private static OSCClientUI _oscClientUI;
    private static MonitorUI _monitorUI;
    private static NetworkConnectionManager _networkConnectionManager;
    private static PerformanceManager _performanceManager;
    private static NetworkOSCManager _networkOSCManager;
    private static OSCManager _oscManager;
    private static AudioManager _audioManager;
    private static MyMessageBroker _myMessageBroker;
    private static BuildTypeManager _buildTypeManager;
    private static ScoreManager _scoreManager;
    
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
    
    public static NetworkConnectionManager NetworkConnectionManager {
        get
        {
            if (_networkConnectionManager == null)
                _networkConnectionManager = Object.FindObjectOfType<NetworkConnectionManager>();
            
            if (_networkConnectionManager == null)
                throw new System.Exception($"Couldn't find NetworkConnectionManager!");
            
            return _networkConnectionManager;
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
    
    public static NetworkOSCManager NetworkOSCManager {
        get
        {
            if (_networkOSCManager == null)
                _networkOSCManager = Object.FindObjectOfType<NetworkOSCManager>();
            
            if (_networkOSCManager == null)
                throw new System.Exception($"Couldn't find NetworkOSCManager!");
            
            return _networkOSCManager;
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
    
    public static AudioManager AudioManager {
        get
        {
            if (_audioManager == null)
                _audioManager = Object.FindObjectOfType<AudioManager>();

            if (_audioManager == null)
                throw new System.Exception($"Couldn't find AudioManager!");
            
            return _audioManager;
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
    
    public static BuildTypeManager BuildTypeManager {
        get
        {
            if (_buildTypeManager == null)
                _buildTypeManager = Object.FindObjectOfType<BuildTypeManager>();

            if (_buildTypeManager == null)
                throw new System.Exception($"Couldn't find BuildTypeManager!");
            
            return _buildTypeManager;
        }
    }
    
    public static ScoreManager ScoreManager {
        get
        {
            if (_scoreManager == null)
                _scoreManager = Object.FindObjectOfType<ScoreManager>();

            if (_scoreManager == null)
                throw new System.Exception($"Couldn't find ScoreManager!");
            
            return _scoreManager;
        }
    }
}
