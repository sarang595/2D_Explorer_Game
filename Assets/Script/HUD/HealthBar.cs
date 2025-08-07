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
    void createHeart()
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
    
  
       
}
