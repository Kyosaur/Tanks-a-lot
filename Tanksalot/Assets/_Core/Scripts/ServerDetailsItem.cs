using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerDetailsItem : MonoBehaviour {

    private GameObject m_ServerDetailsObject;

    [SerializeField] private Text m_Rule;
    [SerializeField] private Text m_Result;


    public void Setup(GameObject serverDetailsObject)
    {
        m_ServerDetailsObject = serverDetailsObject;
    }

    public GameObject GetServerDetailObject()
    {
        return m_ServerDetailsObject;
    }

    public void SetServerRule(string rule)
    {
        m_Rule.text = rule;
    }

	public Text GetServerRule()
    {
        return m_Rule;
    }


    public void SetServerRuleResult(string result)
    {
        m_Result.text = result;
    }

    public Text GetServerRuleResult()
    {
        return m_Result;
    }
}
