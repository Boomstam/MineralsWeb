using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class NetworkOSCManager : NetworkBehaviour
{
    public void OnNextChapter(int chapter)
    {
        Instances.OSCManager.SendOSCMessage("/chapter", $"{chapter}");
    }

    public void OnChoicesChanged(int choice1, int choice2, int choice3, int choice4)
    {
        Instances.OSCManager.SendOSCMessage("/choices", $"{choice1},{choice2}, {choice3}, {choice4}");
    }
}
