using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


public class CreateServer : MonoBehaviour
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
        if (NetworkManager.singleton.matchMaker == null) NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.CreateMatch(m_ServerName, 10, true, m_ServerPassword, "", "", 0, 0, this.OnMatchCreate);
    }


    public void OnServerNameChange(string name)
    {
        m_ServerName = name;
    }

    public void OnPasswordChange(string password)
    {
        m_ServerPassword = password;
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("success");

            Server s = new Server();
            s.Setup(matchInfo.networkId, m_ServerName, m_ServerPassword);

            s.SetMapName(GetSelectedServerRuleOption(MAP_RULE_TEXT));
            s.SetMaxPlayerCount(int.Parse(GetSelectedServerRuleOption(MAXPLAYERS_RULE_TEXT)));

          //  GameManager.Instance.RegisterServer((ulong) matchInfo.networkId, s.SaveToString());

        }
        else Debug.Log("nope");
        NetworkManager.singleton.OnMatchCreate(success, extendedInfo, matchInfo);
    }
}
