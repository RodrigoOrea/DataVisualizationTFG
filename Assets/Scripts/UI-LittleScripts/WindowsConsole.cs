using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class WindowsConsole : MonoBehaviour
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        AllocConsole();
        Debug.Log("Consola de Windows inicializada");
#endif
    }
}
