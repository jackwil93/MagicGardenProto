using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InvokeRepeating("FixLookAt", 0.1f, 0.2f); // Small issue, but I'd rather it wasn't running in Update. This is the fix
	}

    void FixLookAt()
    {
        transform.LookAt(Camera.main.transform);
    }
}
