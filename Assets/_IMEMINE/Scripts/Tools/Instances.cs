using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Instances
{
    public static int RowNumber { get; set; }
    public static int SeatNumber { get; set; }

    #region Backing Fields

    private static ConnectionStarter _connectionStarter;
    private static BuildTypeManager _buildTypeManager;
    private static PerformanceManager _performanceManager;
    private static OSCManager _oscManager;
    private static AudioManager _audioManager;
    private static ScoreManager _scoreManager;
    
    private static MyMessageBroker _myMessageBroker;
    private static NetworkConnectionManager _networkConnectionManager;
    private static NetworkOSCManager _networkOSCManager;
    private static NetworkedVoting _networkedVoting;
    private static NetworkedMonitor _networkedMonitor;
    private static NetworkedAppState _networkedAppState;
    
    private static WebGLClientUI _webGLClientUI;
    private static OSCClientUI _oscClientUI;
    private static MonitorUI _monitorUI;

    #endregion
    
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
                return BuildType.Score;
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

    #region Forced Local
    
#if UNITY_EDITOR
    private static bool LocalServer => ConnectionStarter.runLocally && (ParrelSync.ClonesManager.IsClone() == false);
    private static bool LocalMonitor => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() 
                                                                     && ParrelSync.ClonesManager.GetArgument() == "clone 0";
    private static bool LocalOSCClient => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() 
                                                                       && ParrelSync.ClonesManager.GetArgument() == "clone 1";
    private static bool LocalWebGLClient => ConnectionStarter.runLocally && ParrelSync.ClonesManager.IsClone() &&
                                            (ParrelSync.ClonesManager.GetArgument() != "clone 0" && ParrelSync.ClonesManager.GetArgument() != "clone 1");
#endif

    #endregion

    #region Main Scripts

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

    #endregion

    #region Network

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
    
    public static NetworkedMonitor NetworkedMonitor {
        get
        {
            if (_networkedMonitor == null)
                _networkedMonitor = Object.FindObjectOfType<NetworkedMonitor>();
            
            if (_networkedMonitor == null)
                throw new System.Exception($"Couldn't find NetworkedMonitor!");
            
            return _networkedMonitor;
        }
    }
    
    public static NetworkedVoting NetworkedVoting {
        get
        {
            if (_networkedVoting == null)
                _networkedVoting = Object.FindObjectOfType<NetworkedVoting>();
            
            if (_networkedVoting == null)
                throw new System.Exception($"Couldn't find NetworkedVoting!");
            
            return _networkedVoting;
        }
    }
    
    public static NetworkedAppState NetworkedAppState {
        get
        {
            if (_networkedAppState == null)
                _networkedAppState = Object.FindObjectOfType<NetworkedAppState>();
            
            if (_networkedAppState == null)
                throw new System.Exception($"Couldn't find NetworkedAppState!");
            
            return _networkedAppState;
        }
    }
    
    #endregion

    #region UI
    
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

    #endregion
}
