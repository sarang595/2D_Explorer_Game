using UnityEngine;
using System.Collections;

public class UIManager : GameService<UIManager>
{
    [HideInInspector]
    public Transform Player;
    [SerializeField] public int PlayerHealth;
    [SerializeField] public float Stamina;
    [SerializeField]
    [Range(0,5)] public int MaxHealth = 5;

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
}
