using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private GameObject confirmPanel;

    [SerializeField] private GameObject gameOverPanel;


    [SerializeField] private Slider bossHpSlider; // 추가!

    [SerializeField] public GameObject coinPrefab;

    private int coin = 0;

    [HideInInspector] public bool isGameOVer = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void IncreaseCoin() {
        coin += 1;
        text.SetText(coin.ToString());

    }

    public void SaveCoin() {
    int totalCoin = PlayerPrefs.GetInt("TotalCoin", 0);
    totalCoin += coin; // 현재 플레이에서 모은 코인을 더한다
    PlayerPrefs.SetInt("TotalCoin", totalCoin);
    PlayerPrefs.Save();
    }

    public void SetGameOver() {
        isGameOVer = true;

        EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner != null) {
            enemySpawner.StopEnemyRoutine();
        }

        DebrisSpawner debrisSpawner = FindFirstObjectByType<DebrisSpawner>();
        if (enemySpawner != null) {
            debrisSpawner.StopDebrisRoutine();
        }

        SaveCoin();
        Invoke("ShowGameOverPanel", 1f); // 1초뒤에 게임오버패널을 띄어줌
        HideBossHpBar();

    }


    void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain() {
        SceneManager.LoadScene("Stage1");
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowBossHpBar(float maxHp) {
        bossHpSlider.gameObject.SetActive(true);
        bossHpSlider.maxValue = maxHp;
        bossHpSlider.value = maxHp;
    }

     public void UpdateBossHpBar(float currentHp) {
        bossHpSlider.value = currentHp;
    }

    public void HideBossHpBar() {
        bossHpSlider.gameObject.SetActive(false);
    }



    // 일시정지 메뉴 열기
    public void OpenPauseMenu() {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
    }

    // 일시정지 메뉴 닫기 (Resume)
    public void ClosePauseMenu() {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }

    // 메인메뉴로 돌아가기
    public void PauseBackToMainMenu()
    {
        Time.timeScale = 1f; // 반드시 먼저 게임 재개 시키고 이동해야 함!
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenConfirmPanel() {
        pauseMenuPanel.SetActive(false);
        confirmPanel.SetActive(true);
    }

    // '예' 버튼: 메인메뉴로 이동
    public void ConfirmYes() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // '아니요' 버튼: 다시 PauseMenuPanel로 복귀
    public void ConfirmNo() {
        confirmPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
