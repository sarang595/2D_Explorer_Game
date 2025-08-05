using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Transform Player;
    int CurrentHealth;
    int MaxHealth = 5;
    public GameObject heartPrefab;
    public GameObject heartfill;
    public Animator Healthanim;
    float Spacing = -40f;
    void Start()
    {
        InitiateHealthBar();
        createHeart();
    }


    void Update()
    {
        ReduceLife();


    }
    void InitiateHealthBar()
    {
        var layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = Spacing;    // Adjust spacing at runtime if needed
        }

        Healthanim = GetComponent<Animator>();
      
        if (this.Player == null)
        {
            PlayerController playercontrollerInstance = PlayerController.Instance;
            if (playercontrollerInstance != null)
            {
                this.Player = PlayerController.Instance.transform;
            }
            if (this.Player == null)
            {
                return;
            }
        }
        CurrentHealth = PlayerController.Instance.PlayerHealth;
    }
    void ReduceLife()
    {
        bool IsDamage = PlayerController.Instance.Damage();
        if(IsDamage)
        {
            Healthanim.SetBool("HealthDown", true);
        }
    }
    void createHeart()
    {
        for(int i =0; i < MaxHealth; i++)
        {
            GameObject Heart = Instantiate(heartPrefab, transform);
            GameObject Hearfill = Instantiate(heartfill, Heart.transform);
        }
    }
       
}
