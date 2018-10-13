using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;



public class GameManager : NetworkBehaviour
{

#region Singleton
    private static GameManager m_instance;


    public static GameManager Instance
    {
        get { return m_instance;  }
    }

    private void Awake()
    {
        if(m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Debug.Log("GameManager created instance");
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion



    Dictionary<ulong, Server> m_ServerDictionary = new Dictionary<ulong, Server>();


    public void Start()
    {
        
    }


    public void RegisterServer(ulong id, string serverJson)
    {
        if (!isServer) CmdRegisterServer(id, serverJson);
    }


    [Command]
    public void CmdRegisterServer(ulong id, string serverJson)
    {
        Debug.Log("Registering server (dic size: " + m_ServerDictionary.Count + ")...");
        m_ServerDictionary.Add(id, JsonUtility.FromJson<Server>(serverJson));
        Debug.Log("Added server " + id + "(dic size: " + m_ServerDictionary.Count + ")...");

    }

    [Command]
    public void CmdUnregisterServer(ulong id)
    {
        m_ServerDictionary.Remove(id);
    }


    public void GetServer(ulong id)
    {
        if (!isServer) CmdGetServer(id);
    }

    [Command]
    public void CmdGetServer(ulong id)
    {
        if (m_ServerDictionary == null) Debug.Log("dic is null????");

        if (m_ServerDictionary.ContainsKey(id))
        {
            //return m_ServerDictionary[id].SaveToString();
            Debug.Log(m_ServerDictionary[id].SaveToString());
        }
        else
        {
            Debug.Log("KEY: " + id + " not found. Dumping dictionary (" + m_ServerDictionary.Keys.Count +" KEYS)");
            foreach(NetworkID i in m_ServerDictionary.Keys)
            {
                Debug.Log("ID: " + i);
            }
            //return "";

        }
    }



}
