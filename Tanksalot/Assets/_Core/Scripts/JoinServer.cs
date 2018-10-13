using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class JoinServer : NetworkBehaviour
{

    NetworkManager m_NetworkManager;

    List<ServerListItem> m_ServerList = new List<ServerListItem>();
    List<ServerDetailsItem> m_ServerDetails = new List<ServerDetailsItem>();

    [SerializeField] private Color m_SelectedListItemColor;
    [SerializeField] private Color m_DefaultListItemColor;

    private ServerListItem m_CurrentlySelected;

    [SerializeField] private Transform m_ServerListParent;
    [SerializeField] private GameObject m_ServerListItemPrefab;

    [SerializeField] private Transform m_ServerDetailsParent;
    [SerializeField] private GameObject m_ServerDetailsItemPrefab;

    // Use this for initialization
    void Start()
    {
        m_NetworkManager = NetworkManager.singleton;
        if (m_NetworkManager.matchMaker == null)
        {
            m_NetworkManager.StartMatchMaker();
        }

        RefreshServerList();
    }


    void ClearServerDetailsList()
    {
        foreach (ServerDetailsItem item in m_ServerDetails)
        {
            Destroy(item.GetServerDetailObject());
        }
        m_ServerDetails.Clear();
    }

    void ClearServerList()
    {
        foreach(ServerListItem server in m_ServerList)
        {
            Destroy(server.GetListObject());
        }
        m_ServerList.Clear();
    }

    public void RefreshServerList()
    {
        ClearServerList();
        ClearServerDetailsList();
        m_NetworkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
    }


    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        bool executedOnce = false;

        if(matches != null)
        {
            foreach(MatchInfoSnapshot server in matches)
            {
                GameObject serverListItem = Instantiate(m_ServerListItemPrefab);
                serverListItem.transform.SetParent(m_ServerListParent, false);

                ServerListItem sli = serverListItem.GetComponent<ServerListItem>();
                if(sli != null)
                {
                    sli.Setup(server, serverListItem, OnServerSelected);

                    if (executedOnce == false)
                    {
                        AddAllServerDetails(sli.GetServer());
                        executedOnce = true;

                    }
                }
                m_ServerList.Add(sli);
            }
        }
    }

    public void OnClickJoin()
    {
        if (m_CurrentlySelected != null)
        {
            Debug.Log("CurrentlySelected not null");
            if (m_NetworkManager == null) Debug.Log("network null stupid");
            if (m_CurrentlySelected.GetServer() == null) Debug.Log("GetServer NULL");
       

            m_NetworkManager.matchMaker.JoinMatch((NetworkID) m_CurrentlySelected.GetServer().GetServerID(), "", "", "", 0, 0, m_NetworkManager.OnMatchJoined);
        }
    }

    public void AddAllServerDetails(Server s)
    {
        AddServerDetail("Name:", s.GetServerName());
        AddServerDetail("", "");
        AddServerDetail("Map:", s.GetMapName());
        AddServerDetail("Player Count:", s.GetPlayerCount().ToString());
        AddServerDetail("Max Players:", s.GetMaxPlayerCount().ToString());
    }

    private void AddServerDetail(string rule, string result)
    {
        GameObject serverDetailsItem = Instantiate(m_ServerDetailsItemPrefab);

        ServerDetailsItem sdi = serverDetailsItem.GetComponent<ServerDetailsItem>();
        if (sdi != null)
        {
            sdi.Setup(serverDetailsItem);

            sdi.SetServerRule(rule);
            sdi.SetServerRuleResult(result);


            m_ServerDetails.Add(sdi);
        }

        serverDetailsItem.transform.SetParent(m_ServerDetailsParent, false);

    }

    public void OnServerSelected(ServerListItem selected)
    {
        Debug.Log("OnServerSelected");

        if(m_CurrentlySelected != null)
        {
            ColorServerListItem(m_CurrentlySelected, m_DefaultListItemColor);
        }

        m_CurrentlySelected = selected;

        ColorServerListItem(m_CurrentlySelected, m_SelectedListItemColor);

        ClearServerDetailsList();
        AddAllServerDetails(m_CurrentlySelected.GetServer());
        
    }

    public void ColorServerListItem(ServerListItem selected, Color color)
    {
        if (selected.GetListObject() != null)
        {
            Image selectedServer = selected.GetListObject().GetComponent<Image>();
            if (selectedServer != null)
            {
                selectedServer.color = color;
            }
        }
    }

}
