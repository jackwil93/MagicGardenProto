using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePoint : MonoBehaviour {

    public GameObject pointerIcon;
    public bool empty; // checked from GameManager
    bool pointerOn;
    float pointerSavedY;

    private void Awake()
    {
        empty = true; // Run here before the World Item spawn-in happens
    }

    private void Start()
    {
        pointerSavedY = pointerIcon.transform.position.y;
    }

    private void Update()
    {
        if (pointerOn)
        {
            pointerIcon.transform.position = new Vector3(pointerIcon.transform.position.x,
                pointerSavedY + 0.2f * Mathf.Sin(3 * Time.time),
                pointerIcon.transform.position.z);
        }
    }

    public void ShowPointer()
    {
        if (empty)
        {
        pointerIcon.SetActive(true);
        pointerOn = true;
        }
    }

    public void HidePointer()
    {
        pointerIcon.SetActive(false);
        pointerOn = false;
    }
}
