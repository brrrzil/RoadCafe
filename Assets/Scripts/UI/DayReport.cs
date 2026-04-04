using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DayReport : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private TMP_Text dayNumberText;
    [SerializeField] private TMP_Text moneyEarnedText;
    [SerializeField] private TMP_Text moneySpentText;
    [SerializeField] private TMP_Text pollutionChangeText;
    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button exitButton;    

    private void Start()
    {
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(OnNextDayClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (DayManager.Instance != null)
        {
            DayManager.OnNewDayStarted += OnNewDayStarted;
        }

        gameObject.SetActive(false);
    }

    public void UpdateReportUI()
    {
        Debug.Log("DayReport UpdateReportUI called");

        if (DayManager.Instance == null) return;

        float moneyEarned = DayManager.Instance.GetMoneyEarnedToday();
        float moneySpent = DayManager.Instance.GetMoneySpentToday();
        float pollutionChange = DayManager.Instance.GetPollutionChangeToday();
        int currentDay = DayManager.Instance.GetCurrentDay();

        if (dayNumberText != null)
            dayNumberText.text = $"День {currentDay} завершён";

        if (moneyEarnedText != null)
            moneyEarnedText.text = $"Денег заработано: {moneyEarned:F0}";

        if (moneySpentText != null)
            moneySpentText.text = $"Денег потрачено: {moneySpent:F0}";

        if (totalMoneyText != null)
            totalMoneyText.text = $"Баланс: {Economy.CurrentMoney:F0}";

        if (pollutionChangeText != null)
        {
            string sign = pollutionChange >= 0 ? "+" : "";
            pollutionChangeText.text = $"Загрязнение: {sign}{pollutionChange:F1}%";
            pollutionChangeText.color = pollutionChange >= 0 ? Color.red : Color.green;
        }
    }

    private void OnNewDayStarted()
    {
        gameObject.SetActive(false);
    }

    private void OnNextDayClicked()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.NextDay();
        }
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (nextDayButton != null)
            nextDayButton.onClick.RemoveListener(OnNextDayClicked);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);

        if (DayManager.Instance != null)
        {
            DayManager.OnNewDayStarted -= OnNewDayStarted;
        }
    }

    private void OnEnable()
    {
        Debug.Log("DayReport OnEnable called");
    }
}