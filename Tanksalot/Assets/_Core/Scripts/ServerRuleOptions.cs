using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ServerRuleOptions : MonoBehaviour
{
    private GameObject m_ServerRuleOptionObject;

    [SerializeField] private Text m_Rule;
    [SerializeField] private Dropdown m_Options;

    public void Setup(GameObject serverRuleObject)
    {
        m_ServerRuleOptionObject = serverRuleObject;
    }

    public void Setup(GameObject serverRuleObject, string serverName, List<string> serverOptions)
    {
        m_ServerRuleOptionObject = serverRuleObject;

        AddServerRuleOptions(serverName, serverOptions);
    }


    public void AddServerRuleOptions(string rule, List<string> options)
    {
        m_Rule.text = rule;
        m_Options.AddOptions(options);
    }

    public string GetServerRule()
    {
        return m_Rule.text;
    }

    public List<string> GetServerRuleOptions()
    {
        List<string> options = new List<string>();

        foreach(Dropdown.OptionData o in m_Options.options)
        {
            options.Add(o.text);
        }

        return options;
    }

    public string GetSelectedRuleOption()
    {
        int idx = m_Options.value;
        return m_Options.options[idx].text;
    }

}
