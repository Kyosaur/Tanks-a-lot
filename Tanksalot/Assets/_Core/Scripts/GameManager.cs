using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject m_PlayerObjectPrefab;
    [SerializeField] private string m_SpawnPointTag;


    private GameObject[] m_SpawnPoints;

	// Use this for initialization
	void Awake ()
    {
        m_SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
	}

    public void Start()
    {
        if (m_SpawnPoints != null && m_SpawnPoints.Length > 0)
        {
            Debug.Log("Trying to instantiate...");
            PhotonNetwork.Instantiate(m_PlayerObjectPrefab.name, m_SpawnPoints[0].transform.position, m_SpawnPoints[0].transform.rotation);
 

        }
        else Debug.Log("EMPTY SPAWN ARRAY, DUMMY!!!");
    }
}
