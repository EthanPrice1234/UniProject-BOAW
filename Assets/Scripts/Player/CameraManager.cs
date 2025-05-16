using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    [Header("Camera Settings")]
    public Transform targetTransform; 
    public Transform cameraPivot; 
    public Transform cameraTransform;

    public LayerMask collisionLayers; 
    public LayerMask ignoreLayers;
    private float defaultPosition; 

    private Vector3 cameraFollowVelocity = Vector3.zero;  
    private Vector3 cameraVectorPosition; 

    public float cameraCollisionOffset = 0.2f; 
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;

    public float cameraFollowSpeed = 0.2f;
    public float lookAngle;
    public float pivotAngle;
    public float minPivotAngle = -35;
    public float maxPivotAngle = + 35;

    private float cameraLookSpeed = 2;
    private float cameraPivotSpeed = 2;

    private void Start()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform; 
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;  
    }

    public void ActivateCamera()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement() 
    {
        FollowPlayer();
        RotateCamera(); 
        HandleCameraCollisions();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition; 
    }

    private void RotateCamera() 
    {
        Vector3 rotation; 
        Quaternion targetRotation; 

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);

        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions() 
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset) 
        {
            targetPosition =- minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
