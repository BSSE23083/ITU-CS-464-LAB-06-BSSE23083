using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Objective { CollectCoins, ReachGoal, SurviveTimer, CoinsThenGoal }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State { Playing, Won, Lost }

    [SerializeField] Objective objective = Objective.CollectCoins;
    [SerializeField] int targetScore = 10, lives = 3;
    [SerializeField] float timeLimit = 0f;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] bool isFinalLevel;
    [SerializeField] string levelTitle = "Level";
    [SerializeField] string objectiveLine = "";
    [SerializeField] string scoreLabel = "Coins";
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] GoalZone goal;
    [SerializeField] TMP_Text titleText, scoreText, livesText, timerText, statusText, winText;
    [SerializeField] GameObject winPanel, losePanel;

    int score;
    float timeLeft;
    bool respawning;
    PlayerController controller;

    public State CurrentState { get; private set; } = State.Playing;
    public bool Respawning => respawning;
    public int Score => score;
    public int Lives => lives;
    public float TimeLeft => timeLeft;
    public Objective LevelObjective => objective;
    public bool GoalOpen => objective == Objective.ReachGoal || score >= targetScore;
    public Health Player => player;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        controller = player.GetComponent<PlayerController>();
        timeLeft = timeLimit;
    }

    void Start()
    {
        titleText.text = levelTitle + "\n" + objectiveLine;
        if (goal != null) goal.SetOpen(objective == Objective.ReachGoal);
        UpdateUI();
    }

    void Update()
    {
        if (CurrentState != State.Playing) return;
        UpdateStatus();
        if (timeLimit <= 0f) return;
        timeLeft -= Time.deltaTime;
        UpdateTimer();
        if (timeLeft > 0f) return;
        if (objective == Objective.SurviveTimer) Win();
        else Lose();
    }

    public void AddScore(int amount)
    {
        if (CurrentState != State.Playing) return;
        score += amount;
        UpdateUI();
        if (score < targetScore) return;
        if (objective == Objective.CollectCoins) Win();
        else if (objective == Objective.CoinsThenGoal && goal != null) goal.SetOpen(true);
    }

    public void ReachedGoal()
    {
        if (CurrentState == State.Playing && !respawning && GoalOpen) Win();
    }

    public void SetCheckpoint(Transform point) => respawnPoint = point;

    public void UpdateUI()
    {
        bool counts = objective == Objective.CollectCoins || objective == Objective.CoinsThenGoal;
        scoreText.text = counts ? $"{scoreLabel}: {score} / {targetScore}" : "";
        livesText.text = $"Lives: {Mathf.Max(lives, 0)}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
        UpdateTimer();
        UpdateStatus();
    }

    void UpdateTimer()
    {
        timerText.text = timeLimit > 0f ? $"Time: {Mathf.CeilToInt(Mathf.Max(timeLeft, 0f))}" : "";
    }

    void UpdateStatus()
    {
        if (controller.BoostRemaining > 0f) statusText.text = "Speed Boost";
        else if (player.shielded) statusText.text = "Shield";
        else if (objective == Objective.CoinsThenGoal && GoalOpen) statusText.text = "Exit Open";
        else statusText.text = "";
    }

    public void PlayerDied()
    {
        if (CurrentState != State.Playing || respawning) return;
        lives--;
        UpdateUI();
        if (lives <= 0) Lose();
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        respawning = true;
        controller.enabled = false;
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        player.Restore(spawnProtection);
        controller.enabled = true;
        respawning = false;
    }

    void Win()
    {
        CurrentState = State.Won;
        winText.text = isFinalLevel ? "You Finished The Game" : "You Win";
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        CurrentState = State.Lost;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
