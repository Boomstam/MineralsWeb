using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class PlayerPrefsExtensions
{
    private static string[] vect3Prefixes =
    {
        "x_",
        "y_",
        "z_",
    };

    #region Set Methods
    
    public static void SetBool(string key, bool val)
    {
        PlayerPrefs.SetInt(key, Convert.ToInt32(val));
        PlayerPrefs.Save();
    }

    public static void SetVector3(string key, Vector3 vect)
    {
        PlayerPrefs.SetFloat(key + vect3Prefixes[0], vect.x);
        PlayerPrefs.SetFloat(key + vect3Prefixes[1], vect.y);
        PlayerPrefs.SetFloat(key + vect3Prefixes[2], vect.z);
        PlayerPrefs.Save();
    }
    
    public static void SetQuaternion(string key, Quaternion quat)
    {
        Vector3 eulers = quat.eulerAngles;
        SetVector3(key, eulers);
        PlayerPrefs.Save();
    }

    #endregion

    #region Get Methods

    public static Vector3 GetVector3(string key)
    {
        float x = PlayerPrefs.GetFloat(key + vect3Prefixes[0]);
        float y = PlayerPrefs.GetFloat(key + vect3Prefixes[1]);
        float z = PlayerPrefs.GetFloat(key + vect3Prefixes[2]);

        return new Vector3(x, y, z);
    }
    
    public static Quaternion GetQuaternion(string key)
    {
        Vector3 eulers = GetVector3(key);
        return Quaternion.Euler(eulers);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        int defaultValueAsInt = Convert.ToInt32(defaultValue);
        
        bool val = Convert.ToBoolean(PlayerPrefs.GetInt(key, defaultValueAsInt));
        return val;
    }
    
    #endregion
}