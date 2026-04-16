using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI progressText;

    [Header("Result UI")]
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI finalScoreText;
    public Button actionButton; // Restart or Next Level

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void ShowGamePanel()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        resultPanel.SetActive(false);
        UpdateGameUI();
    }

    public void ShowResultPanel(bool isWin, int score)
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(false);
        resultPanel.SetActive(true);

        resultTitleText.text = isWin ? "Level Complete!" : "Game Over!";
        finalScoreText.text = "Score: " + score;
        
        TextMeshProUGUI btnText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = isWin ? "Next Level" : "Try Again";
    }

    public void UpdateGameUI()
    {
        if (GameManager.Instance == null) return;
        
        scoreText.text = "Score: " + GameManager.Instance.score;
        comboText.text = GameManager.Instance.combo > 0 ? "Combo: " + GameManager.Instance.combo : "";
        levelText.text = "Level: " + GameManager.Instance.currentLevel;
        progressText.text = "Progress: " + GameManager.Instance.successfulFits + "/" + GameManager.Instance.targetFits;
    }

    // Button actions
    public void OnStartButtonClick()
    {
        GameManager.Instance.StartLevel(1);
        ShowGamePanel();
    }

    public void OnActionButtonClick()
    {
        if (GameManager.Instance.currentState == GameState.LevelComplete)
        {
            GameManager.Instance.StartLevel(GameManager.Instance.currentLevel + 1);
            ShowGamePanel();
        }
        else
        {
            GameManager.Instance.StartLevel(1);
            GameManager.Instance.score = 0;
            ShowGamePanel();
        }
    }
}
