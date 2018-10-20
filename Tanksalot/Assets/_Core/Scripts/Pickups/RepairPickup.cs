using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPickup : MonoBehaviourPun {

    [SerializeField] private float m_HealAmount = 100f;
    [SerializeField] private float m_ArmourAmount = 100f;

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerStay(Collider collision)
    {

        Damagable obj = collision.gameObject.GetComponentInChildren<Damagable>();

        if (obj != null)
        {
            float armour = obj.GetInitialArmour();
            float health = obj.GetInitialHealth();


            if (obj.GetHealth() < health || obj.GetArmour() < armour)
            {
                obj.SetHealth(obj.GetInitialHealth());
                obj.SetArmour(obj.GetInitialArmour());


                this.photonView.RPC("RepairPickupDestroy", RpcTarget.AllBuffered);
            }
        }

    }

    [PunRPC]
    public void RepairPickupDestroy()
    {
        // PhotonNetwork.Destroy(this.gameObject);

        this.gameObject.SetActive(false);

    }

}
