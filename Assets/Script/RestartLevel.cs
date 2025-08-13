using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
   
 private async void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        int _maxDamage = UIManager.Instance.MaxHealth;
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
             HealthBar healthBar = FindAnyObjectByType<HealthBar>();
            if (healthBar != null)
            {
                UIManager.Instance.HealthDamage(_maxDamage);
                healthBar.AllHeartLost();
                await ReloadScene();
            }
            else
            {
                Debug.LogError("HealthBar not found in scene!");
            }
            
        }
    }
}
    private async Awaitable ReloadScene()
    {
       int _maxHealth = UIManager.Instance.MaxHealth;
        await Awaitable.WaitForSecondsAsync(0.5f);
        HealthBar healthBar = FindAnyObjectByType<HealthBar>();
        if (healthBar != null)
        {
            UIManager.Instance.PlayerHealth = _maxHealth;
            healthBar.RestoreAllHeart();
            int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(CurrentSceneIndex);

        }
       
    }
}
