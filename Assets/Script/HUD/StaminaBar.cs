using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] Image StaminaHolder;
    [SerializeField] Image StaminaFill;

    private float maxStamina;
    private float currentStamina;

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            maxStamina = UIManager.Instance.Stamina;
        }
    }

    private void Update()
    {
        StaminaVisualizer();
    }

    void StaminaVisualizer()
    {
        if (PlayerController.Instance != null)
        {
            // Get current drift time from PlayerAction
            PlayerAction playerAction = PlayerController.Instance.GetComponent<PlayerAction>();
            if (playerAction != null)
            {
                currentStamina = playerAction.driftTime; 

                // Normalize the fill amount (0-1 range)
                StaminaFill.fillAmount = maxStamina > 0 ? currentStamina / maxStamina : 0f;
            }
        }
    }

}
