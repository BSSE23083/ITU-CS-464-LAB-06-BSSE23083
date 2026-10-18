using UnityEngine;

public enum CoinType { Normal, Red, Blue }

public class Coin : MonoBehaviour
{
    [SerializeField] CoinType type = CoinType.Normal;
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 90f;

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Coin touched by {other.name}");
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case CoinType.Normal:
                GameManager.Instance.AddScore(value);
                break;
            case CoinType.Red:
                GameManager.Instance.CollectRedCoin(other.transform.position);
                break;
            case CoinType.Blue:
                GameManager.Instance.AddBonus();
                break;
        }
        Destroy(gameObject);
    }
}