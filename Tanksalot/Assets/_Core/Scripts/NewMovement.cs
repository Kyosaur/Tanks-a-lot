using Photon.Pun;
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



public class NewMovement : MonoBehaviourPun
{

    //This script uses physics to move, so what ever its attached to NEEDS to have a rigidbody 
    private Rigidbody m_Object;
    private Animator m_Animator;

    private Vector2 m_LastState;

    [Header("Movement Control")]
    //Movement speed controls...since one is a rotation (degrees) and the other is not, their values wont match up to well.
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_RotateSpeed = 90f;

    [Space(10)]
    [SerializeField] private bool m_InvertReverseTurning = true;
    [SerializeField] private bool m_SmoothTransitions = true;

    [Space(10)]
    [SerializeField] private bool m_DisableOnFlip = true;

    private float m_Direction;
    private bool m_DirectionalLock = false;



    //Start() function is called when the script first starts
    void Start()
    {
        //Grab our rigidbody from out component list
        m_Object = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();

        //initialize
        m_LastState = new Vector2(0, 0);

    }



    //FixedUpdate() function is called inbetween physics updates. If you do any sort of movement involving physics, its best to use this instead of Update() function, which is framecount based.
    void FixedUpdate()
    {

        if (photonView.IsMine)
        {

            if (m_DisableOnFlip)
            {
                if (Vector3.Dot(transform.up, Vector3.up) < 0) return;
            }



            //Getting our WASD using GetAxis. Doing it this way allows arrow keys, controllers, and joysticks.....vs  Input.GetButtonDown  being statically tied to W, A, S, and D only.
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");



            //"Smooth" transitions is simply a temporary fix to a problem that comes from inverting reverse turning while using joysticks. 
            //We invert turning due to wanting the movements in reverse to behave like a vehicle. Without invert reverse turning a
            //forward+left = left turn, but reverse+left equals a right turn (Think of a X/Y graph in math using an arrow (vector) and it makes sense why)

            //this quick fix simply sets the "range" (see IgnoreUntilVal) of the joystick we'll allow until we transition to reverse. Without it, you could
            //accidently switch to reversing if you over rotate the joystick around while turning (bringing the vertical value to negatives).




            /* This version has a small range in which it will allow reverse 
             *
             * if (m_InvertReverseTurning && m_SmoothTransitions)
              {
                  //if (vertical <= 0 && vertical > -0.6) vertical *= -1;

                  if(vertical <= 0) //Is player reversing?
                  {
                      //this is a RANGE:  -ingoreUntilVal  to  ignoreUntilVal
                      float ignoreUntilVal = 0.8f;

                      if((horizontal >= -(ignoreUntilVal) && horizontal <= ignoreUntilVal) == false)
                      {
                          //Not in the range...NO reversing. Flip movement to positive 
                          vertical *= -1;
                      }
                  }

              }*/


            /*
             * This version locks moving directions until you lift up on the joystick.
             * 
            if (m_SmoothTransitions)
            {
                bool isMoving = ((vertical > 0.1 || vertical < -0.1) || (horizontal > 0.1 || horizontal < -0.1)) ? true : false;

                if (isMoving)
                {
                    if (m_DirectionalLock == false)
                    {
                        m_DirectionalLock = true;
                        m_Direction = vertical;
                    }
                    else
                    {
                        if (m_Direction > 0) vertical = (vertical > 0) ? vertical : vertical *= -1; 
                        else vertical = (vertical < 0) ? vertical : vertical *= -1;
                    }
                }
                else m_DirectionalLock = false;
            }   */




            //pass movement values to animator
            m_Animator.SetFloat("Turn", horizontal);
            m_Animator.SetFloat("Move", vertical);




            //Create a magnitude (difference between to points--think of a line in math). This will be where the object moves. 
            //The math is our current forward vector * our W/S input (which RANGES from -1 to 1.....Its usually -1 for S, 0 for none, and 1 for W)  *  our speed to get an actual movement). 
            //Time.deltaTime is the time from the last frame (this makes it framerate independent, so higher/lower framerates run the same).
            Vector3 movement = m_Object.transform.forward * vertical * m_Speed * Time.deltaTime;
            m_Object.MovePosition(m_Object.position + movement);



            //Basically the same as above, except this rotation instead of position. 
            float dir = horizontal * m_RotateSpeed * Time.deltaTime;

            if (m_InvertReverseTurning)
            {
                if (vertical < 0) dir *= -1; //if movement is backwards...inverse turn
            }

            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, dir, 0f));
            m_Object.MoveRotation(m_Object.rotation * deltaRotation);
        }

    }

    public void SetSpeed(float speed)
    {
        m_Speed = speed;
    }

    public float GetSpeed()
    {
        return m_Speed;
    }

    public void SetRotateSpeed(float speed)
    {
        m_RotateSpeed = speed;
    }

    public float GetRotateSpeed()
    {
        return m_RotateSpeed;
    }


}
