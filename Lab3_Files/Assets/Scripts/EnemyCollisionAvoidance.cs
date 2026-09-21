using UnityEngine;

// Nudges this enemy away from nearby enemies so they don't stack on top of each other
// Requires a Collider2D on every enemy, assigned to the layer passed into enemyLayer
public class EnemyCollisionAvoidance : MonoBehaviour
{
    [SerializeField] private float avoidRadius = 1.2f;
    [SerializeField] private float avoidStrength = 3f;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        transform.position += (Vector3)GetSeparationOffset() * avoidStrength * Time.deltaTime;
    }

    // Sums a "push away" vector from every nearby enemy, weighted more heavily the closer they are
    private Vector2 GetSeparationOffset()
    {
        Vector2 separation = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, avoidRadius, enemyLayer);

        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.transform == transform) continue;

            Vector2 away = (Vector2)(transform.position - neighbor.transform.position);
            float distance = away.magnitude;
            if (distance > 0f)
                separation += away.normalized / distance;
        }

        return separation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}
