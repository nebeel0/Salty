using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public GameMaster gameMaster;

    public static string Online = "online";
    public static string Local = "local";

    //Only support local, but for the sake of testing the UI I will add two.
    public static List<string> supportedNetworkTypes = new List<string>()
    {
        Local,
        Online
    };

    public string currentNetworkType;


    void Start()
    {
        if(currentNetworkType == null)
        {
            currentNetworkType = Local;
        }
    }

}
