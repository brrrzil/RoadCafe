using System.Collections;
using UnityEngine;
using System;

public class Ecology : MonoBehaviour
{
    [Header("Пассивное уменьшение")]
    [SerializeField, Range(0, 10)] private float passiveDecreasePerSecond = 0.5f;
    [SerializeField] private float decreaseInterval = 1f;

    [Header("Лимиты")]
    [SerializeField, Range(0, 100)] private float maxPollution = 100f;
    [SerializeField, Range(0, 100)] private float minPollution = 0f;

    private static float currentPollution = 0f;
    private Coroutine passiveDecreaseCoroutine;

    public static event Action<float> OnPollutionChanged;
    public static float CurrentPollution => currentPollution;
    public static float NormalizedPollution => currentPollution / 100f;

    public static Ecology Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentPollution = Mathf.Clamp(0f, minPollution, maxPollution);
    }

    private void Start()
    {
        StartPassiveDecrease();
    }

    private void StartPassiveDecrease()
    {
        if (passiveDecreaseCoroutine != null)
            StopCoroutine(passiveDecreaseCoroutine);

        passiveDecreaseCoroutine = StartCoroutine(PassiveDecreaseRoutine());
    }

    private IEnumerator PassiveDecreaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);

            if (currentPollution > minPollution)
            {
                float decrease = passiveDecreasePerSecond * decreaseInterval;
                ChangePollution(-decrease);
            }
        }
    }

    // Метод для изменения загрязнения с течением времени
    public static void StartPollutionOverTime(float ratePerSecond, float duration)
    {
        if (Instance == null) return;
        Instance.StartCoroutine(Instance.PollutionOverTimeRoutine(ratePerSecond, duration));
    }

    private IEnumerator PollutionOverTimeRoutine(float ratePerSecond, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float change = ratePerSecond * Time.deltaTime;
            ChangePollution(change);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Мгновенное изменение загрязнения
    public static void AddPollution(float amount)
    {
        if (amount <= 0) return;
        ChangePollution(amount);
    }

    public static void RemovePollution(float amount)
    {
        if (amount <= 0) return;
        ChangePollution(-amount);
    }

    private static void ChangePollution(float delta)
    {
        if (delta == 0) return;

        float newPollution = currentPollution + delta;

        if (Instance != null)
        {
            newPollution = Mathf.Clamp(newPollution, Instance.minPollution, Instance.maxPollution);
        }

        currentPollution = newPollution;
        OnPollutionChanged?.Invoke(currentPollution);
    }

    public static void CleanPollution(float amount)
    {
        RemovePollution(amount);
    }

    public static void ResetPollution(float startValue = 0f)
    {
        float min = Instance != null ? Instance.minPollution : 0f;
        float max = Instance != null ? Instance.maxPollution : 100f;
        currentPollution = Mathf.Clamp(startValue, min, max);
        OnPollutionChanged?.Invoke(currentPollution);
    }

    private void OnDestroy()
    {
        if (passiveDecreaseCoroutine != null)
            StopCoroutine(passiveDecreaseCoroutine);
    }
}