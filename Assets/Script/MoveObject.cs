using UnityEngine;

public class MoveObject : MonoBehaviour
{
    Rigidbody2D rb2;
    bool playerPushing = false;
    Transform player;
    private void Start()
    {
        player = PlayerController.Instance.transform;
        rb2 = GetComponent<Rigidbody2D>();
        rb2.bodyType = RigidbodyType2D.Kinematic;
    }
    private void Update()
    {
        UpdatePhysics();
    }
   private void UpdatePhysics()
    {
        bool moveable = PlayerController.Instance.CanPush();

        if (moveable && playerPushing)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb2.bodyType = RigidbodyType2D.Kinematic;
            rb2.linearVelocity = Vector2.zero;
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
       
        if (collision.gameObject.transform==player)
        {
            playerPushing = true;
        }
    }
    
}
