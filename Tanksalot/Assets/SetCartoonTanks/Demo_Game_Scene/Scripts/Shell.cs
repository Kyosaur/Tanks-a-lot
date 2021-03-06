﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script is for the  shell
public class Shell : MonoBehaviour
{
    public float lifeTime = 2.0f;
    public int shellDamage = 10;
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;

        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject, 2f);
    }

}
