using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Global
{
    public List<string> playerList;

    private static Global instance = null;
    public static Global GetInstance()
    {
        if(null == instance)
        {
            instance = new Global();
        }
        return instance;
    }

    public Global()
    {
        playerList = PlayerPrefs.GetString("Players").Split(' ').ToList();
    }

    public void SavePlayerList()
    {
        
    }
}
