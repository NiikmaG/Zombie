using UnityEngine;

public class DoorZone : MonoBehaviour
{
    public bool rightDoor;

    private GameManager game;

    void Start()
    {
        game = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody)
            return;

        if (rightDoor)
            game.TryRightDoor();
        else
            game.WrongDoor();
    }
}