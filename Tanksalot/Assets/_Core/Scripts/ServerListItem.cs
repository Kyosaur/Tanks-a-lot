using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

[System.Serializable]
public class MyIntEvent : UnityEvent<ServerListItem>
{
}


public class ServerListItem : MonoBehaviour {

  
    [SerializeField] private Text m_ServerListItemText;

    public delegate void SelectServerDelegate(ServerListItem match);
    private SelectServerDelegate m_OnSelectCallback;

    MatchInfoSnapshot m_Server;
    GameObject m_ListObject;

    string m_ServerName;
    string m_MapName;

    int m_PlayerCount;
    int m_MaxPlayers;

    public void Setup(MatchInfoSnapshot match, GameObject listObject, SelectServerDelegate onSelectCallback)
    {
        m_OnSelectCallback = onSelectCallback;

        m_Server = match;

        m_MapName = "Forest";

        m_ListObject = listObject;
        m_ServerName = match.name;

        m_PlayerCount = match.currentSize;
        m_MaxPlayers = match.maxSize;

        m_ServerListItemText.text = m_ServerName + " (" + m_PlayerCount + "/" + m_MaxPlayers + ")";
    }

    public void OnClicked()
    {
        m_OnSelectCallback.Invoke(this);
    }


    public int GetMaxPlayerCount()
    {
        if (m_Server != null)
        {
            return m_Server.maxSize;
        }
        return 0;
    }

    public int GetPlayerCount()
    {
        if (m_Server != null)
        {
            return m_Server.currentSize;
        }
        return 0;
    }

    public GameObject GetListObject()
    {
        return m_ListObject;
    }

    public MatchInfoSnapshot GetServer()
    {
        return m_Server;
    }

    public void SetServerName(string name)
    {
        m_ServerName = name;
    }

	public string GetServerName()
    {
        return m_ServerName;
    }

    public void SetMapName(string name)
    {
        m_MapName = name;
    }

    public string GetMapName()
    {
        return m_MapName;
    }
	
}
