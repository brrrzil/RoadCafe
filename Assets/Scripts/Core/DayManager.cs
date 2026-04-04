using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    [Header("Настройки дня")]
    [SerializeField] private float dayDurationInSeconds = 180f;
    [SerializeField] private GameObject reportPanel;

    private float timeRemaining;
    private bool isDayActive = true;
    private static int currentDay = 1;
    public static int CurrentDay {  get { return currentDay; } }

    private float moneyAtStart;
    private float pollutionAtStart;

    public static event Action<float> OnTimeUpdated;
    public static event Action OnDayEnded;
    public static event Action OnNewDayStarted;

    public static DayManager Instance { get; private set; }

    public float CurrentTimeRemaining => timeRemaining;
    public bool IsDayActive => isDayActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (currentDay == 0)
        {
            currentDay = 1;
        }
    }

    private void Start()
    {
        // Ищем панель отчёта на Canvas
        if (reportPanel == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                reportPanel = canvas.transform.Find("DayReportPanel")?.gameObject;
            }
        }

        if (reportPanel != null)
        {
            reportPanel.SetActive(false);
        }

        StartNewDay();
    }

    private void Update()
    {
        if (!isDayActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            OnTimeUpdated?.Invoke(timeRemaining);

            if (timeRemaining <= 0)
            {
                EndDay();
            }
        }
    }

    public void StartNewDay()
    {
        timeRemaining = dayDurationInSeconds;
        isDayActive = true;
        Time.timeScale = 1f;

        moneyAtStart = Economy.CurrentMoney;
        pollutionAtStart = Ecology.CurrentPollution;

        if (reportPanel != null)
        {
            reportPanel.SetActive(false);
        }

        OnNewDayStarted?.Invoke();
        Debug.Log($"День {currentDay} начался");
    }

    private void EndDay()
    {
        Debug.Log("EndDay called");

        isDayActive = false;
        Time.timeScale = 0f;

        StopAllCustomers();
        StopPlayer();

        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            DayReport report = reportPanel.GetComponent<DayReport>();
            if (report != null)
            {
                report.UpdateReportUI();
            }
        }
        else
        {
            Debug.LogError("Report panel is null!");
        }

        OnDayEnded?.Invoke();
        Debug.Log($"День {currentDay} закончен");
    }

    private void StopAllCustomers()
    {
        CustomerBrain[] customers = FindObjectsByType<CustomerBrain>(FindObjectsSortMode.None);
        foreach (CustomerBrain customer in customers)
        {
            customer.StopMovement();
        }
    }

    private void StopPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            UnityEngine.AI.NavMeshAgent agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
    }

    public void NextDay()
    {
        currentDay++;
        StartNewDay();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public float GetMoneyEarnedToday()
    {
        return Mathf.Max(0, Economy.CurrentMoney - moneyAtStart);
    }

    public float GetMoneySpentToday()
    {
        return Mathf.Max(0, moneyAtStart - Economy.CurrentMoney);
    }

    public float GetPollutionChangeToday()
    {
        return Ecology.CurrentPollution - pollutionAtStart;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public string FormatTime(float time)
    {
        if (time < 0) time = 0;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}