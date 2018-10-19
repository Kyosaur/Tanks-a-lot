using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject m_PlayerObjectPrefab;
    [SerializeField] private string m_SpawnPointTag;
    [SerializeField] private string m_SceneObjectTag;


    private GameObject[] m_SpawnPoints;
    private GameObject[] m_SceneObjects;

	// Use this for initialization
	void Awake ()
    {
        m_SpawnPoints = GameObject.FindGameObjectsWithTag(m_SpawnPointTag);
        m_SceneObjects = GameObject.FindGameObjectsWithTag(m_SceneObjectTag);

	}

    public void Start()
    {

        if (m_SceneObjects != null)
        {
            foreach(GameObject obj in m_SceneObjects)
            {

                if (PhotonNetwork.IsMasterClient)
                {
                    string name = obj.name.Split(' ')[0]; //Grab the object name, and make sure it doesnt have a number attached to it (Ex: "Health (1)").
                    PhotonNetwork.InstantiateSceneObject(name, obj.transform.position, obj.transform.rotation); //Instantiate said object (prefab has a photonView).
                }
                obj.gameObject.SetActive(false);

            }

        }

        if (m_SpawnPoints != null && m_SpawnPoints.Length > 0)
        {
            Debug.Log("Trying to instantiate...");
            PhotonNetwork.Instantiate(m_PlayerObjectPrefab.name, m_SpawnPoints[0].transform.position, m_SpawnPoints[0].transform.rotation);
 

        }
        else Debug.Log("EMPTY SPAWN ARRAY, DUMMY!!!");
    }


}
