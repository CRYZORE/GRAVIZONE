using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public Transform cameraPos; 
    // Update is called once per frame
    void Update()
    {
        if(cameraPos == null)
            cameraPos = GameObject.FindGameObjectWithTag("CameraPos").transform; 
        transform.position = cameraPos.position; 
    }
}
