using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    bool Launched = false;
    public Transform Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!Launched && Player != null)
        {
            // Calculate horizontal direction only
            Vector2 horizontalDirection = new Vector2(Player.position.x - transform.position.x, 0).normalized;

            // Combine horizontal direction with upward force
            Vector2 launchDirection = horizontalDirection * 250 + Vector2.up * 400;

            rb.AddForce(launchDirection, ForceMode2D.Force);
            Launched = true;

        }
    }
}
