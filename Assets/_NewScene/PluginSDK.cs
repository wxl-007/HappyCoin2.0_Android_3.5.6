using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
public class PluginSDK
{
    public static void TakePhoto()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
           _TakePhoto();
#endif
    }
}