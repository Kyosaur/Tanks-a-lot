using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    public float m_HealAmount = 50f;
    

	// Use this for initialization
	void Start () {
		
	}


    private void OnTriggerStay(Collider collision)
    {

        Damagable obj = collision.gameObject.GetComponentInChildren<Damagable>();

        if (obj != null)
        {
            if(obj.m_Health < obj.GetInitialHealth())
            {
                obj.m_Health += m_HealAmount;
                obj.ForceHealthUpdate();

                this.gameObject.SetActive(false);
            }
        }
        
    }


}
