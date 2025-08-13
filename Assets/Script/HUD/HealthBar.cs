using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    Transform _Player;
    public GameObject heartPrefab;
    public GameObject heartfill;
    [HideInInspector]
    public Animator Healthanim;
    float Spacing = -43f;
    private List<Animator> instantiatedHealthanim = new List<Animator>();
    private int currentHealth;
    private int previousHealth = -1; // Track previous health to detect changes
    int _maxHealth;

    void Start()
    {
        PlayerInitialization();
        InitiateHealthBar();
        createHeart();
        UpdateHealthDisplay(); // Set initial state
    }

    void Update()
    {
        UpdateHealthDisplay();
    }

    private void PlayerInitialization()
    {
        _Player = UIManager.Instance.Player;
        _maxHealth = UIManager.Instance.MaxHealth;
    }

    void InitiateHealthBar()
    {
        var layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = Spacing;
        }
    }

    public void createHeart()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            GameObject Heart = Instantiate(heartPrefab, transform);
            GameObject Heartfill = Instantiate(heartfill, Heart.transform);
            Healthanim = Heartfill.GetComponent<Animator>();
            instantiatedHealthanim.Add(Healthanim);
        }
    }

    void UpdateHealthDisplay()
    {
        currentHealth = UIManager.Instance.PlayerHealth;

        // Only update if health changed
        if (currentHealth != previousHealth)
        {
            // Update all hearts based on current health
            for (int i = 0; i < instantiatedHealthanim.Count; i++)
            {
                if (i < currentHealth)
                {
                    // Heart should be active (filled)
                    instantiatedHealthanim[i].SetBool("HealthDown", false);
                }
                else
                {
                    // Heart should be inactive (empty)
                    instantiatedHealthanim[i].SetBool("HealthDown", true);
                }
            }

            previousHealth = currentHealth;
            Debug.Log($"Health updated to: {currentHealth}");
        }
    }
    public void AllHeartLost()
    {
        int maxLife = _maxHealth; 
        if (maxLife > 0)
        {
            //  Simple countdown
            for (int i = maxLife; i >= 0; i--)
            {
            
                int AllLostIndex = i;
                if (AllLostIndex >= 0 && AllLostIndex <instantiatedHealthanim.Count && instantiatedHealthanim !=null)
                {
                    instantiatedHealthanim[AllLostIndex].SetBool("HealthDown", true);
                }
            }
            currentHealth = 0;
        }
        else
        {
            Debug.Log("No hearts to lose!");
            return;
        }
    }
    public void RestoreAllHeart()
    {
        int maxLife = _maxHealth; // Use int instead of float
        if (maxLife > 0)
        {
            //  Simple countdown
            for (int i = maxLife; i >= 0; i--)
            {

                int AllLostIndex = i;
                if (AllLostIndex >= 0 && AllLostIndex < instantiatedHealthanim.Count && instantiatedHealthanim != null)
                {
                    instantiatedHealthanim[AllLostIndex].SetBool("HealthDown", false);
                }
            }
      
        }
        else
        {
            Debug.Log("No hearts to lose!");
            return;
        }
    }

}
