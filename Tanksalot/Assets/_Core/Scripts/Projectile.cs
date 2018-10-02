using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    public float m_LifeTime = 2.0f;
    public int m_ShellDamage = 10;

    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;


    void Start()
    {
        Invoke("StartProjectileLife", 0.05f);
    }

    void OnCollisionEnter(Collision col)
    {
        if(m_ExplosionParticles != null) m_ExplosionParticles.Play();
        if(m_ExplosionAudio != null) m_ExplosionAudio.Play();

        SetAllCollidersStatus(false);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject,0.3f);
    }


    private void StartProjectileLife()
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;

        Invoke("EndProjectileLife", m_LifeTime);
    }

    private void EndProjectileLife()
    {
        Destroy(gameObject);
    }


    public void SetAllCollidersStatus(bool active)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = active;
        }
    }

}
