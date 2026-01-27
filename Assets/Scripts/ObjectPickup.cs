using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float minHoldDistance = 1f;
    [SerializeField] private float maxHoldDistance = 5f;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private KeyCode rotateKey = KeyCode.R;

    [Header("Physics Settings")]
    [SerializeField] private float pickupForce = 150f;
    [SerializeField] private float throwForce = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Hold Offset (Camera Relative)")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0.6f, -0.4f, 0.2f);

    private Camera playerCamera;
    private GameObject heldObject;
    private Rigidbody heldObjectRigidbody;
    private float currentHoldDistance;
    private bool isRotating;
    private Vector3 lastMousePosition;

    // NEW: extra rotation relative to camera
    private Quaternion rotationOffset = Quaternion.identity;

    void Start()
    {
        playerCamera = Camera.main;
        currentHoldDistance = holdDistance;
    }

    void Update()
    {
        if (heldObject == null)
        {
            if (Input.GetKeyDown(pickupKey))
                TryPickupObject();
        }
        else
        {
            if (Input.GetKeyDown(pickupKey))
                DropObject(false);

            if (Input.GetMouseButtonDown(0))
                DropObject(true);

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                currentHoldDistance += scroll * scrollSpeed;
                currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
            }

            if (Input.GetKeyDown(rotateKey))
            {
                isRotating = !isRotating;
                lastMousePosition = Input.mousePosition;
            }

            if (isRotating)
                RotateObject();

            // ALWAYS rotate with camera
            ApplyCameraRotation();
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null && heldObjectRigidbody != null)
        {
            Vector3 targetPosition =
                playerCamera.transform.position +
                playerCamera.transform.forward * currentHoldDistance +
                playerCamera.transform.right * holdOffset.x +
                playerCamera.transform.up * holdOffset.y +
                playerCamera.transform.forward * holdOffset.z;

            Vector3 direction = targetPosition - heldObject.transform.position;
            heldObjectRigidbody.linearVelocity = direction * pickupForce * Time.fixedDeltaTime;
        }
    }

    private void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Moveable"))
                PickupObject(hit.collider.gameObject);
        }
    }

    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRigidbody = obj.GetComponent<Rigidbody>();

        if (heldObjectRigidbody == null)
        {
            heldObject = null;
            return;
        }

        heldObjectRigidbody.useGravity = false;
        heldObjectRigidbody.linearDamping = 10f;
        heldObjectRigidbody.angularDamping = 5f;
        heldObjectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        currentHoldDistance = holdDistance;
        rotationOffset = Quaternion.identity;
        isRotating = false;
    }

    private void DropObject(bool throwObject)
    {
        if (heldObjectRigidbody != null)
        {
            heldObjectRigidbody.useGravity = true;
            heldObjectRigidbody.linearDamping = 0f;
            heldObjectRigidbody.angularDamping = 0.05f;

            if (throwObject)
                heldObjectRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
        }

        heldObject = null;
        heldObjectRigidbody = null;
        isRotating = false;
    }

    private void RotateObject()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

        Quaternion yaw =
            Quaternion.AngleAxis(-mouseDelta.x * rotationSpeed * Time.deltaTime, Vector3.up);
        Quaternion pitch =
            Quaternion.AngleAxis(mouseDelta.y * rotationSpeed * Time.deltaTime, Vector3.right);

        rotationOffset = yaw * pitch * rotationOffset;

        lastMousePosition = Input.mousePosition;
    }

    private void ApplyCameraRotation()
    {
        if (heldObject == null) return;

        heldObject.transform.rotation =
            playerCamera.transform.rotation * rotationOffset;
    }
}
