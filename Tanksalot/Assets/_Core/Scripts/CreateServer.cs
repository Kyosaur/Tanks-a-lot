using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


public class CreateServer : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private Transform m_ServerRuleListParent;
    [SerializeField] private GameObject m_ServerCreateRulesPrefab;

    private string m_ServerName = "";
    private string m_ServerPassword = "";

    List<ServerRuleOptions> m_ServerRules;

    const string GAMEMODE_RULE_TEXT = "Game Mode";
    const string MAXPLAYERS_RULE_TEXT = "Max Players";
    const string MAP_RULE_TEXT = "Map";

    private void Start()
    {
        m_ServerRules = new List<ServerRuleOptions>();
        AddAllServerRuleOptions();

    }


    private void AddAllServerRuleOptions()
    {
        AddServerRuleOption(GAMEMODE_RULE_TEXT, "Death Match", "Capture the Flag", "Team Death Match");
        AddServerRuleOption(MAXPLAYERS_RULE_TEXT,"2 ", "4", "6", "8");
        AddServerRuleOption(MAP_RULE_TEXT, "Forest", "Example", "Example 2");

    }


    private void AddServerRuleOption(string rule, params string[] options)
    {
        List<string> optionList = new List<string>();

        foreach(string arg in options)
        {
            optionList.Add(arg);
        }

        GameObject serverRuleOptionsItem = Instantiate(m_ServerCreateRulesPrefab);

        ServerRuleOptions sro = serverRuleOptionsItem.GetComponent<ServerRuleOptions>();
        if(sro != null)
        {
            sro.Setup(serverRuleOptionsItem, rule, optionList);
            m_ServerRules.Add(sro);
        }

        serverRuleOptionsItem.transform.SetParent(m_ServerRuleListParent, false);
    }

    private string GetSelectedServerRuleOption(string rule)
    {
        foreach(ServerRuleOptions options in m_ServerRules)
        {
            if (options.GetServerRule() == rule) return options.GetSelectedRuleOption();
        }
        return null;
    }

    public void CreateNewServer()
    {
        Debug.Log("create new server");
        if(!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();

    }

    public void OnServerNameChange(string name)
    {
        m_ServerName = name;
    }

    public void OnPasswordChange(string password)
    {
        m_ServerPassword = password;
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("OnConnectedToMaster() was called by PUN....creating room");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;

        //Properties visible INSIDE the game.
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { GAMEMODE_RULE_TEXT, GetSelectedServerRuleOption(GAMEMODE_RULE_TEXT) },
            { MAP_RULE_TEXT, GetSelectedServerRuleOption(MAP_RULE_TEXT) }
        };

        //Properties visible OUTSIDE the game
        roomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            GAMEMODE_RULE_TEXT ,
            MAP_RULE_TEXT 
        };

        PhotonNetwork.CreateRoom(m_ServerName, roomOptions, TypedLobby.Default);
       

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("Room created!");
        PhotonNetwork.LoadLevel("Forest");
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        Debug.Log("FAILED TO CREATE ROOM! Error Code: " + returnCode + " | Message: " + message);
    }
}
