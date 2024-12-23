using UnityEngine;

public class CameraController : MonoBehaviour, ISavable
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
    bool hasDragStartPosition = false;
    bool hasRotationStartPosition = false;

    public int maxZoom;
    public int minZoom;

    bool lockedOn = false;
    bool continuous = false;
    GameObject lockedOnTarget;
    Vector3 lockedOnZoom;
    bool reachedLockOn = false;

    private void Start() {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void Update() {
        HandleLockOn();
        
        if(lockedOn) return;

        HandleMovementInput();
        HandleMouseInput();
    }

    public void HandleLockOn(){
        if(lockedOn){
            transform.position = Vector3.Lerp(transform.position, lockedOnTarget.transform.position, Time.unscaledDeltaTime * movementTime);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, lockedOnZoom, Time.unscaledDeltaTime * movementTime);
        
            if(reachedLockOn || (Vector3.Distance(transform.position, lockedOnTarget.transform.position) < 0.1 && Vector3.Distance(cameraTransform.localPosition, lockedOnZoom) < 0.1)){
                reachedLockOn = true;
                newPosition = transform.position;
                newZoom = cameraTransform.localPosition;
                
                if(!continuous){
                    lockedOn = false;
                    reachedLockOn = false;
                }else {
                    if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){
                        lockedOn = false;
                        reachedLockOn = false;
                    }
                    
                    HandleMouseInput();
                    HandleMovementInput();
                    
                }
            }
        }

    }

    public void HandleMouseInput(){
        if (Input.GetMouseButtonUp(0)){
            hasDragStartPosition = false;
        }
        if (Input.GetMouseButtonUp(1)){
            hasRotationStartPosition = false;
        }

        if (RaycastChecker.Check()) return;

        if(Input.mouseScrollDelta.y != 0){
            newZoom += Input.mouseScrollDelta.y * zoomAmount;

            if(newZoom.y > maxZoom){
                newZoom.y = maxZoom;
                newZoom.z = -maxZoom;
            }

            if(newZoom.y < minZoom){
                newZoom.y = minZoom;
                newZoom.z = -minZoom;
            }
        }
        
        if(Input.GetMouseButtonDown(0)){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry)){
                dragStartPosition = ray.GetPoint(entry);
                hasDragStartPosition = true;
            }
        }

        if(Input.GetMouseButton(0) && hasDragStartPosition){
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry)){
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        if(Input.GetMouseButtonDown(1)){
            hasRotationStartPosition = true;
            rotateStartPosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(1) && hasRotationStartPosition){
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
            
            if(newZoom.y < minZoom){
                newZoom.y = minZoom;
                newZoom.z = -minZoom;
            }
        }

        if(Input.GetKey(KeyCode.F)){
            newZoom -= zoomAmount;

            if(newZoom.y > maxZoom){
                newZoom.y = maxZoom;
                newZoom.z = -maxZoom;
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.unscaledDeltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.unscaledDeltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.unscaledDeltaTime * movementTime);
    }

    public void LockOn(Followable followable){
        lockedOn = true;
        lockedOnTarget = followable.gameObject;
        lockedOnZoom = followable.zoom;
        continuous = followable.continuous;
    }

    public void LoadData(GameData data)
    {
        Vector3 cameraRigPosition = new(data.camera.cameraRigTransform.position.x, data.camera.cameraRigTransform.position.y, data.camera.cameraRigTransform.position.z);

        Vector3 cameraTransformPosition = new(data.camera.cameraTransform.position.x, data.camera.cameraTransform.position.y, data.camera.cameraTransform.position.z);

        transform.position = cameraRigPosition;
        transform.rotation = Quaternion.Euler(data.camera.cameraRigTransform.rotation.x, data.camera.cameraRigTransform.rotation.y, data.camera.cameraRigTransform.rotation.z);
    
        cameraTransform.localPosition = cameraTransformPosition;
        cameraTransform.rotation = Quaternion.Euler(data.camera.cameraTransform.rotation.x, data.camera.cameraTransform.rotation.y, data.camera.cameraTransform.rotation.z);
    
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    public void SaveData(GameData data)
    {
        TransformSerialized cameraTransformSerialized = new(cameraTransform.localPosition.x, cameraTransform.localPosition.y, cameraTransform.localPosition.z, 
                                                            cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        TransformSerialized cameraRigTransformSerialized = new(transform.position.x, transform.position.y, transform.position.z,
                                                                transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        data.camera = new(cameraTransformSerialized, cameraRigTransformSerialized);
    }

    public int GetPriority()
    {
        return 0;
    }
}
