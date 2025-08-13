using UnityEditor;
using UnityEngine;

//public class ProjectileBehaviour : MonoBehaviour
//{
//    private Transform target;
//    private float ProjectileSpeed;
//    // Vector2 midpoint;

//    private float journeyTime = 0f;
//    private float journeyDuration = 2f;


//    void Start()
//    {

//    }


//    //void Update()
//    //{
//    //    // Vector3 ProjectileDirection = (target.position - transform.position).normalized;
//    //    //// transform.position += ProjectileDirection * ProjectileSpeed * Time.deltaTime;
//    //    // //midpoint = ((target.transform.position + transform.position) / 2);
//    //    // //midpoint.y = midpoint.y + 5;
//    //    //Vector3 slp = Vector3.Slerp(target.position, transform.position, 0f);
//    //    //transform.position += slp * ProjectileSpeed * Time.deltaTime;
//    //    // Debug.Log(slp);
//    //    //// Debug.Log(hoffst);
//    //    // if (Vector2.Distance(transform.position, target.position) < 1f)
//    //    // {
//    //    //     Destroy(gameObject);
//    //    // }


//    //}
//    //private void OnDrawGizmos()
//    //{

//    //    Gizmos.color = Color.red;
//    //    Gizmos.DrawSphere(midpoint, 0.5f);
//    //}
//    void Update()
//    {
//        journeyTime += Time.deltaTime;
//        float t = journeyTime / journeyDuration;

//        if (t <= 1f)
//        {
//            // Get direction vectors (normalized)
//            Vector3 startDirection = Vector3.forward; // Or any initial direction
//            Vector3 targetDirection = (target.position - transform.position).normalized;

//            // Slerp between directions
//            Vector3 currentDirection = Vector3.Slerp(startDirection, targetDirection, t);

//            // Move in the interpolated direction
//            transform.position += currentDirection * ProjectileSpeed * Time.deltaTime;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    public void InitializeProjectile(Transform target, float ProjectileSpeed)
//    {
//        this.target = target;
//        this.ProjectileSpeed = ProjectileSpeed;
//    }

//}
//public void InitializeProjectile(Transform target, float ProjectileSpeed)
//{
//    this.target = target;
//    this.ProjectileSpeed = ProjectileSpeed;
//}
//}
public class ProjectileBehaviour : MonoBehaviour
{
    public Transform target;
    public float ProjectileSpeed = 5f;
    public float maxArcHeight = 5f;
    public float minArcHeight = 1f;
    public float maxDistanceForArc = 10f; // Distance at which arc height reaches maximum

    private Vector3 startPos;
    private Vector3 controlPoint;
    private float journeyTime = 0f;
    private float journeyLength;
    private float dynamicArcHeight;
    float distance;
    int damage;

    void Start()
    {
        startPos = transform.position;

        // Calculate distance and dynamic arc height
         distance = Vector3.Distance(startPos, target.position);
        dynamicArcHeight = CalculateDynamicArcHeight(distance);

        // Create control point for arc using dynamic height
        Vector3 midpoint = (startPos + target.position) / 2f;
        controlPoint = new Vector3(midpoint.x, midpoint.y + dynamicArcHeight, midpoint.z);

        journeyLength = distance / ProjectileSpeed;
    }

    void Update()
    {
        journeyTime += Time.deltaTime;
        float t = journeyTime / journeyLength;

        if (t <= 1f)
        {
            // Quadratic Bezier curve
            transform.position = Mathf.Pow(1 - t, 2) * startPos +
                                2 * (1 - t) * t * controlPoint +
                                Mathf.Pow(t, 2) * target.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private float CalculateDynamicArcHeight(float distance)
    {
        // Clamp distance to prevent values beyond our range
        distance = Mathf.Clamp(distance, 0f, maxDistanceForArc);

        // Linear interpolation between min and max arc height
        float normalizedDistance = distance / maxDistanceForArc;
        return Mathf.Lerp(minArcHeight, maxArcHeight, normalizedDistance);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerController on the collided object
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
               UIManager.Instance.HealthDamage(damage); // Apply damage to player
            }

            Destroy(gameObject); // Optionally destroy the projectile after hitting
        }
    }


    public void InitializeProjectile(Transform target, float ProjectileSpeed, int damage)
    {
        this.target = target;
        this.ProjectileSpeed = ProjectileSpeed;
        this.damage = damage;

        // Recalculate arc height if target changes
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            dynamicArcHeight = CalculateDynamicArcHeight(distance);

            Vector3 midpoint = (transform.position + target.position) / 2f;
            controlPoint = new Vector3(midpoint.x, midpoint.y + dynamicArcHeight, midpoint.z);

            journeyLength = distance / ProjectileSpeed;
            journeyTime = 0f; // Reset journey time
        }
    }
}

