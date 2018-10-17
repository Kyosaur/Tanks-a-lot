using Photon.Realtime;
using UnityEngine;


public class Server {

    [SerializeField] private RoomInfo m_ServerReference;

    [SerializeField] private string m_Name;
    [SerializeField] private string m_Password;

    [SerializeField] private int m_MaxPlayers;
    [SerializeField] private int m_CurrentPlayers;

    [SerializeField] private string m_Map;



    public void Setup (RoomInfo roomInfo, string password = "")
    {
        m_ServerReference = roomInfo;

        m_Name = roomInfo.Name;
        m_Password = password;

        m_MaxPlayers = roomInfo.MaxPlayers;
        m_CurrentPlayers = roomInfo.PlayerCount;

        m_Map = (string) roomInfo.CustomProperties["Map"];
        Debug.Log(m_Map);
        
    }

    
    public string GetGameModeName()
    {
        return (string) m_ServerReference.CustomProperties["Game Mode"];
    }

    public string GetMapName()
    {
        return m_Map;
    }

    public string GetServerName()
    {
        return m_Name;
    }


    public int GetMaxPlayerCount()
    {
        return m_ServerReference.MaxPlayers;
    }


    public int GetPlayerCount()
    {
        return m_ServerReference.PlayerCount;
    }

}
