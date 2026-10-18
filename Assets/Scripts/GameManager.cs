using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetScore = 10;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject winPanel;

    [Header("Bonus event")]
    [SerializeField] Spawner spawner;
    [SerializeField] TMP_Text bonusText;
    [SerializeField] int bonusTotal = 50;

    int score;
    int redTriggerScore;
    bool redSpawned;
    int bonusCollected;

    public int Score => score;
    public int TargetScore => targetScore;

    void Awake() => Instance = this;

    void Start()
    {
        redTriggerScore = Random.Range(1, targetScore + 1);   // 1..10 (max is exclusive)
        Debug.Log($"Red coin will spawn at score {redTriggerScore}");
        bonusText.gameObject.SetActive(false);                 // hidden until the bonus starts
        UpdateUI();
    }
public void AddScore(int amount)
{
    score += amount;
    UpdateUI();

    if (score >= targetScore)          // reaching the target always ends the game
    {
        Win();
        return;
    }

    if (!redSpawned && score >= redTriggerScore)
    {
        redSpawned = true;
        spawner.SpawnRedCoin();
    }
}

public void AddBonus()
{
    bonusCollected++;
    UpdateBonusUI();
}

    public void CollectRedCoin(Vector3 playerPos)
    {
        bonusCollected = 0;
        bonusText.gameObject.SetActive(true);
        UpdateBonusUI();
        spawner.SpawnBonusCoins(playerPos, bonusTotal);
    }


    void UpdateUI() => scoreText.text = $"Score: {score} / {targetScore}";
    void UpdateBonusUI() => bonusText.text = $"BonusCoins: {bonusCollected}/{bonusTotal}";

    void Win()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}