using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    public enum State { Patrol, Chase, Return }

    [SerializeField] Transform[] points;
    [SerializeField] float patrolSpeed = 2.4f, chaseSpeed = 4.5f, turnSpeed = 360f;
    [SerializeField] float sightRange = 7f, sightHalfAngle = 40f, loseSightAfter = 1.2f;
    [SerializeField] LayerMask blockers;
    [SerializeField] Transform player;

    State state = State.Patrol;
    int index;
    float lastSeen;
    Vector3 lastSeenAt;
    readonly List<Vector3> trail = new List<Vector3>();

    public State CurrentState => state;
    public float SightRange => sightRange;
    public float SightHalfAngle => sightHalfAngle;

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (CanSee()) StartChase();
                break;
            case State.Chase:
                Chase();
                if (GameManager.Instance.Respawning || Time.time - lastSeen > loseSightAfter) state = State.Return;
                break;
            case State.Return:
                ReturnToRoute();
                if (CanSee()) StartChase();
                break;
        }
    }

    void Patrol()
    {
        Vector3 target = points[index].position;
        Step(target, patrolSpeed);
        if (transform.position == target) index = (index + 1) % points.Length;
    }

    void StartChase()
    {
        state = State.Chase;
        lastSeen = Time.time;
        lastSeenAt = player.position;
        if (trail.Count == 0) trail.Add(transform.position);
    }

    void Chase()
    {
        if (CanSee())
        {
            lastSeen = Time.time;
            lastSeenAt = player.position;
        }
        Vector3 target = new Vector3(lastSeenAt.x, transform.position.y, lastSeenAt.z);
        Vector3 here = transform.position;
        Vector3 next = Vector3.MoveTowards(here, target, chaseSpeed * Time.deltaTime);

        // Try the full step, then slide along one axis if a wall or a pit is in the way.
        if (!CanStandAt(next))
        {
            Vector3 alongX = new Vector3(next.x, here.y, here.z);
            Vector3 alongZ = new Vector3(here.x, here.y, next.z);
            if (CanStandAt(alongX)) next = alongX;
            else if (CanStandAt(alongZ)) next = alongZ;
            else return;
        }

        Face(target - transform.position);
        transform.position = next;
        if (Vector3.Distance(transform.position, trail[trail.Count - 1]) > 0.5f) trail.Add(transform.position);
    }

    // Walk back the way it came, so that it never has to cross a wall to get home.
    void ReturnToRoute()
    {
        if (trail.Count == 0)
        {
            state = State.Patrol;
            return;
        }
        Vector3 target = trail[trail.Count - 1];
        Step(target, patrolSpeed);
        if (transform.position == target) trail.RemoveAt(trail.Count - 1);
    }

    void Step(Vector3 target, float speed)
    {
        Face(target - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void Face(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    bool CanStandAt(Vector3 position)
    {
        bool hitsWall = Physics.CheckSphere(position + Vector3.up * 0.7f, 0.55f, blockers, QueryTriggerInteraction.Ignore);
        bool onFloor = Physics.Raycast(position + Vector3.up, Vector3.down, 2f, blockers, QueryTriggerInteraction.Ignore);
        return !hitsWall && onFloor;
    }

    bool CanSee()
    {
        if (GameManager.Instance.Respawning) return false;
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.magnitude > sightRange) return false;
        if (state != State.Chase &&
            Vector3.Dot(transform.forward, toPlayer.normalized) < Mathf.Cos(sightHalfAngle * Mathf.Deg2Rad)) return false;
        Vector3 eye = new Vector3(transform.position.x, player.position.y, transform.position.z);
        return !Physics.Linecast(eye, player.position, blockers, QueryTriggerInteraction.Ignore);
    }
}
