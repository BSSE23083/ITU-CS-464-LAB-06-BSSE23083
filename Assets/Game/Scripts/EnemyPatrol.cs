using UnityEngine;

public enum PatrolMode { Loop, PingPong, Random }

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] float speed = 2.6f, waitTime = 0f, turnSpeed = 360f;
    int index, step = 1;
    float waitUntil;

    void Update()
    {
        if (Time.time < waitUntil) return;
        Vector3 target = points[index].position, toTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (toTarget != Vector3.zero) transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
        if (transform.position == target) { index = NextIndex(); waitUntil = Time.time + waitTime; }
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop) return (index + 1) % points.Length;
        if (mode == PatrolMode.PingPong)
        {
            if (index + step < 0 || index + step >= points.Length) step = -step;
            return index + step;
        }
        int next = Random.Range(0, points.Length);
        return next == index ? (next + 1) % points.Length : next;
    }

    void OnDrawGizmosSelected()
    {
        if (points == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawWireSphere(points[i].position, 0.3f);
            if (i == points.Length - 1 && mode != PatrolMode.Loop) continue;
            Transform next = points[(i + 1) % points.Length];
            if (next != null) Gizmos.DrawLine(points[i].position, next.position);
        }
    }
}
