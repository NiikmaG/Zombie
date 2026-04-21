using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameManager game;
    public Transform followPoint;
    public float height = 0.5f;

    public bool rotate = true;
    public float rotationSpeed = 90f;

    public bool floatAnimation = true;
    public float floatHeight = 0.2f;
    public float floatSpeed = 2f;

    private Vector3 baseScale;
    private float timer;

    void Awake()
    {
        baseScale = transform.localScale;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.isTrigger = true;
        }
    }

    void LateUpdate()
    {
        if (followPoint == null)
            return;

        timer += Time.deltaTime;

        float extraHeight = 0f;

        if (floatAnimation)
            extraHeight = Mathf.Sin(timer * floatSpeed) * floatHeight;

        transform.position = followPoint.position + followPoint.up * (height + extraHeight);

        if (rotate)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.rotation = followPoint.rotation;
        }

        transform.localScale = baseScale;
    }

    void OnTriggerEnter(Collider other)
    {
        ZombieMarker zombie = other.GetComponentInParent<ZombieMarker>();

        if (zombie == null)
            return;

        if (game != null)
            game.TakeCollectible();
    }
}