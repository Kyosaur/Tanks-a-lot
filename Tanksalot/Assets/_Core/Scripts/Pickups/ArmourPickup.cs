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
            if (obj.GetArmour() < obj.GetInitialArmour())
            {
                obj.SetArmour(obj.GetArmour() + m_ArmourAmount);


                this.photonView.RPC("ArmourPickupDestroy", RpcTarget.AllBuffered);

            }
        }

    }

    [PunRPC]
    public void ArmourPickupDestroy()
    {
        this.gameObject.SetActive(false);
    }

}
