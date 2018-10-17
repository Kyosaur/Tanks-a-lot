
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Player : MonoBehaviourPun
{
    [SerializeField]
    private GameObject m_PlayerTankPrefab;

    [System.NonSerialized]
    public Tank m_PlayerTank;


    public void Start()
    {

        if (this.photonView.IsMine)
        {
            Debug.Log("PlayerObject::Start -- Spawning my own personal tank");
            PhotonNetwork.Instantiate(m_PlayerTankPrefab.name, this.transform.position, this.transform.rotation);
            Debug.Log("after tank instantiate");
        }

    }

    



	
}
