using UnityEngine;

public class Podium : MonoBehaviour
{
    [SerializeField] private GameObject controlledCube;
    [SerializeField] private GameObject cameraPositionCube;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float cubeSpeed = 15f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private Goaltrigger goalTrigger;
    [SerializeField] private GameObject ignoreFloorObject;

    private bool isControllingCube = false;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private float cameraMoveSpeed = 2f;
    private float cameraRotationSpeed = 5f;
    private Vector3 currentDirection = Vector3.zero;
    private float wallDetectionDistance = 0.5f;
    private bool isMoving = false;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Check if player is within range
        if (IsWithinRange())
        {
            // Press E to start controlling the cube
            if (Input.GetKeyDown(KeyCode.E))
            {
                isControllingCube = true;
                DisablePlayerMovement(true);
                SaveAndAdjustCamera();
            }
        }
        else
        {
            // If outside range, lose control
            if (isControllingCube)
            {
                isControllingCube = false;
                DisablePlayerMovement(false);
                RestoreCamera();
            }
        }

        // Check if goal has been reached while controlling
        if (isControllingCube && goalTrigger != null && goalTrigger.IsGoalReached())
        {
            isControllingCube = false;
            isMoving = false;
            currentDirection = Vector3.zero;
            DisablePlayerMovement(false);
            RestoreCamera();
        }

        // Control the cube if actively controlling
        if (isControllingCube && controlledCube != null)
        {
            MoveCube();
            
            // Keep camera looking at the cube with smooth rotation
            if (playerCamera != null && cameraPositionCube != null)
            {
                Vector3 cameraPos = cameraPositionCube.transform.position + Vector3.down * 1f;
                playerCamera.transform.position = cameraPos;
                
                // Smoothly rotate to look at the cube
                Vector3 directionToCube = (controlledCube.transform.position - cameraPos).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCube);
                playerCamera.transform.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, targetRotation, cameraRotationSpeed * Time.deltaTime * 100f);
            }
        }
    }

    private bool IsPlayerLookingAtPodium()
    {
        if (playerCamera == null)
            return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    private bool IsWithinRange()
    {
        if (playerBody == null && playerCamera == null)
            return false;

        Transform rangeSource = playerBody != null ? playerBody : playerCamera.transform;
        float distance = Vector3.Distance(rangeSource.position, transform.position);
        return distance <= interactionRange;
    }

    private void MoveCube()
    {
        if (controlledCube == null || playerCamera == null)
            return;

        // Only accept new input if cube is not currently moving
        if (!isMoving)
        {
            Vector3 newDirection = Vector3.zero;

            // Check for new direction input - relative to camera view, snapped to cardinal directions
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                newDirection = playerCamera.transform.forward;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                newDirection = -playerCamera.transform.forward;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                newDirection = -playerCamera.transform.right;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                newDirection = playerCamera.transform.right;
            }

            // Set direction and start moving if new input received
            if (newDirection != Vector3.zero)
            {
                newDirection.y = 0;
                newDirection = newDirection.normalized;
                
                // Snap to cardinal direction (N, S, E, W in world space)
                currentDirection = GetNearestCardinalDirection(newDirection);
                isMoving = true;
            }
        }

        // Move cube in current direction if moving
        if (isMoving && currentDirection != Vector3.zero)
        {
            // Check if there's a wall ahead
            if (!IsWallAhead(currentDirection))
            {
                Vector3 movement = currentDirection * cubeSpeed * Time.deltaTime;
                controlledCube.transform.position += movement;
            }
            else
            {
                // Stop movement when hitting a wall
                currentDirection = Vector3.zero;
                isMoving = false;
            }
        }
    }

    private Vector3 GetNearestCardinalDirection(Vector3 direction)
    {
        // Snap to nearest cardinal direction (forward, back, left, right)
        Vector3 absDir = new Vector3(Mathf.Abs(direction.x), 0, Mathf.Abs(direction.z));
        
        if (absDir.x > absDir.z)
        {
            // Return left or right
            return direction.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            // Return forward or back
            return direction.z > 0 ? Vector3.forward : Vector3.back;
        }
    }

    private bool IsWallAhead(Vector3 direction)
    {
        if (controlledCube == null)
            return false;

        // Raycast in the current direction to detect walls
        Ray ray = new Ray(controlledCube.transform.position, direction);
        RaycastHit hit;

        // Check if there's a collider in the way
        if (Physics.Raycast(ray, out hit, wallDetectionDistance))
        {
            // Don't count the cube itself, the floor object, or the goal trigger
            if (hit.collider.gameObject != controlledCube && 
                (ignoreFloorObject == null || hit.collider.gameObject != ignoreFloorObject) &&
                (goalTrigger == null || hit.collider.gameObject != goalTrigger.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    private void DisablePlayerMovement(bool disable)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = !disable;
        }
    }

    private void SaveAndAdjustCamera()
    {
        if (playerCamera == null)
            return;

        // Save original position and rotation
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        // Calculate target position (at camera cube position, moved down a bit)
        Vector3 targetPosition = cameraPositionCube != null 
            ? cameraPositionCube.transform.position + Vector3.down * 1f 
            : originalCameraPosition + Vector3.up * 10f;
        
        Quaternion targetRotation = Quaternion.LookRotation(controlledCube.transform.position - targetPosition);

        // Start smooth transition
        StartCoroutine(SmoothCameraTransition(originalCameraPosition, targetPosition, originalCameraRotation, targetRotation));
    }

    private void RestoreCamera()
    {
        if (playerCamera == null)
            return;

        // Get current position and rotation
        Vector3 currentPosition = playerCamera.transform.position;
        Quaternion currentRotation = playerCamera.transform.rotation;

        // Start smooth transition back to original
        StartCoroutine(SmoothCameraTransition(currentPosition, originalCameraPosition, currentRotation, originalCameraRotation));
    }

    private System.Collections.IEnumerator SmoothCameraTransition(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        float elapsedTime = 0f;
        float duration = 1f / cameraMoveSpeed;

        while (elapsedTime < duration)
        {
            if (playerCamera == null)
                yield break;

            float t = elapsedTime / duration;
            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (playerCamera != null)
        {
            playerCamera.transform.position = endPos;
            playerCamera.transform.rotation = endRot;
        }
    }
}
    

