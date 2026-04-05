using UnityEngine;
using System.Collections;

public class TrashBin : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float timeToCallTruck = 60f;
    [SerializeField] private int truckCallCost = 15;
    [SerializeField] private GameObject truckPrefab;
    [SerializeField] private Transform truckSpawnPoint;

    [Header("Иконка")]
    [SerializeField] private GameObject actionIconPrefab;
    [SerializeField] private Transform iconPosition;

    private GameObject currentIcon;
    private bool canCall = false;
    private bool iconShown = false;
    private bool isPlayerNear = false;

    public bool IsPlayerNear
    {
        get { return isPlayerNear; }
        set
        {
            isPlayerNear = value;
            if (!value && iconShown)
            {
                HideActionIcon();
            }
        }
    }

    private void Start()
    {
        StartCoroutine(TruckCallTimer());
    }

    private void Update()
    {
        if (isPlayerNear && canCall && !iconShown)
        {
            ShowActionIcon();
        }
    }

    private IEnumerator TruckCallTimer()
    {
        yield return new WaitForSeconds(timeToCallTruck);
        canCall = true;
    }

    private void ShowActionIcon()
    {
        if (iconShown) return;
        if (actionIconPrefab == null)
        {
            Debug.LogError("Action icon prefab is null!");
            return;
        }

        iconShown = true;
        Transform targetPos = iconPosition != null ? iconPosition : transform;
        currentIcon = Instantiate(actionIconPrefab, targetPos);
        currentIcon.transform.localPosition = Vector3.zero;
        currentIcon.transform.localScale = Vector3.one;

        BoxCollider collider = currentIcon.AddComponent<BoxCollider>();
        collider.size = new Vector3(1f, 1f, 0.1f);

        IconClickHandler clickHandler = currentIcon.AddComponent<IconClickHandler>();
        clickHandler.Initialize(this);
    }

    private void HideActionIcon()
    {
        if (currentIcon != null)
        {
            Destroy(currentIcon);
            iconShown = false;
        }
    }

    public void CallTruck()
    {
        if (!canCall)
        {
            Debug.Log("Cannot call truck yet");
            return;
        }

        // Проверяем, хватает ли денег
        if (!Economy.CanAfford(truckCallCost))
        {
            Debug.Log($"Not enough money! Need {truckCallCost} coins");
            return;
        }

        // Списываем деньги
        Economy.SpendMoney(truckCallCost);

        canCall = false;
        HideActionIcon();

        if (truckPrefab != null && truckSpawnPoint != null)
        {
            GameObject truck = Instantiate(truckPrefab, truckSpawnPoint.position, truckSpawnPoint.rotation);
            Truck truckScript = truck.GetComponent<Truck>();
            if (truckScript != null)
            {
                truckScript.StartTruck();
            }
        }

        // Запускаем таймер для следующего вызова
        StartCoroutine(TruckCallTimer());
    }
}