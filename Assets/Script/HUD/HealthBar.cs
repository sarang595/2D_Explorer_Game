using System.Collections.Generic;
using UnityEngine;
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
    int _maxHealth;
    int heartIndex;
    void Start()
    {
        PlayerInitialization();
        InitiateHealthBar();
        createHeart();   
    }


    void Update()
    {
        ReduceLife();
       
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
            layoutGroup.spacing = Spacing;    // Adjust spacing at runtime if needed
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

     void ReduceLife()
    {
        currentHealth = PlayerController.Instance.CurrentPlayerHealth;        
        bool IsDamage = PlayerController.Instance.Damage();
        bool ReduceHeart = heartIndex >= 0 && heartIndex <= instantiatedHealthanim.Count && instantiatedHealthanim != null;
        if (IsDamage)
        {            
                heartIndex = currentHealth;
                if(ReduceHeart)
                {              
                  instantiatedHealthanim[heartIndex].SetBool("HealthDown", true); 
                }     
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
