using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float _speed;
    float _velocityX;

    public bool cameraIsMoving;
	
	// Update is called once per frame
	void Update () {

        _speed -= 0.0001f;

        if (_speed < 0.000001f)
            _speed = 0;

            this.transform.Rotate(new Vector3(0, -_velocityX * _speed, 0));

        if (_velocityX != 0)
            cameraIsMoving = true;
        else
            cameraIsMoving = false;
	}

    public void RotateCamera(Vector3 moveVelocity)
    {
        _velocityX = Mathf.Clamp(moveVelocity.x, -1000, 1000);
        _speed = 0.005f;
    }

}
