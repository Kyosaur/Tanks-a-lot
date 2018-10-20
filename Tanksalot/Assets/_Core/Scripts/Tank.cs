using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class Tank : Damagable
{

    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private Renderer[] m_Renderer;


    [Space(15)]
    [Header("--Tank Settings--")]
    [Space(10)]

    public Camera m_CameraPrefab;
    public Color m_Color = Color.white;

    [Space(15)]
    [Header("--Turret Control Settings--")]
    [Space(10)]

    public int m_AimDelayTime = 500; //ms
    public Transform m_TurretBone;


    [Space(15)]
    [Header("--Turret Fire Data--")]
    [Space(10)]

    public float m_FireDelayTime = 700;  //ms
    public float m_ProjectileSpeed = 80f;

    public Transform m_GunEnd;
    public Rigidbody m_Projectile;

    private Camera m_Camera;


    private bool m_TurretDelay = false;
    private bool m_FireDelay = false;



    private void Awake()
    {

        m_Camera = GetComponentInChildren<Camera>(true);
        m_Rigidbody = GetComponentInChildren<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();
        m_Renderer = GetComponentsInChildren<Renderer>();

    }


    new void Start ()
    {
        base.Start();

        if(photonView.IsMine)
        {
            GameObject c = GameObject.FindGameObjectWithTag("MainCamera");
            if (c != null)
            {
                c.SetActive(false);
            }

            m_Camera.gameObject.tag = "MainCamera";
            m_Camera.gameObject.SetActive(true);
        }

        if (m_Renderer.Length > 0)
        {
            foreach(Renderer r in m_Renderer)
            {
                r.material.color = m_Color;
            }
        }
        Debug.Log("end of start");
    }

    void Update()
    {
        if(photonView.IsMine)
        {

            if (!m_TurretDelay)
            {
                RaycastHit hit;
                Ray ray = m_Camera.ScreenPointToRay(CrossPlatformInputManager.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    this.photonView.RPC("RpcAim", RpcTarget.All, hit.point);

                    m_TurretDelay = true;
                    Invoke("TurretDelayReset", m_AimDelayTime / 1000); //Divide by 1000 to get MS time
                }
            }

            if (!m_FireDelay)
            {
                if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                {
                    this.photonView.RPC("RpcFire", RpcTarget.All);

                    m_FireDelay = true;
                    Invoke("ShootDelayReset", m_FireDelayTime / 1000);
                }
            }
        }

    }

    [PunRPC]
    public void RpcFire()
    {
        var rot = m_TurretBone.rotation * Quaternion.Euler(180, 0, 0);

        Rigidbody shellInstance = Instantiate(m_Projectile, m_GunEnd.position, rot) as Rigidbody;
        shellInstance.velocity = m_ProjectileSpeed * -m_TurretBone.transform.up;
    }


    [PunRPC]
    private void RpcAim(Vector3 pos)
    {
        Vector3 targetDir = pos - m_TurretBone.transform.position;

        Vector3 newDir = Vector3.RotateTowards(m_TurretBone.transform.forward, targetDir, 1f, 0);
        Quaternion target = Quaternion.LookRotation(newDir);

        m_TurretBone.transform.rotation = Quaternion.Euler(-90, target.eulerAngles.y, 0);
    }


    new void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }


    public override void DestroyObject()
    {
        HideHealthbars();

        m_Rigidbody.isKinematic = true;
        SetAllCollidersStatus(false);

        Invoke("FadeOut", 1.5f);

    }


    public void FadeOut()
    {
        m_Rigidbody.isKinematic = false;
        Invoke("FinalDestroy", 1);
    }

    public void FinalDestroy()
    {
        Destroy(transform.root.gameObject);
    }

    private void ShootDelayReset()
    {
        m_FireDelay = false;
    }

    private void TurretDelayReset()
    {
        m_TurretDelay = false;
    }


    //-------------------------------------------------------------------General useful functions -------------------------------------------------------------------------------------------


    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    public void SetAllCollidersStatus(bool active)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = active;
        }
    }

}
