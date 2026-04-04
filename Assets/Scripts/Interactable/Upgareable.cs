using UnityEngine;
using System;

public class Upgradeable : MonoBehaviour
{
    [Header("Настройки улучшения")]
    [SerializeField] private string upgradeName;
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private int[] upgradeCosts;

    public static event Action<Upgradeable> OnUpgradeAttempt;
    public static event Action<Upgradeable> OnUpgradeSuccess;

    public string UpgradeName => upgradeName;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public int NextLevelCost => (currentLevel < maxLevel && currentLevel < upgradeCosts.Length) ? upgradeCosts[currentLevel] : -1;
    public bool CanUpgrade => currentLevel < maxLevel;

    private void Start()
    {
        if (upgradeCosts == null || upgradeCosts.Length == 0)
        {
            upgradeCosts = new int[maxLevel];
            for (int i = 0; i < maxLevel; i++)
            {
                upgradeCosts[i] = 100 * (i + 1);
            }
        }
    }

    public void AttemptUpgrade()
    {
        if (!CanUpgrade) return;

        OnUpgradeAttempt?.Invoke(this);

        int cost = NextLevelCost;

        if (Economy.CanAfford(cost))
        {
            Economy.SpendMoney(cost);
            currentLevel++;
            OnUpgradeSuccess?.Invoke(this);
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 0, maxLevel);
    }
}