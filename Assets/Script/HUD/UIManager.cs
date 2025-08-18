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
    bool isGameover = false;


    private void Start()
    {

        StartCoroutine(InitiatePlayerDelayed());
    }
    
    private IEnumerator InitiatePlayerDelayed()
    {
        // Wait for other singletons to initialize
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
                // Retry after a short delay
                Invoke(nameof(InitiatePlayer), 0.1f);
            }
        }
    }
    public int HealthDamage(int damage)
    {
        PlayerController.Instance.isDamage = true;
        PlayerHealth -= damage;
        if (PlayerHealth < 0) PlayerHealth = 0;
        Debug.Log("Player health is now: " + PlayerHealth);  // <-- Add this line
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
        PlayerController.Instance.isDamage = false;
    }

    public async void LoadGameOver()
    {
        if (isGameover) return;
        if (PlayerController.Instance == null) return;
        PlayerAction _playerAction = PlayerController.Instance.GetComponent<PlayerAction>();
        if (PlayerController.Instance.getPlayerState() == PlayerController.PlayerState.Dead)
        {
            _playerAction.Dead();
            await GameOverscene();
        
        }

    }
    private async Awaitable GameOverscene()
    {
        isGameover = true;
        await Awaitable.WaitForSecondsAsync(0.8f);
        gameOverScreen.SetActive(true);

    }
    public  void ReloadScene()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        gameOverScreen.SetActive(false);
        HealthBar healthBar = FindAnyObjectByType<HealthBar>();
        if (healthBar != null)
        {
            PlayerHealth = MaxHealth;
            healthBar.RestoreAllHeart();
            SceneManager.LoadScene(CurrentSceneIndex);

        }

    }
}