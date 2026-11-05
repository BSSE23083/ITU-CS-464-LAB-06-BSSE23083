using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] float speed = 3f, walkDistance = 16f;
    float walked;

    void Update()
    {
        float stepLength = speed * Time.deltaTime;
        transform.position += transform.forward * stepLength;
        walked += stepLength;
        if (walked >= walkDistance) Destroy(gameObject);
    }
}
