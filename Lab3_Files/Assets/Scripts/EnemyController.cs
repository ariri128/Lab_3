using UnityEngine;

// Orbits an enemy around the player using a 2D rotation matrix, keeps it facing the player, and scales its orbit speed by how close it currently is to the player (via sqrMagnitude)
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeedDeg = 40f;
    [SerializeField] private float minSpeedMultiplier = 0.5f;
    [SerializeField] private float maxSpeedMultiplier = 2f;
    [SerializeField] private float nearDistance = 2f;
    [SerializeField] private float farDistance = 8f;
    [SerializeField] private float rotationLerpSpeed = 6f;
    [SerializeField] private float orbitCenterFollowSpeed = 4f;

    private float currentAngleDeg;
    private float orbitDirection = 1f;
    private Vector3 currentOrbitCenter;

    // Lets EnemySpawner assign the player reference on a freshly instantiated prefab, before this object's own Start() runs
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    // Lets EnemySpawner give this instance its own orbit radius, so enemies land on different orbit radii
    public void SetOrbitRadius(float radius)
    {
        orbitRadius = radius;
    }

    // Lets EnemySpawner assign +1 (counter-clockwise) or -1 (clockwise) so an entire radius layer spins one way
    public void SetOrbitDirection(float direction)
    {
        orbitDirection = direction;
    }

    private void Start()
    {
        // Start each enemy at its own current offset angle so they don't all snap to the same orbit point
        Vector2 startOffset = transform.position - player.position;
        currentAngleDeg = Mathf.Atan2(startOffset.y, startOffset.x) * Mathf.Rad2Deg;
        currentOrbitCenter = player.position;
    }

    private void Update()
    {
        float speedMultiplier = GetSpeedMultiplierByDistance();
        Orbit(speedMultiplier);
        FacePlayer();
    }

    // Advances the orbit angle, then rebuilds the offset vector with a 2D rotation matrix (cos/sin)
    // The orbit's center eases toward the player with Vector3.Lerp rather than snapping to it, so the ring trails behind player movement instead of translating in lockstep
    private void Orbit(float speedMultiplier)
    {
        currentAngleDeg += orbitSpeedDeg * orbitDirection * speedMultiplier * Time.deltaTime;
        float angleRad = currentAngleDeg * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * orbitRadius;
        currentOrbitCenter = Vector3.Lerp(currentOrbitCenter, player.position, orbitCenterFollowSpeed * Time.deltaTime);
        transform.position = currentOrbitCenter + (Vector3)offset;
    }

    // Computes the angle to the player with Atan2, then rotates toward it smoothly with Quaternion.Slerp
    private void FacePlayer()
    {
        Vector2 direction = player.position - transform.position;
        float targetAngleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngleDeg);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    }

    // Uses sqrMagnitude to turn distance-to-player into a speed multiplier: slow when close in, fast out on the rim
    private float GetSpeedMultiplierByDistance()
    {
        float distSqr = ((Vector2)(player.position - transform.position)).sqrMagnitude;
        float t = Mathf.InverseLerp(nearDistance * nearDistance, farDistance * farDistance, distSqr);
        return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, t);
    }

    // Visualizes the orbit radius in the Scene view so it's easy to tune without hunting through the Inspector
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, orbitRadius);
    }
}