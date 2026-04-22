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

    private Vector3 startScale;
    private float timer;
    private bool pickedUp;

    void Awake()
    {
        startScale = transform.localScale;

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
        if (followPoint == null || pickedUp)
            return;

        timer += Time.deltaTime;

        float extraHeight = 0f;

        if (floatAnimation)
            extraHeight = Mathf.Sin(timer * floatSpeed) * floatHeight;

        transform.position = followPoint.position + followPoint.up * (height + extraHeight);

        if (rotate)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        transform.localScale = startScale;
    }

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp)
            return;

        ZombieMarker zombie = other.GetComponentInParent<ZombieMarker>();

        if (zombie == null)
            return;

        pickedUp = true;

        if (game != null)
            game.TakeCollectible();
    }
}