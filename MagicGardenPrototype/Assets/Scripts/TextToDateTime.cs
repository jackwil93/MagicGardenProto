using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToDateTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = System.DateTime.Now.ToString("dddd dd MMM h:mm");
	}

    private void OnGUI()
    {
        GetComponent<Text>().text = System.DateTime.Now.ToString("dddd MMM dd h:mm:ss tt");
        Debug.Log("Updated time OnGUI");
    }
}
