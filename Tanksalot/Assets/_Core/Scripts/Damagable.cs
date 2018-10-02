using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


//Note:  Script requires objects to have a Collider for healthbar positioning (Since its damagable, it SHOULD have Collider anyways).


public enum HealthDisplayOptions
{
    AlwaysOn = 0,
    ShowWhenDamaged,
    NeverShow
}


public class Damagable : NetworkBehaviour
{

    [Header("--Health Stats--")]

    [SyncVar]
    public float m_Armour = 0.0f;

    [SyncVar]
    public float m_Health = 100f;

    [Space(10)]
    public HealthDisplayOptions m_HealthbarDisplay;

    public Canvas m_ArmourbarSlider;
    public Canvas m_HealthbarSlider;  //This is a canvas, but is named "Slider" for the editor's inspector (makes it a bit easier to understand).

    private Canvas m_ArmourbarInstance;
    private Canvas m_HealthbarInstance;

    private float m_InitialArmour;
    private float m_InitialHealth;

    private float m_HideHealthTime = 1.7f;  //Time until we hide our healthbars (if HealthDisplayOptions == ShowWhenDamaged) in seconds
    
    protected virtual void Start()
    {
        m_InitialArmour = m_Armour;
        m_InitialHealth = m_Health;


        if (m_HealthbarSlider != null) //make sure that m_HealthbarSlider is specified in the Editor's Inspector
        {
            //Instantiate a copy of our Healthbar prefab, and set its parent to the attached object (for local positioning).
            m_HealthbarInstance = Instantiate(m_HealthbarSlider);
            m_HealthbarInstance.transform.SetParent(this.transform, false);

            //Set the healthbars initial value (100% full).
            m_HealthbarInstance.GetComponentInChildren<Slider>().value = m_Health / m_InitialHealth;

            //Set the healthbar's  Y  position above object (based on the collider attached).
            // float offset = this.gameObject.GetComponentInChildren<Collider>().bounds.size.y + 1;
            Collider c = GetHighestCollider();
           float offset = (c != null) ? c.bounds.size.y + 1 : this.gameObject.GetComponentInChildren<Collider>().bounds.size.y + 1;

            m_HealthbarInstance.transform.position += new Vector3(0, offset, 0);

            //If the healthbar isnt always displayed, we want to hide it.
            if(m_HealthbarDisplay != HealthDisplayOptions.AlwaysOn) m_HealthbarInstance.gameObject.SetActive(false);
            
        }

        //same as above
        if (m_ArmourbarSlider != null)
        {
            m_ArmourbarInstance = Instantiate(m_ArmourbarSlider);
            m_ArmourbarInstance.transform.SetParent(m_HealthbarInstance.transform, false);

           
            m_ArmourbarInstance.GetComponentInChildren<Slider>().value = m_Armour / m_InitialArmour;

            float offset = 0;
            if(m_HealthbarInstance != null)
            {
                offset = m_HealthbarInstance.GetComponentInChildren<RectTransform>().rect.height;
            }
            m_ArmourbarInstance.transform.position += new Vector3(0, offset, 0);

            if (m_HealthbarDisplay != HealthDisplayOptions.AlwaysOn || m_InitialArmour <= 0) m_ArmourbarInstance.gameObject.SetActive(false);

        }

    }

    public void ForceHealthUpdate() //ADDED for health/armour pickups...so we can show increased health when ShowWhenDamaged is enabled
    {
        if(m_HealthbarDisplay == HealthDisplayOptions.ShowWhenDamaged)
        {
            if (m_HealthbarInstance != null) m_HealthbarInstance.gameObject.SetActive(true);
            if (m_ArmourbarInstance != null && m_Armour > 0) m_ArmourbarInstance.gameObject.SetActive(true);

            CancelInvoke("HideHealthbars");
            Invoke("HideHealthbars", m_HideHealthTime);
        }


        if (m_ArmourbarInstance != null) m_ArmourbarInstance.GetComponentInChildren<Slider>().value = m_Armour / m_InitialArmour;
        if (m_HealthbarInstance != null) m_HealthbarInstance.GetComponentInChildren<Slider>().value = m_Health / m_InitialHealth;
    }

    public virtual void OnCollisionEnter(Collision col)
    {
        DamageSource source = col.gameObject.GetComponent<DamageSource>();

        if ( source != null)
        {

            if (m_Armour <= 0) //do they have NO armour?
            {
                m_Health = Mathf.Clamp(m_Health - source.m_Damage, 0, m_Health); // To avoid negatives...lock new health value to 0 minumum and the current m_Health value for max. 
            }
            else if (m_Armour >= source.m_Damage) //If they have Armour, check if the damage doesn't exceed the armour
            {
                m_Armour = Mathf.Clamp(m_Armour - source.m_Damage, 0, m_Armour); //Dummy proof this, so negative values of DamageSource do not increase armour (negative minus a negative is a positive).
            }
            else //if the damage exceeds armour value, continue damage into health.
            {
                float difference = source.m_Damage - m_Armour;

                m_Armour = 0;
                m_Health = Mathf.Clamp(m_Health - difference, 0, m_Health); //Same as above...lock to 0 minimum and the current health value maximum.
            }


            //Update healthbar percentage 
            if(m_ArmourbarInstance != null) m_ArmourbarInstance.GetComponentInChildren<Slider>().value = m_Armour / m_InitialArmour;
            if(m_HealthbarInstance != null) m_HealthbarInstance.GetComponentInChildren<Slider>().value = m_Health / m_InitialHealth;


            //check to see if armour is 0...if so, hide armour bar
            if (m_Armour <= 0) 
            {
                if (m_ArmourbarInstance != null) m_ArmourbarInstance.gameObject.SetActive(false);
            }

            //destroy object if it has no health
            if (m_Health <= 0) DestroyObject();

            //If the healthbar display setting is "Show when damaged", show the healthbar, and then set a "timer" to hide the healthbar again.
            if(m_HealthbarDisplay == HealthDisplayOptions.ShowWhenDamaged)
            {
                if (m_HealthbarInstance != null) m_HealthbarInstance.gameObject.SetActive(true);
                if(m_ArmourbarInstance != null && m_Armour > 0) m_ArmourbarInstance.gameObject.SetActive(true);

                CancelInvoke("HideHealthbars"); 
                Invoke("HideHealthbars", m_HideHealthTime);
            }
         
        }
    }


    public void HideHealthbars()
    {
        if (m_HealthbarInstance != null) m_HealthbarInstance.gameObject.SetActive(false);
        if (m_ArmourbarInstance != null) m_ArmourbarInstance.gameObject.SetActive(false);

    }

    public virtual void DestroyObject()
    {
        HideHealthbars();
        Destroy(this.gameObject, 0.05f);
    }

    public float GetInitialHealth()
    {
        return m_InitialHealth;
    }

    public float GetInitialArmour()
    {
        return m_InitialArmour;
    }


    public Collider GetHighestCollider()
    {
        float highest = -5f;
        Collider col = null;

        foreach (Collider c in transform.root.gameObject.GetComponentsInChildren<Collider>())
        {
            if (c != null)
            {
                if (c.bounds.size.y > highest)
                {
                    highest = c.bounds.size.y;
                    col = c;
                }
            }
        }
        return col;
    }


}
