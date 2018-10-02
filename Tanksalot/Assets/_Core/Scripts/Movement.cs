using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

//----------------------------------  NOTE -----------------------------------------------------------------------------
//For movement animations: this script requires FLOAT type parameters "Move", and "Turn" inside the animator controller attached to this object/a child of this object. 
//These values should affect a blend tree, in said controller, that blends the moving/turning animations. Example: a if Move > 0.1 thats moving forward, so we then have
//a "Movingforward" blendtree that that uses our "Turn" parameter to blend turning left (-1), moving forward (0), and turning right (1). Do the inverse for moving back....etc.
//-----------------------------------------------------------------------------------------------------------------------



public class Movement : NetworkBehaviour
{
    
    //This script uses physics to move, so what ever its attached to NEEDS to have a rigidbody 
    private Rigidbody m_Object;

    //OPTIONAL - If you want audio while moving, fill out all the audio fields
    private AudioSource m_Audio;


    [Header("Movement Control")]
    //Movement speed controls...since one is a rotation (degrees) and the other is not, their values wont match up to well.
    public float m_Speed = 10f;
    public float m_RotateSpeed = 90f;
    public bool m_InvertReverseTurning = true;

    [Space(10)]
    public bool m_DisableOnFlip = true;

    private Animator m_Animator;



    //Start() function is called when the script first starts
	void Start ()
    {
        //Grab our rigidbody from out component list
        m_Object = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();

	}



    //FixedUpdate() function is called inbetween physics updates. If you do any sort of movement involving physics, its best to use this instead of Update() function, which is framecount based.
    void FixedUpdate ()
    {
        if(m_DisableOnFlip)
        {
            if (Vector3.Dot(transform.up, Vector3.up) < 0) return; 
        }

        //Getting our WASD using Input manager. Doing it this way allows arrow keys, controllers, and joysticks.....vs  Input.GetButtonDown  being statically tied to W, A, S, and D only.
        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var z = CrossPlatformInputManager.GetAxis("Vertical");


        //pass movement values to animator
        m_Animator.SetFloat("Move", z);
        m_Animator.SetFloat("Turn", x);

   

        //Create a magnitude (difference between to points--think of a line in math). This will be where the object moves. 
        //The math is our current forward vector * our W/S input (which RANGES from -1 to 1.....Its usually -1 for S, 0 for none, and 1 for W)  *  our speed to get an actual movement). 
        //Time.deltaTime is the time from the last frame (this makes it framerate independent, so higher/lower framerates run the same).
        Vector3 movement = m_Object.transform.forward * z * m_Speed * Time.deltaTime;
        m_Object.MovePosition(m_Object.position + movement);



        //Basically the same as above, except this rotation instead of position. 
        float dir = x * m_RotateSpeed * Time.deltaTime;

        if (m_InvertReverseTurning)
        {
            if (z < 0) dir *= -1; //if movement is backwards...inverse turn
        }

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, dir, 0f));
        m_Object.MoveRotation(m_Object.rotation * deltaRotation);
        

     
    }



}
