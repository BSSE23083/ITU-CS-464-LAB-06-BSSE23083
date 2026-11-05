using UnityEngine;

public class TimedSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] float first = 1f, interval = 1.6f;
    [SerializeField] int maxAlive = 4;
    [SerializeField] Vector2 areaHalfSize = new Vector2(8f, 8f);
    [SerializeField] float height = 0.6f, playerClearance = 2f, spawnRadius = 0.6f;
    [SerializeField] LayerMask keepClearOf;
    [SerializeField] Transform player;

    void Start() => InvokeRepeating(nameof(Spawn), first, interval);

    void Spawn()
    {
        if (transform.childCount >= maxAlive) return;
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 p = transform.position + new Vector3(
                Random.Range(-areaHalfSize.x, areaHalfSize.x), height, Random.Range(-areaHalfSize.y, areaHalfSize.y));
            if (Vector3.Distance(p, player.position) < playerClearance) continue;
            if (Physics.CheckSphere(p, spawnRadius, keepClearOf, QueryTriggerInteraction.Collide)) continue;
            if (!Physics.Raycast(p, Vector3.down, 2f)) continue;   // never over a pit
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], p, Quaternion.identity, transform);
            return;
        }
    }
}
