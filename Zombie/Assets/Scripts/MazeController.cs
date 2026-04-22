using UnityEngine;
using UnityEngine.InputSystem;

public class MazeController : MonoBehaviour
{
    public float maxTilt = 12f;

    public float tiltSpeed = 45f;
    public float returnSpeed = 12f;

    public bool returnToCenter = true;

    private Rigidbody rb;
    private Quaternion startRotation;

    private float currentX;
    private float currentZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 startAngles = transform.eulerAngles;
        startRotation = Quaternion.Euler(0f, startAngles.y, 0f);

        transform.rotation = startRotation;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    void FixedUpdate()
    {
        Vector2 input = GetInput();

        bool hasInput = input.sqrMagnitude > 0.01f;

        float targetX = currentX;
        float targetZ = currentZ;

        if (hasInput)
        {
            targetX = input.y * maxTilt;
            targetZ = -input.x * maxTilt;
        }
        else if (returnToCenter)
        {
            targetX = 0f;
            targetZ = 0f;
        }

        float speed = hasInput ? tiltSpeed : returnSpeed;

        currentX = Mathf.MoveTowards(currentX, targetX, speed * Time.fixedDeltaTime);
        currentZ = Mathf.MoveTowards(currentZ, targetZ, speed * Time.fixedDeltaTime);

        Quaternion newRotation = startRotation * Quaternion.Euler(currentX, 0f, currentZ);

        if (rb != null)
            rb.MoveRotation(newRotation);
        else
            transform.rotation = newRotation;
    }

    Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null)
            return input;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y = 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y = -1f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x = 1f;

        return input;
    }
}