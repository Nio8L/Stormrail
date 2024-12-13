using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followable : MonoBehaviour
{
    CameraController cameraController;
    public Vector3 zoom;
    public bool continuous;

    private void Start() {
        cameraController = FindObjectOfType<CameraController>();
    }

    private void OnMouseDown() {
        LockOn();
    }

    public void LockOn(){
        cameraController.LockOn(this);
    }
}
