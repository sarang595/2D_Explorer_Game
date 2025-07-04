using UnityEngine;

public class MoveObject : MonoBehaviour
{
    Rigidbody2D rb2;
    private void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        //rb2.bodyType = RigidbodyType2D.Kinematic;
    }
    private void Update()
    {
       // Moveobj();
    }
    public void Moveobj()
    {
        bool moveable = PlayerController.Instance.CanPush();
        if (moveable)
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb2.bodyType = RigidbodyType2D.Kinematic;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        bool moveable = PlayerController.Instance.CanPush();
        if (collision.gameObject.CompareTag("Player"))
        {

          
            if (moveable)
            {
                rb2.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rb2.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}
