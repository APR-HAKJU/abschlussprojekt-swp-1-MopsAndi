using UnityEngine;

public class scroll : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0f)
        {
            Quaternion rotation = Quaternion.Euler(
                scroll * rotationSpeed * Time.fixedDeltaTime,
                0f,
                0f
            );

            rb.MoveRotation(rb.rotation * rotation);
        }
    }
}
