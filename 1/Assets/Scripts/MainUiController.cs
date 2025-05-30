using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainUiController : MonoBehaviour
{
    [SerializeField] GameObject stagePanel;
    [SerializeField] GameObject characterPanel;
    [SerializeField] GameObject optionsPanel;

    [SerializeField] TextMeshProUGUI totalCoinText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    public void OpenStagePanel() {
        stagePanel.SetActive(true);
        characterPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OpenCharacterPanel() {
        characterPanel.SetActive(true);
        stagePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OpenOptionsPanel() {
        optionsPanel.SetActive(true);
        stagePanel.SetActive(false);
        characterPanel.SetActive(false);
    }

    public void BackToTitle() {
        SceneManager.LoadScene("Title");
    }

    public void StartStage1() {
        SceneManager.LoadScene("Stage1");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int totalCoin = PlayerPrefs.GetInt("TotalCoin", 0);
        totalCoinText.text = GameManager.AbbreviateNumber(totalCoin);

        // 저장된 최고 점수 표시
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
