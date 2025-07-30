using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [SerializeField] private string spawnPointName = "DefaultSpawn";
    [SerializeField] private bool isDefaultSpawn = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 1f;

    public string SpawnPointName => spawnPointName;
    public bool IsDefaultSpawn => isDefaultSpawn;

    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }

    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }

    private void OnDrawGizmos()
    {
        // Draw a visual indicator in the scene view
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);

        // Draw an arrow to show spawn direction
        Vector3 forward = transform.right; // Assuming 2D game where right is forward
        Gizmos.DrawRay(transform.position, forward * gizmoSize);

        // Draw spawn point name
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoSize + 0.5f), spawnPointName);
#endif
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a more prominent indicator when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, gizmoSize * 0.3f);
    }
}