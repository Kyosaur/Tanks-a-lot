using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinServer : MonoBehaviourPunCallbacks
{

    List<ServerListItem> m_ServerList = new List<ServerListItem>();
    List<ServerDetailsItem> m_ServerDetails = new List<ServerDetailsItem>();

    [SerializeField] private Color m_SelectedListItemColor;
    [SerializeField] private Color m_DefaultListItemColor;

    private ServerListItem m_CurrentlySelected;

    [SerializeField] private Transform m_ServerListParent;
    [SerializeField] private GameObject m_ServerListItemPrefab;

    [SerializeField] private Transform m_ServerDetailsParent;
    [SerializeField] private GameObject m_ServerDetailsItemPrefab;

    private Dictionary<string, RoomInfo> m_CachedRoomList;

    private void Awake()
    {
        m_CachedRoomList = new Dictionary<string, RoomInfo>();
    }

    // Use this for initialization
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
            
        //RefreshServerList();
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

        bool executedOnce = false;

        if ( m_CachedRoomList != null)
        {
            foreach (RoomInfo server in m_CachedRoomList.Values)
            {
                GameObject serverListItem = Instantiate(m_ServerListItemPrefab);
                serverListItem.transform.SetParent(m_ServerListParent, false);

                ServerListItem sli = serverListItem.GetComponent<ServerListItem>();
                if (sli != null)
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

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
        
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Invoke("RefreshServerList", 0.3f);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room JOINED!");

        PhotonNetwork.LoadLevel("Forest");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Room NOT joined. Error #: " + returnCode + "  Message: " + message);
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        UpdateCachedRoomList(roomList);

    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (m_CachedRoomList.ContainsKey(info.Name))
                {
                    m_CachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (m_CachedRoomList.ContainsKey(info.Name))
            {
                m_CachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                m_CachedRoomList.Add(info.Name, info);
            }
        }
    }


    public void OnClickJoin()
    {
        if (m_CurrentlySelected != null)
        {
            Debug.Log("CurrentlySelected not null");
            if (m_CurrentlySelected.GetServer() == null) Debug.Log("GetServer NULL");

            PhotonNetwork.JoinRoom(m_CurrentlySelected.GetServer().GetServerName());
        }
    }

    public void AddAllServerDetails(Server s)
    {
        AddServerDetail("Name:", s.GetServerName()); //.Trim().Substring(0, 25));

        AddServerDetail("", "");
        AddServerDetail("Mode:", s.GetGameModeName());
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
