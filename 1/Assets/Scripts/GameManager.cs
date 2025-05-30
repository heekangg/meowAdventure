using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI text;           // 기존 코인 텍스트
    [SerializeField] private TextMeshProUGUI scoreText;      // 추가: 점수 텍스트
    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("Panels & Prefabs")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Slider      bossHpSlider;
    [SerializeField] public  GameObject coinPrefab;

    // 내부 상태
    private int   coin             = 0;     // 획득 코인 수
    private int   score            = 0;     // 누적 점수
    private float totalDistance    = 0f;    // 누적 이동 거리
    private int   lastDistanceInt  = 0;     // 거리 점수 계산용

    private float scoreTimer = 0f;
    private const float SCORE_INTERVAL = 0.1f;

    [HideInInspector] public bool isGameOVer = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 초기 UI 세팅
        UpdateCoinUI();
        UpdateScoreUI();
        distanceText.SetText("0 m");
        bossHpSlider.gameObject.SetActive(false);
    }

     void Update()
    {
        // 0.1초마다 real score에 +1점
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= SCORE_INTERVAL)
        {
            scoreTimer -= SCORE_INTERVAL;
            AddScore(1);
        }
    }

    public void AddDistance(float meters)
    {
        totalDistance += meters;
        int floorDist = Mathf.FloorToInt(totalDistance);
        distanceText.SetText($"{floorDist} m");

        int delta = floorDist - lastDistanceInt;
        if (delta > 0)
        {
            AddScore(delta * 10);   // 거리 m당 10점
            lastDistanceInt = floorDist;
        }
    }

    /// 코인 획득 처리 (1개당 10점)
    public void IncreaseCoin()
    {
        coin += 1;
        AddScore(10);             // 코인 1개당 10점
        UpdateCoinUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateCoinUI()
    {
        text.SetText( AbbreviateNumber(coin) );
    }

    private void UpdateScoreUI()
    {
        scoreText.SetText( score.ToString() );
    }

    public void SaveCoin()
    {
        int totalCoin = PlayerPrefs.GetInt("TotalCoin", 0);
        totalCoin += coin;
        PlayerPrefs.SetInt("TotalCoin", totalCoin);
        PlayerPrefs.Save();
    }

    public static string AbbreviateNumber(long num)
    {
        if (num >= 1_000_000_000) return (num / 1_000_000_000f).ToString("0.#") + "B";
        if (num >= 1_000_000)     return (num / 1_000_000f).ToString("0.#") + "M";
        if (num >= 1_000)         return (num / 1_000f).ToString("0.#") + "K";
        return num.ToString();
    }

    public void SetGameOver()
    {
        isGameOVer = true;

        var enemySpawner  = FindFirstObjectByType<EnemySpawner>();
        var debrisSpawner = FindFirstObjectByType<DebrisSpawner>();
        if (enemySpawner  != null) enemySpawner.StopEnemyRoutine();
        if (debrisSpawner != null) debrisSpawner.StopDebrisRoutine();

        // 최고 점수 갱신
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore) {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        SaveCoin();
        Invoke(nameof(ShowGameOverPanel), 1f);
        bossHpSlider.gameObject.SetActive(false);
        
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowBossHpBar(float maxHp)
    {
        bossHpSlider.gameObject.SetActive(true);
        bossHpSlider.maxValue = maxHp;
        bossHpSlider.value    = maxHp;
    }

    public void UpdateBossHpBar(float currentHp)
    {
        bossHpSlider.value = currentHp;
    }

    public void HideBossHpBar()
    {
        bossHpSlider.gameObject.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePauseMenu()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseBackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenConfirmPanel()
    {
        pauseMenuPanel.SetActive(false);
        confirmPanel.SetActive(true);
    }

    public void ConfirmYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ConfirmNo()
    {
        confirmPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
