using UnityEngine;
using UnityEngine.InputSystem;

public class MazeController : MonoBehaviour
{
    public float tiltSpeed = 45f;
    public float maxTilt = 15f;

    private Rigidbody rb;
    private float tiltForward;
    private float tiltSide;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector2 input = GetInput();

        tiltForward += input.y * tiltSpeed * Time.fixedDeltaTime;
        tiltSide -= input.x * tiltSpeed * Time.fixedDeltaTime;

        tiltForward = Mathf.Clamp(tiltForward, -maxTilt, maxTilt);
        tiltSide = Mathf.Clamp(tiltSide, -maxTilt, maxTilt);

        Quaternion rotation = Quaternion.Euler(tiltForward, 0f, tiltSide);

        if (rb != null)
            rb.MoveRotation(rotation);
        else
            transform.rotation = rotation;
    }

    Vector2 GetInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x += 1f;

        return input;
    }
}