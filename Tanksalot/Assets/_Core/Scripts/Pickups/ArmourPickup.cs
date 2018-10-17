using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourPickup : MonoBehaviourPun {

    public float m_ArmourAmount = 50f;

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerStay(Collider collision)
    {

        Damagable obj = collision.gameObject.GetComponentInChildren<Damagable>();

        if (obj != null)
        {
            if (obj.m_Armour < obj.GetInitialArmour())
            {
                obj.m_Armour += m_ArmourAmount;
                obj.ForceHealthUpdate();

                this.photonView.RPC("ArmourPickupDestroy", RpcTarget.MasterClient);

            }
        }

    }

    [PunRPC]
    public void ArmourPickupDestroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

}
