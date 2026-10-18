using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform player;
    [SerializeField] float interval = 1.5f;
    [SerializeField] int maxCoins = 5;
    [SerializeField] float area = 7f;

    [Header("Special coins")]
    [SerializeField] GameObject redCoinPrefab;
    [SerializeField] GameObject blueCoinPrefab;
    [SerializeField] float bonusMinRadius = 2f;
    [SerializeField] float bonusMaxRadius = 6f;

    void Start() => InvokeRepeating(nameof(SpawnCoin), 1f, interval);

    void SpawnCoin()
    {
        if (transform.childCount >= maxCoins) return;
        float x = Random.Range(-area, area);
        float z = Random.Range(-area, area);
        Vector3 pos = new Vector3(x, 0.5f, z);
        if (Vector3.Distance(pos, player.position) < 2f) return;
        Instantiate(coinPrefab, pos, Quaternion.identity, transform);
    }

    // Task 1: exactly one red coin, not parented, so it doesn't use up a maxCoins slot
    public void SpawnRedCoin()
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 20; i++)
        {
            pos = new Vector3(Random.Range(-area, area), 0.5f, Random.Range(-area, area));
            if (Vector3.Distance(pos, player.position) >= 2f) break;
        }
        Instantiate(redCoinPrefab, pos, Quaternion.identity);
    }

    // Task 2: coins in a ring (annulus) around the player
    public void SpawnBonusCoins(Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Vector3.zero;
            for (int t = 0; t < 20; t++)                    // retry until the point is inside the arena
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                float r = Mathf.Sqrt(Random.Range(
                    bonusMinRadius * bonusMinRadius,
                    bonusMaxRadius * bonusMaxRadius));      // sqrt keeps the spread even
                pos = new Vector3(center.x + dir.x * r, 0.5f, center.z + dir.y * r);
                if (Mathf.Abs(pos.x) <= area && Mathf.Abs(pos.z) <= area) break;
            }
            pos.x = Mathf.Clamp(pos.x, -area, area);        // last resort so all 50 always spawn
            pos.z = Mathf.Clamp(pos.z, -area, area);
            Instantiate(blueCoinPrefab, pos, Quaternion.identity);
        }
    }
}