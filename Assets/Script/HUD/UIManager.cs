using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : GameService<UIManager>
{
    [HideInInspector]
    public Transform Player;
    [SerializeField] public int PlayerHealth = 5;
    [SerializeField] public float Stamina = 5;
    [SerializeField]
    [Range(0, 5)] public int MaxHealth = 5;
    public GameObject gameOverScreen;
    public GameObject LoadingScreen;
    bool isGameover = false;
    bool isReloaded = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(InitiatePlayerDelayed());
    }

    private IEnumerator InitiatePlayerDelayed()
    {
        yield return new WaitForEndOfFrame();
        InitiatePlayer();
    }

    public void InitiatePlayer()
    {
        if (this.Player == null)
        {
            PlayerController playercontrollerInstance = PlayerController.Instance;
            if (playercontrollerInstance != null)
            {
                this.Player = PlayerController.Instance.transform;
            }
            else
            {
                Debug.LogWarning("PlayerController not found, retrying...");
                Invoke(nameof(InitiatePlayer), 0.1f);
            }
        }
    }

    public int HealthDamage(int damage)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.isDamage = true;
        }

        PlayerHealth -= damage;
        if (PlayerHealth < 0) PlayerHealth = 0;
        Debug.Log("Player health is now: " + PlayerHealth);

        if (PlayerHealth <= 0 && !isGameover)
        {
            LoadGameOver();
        }
        DamageReset(0.1f);
        return PlayerHealth;
    }

    IEnumerator DamageReset(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.isDamage = false;
        }
    }

    public async void LoadGameOver()
    {
        if (isGameover) return;
        if (PlayerController.Instance == null) return;
        PlayerAction _playerAction = PlayerController.Instance.GetComponent<PlayerAction>();
        if (PlayerController.Instance.getPlayerState() == PlayerController.PlayerState.Dead)
        {
            if (_playerAction != null)
            {
                _playerAction.Dead();
            }
            await GameOverscene();
        }
    }

    private async Awaitable GameOverscene()
    {
        isGameover = true;
        await Awaitable.WaitForSecondsAsync(0.8f);
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void ReloadScene()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        if (LoadingScreen != null)
        {
            LoadingScreen.SetActive(true);
        }
        HealthBar healthBar = FindAnyObjectByType<HealthBar>();
        if (healthBar != null)
        {
            PlayerHealth = MaxHealth;
            healthBar.RestoreAllHeart();
            SceneManager.LoadScene(CurrentSceneIndex);
        }
        else
        {
            Debug.LogWarning("HealthBar not found in scene!");
            SceneManager.LoadScene(CurrentSceneIndex);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameover = false;
        isReloaded = true;
        if (LoadingScreen != null)
        {
            LoadingScreen.SetActive(false);
        }
    }
}
