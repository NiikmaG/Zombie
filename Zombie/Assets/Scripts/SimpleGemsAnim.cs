using UnityEngine;

public class SimpleGemsAnim : MonoBehaviour
{
    public bool rotate = true;
    public bool rotateX;
    public bool rotateY = true;
    public bool rotateZ;

    public float rotationSpeed = 90f;

    public bool floatUpDown = true;
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;

    public bool scaleAnimation;
    public Vector3 smallScale = Vector3.one;
    public Vector3 bigScale = new Vector3(1.15f, 1.15f, 1.15f);
    public float scaleSpeed = 2f;

    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private float timer;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;

        if (smallScale == Vector3.zero)
            smallScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (rotate)
            RotateGem();

        if (floatUpDown)
            FloatGem();

        if (scaleAnimation)
            ScaleGem();
    }

    void RotateGem()
    {
        Vector3 axis = new Vector3(
            rotateX ? 1f : 0f,
            rotateY ? 1f : 0f,
            rotateZ ? 1f : 0f
        );

        transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.Self);
    }

    void FloatGem()
    {
        float y = Mathf.Sin(timer * floatSpeed) * floatHeight;
        transform.localPosition = startLocalPosition + new Vector3(0f, y, 0f);
    }

    void ScaleGem()
    {
        float t = (Mathf.Sin(timer * scaleSpeed) + 1f) / 2f;
        transform.localScale = Vector3.Lerp(smallScale, bigScale, t);
    }
}