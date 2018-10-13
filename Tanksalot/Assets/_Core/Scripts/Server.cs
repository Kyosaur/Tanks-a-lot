using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class Server {

    [SerializeField] private ulong m_ServerID;

    [SerializeField] private string m_Name;
    [SerializeField] private string m_Password;

    [SerializeField] private int m_MaxPlayers;
    [SerializeField] private int m_CurrentPlayers;

    [SerializeField] private string m_Map;



    public void Setup (NetworkID id, string name, string password)
    {
        m_ServerID = (ulong) id;

        m_Name = name;
        m_Password = password;
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public ulong GetServerID()
    {
        return m_ServerID;
    }

    public void SetMapName(string map)
    {
        m_Map = map;
    }

    public string GetMapName()
    {
        return m_Map;
    }

    public string GetServerName()
    {
        return m_Name;
    }

    public void SetMaxPlayerCount(int maxPlayers)
    {
        m_MaxPlayers = maxPlayers;
    }

    public int GetMaxPlayerCount()
    {
        return m_MaxPlayers;
    }

    public void SetPlayerCount(int players)
    {
        m_CurrentPlayers = players;
    }

    public int GetPlayerCount()
    {
        return m_CurrentPlayers;
    }

}
