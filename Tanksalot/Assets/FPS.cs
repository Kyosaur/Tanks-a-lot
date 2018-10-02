using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

    public Text text;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private void Update()
    {
        text.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime));
    }

}
