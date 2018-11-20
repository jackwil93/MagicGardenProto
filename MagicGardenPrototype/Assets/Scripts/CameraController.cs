using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float _speed;
    float _velocityX;

    public List<Quaternion> presetYPosList = new List<Quaternion>();
    int currentCamPos;
    bool movingToPreset;
    Quaternion targetRotation;

    public bool cameraIsMoving;
	
	// Update is called once per frame
	void Update () {

        _speed -= 0.0001f;

        if (_speed < 0.000001f)
            _speed = 0;

        if (!movingToPreset)
        {
            this.transform.Rotate(new Vector3(0, -_velocityX * _speed, 0));

        if (Mathf.Abs(_velocityX) > 5f)
            cameraIsMoving = true;
        else
            cameraIsMoving = false;
        }
	}

    private void FixedUpdate()
    {
        if (movingToPreset)
        {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);

            float dot = Quaternion.Dot(transform.rotation, targetRotation);
            Debug.Log("Dot to target = " + dot);

            

        if (Mathf.Abs (dot) > 0.99999f)
            movingToPreset = false;
        }

              
    }

    public void RotateCamera(Vector3 moveVelocity)
    {
        _velocityX = Mathf.Clamp(moveVelocity.x, -1000, 1000);
        _speed = 0.005f;
    }

    public void CamMoveRight()
    {
       
        int targetIndex = presetYPosList.IndexOf(GetClosestRotationPos()) + 1;

        if (targetIndex == presetYPosList.Count)
            targetIndex = 0;

        targetRotation = presetYPosList[targetIndex];

        movingToPreset = true;

    }

    public void CamMoveLeft()
    {
        int targetIndex = presetYPosList.IndexOf(GetClosestRotationPos()) - 1;

        if (targetIndex < 0)
            targetIndex = presetYPosList.Count - 1;

        targetRotation = presetYPosList[targetIndex];

        movingToPreset = true;
    }

    Quaternion GetClosestRotationPos()
    {
        Quaternion currentRotation = this.transform.rotation;
        Quaternion closestRotation = new Quaternion();
        float smallestAngle = 999;


        foreach (Quaternion presetRot in presetYPosList)
        {
            float angle = Quaternion.Angle(currentRotation, presetRot);

            if (angle < smallestAngle)
            {
                Debug.Log("Closest rotation = " + presetRot + "| Angle of " + angle);
                smallestAngle = angle;
                closestRotation = presetRot;
            }
        }

        return closestRotation;
    }

}
