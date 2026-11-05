using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int firstWave = 2, extraPerWave = 2;
    [SerializeField] float spawnGap = 0.5f, restBetweenWaves = 3f, startDelay = 2f;
    int wave;

    public int Wave => wave;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            wave++;
            int count = firstWave + (wave - 1) * extraPerWave;
            for (int i = 0; i < count; i++)
            {
                Transform at = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefab, at.position, at.rotation, transform);
                yield return new WaitForSeconds(spawnGap);
            }
            yield return new WaitUntil(() => transform.childCount == 0);
            yield return new WaitForSeconds(restBetweenWaves);
        }
    }
}
