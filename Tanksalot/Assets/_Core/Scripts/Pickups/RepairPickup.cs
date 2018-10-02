﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPickup : MonoBehaviour {

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


            if (obj.m_Health < health || obj.m_Armour < armour)
            {
                obj.m_Health = health;
                obj.m_Armour = armour;

                obj.ForceHealthUpdate();

                this.gameObject.SetActive(false);
            }
        }

    }

}