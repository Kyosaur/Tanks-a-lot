using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownFixUI : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        RectTransform rect = this.GetComponent<RectTransform>();
        LayoutElement layout = this.GetComponent<LayoutElement>();

        if(rect != null)
        {
            if(layout != null)
            {
                rect.sizeDelta = new Vector2(layout.preferredWidth, layout.preferredHeight);
            }
            else rect.sizeDelta = new Vector2(0, 400);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
