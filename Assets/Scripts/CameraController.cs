using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;

    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    private void Start() {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void Update() {
        HandleMovementInput();
        HandleMouseInput();
    }

    public void HandleMouseInput(){
        if(Input.mouseScrollDelta.y != 0){
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        
        if(Input.GetMouseButtonDown(0)){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry)){
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if(Input.GetMouseButton(0)){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry)){
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        if(Input.GetMouseButtonDown(1)){
            rotateStartPosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(1)){
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    public void HandleMovementInput(){
        if(Input.GetKey(KeyCode.W)){
            newPosition += transform.forward * movementSpeed;
        }

        if(Input.GetKey(KeyCode.S)){
            newPosition += transform.forward * -movementSpeed;
        }

        if(Input.GetKey(KeyCode.D)){
            newPosition += transform.right * movementSpeed;
        }

        if(Input.GetKey(KeyCode.A)){
            newPosition += transform.right * -movementSpeed;
        }

        if(Input.GetKey(KeyCode.Q)){
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if(Input.GetKey(KeyCode.E)){
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if(Input.GetKey(KeyCode.R)){
            newZoom += zoomAmount;
        }

        if(Input.GetKey(KeyCode.F)){
            newZoom -= zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
