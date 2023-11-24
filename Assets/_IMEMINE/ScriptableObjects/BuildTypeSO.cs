using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildTypeSO", menuName = "Minerals/BuildTypeSO")]
public class BuildTypeSO : ScriptableObject
{
    public ConnectionStarter.ConnectionType ConnectionType;
}
