using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
   
 private   void OnTriggerEnter2D(Collider2D collision)
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
                UIManager.Instance.LoadGameOver();
             }
            else
            {
                Debug.LogError("HealthBar not found in scene!");
            }
            
        }
    }
}
   
   
}
