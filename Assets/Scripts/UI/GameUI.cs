using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [Header("Деньги")]
    [SerializeField] private TMP_Text moneyText;

    [Header("Загрязнение")]
    [SerializeField] private Slider pollutionSlider;
    [SerializeField] private Image pollutionFillImage;

    [Header("Цвета загрязнения")]
    [SerializeField] private Color cleanColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color criticalColor = Color.red;

    private float targetSliderValue;
    private float currentSliderValue;
    private bool isCritical = false;
    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        Economy.OnMoneyChanged += UpdateMoneyUI;
        Ecology.OnPollutionChanged += UpdatePollutionUI;
    }

    private void OnDisable()
    {
        Economy.OnMoneyChanged -= UpdateMoneyUI;
        Ecology.OnPollutionChanged -= UpdatePollutionUI;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateMoneyUI(Economy.CurrentMoney);
        UpdatePollutionUI(Ecology.CurrentPollution);

        if (pollutionSlider != null)
        {
            targetSliderValue = pollutionSlider.value;
            currentSliderValue = targetSliderValue;
        }
    }

    private void Update()
    {
        if (pollutionSlider != null && Mathf.Abs(currentSliderValue - targetSliderValue) > 0.01f)
        {
            currentSliderValue = Mathf.Lerp(currentSliderValue, targetSliderValue, Time.deltaTime * 10f);
            pollutionSlider.value = currentSliderValue;
        }
    }

    private void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }
    }

    private void UpdatePollutionUI(float pollution)
    {
        float normalized = pollution / 100f;

        if (pollutionSlider != null)
        {
            targetSliderValue = normalized;

            if (pollutionFillImage != null)
            {
                pollutionFillImage.color = GetPollutionColor(pollution);
            }
        }

        if (pollution >= 90f && !isCritical)
        {
            isCritical = true;
            OnCriticalPollution();
        }
        else if (pollution < 90f && isCritical)
        {
            isCritical = false;
            OnPollutionNormalized();
        }
    }

    private Color GetPollutionColor(float pollution)
    {
        if (pollution >= 90f)
            return criticalColor;
        else if (pollution >= 70f)
            return dangerColor;
        else if (pollution >= 40f)
            return warningColor;
        else
            return cleanColor;
    }

    private void OnCriticalPollution()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        blinkCoroutine = StartCoroutine(BlinkWarning());
    }

    private void OnPollutionNormalized()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (pollutionFillImage != null)
        {
            pollutionFillImage.color = GetPollutionColor(Ecology.CurrentPollution);
        }
    }

    private IEnumerator BlinkWarning()
    {
        float elapsed = 0f;
        float blinkDuration = 2f;
        bool isVisible = true;

        while (elapsed < blinkDuration && isCritical)
        {
            if (pollutionFillImage != null)
            {
                pollutionFillImage.color = isVisible ? criticalColor : cleanColor;
            }

            isVisible = !isVisible;
            yield return new WaitForSeconds(0.2f);
            elapsed += 0.2f;
        }

        if (pollutionFillImage != null && isCritical)
        {
            pollutionFillImage.color = criticalColor;
        }
    }
}