using emotitron.NST;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_PlayerTankPrefab;

    [System.NonSerialized]
    public Tank m_PlayerTank;


    public void Start()
    {
  
        if (!isLocalPlayer) return;

        Debug.Log("PlayerObject::Start -- Spawning my own personal tank");

        int netID = (int)GetComponent<NetworkIdentity>().netId.Value;
        GameManager.RegisterPlayer(netID, this);

        Debug.Log("Scene count: " + NetworkManager.singleton.startPositions.Count);

        // Transform spawn = NetworkManager.singleton.GetStartPosition();
        // CmdSpawnPlayer(spawn.position, spawn.rotation);

        
        CmdSpawnPlayer(this.transform.position, this.transform.rotation);
    }




   [Command]
    void CmdSpawnPlayer(Vector3 pos, Quaternion rot)
    {
        GameObject player = Instantiate(m_PlayerTankPrefab);

        player.transform.position = pos;
        player.transform.rotation = rot;

        m_PlayerTank = player.GetComponent<Tank>();
        NetworkServer.SpawnWithClientAuthority(player, connectionToClient);

    }
	
}
