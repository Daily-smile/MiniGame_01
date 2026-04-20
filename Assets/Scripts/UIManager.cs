using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("面板")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    [Header("游戏 UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI progressText;

    [Header("结算 UI")]
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI finalScoreText;
    public Button actionButton; // 重新开始或下一关

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

        resultTitleText.text = isWin ? "关卡完成！" : "游戏结束！";
        finalScoreText.text = "得分: " + score;
        
        TextMeshProUGUI btnText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = isWin ? "下一关" : "再试一次";
    }

    public void UpdateGameUI()
    {
        if (GameManager.Instance == null) return;
        
        scoreText.text = "得分: " + GameManager.Instance.score;
        comboText.text = GameManager.Instance.combo > 0 ? "连击: " + GameManager.Instance.combo : "";
        levelText.text = "关卡: " + GameManager.Instance.currentLevel;
        progressText.text = "进度: " + GameManager.Instance.successfulFits + "/" + GameManager.Instance.targetFits;
    }

    // 按钮动作
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
