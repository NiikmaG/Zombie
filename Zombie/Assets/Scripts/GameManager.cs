using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Zombie")]
    public GameObject zombiePrefab;
    public Transform spawnPoint;
    public int zombieCount = 3;
    public float fallY = -5f;

    [Header("Collectibles")]
    public GameObject collectiblePrefab;
    public Transform collectiblePointsParent;
    public int neededCollectibles = 6;
    public float collectibleHeight = 0.5f;

    [Header("Right Door")]
    public GameObject rightDoorBlocker;

    [Header("UI")]
    public TMP_Text zombiesText;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text messageText;
    public TMP_Text resultText;
    public GameObject endPanel;
    public GameObject pausePanel;

    private GameObject currentZombie;
    private GameObject currentCollectible;
    private Transform[] collectiblePoints;

    private int zombiesLeft;
    private int collected;
    private float timer;

    private bool gameEnded;
    private bool rightDoorOpen;
    private bool gamePaused;

    void Start()
    {
        Time.timeScale = 1f;

        zombiesLeft = zombieCount;

        if (endPanel != null)
            endPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (rightDoorBlocker != null)
            rightDoorBlocker.SetActive(true);

        LoadCollectiblePoints();
        SpawnZombie();
        SpawnCollectible();

        UpdateUI();
        UpdateTimer();
    }

    void Update()
    {
        CheckPauseInput();

        if (gameEnded || gamePaused)
            return;

        timer += Time.deltaTime;
        UpdateTimer();

        if (currentZombie != null && currentZombie.transform.position.y < fallY)
            LoseZombie();
    }

    void CheckPauseInput()
    {
        if (gameEnded)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (gamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (gameEnded)
            return;

        gamePaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void LoadCollectiblePoints()
    {
        if (collectiblePointsParent == null)
        {
            Debug.LogWarning("Collectible Points Parent is not assigned.");
            collectiblePoints = new Transform[0];
            return;
        }

        collectiblePoints = new Transform[collectiblePointsParent.childCount];

        for (int i = 0; i < collectiblePointsParent.childCount; i++)
        {
            collectiblePoints[i] = collectiblePointsParent.GetChild(i);
        }

        Debug.Log("Collectible points loaded: " + collectiblePoints.Length);
    }

    void SpawnZombie()
    {
        if (zombiePrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("Zombie prefab or spawn point is missing.");
            return;
        }

        currentZombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
    }

    void SpawnCollectible()
    {
        if (rightDoorOpen)
            return;

        if (collectiblePrefab == null)
        {
            Debug.LogWarning("Collectible prefab is missing.");
            return;
        }

        if (collectiblePoints == null || collectiblePoints.Length == 0)
        {
            Debug.LogWarning("No collectible points found.");
            return;
        }

        Transform point = collectiblePoints[Random.Range(0, collectiblePoints.Length)];

        currentCollectible = Instantiate(collectiblePrefab);

        Collectible collectible = currentCollectible.GetComponent<Collectible>();

        if (collectible != null)
        {
            collectible.game = this;
            collectible.followPoint = point;
            collectible.height = collectibleHeight;
        }

        currentCollectible.transform.position = point.position + point.up * collectibleHeight;
        currentCollectible.transform.rotation = point.rotation;

        Debug.Log("Collectible spawned at: " + point.name);
    }

    public void TakeCollectible()
    {
        if (gameEnded || gamePaused || rightDoorOpen)
            return;

        if (currentCollectible == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCollectibleSound();

        GameObject collectedObject = currentCollectible;
        currentCollectible = null;

        Destroy(collectedObject);

        collected++;

        if (collected >= neededCollectibles)
            OpenRightDoor();
        else
            SpawnCollectible();

        UpdateUI();
    }

    void OpenRightDoor()
    {
        rightDoorOpen = true;

        if (rightDoorBlocker != null)
            rightDoorBlocker.SetActive(false);

        if (messageText != null)
            messageText.text = "Door opened";
    }

    public void TryRightDoor()
    {
        if (gameEnded || gamePaused)
            return;

        if (!rightDoorOpen)
        {
            if (messageText != null)
                messageText.text = "Collect all items first";

            return;
        }

        EndGame("You win");
    }

    public void WrongDoor()
    {
        if (gameEnded || gamePaused)
            return;

        LoseZombie();
    }

    void LoseZombie()
    {
        if (currentZombie == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayZombieDeathSound();

        GameObject lostZombie = currentZombie;
        currentZombie = null;

        Destroy(lostZombie);

        zombiesLeft--;

        if (zombiesLeft <= 0)
        {
            UpdateUI();
            EndGame("You lose");
            return;
        }

        SpawnZombie();
        UpdateUI();
    }

    void EndGame(string result)
    {
        gameEnded = true;
        gamePaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resultText != null)
            resultText.text = result;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            if (result == "You win")
                AudioManager.Instance.PlayWinSound();

            if (result == "You lose")
                AudioManager.Instance.PlayLoseSound();
        }

        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        if (zombiesText != null)
            zombiesText.text = "Zombies: " + zombiesLeft;

        if (scoreText != null)
            scoreText.text = "Score: " + collected + "/" + neededCollectibles;
    }

    void UpdateTimer()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}