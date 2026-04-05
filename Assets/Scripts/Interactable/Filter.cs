using UnityEngine;

public class Filter : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private int filterCost = 200;
    [SerializeField] private GameObject filterPrefab;
    [SerializeField] private Transform filterSpawnPoint;

    [Header("Иконка")]
    [SerializeField] private GameObject actionIconPrefab;
    [SerializeField] private Transform iconPosition;

    private GameObject currentIcon;
    private bool isPlayerNear = false;
    private GameObject builtFilterObject;
    private static bool isFilterBuilt = false;

    public bool IsPlayerNear
    {
        get { return isPlayerNear; }
        set
        {
            isPlayerNear = value;

            if (value && !isFilterBuilt)
            {
                ShowActionIcon();
            }
            else if (!value && currentIcon != null)
            {
                HideActionIcon();
            }
        }
    }

    private void Start()
    {
        // Загружаем состояние из статической переменной
        if (isFilterBuilt)
        {
            RestoreFilter();
        }
    }

    private void RestoreFilter()
    {
        if (builtFilterObject == null && filterPrefab != null && filterSpawnPoint != null)
        {
            builtFilterObject = Instantiate(filterPrefab, filterSpawnPoint.position, filterSpawnPoint.rotation);
            Debug.Log("Filter restored");
        }
    }

    private void ShowActionIcon()
    {
        if (currentIcon != null) return;
        if (isFilterBuilt) return;
        if (actionIconPrefab == null)
        {
            Debug.LogError("Action icon prefab is null!");
            return;
        }

        Transform targetPos = iconPosition != null ? iconPosition : transform;
        currentIcon = Instantiate(actionIconPrefab, targetPos);
        currentIcon.transform.localPosition = Vector3.zero;
        currentIcon.transform.rotation = Quaternion.identity;

        BoxCollider collider = currentIcon.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = currentIcon.AddComponent<BoxCollider>();
        }
        collider.size = new Vector3(1f, 1f, 0.1f);
        collider.isTrigger = false;

        IconClickHandler clickHandler = currentIcon.GetComponent<IconClickHandler>();
        if (clickHandler == null)
        {
            clickHandler = currentIcon.AddComponent<IconClickHandler>();
        }
        clickHandler.Initialize(this);
    }

    private void HideActionIcon()
    {
        if (currentIcon != null)
        {
            Destroy(currentIcon);
            currentIcon = null;
        }
    }

    public void BuildFilter()
    {
        if (isFilterBuilt)
        {
            Debug.Log("Filter already built");
            return;
        }

        if (!Economy.CanAfford(filterCost))
        {
            Debug.Log($"Not enough money! Need {filterCost} coins");
            return;
        }

        Economy.SpendMoney(filterCost);

        if (filterPrefab != null && filterSpawnPoint != null)
        {
            builtFilterObject = Instantiate(filterPrefab, filterSpawnPoint.position, filterSpawnPoint.rotation);
            isFilterBuilt = true;
            HideActionIcon();
        }
    }

    // Для сброса при отладке
    public static void ResetFilter()
    {
        isFilterBuilt = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNear = false;
        }
    }
}