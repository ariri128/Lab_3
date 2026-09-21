using UnityEngine;

// Spawns a fixed number of EnemyController prefab instances around the player when play starts
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float[] orbitRadii = new float[] { 3f, 5f, 7f, 9f };
    [SerializeField] private int minEnemiesPerRadius = 2;
    [SerializeField] private int maxEnemiesPerRadius = 4;

    private void Start()
    {
        SpawnEnemies();
    }

    // For each radius layer: rolls its own enemy count between min/max, rolls one shared spin direction for the whole layer, then scatters that many enemies at random angles around it
    private void SpawnEnemies()
    {
        foreach (float radius in orbitRadii)
        {
            float layerDirection = Random.value < 0.5f ? 1f : -1f;
            int countForThisLayer = Random.Range(minEnemiesPerRadius, maxEnemiesPerRadius + 1);

            float slotAngleDeg = 360f / countForThisLayer;

            for (int i = 0; i < countForThisLayer; i++)
            {
                // Stratified sampling: one slot per enemy, jittered within a safe fraction of the slot so two enemies on the same ring can never land too close together, while still varying each Play
                float angleDeg = i * slotAngleDeg + Random.Range(-slotAngleDeg * 0.4f, slotAngleDeg * 0.4f);
                float angleRad = angleDeg * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
                Vector3 spawnPosition = player.position + (Vector3)offset;

                EnemyController enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                enemy.SetPlayer(player);
                enemy.SetOrbitRadius(radius);
                enemy.SetOrbitDirection(layerDirection);
            }
        }
    }
}