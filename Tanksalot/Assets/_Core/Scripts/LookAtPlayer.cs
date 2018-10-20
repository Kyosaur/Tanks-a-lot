using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LookAtPlayer : MonoBehaviour {

    private Camera m_Camera;

   
	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("CameraUpdate", 0f, 0.4f);
	}

    void CameraUpdate()
    {
        if (m_Camera == null || !m_Camera.isActiveAndEnabled)
        {
            m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>(true);
        }

    }

    // Update is called once per frame
    void Update ()
    {

        if(m_Camera != null && m_Camera.isActiveAndEnabled) transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);

    }
}
