using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayReport : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private TMP_Text dayNumberText;
    [SerializeField] private TMP_Text moneyEarnedText;
    [SerializeField] private TMP_Text moneySpentText;
    [SerializeField] private TMP_Text pollutionChangeText;
    [SerializeField] private TMP_Text pollutionCurrentText;
    [SerializeField] private TMP_Text squirrelMessageText;
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
        float pollutionCurrent = Ecology.CurrentPollution;
        int currentDay = DayManager.Instance.GetCurrentDay();
        bool squirrelCaught = DayManager.SquirrelCaughtToday;

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
            pollutionChangeText.text = $"Изменение загрязнения: {sign}{pollutionChange:F1}%";
            pollutionChangeText.color = pollutionChange >= 0 ? Color.red : Color.green;
        }

        if (pollutionCurrentText != null)
        {
            pollutionCurrentText.text = $"Текущий уровень загрязнения: {pollutionCurrent:F1}%";

            if (pollutionCurrent >= 90f)
                pollutionCurrentText.color = Color.red;
            else if (pollutionCurrent >= 70f)
                pollutionCurrentText.color = new Color(1f, 0.5f, 0f);
            else if (pollutionCurrent >= 40f)
                pollutionCurrentText.color = Color.yellow;
            else
                pollutionCurrentText.color = Color.gray;
        }

        if (squirrelMessageText != null)
        {
            if (squirrelCaught)
            {
                squirrelMessageText.text = "Сегодня вы поймали белку!";
                squirrelMessageText.color = Color.darkGray;
            }
            else
            {
                squirrelMessageText.text = "Сегодня вы не поймали белку";
                squirrelMessageText.color = Color.darkGray;
            }
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
}