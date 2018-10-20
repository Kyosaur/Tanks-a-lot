using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviourPun {

    public float m_HealAmount = 50f;
    

	// Use this for initialization
	void Start () {
		
	}


    private void OnTriggerStay(Collider collision)
    {

        Damagable obj = collision.gameObject.GetComponentInChildren<Damagable>();

        if (obj != null)
        {
            if(obj.GetHealth() < obj.GetInitialHealth())
            {
                obj.SetHealth(obj.GetHealth() + m_HealAmount);


                this.photonView.RPC("HealthPickupDestroy", RpcTarget.AllBuffered);
            }
        }
        
    }

    [PunRPC]
    public void HealthPickupDestroy()
    {
        this.gameObject.SetActive(false);
    }


}
