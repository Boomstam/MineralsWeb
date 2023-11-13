using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEditor;
using UnityEngine;

public class EditorTest : Editor
{
    make into editor field
        
    [MenuItem("MyMenu/Do")]
    static void Do()
    {
        Debug.Log($"Do");

        EditorWindow.GetWindow<PlayFlowCloudDeploy>().get_status().ToObservable()
            .Subscribe(_ => Debug.Log($"Done {_}"));
    }
}
