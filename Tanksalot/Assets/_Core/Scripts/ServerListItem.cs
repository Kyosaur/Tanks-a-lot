using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Match;
using UnityEngine.UI;



public class ServerListItem : MonoBehaviour
{  
    [SerializeField] private Text m_ServerListItemText;

    public delegate void SelectServerDelegate(ServerListItem match);
    private SelectServerDelegate m_OnSelectCallback;


    private GameObject m_ListObject;
    private Server m_Server;


    public void Setup(MatchInfoSnapshot match, GameObject listObject, SelectServerDelegate onSelectCallback)
    {
        m_OnSelectCallback = onSelectCallback;
        m_ListObject = listObject;

        Debug.Log("SLI setup:: match net id = " + match.networkId);
        m_Server = null;// JsonUtility.FromJson < Server > (GameManager.Instance.CmdGetServer(match.networkId));
    

        Debug.Log("SLI m_server = " + match.networkId);

        m_ServerListItemText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void OnClicked()
    {
        m_OnSelectCallback.Invoke(this);
    }



    public GameObject GetListObject()
    {
        return m_ListObject;
    }

    public Server GetServer()
    {
        return m_Server;
    }

	
}
