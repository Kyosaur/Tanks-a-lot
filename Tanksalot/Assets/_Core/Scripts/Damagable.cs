using Photon.Pun;
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


public class Damagable : MonoBehaviourPun, IPunObservable
{

    [Header("--Health Stats--")]
    private bool m_IsInitialized = false;

    [SerializeField] private float m_InitialArmour = 0f;
    [SerializeField] private float m_InitialHealth = 100f;

    [Space(10)]
    [SerializeField] private float m_Armour;
    [SerializeField] private float m_Health;

    [Space(10)]
    [SerializeField] private HealthDisplayOptions m_HealthbarDisplay;

    [SerializeField] private Canvas m_ArmourbarSlider;
    [SerializeField] private Canvas m_HealthbarSlider;  //This is a canvas, but is named "Slider" for the editor's inspector (makes it a bit easier to understand).

    private Canvas m_ArmourbarInstance;
    private Canvas m_HealthbarInstance;

    private float m_HideHealthTime = 1.7f;  //Time until we hide our healthbars (if HealthDisplayOptions == ShowWhenDamaged) in seconds

    public const string UPDATE_HEALTHBARS_METHOD = "UpdateHealthbars";
    public const string HIDE_HEALTHBARS_METHOD = "HideHealthbarsHook";

    protected virtual void Start()
    {

        InvokeRepeating(UPDATE_HEALTHBARS_METHOD, 0f, 0.3f);

        if (photonView.IsMine)
        {
            m_Health = m_InitialHealth;
            m_Armour = m_InitialArmour;
        }

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
            if (m_HealthbarDisplay != HealthDisplayOptions.AlwaysOn) m_HealthbarInstance.gameObject.SetActive(false);

        }

        //same as above
        if (m_ArmourbarSlider != null)
        {
            m_ArmourbarInstance = Instantiate(m_ArmourbarSlider);
            m_ArmourbarInstance.transform.SetParent(m_HealthbarInstance.transform, false);


            m_ArmourbarInstance.GetComponentInChildren<Slider>().value = m_Armour / m_InitialArmour;

            float offset = 0;
            if (m_HealthbarInstance != null)
            {
                offset = m_HealthbarInstance.GetComponentInChildren<RectTransform>().rect.height;
            }
            m_ArmourbarInstance.transform.position += new Vector3(0, offset, 0);

            if (m_HealthbarDisplay != HealthDisplayOptions.AlwaysOn || m_InitialArmour <= 0) m_ArmourbarInstance.gameObject.SetActive(false);

        }

    }

    public virtual void OnCollisionEnter(Collision col)
    {
        DamageSource source = col.gameObject.GetComponent<DamageSource>();

        if (source != null)
        {

            if (m_Armour <= 0) //do they have NO armour?
            {
                SetHealth(Mathf.Clamp(m_Health - source.m_Damage, 0, m_Health)); // To avoid negatives...lock new health value to 0 minumum and the current m_Health value for max. 
            }
            else if (m_Armour >= source.m_Damage) //If they have Armour, check if the damage doesn't exceed the armour
            {
                SetArmour(Mathf.Clamp(m_Armour - source.m_Damage, 0, m_Armour)); //Dummy proof this, so negative values of DamageSource do not increase armour (negative minus a negative is a positive).
            }
            else //if the damage exceeds armour value, continue damage into health.
            {
                float difference = source.m_Damage - m_Armour;

                SetArmour(0.0f);
                SetHealth(Mathf.Clamp(m_Health - difference, 0, m_Health)); //Same as above...lock to 0 minimum and the current health value maximum.
            }


            //destroy object if it has no health
            if (m_Health <= 0 && m_IsInitialized) DestroyObject();

        }
    }


    public void UpdateHealthbars()
    {
        if (m_ArmourbarInstance != null) m_ArmourbarInstance.GetComponentInChildren<Slider>().value = m_Armour / m_InitialArmour;
        if (m_HealthbarInstance != null) m_HealthbarInstance.GetComponentInChildren<Slider>().value = m_Health / m_InitialHealth;

        if (m_HealthbarDisplay != HealthDisplayOptions.NeverShow)
        {
            if (m_ArmourbarInstance != null) m_ArmourbarInstance.gameObject.SetActive((m_Armour > 0));
            if (m_HealthbarInstance != null) m_HealthbarInstance.gameObject.SetActive(true);
        }

        if (m_HealthbarDisplay == HealthDisplayOptions.ShowWhenDamaged)
        {
            CancelInvoke(HIDE_HEALTHBARS_METHOD);
            Invoke(HIDE_HEALTHBARS_METHOD, m_HideHealthTime);
        }
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(m_Armour);
            stream.SendNext(m_Health);
        }
        else
        {
            // Network player, receive data
            m_Armour = (float)stream.ReceiveNext();
            m_Health = (float)stream.ReceiveNext();

            UpdateHealthbars();

            m_IsInitialized = true;
        }
    }


    public void HideHealthbarsHook()
    {
        this.photonView.RPC(HIDE_HEALTHBARS_METHOD, RpcTarget.All);
    }

    [PunRPC]
    public void HideHealthbars()
    {
        if (m_HealthbarInstance != null) m_HealthbarInstance.gameObject.SetActive(false);
        if (m_ArmourbarInstance != null) m_ArmourbarInstance.gameObject.SetActive(false);

    }


    public float GetArmour()
    {
        return m_Armour;
    }

    public float GetHealth()
    {
        return m_Health;
    }

    public void SetHealth(float amount)
    {
        m_Health = amount;
    }
    
    public void SetArmour(float amount)
    {
        m_Armour = amount;
    }

    public float GetInitialHealth()
    {
        return m_InitialHealth;
    }

    public float GetInitialArmour()
    {
        return m_InitialArmour;
    }


    public virtual void DestroyObject()
    {
        HideHealthbars();
        Destroy(this.gameObject, 0.05f);
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
