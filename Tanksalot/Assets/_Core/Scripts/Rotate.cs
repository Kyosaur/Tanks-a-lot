using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ROTATE_DIRECTION
{
    ROTATE_NONE,
    ROTATE_X,
    ROTATE_Y,
    ROTATE_Z

}

public class Rotate : MonoBehaviour {

    public float m_RotateSpeed = 60f;
    public ROTATE_DIRECTION m_Direction = ROTATE_DIRECTION.ROTATE_X;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(m_Direction == ROTATE_DIRECTION.ROTATE_X)
        {
            transform.Rotate(new Vector3(Time.deltaTime * m_RotateSpeed, 0, 0));
            return;
        }

        if(m_Direction == ROTATE_DIRECTION.ROTATE_Y)
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * m_RotateSpeed, 0));
            return;
        }

        if(m_Direction == ROTATE_DIRECTION.ROTATE_Z)
        {
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * m_RotateSpeed));
            return;
        }
	}
}
