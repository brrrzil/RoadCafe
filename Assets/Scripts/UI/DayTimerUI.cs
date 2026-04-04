using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentDayText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;

    private void Start()
    {
        UpdateDayNumber();

        if (DayManager.Instance != null)
        {
            DayManager.OnTimeUpdated += UpdateTimer;
            DayManager.OnNewDayStarted += OnNewDayStarted; // Подписка на новый день
            UpdateTimer(DayManager.Instance.CurrentTimeRemaining);
        }
        else
        {
            Debug.LogError("DayManager instance not found!");
        }
    }

    private void UpdateTimer(float timeRemaining)
    {
        currentDayText.text = "День " + DayManager.CurrentDay.ToString();
        if (timerText != null && DayManager.Instance != null)
        {
            timerText.text = DayManager.Instance.FormatTime(timeRemaining);
        }

        if (timerFill != null)
        {
            float fillAmount = timeRemaining / 180f; // 180 секунд - длительность дня
            timerFill.fillAmount = fillAmount;

            // Меняем цвет в зависимости от времени
            if (fillAmount < 0.3f)
                timerFill.color = Color.red;
            else if (fillAmount < 0.6f)
                timerFill.color = Color.yellow;
            else
                timerFill.color = Color.green;
        }
    }

    private void OnNewDayStarted()
    {
        UpdateDayNumber();
    }

    private void UpdateDayNumber()
    {
        if (currentDayText != null)
        {
            currentDayText.text = "День " + DayManager.CurrentDay.ToString();
        }
    }

    private void OnDestroy()
    {
        if (DayManager.Instance != null)
        {
            DayManager.OnTimeUpdated -= UpdateTimer;
            DayManager.OnNewDayStarted -= OnNewDayStarted;
        }
    }
}