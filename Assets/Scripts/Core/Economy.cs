using UnityEngine;
using System;

public class Economy : MonoBehaviour
{
    public static Economy Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private int startMoney = 500;
    [SerializeField] private int minMoney = 0;
    [SerializeField] private int maxMoney = 999999;

    private int currentMoney;

    public static event Action<int> OnMoneyChanged;
    public static int CurrentMoney => Instance?.currentMoney ?? 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentMoney = startMoney;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void SetMoney(int amount)
    {
        currentMoney = Mathf.Clamp(amount, minMoney, maxMoney);
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool IncreaseMoney(int income)
    {
        if (income <= 0) return false;

        int newMoney = currentMoney + income;
        currentMoney = Mathf.Min(newMoney, maxMoney);

        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public bool DecreaseMoney(int outcome)
    {
        if (outcome <= 0) return false;

        int newMoney = currentMoney - outcome;

        if (newMoney < minMoney)
        {
            return false;
        }

        currentMoney = newMoney;
        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }

    public static bool AddMoney(int amount) => Instance?.IncreaseMoney(amount) ?? false;
    public static bool SpendMoney(int amount) => Instance?.DecreaseMoney(amount) ?? false;
    public static bool CanAfford(int amount) => Instance?.HasEnoughMoney(amount) ?? false;
}