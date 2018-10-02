using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using DigitalRubyShared;
using System;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    [Tooltip("Fingers Joystick Script")]
    public FingersJoystickScript m_MoveJoystickScript;
    public FingersJoystickScript m_AimJoystickScript;

    public GameObject m_TurretBoneToRotate;
    public GameObject m_TankBody;

    [Tooltip("Enable/Disable dual joystick controls")]
    public bool m_DualStickControls = false;

    [Space(15)]
    [Header("Input Names:")]
    public string m_FireButtonName = "Fire1";
    CrossPlatformInputManager.VirtualButton m_FireButton;

    public string m_HorizontalAxisName = "Horizontal";
    public string m_VerticalAxisName = "Vertical";


    CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;
    CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;


    private void Awake()
    {

#if MOBILE_INPUT
        //JoystickScript.enabled = true;
        m_MoveJoystickScript.gameObject.SetActive(true);
        if (m_DualStickControls) m_AimJoystickScript.gameObject.SetActive(true);
#else
        m_MoveJoystickScript.gameObject.SetActive(false);
        m_AimJoystickScript.gameObject.SetActive(false);
#endif

    }

    private void Start()
    {

        //setup our callbacks for the joystick(s)
        m_MoveJoystickScript.JoystickExecuted = MoveJoystickExecuted;
        m_AimJoystickScript.JoystickExecuted = AimJoystickExecuted;

        TapGestureRecognizer oneFingerTap = new TapGestureRecognizer();
        oneFingerTap.StateUpdated += TapCallback;
        oneFingerTap.AllowSimultaneousExecutionWithAllGestures();

        FingersScript.Instance.AddGesture(oneFingerTap);



        PanGestureRecognizer PanGesture = new PanGestureRecognizer { MinimumNumberOfTouchesToTrack = 1, MaximumNumberOfTouchesToTrack = 1, ThresholdUnits = 0.2f };
        PanGesture.StateUpdated += Pan_StateUpdated;

        FingersScript.Instance.AddGesture(PanGesture);

        CrossPlatformInputManager.SetVirtualMousePositionX(Screen.width / 2);
        CrossPlatformInputManager.SetVirtualMousePositionY(Screen.height / 2); 


        //CrossPlatformInputManager code to hookup fire/movement
        m_FireButton = new CrossPlatformInputManager.VirtualButton("Fire1");
        CrossPlatformInputManager.RegisterVirtualButton(m_FireButton);

        m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(m_HorizontalAxisName);
        CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);

        m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(m_VerticalAxisName);
        CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);


    }


    private void Pan_StateUpdated(GestureRecognizer gesture)
    {
        if (m_DualStickControls) return;

        if (gesture.State == GestureRecognizerState.Executing)
        {
            CrossPlatformInputManager.SetVirtualMousePositionX(gesture.FocusX);
            CrossPlatformInputManager.SetVirtualMousePositionY(gesture.FocusY);
        }
    }


    private void TapCallback(GestureRecognizer gesture)
    {

        if (gesture.State == GestureRecognizerState.Ended)
        {

            m_FireButton.Pressed();

            if (!m_DualStickControls)
            {
                CrossPlatformInputManager.SetVirtualMousePositionX(gesture.FocusX);
                CrossPlatformInputManager.SetVirtualMousePositionY(gesture.FocusY);
            }
            m_FireButton.Released();
        }
    }




    private void MoveJoystickExecuted(FingersJoystickScript script, Vector2 amount)
    {

        m_HorizontalVirtualAxis.Update(amount.x);
        m_VerticalVirtualAxis.Update(amount.y);
    }


    private void AimJoystickExecuted(FingersJoystickScript script, Vector2 amount)
    {

        if (!m_DualStickControls) return;


        if (amount.x == 0 && amount.y == 0)
        {
            m_FireButton.Pressed();
            m_FireButton.Released();
            return;
        }

        float joystick_angle = Mathf.Atan2(amount.x, amount.y) * Mathf.Rad2Deg;
        //joystick_angle = (joystick_angle < 0) ? (joystick_angle + 360) : joystick_angle;

        m_TurretBoneToRotate.transform.eulerAngles = new Vector3(-90, 0, m_TankBody.transform.eulerAngles.y + joystick_angle);// + joystick_angle);


        //Camera c = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        Camera c = Camera.main;

        Vector3 tmp = m_TurretBoneToRotate.transform.position + m_TurretBoneToRotate.transform.up * -10f;
        Vector3 pos = c.WorldToScreenPoint(tmp);


        CrossPlatformInputManager.SetVirtualMousePositionX(pos.x);
        CrossPlatformInputManager.SetVirtualMousePositionY(pos.y);

    }


    public void Toggle()
    {
        m_DualStickControls = !m_DualStickControls;
        m_AimJoystickScript.gameObject.SetActive(m_DualStickControls);

    }
}

