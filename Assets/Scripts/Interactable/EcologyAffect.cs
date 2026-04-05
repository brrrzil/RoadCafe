using UnityEngine;
using System.Collections;

public class EcologyAffect : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private float pollutionPerSecond = 0f;
    [SerializeField] private AffectMode mode = AffectMode.Constant;

    [Header("Настройки для режима Once")]
    [SerializeField] private float totalPollutionAmount = 10f;
    [SerializeField] private float effectDuration = 5f;

    [Header("Настройки для режима Conditional")]
    [SerializeField] private ConditionType condition = ConditionType.None;
    [SerializeField] private float conditionThreshold = 50f;
    [SerializeField] private bool activeWhenConditionMet = true;

    [Header("Настройки интервала")]
    [SerializeField] private float affectInterval = 1f;

    private Coroutine affectCoroutine;
    private Coroutine conditionCoroutine;
    private bool isAffecting = false;

    public enum AffectMode
    {
        Constant,
        Once,
        Conditional
    }

    public enum ConditionType
    {
        None,
        PollutionBelow,
        PollutionAbove,
        HasItemInStorage,
        IsProducing
    }

    private void Start()
    {
        if (mode == AffectMode.Constant)
        {
            StartConstantAffect();
        }
        else if (mode == AffectMode.Once)
        {
            StartOnceAffect();
        }
        else if (mode == AffectMode.Conditional)
        {
            StartConditionCheck();
        }
    }

    private void StartConstantAffect()
    {
        if (affectCoroutine != null)
            StopCoroutine(affectCoroutine);

        isAffecting = true;
        affectCoroutine = StartCoroutine(ConstantAffectRoutine());
    }

    private IEnumerator ConstantAffectRoutine()
    {
        while (isAffecting)
        {
            float pollutionChange = pollutionPerSecond * affectInterval;

            if (pollutionChange > 0)
            {
                Ecology.AddPollution(pollutionChange);
            }
            else if (pollutionChange < 0)
            {
                Ecology.RemovePollution(-pollutionChange);
            }

            yield return new WaitForSeconds(affectInterval);
        }
    }

    private void StartOnceAffect()
    {
        if (affectCoroutine != null)
            StopCoroutine(affectCoroutine);

        affectCoroutine = StartCoroutine(OnceAffectRoutine());
    }

    private IEnumerator OnceAffectRoutine()
    {
        isAffecting = true;

        float intervalsCount = effectDuration / affectInterval;
        float pollutionPerInterval = totalPollutionAmount / intervalsCount;

        float elapsed = 0f;

        while (elapsed < effectDuration)
        {
            if (totalPollutionAmount > 0)
            {
                Ecology.AddPollution(pollutionPerInterval);
            }
            else if (totalPollutionAmount < 0)
            {
                Ecology.RemovePollution(-pollutionPerInterval);
            }

            elapsed += affectInterval;
            yield return new WaitForSeconds(affectInterval);
        }

        isAffecting = false;
        affectCoroutine = null;

        if (GetComponent<Item>() != null)
        {
            Destroy(gameObject);
        }
    }

    private void StartConditionCheck()
    {
        if (conditionCoroutine != null)
            StopCoroutine(conditionCoroutine);

        conditionCoroutine = StartCoroutine(ConditionCheckRoutine());
    }

    private IEnumerator ConditionCheckRoutine()
    {
        while (true)
        {
            bool conditionMet = EvaluateCondition();
            bool shouldBeActive = conditionMet == activeWhenConditionMet;

            if (shouldBeActive && !isAffecting)
            {
                StartConstantAffect();
            }
            else if (!shouldBeActive && isAffecting)
            {
                StopAffect();
            }

            yield return new WaitForSeconds(affectInterval);
        }
    }

    private bool EvaluateCondition()
    {
        switch (condition)
        {
            case ConditionType.PollutionBelow:
                return Ecology.CurrentPollution < conditionThreshold;

            case ConditionType.PollutionAbove:
                return Ecology.CurrentPollution > conditionThreshold;

            case ConditionType.HasItemInStorage:
                Storage storage = GetComponent<Storage>();
                return storage != null && storage.CurrentCount > 0;

            case ConditionType.IsProducing:
                Production production = GetComponent<Production>();
                return production != null && production.IsProducing;

            default:
                return true;
        }
    }

    public void StopAffect()
    {
        if (affectCoroutine != null)
        {
            StopCoroutine(affectCoroutine);
            affectCoroutine = null;
        }
        isAffecting = false;
    }

    public void StartAffect()
    {
        if (mode == AffectMode.Constant)
        {
            StartConstantAffect();
        }
        else if (mode == AffectMode.Once)
        {
            StartOnceAffect();
        }
    }

    public void SetActive(bool active)
    {
        if (active && !isAffecting)
        {
            StartAffect();
        }
        else if (!active && isAffecting)
        {
            StopAffect();
        }
    }

    private void OnDestroy()
    {
        StopAffect();

        if (conditionCoroutine != null)
            StopCoroutine(conditionCoroutine);
    }
}