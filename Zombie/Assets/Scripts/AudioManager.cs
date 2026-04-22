using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public float musicVolume = 0.35f;

    [Header("SFX")]
    public AudioClip collectibleSound;
    public AudioClip zombieDeathSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public float sfxVolume = 0.8f;

    [Header("Volume UI")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float masterVolume = 1f;

    void Awake()
    {
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(masterVolume);
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        ApplyVolume();
        PlayMusic();
    }

    void PlayMusic()
    {
        if (backgroundMusic == null)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);

        ApplyVolume();

        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
    }

    void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (volumeText != null)
            volumeText.text = "Volume: " + Mathf.RoundToInt(masterVolume * 100f) + "%";
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlayCollectibleSound()
    {
        PlaySfx(collectibleSound);
    }

    public void PlayZombieDeathSound()
    {
        PlaySfx(zombieDeathSound);
    }

    public void PlayWinSound()
    {
        PlaySfx(winSound);
    }

    public void PlayLoseSound()
    {
        PlaySfx(loseSound);
    }
}