using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private AudioSource helloWorld;
    
    private void Start()
    {
        Debug.Log("Start player");
    }
    
    public override void OnStartClient()
    {
        Debug.Log("Start client");
        
        GameObject.Find("STATUS").GetComponent<TextMeshProUGUI>().text = "Start Client";
    }
    
    public override void OnStartServer()
    {
        Debug.Log("Start server");
    }
}
