using UnityEngine;
using System.Collections;

public class Production : MonoBehaviour
{
    [Header("Настройки производства")]
    [SerializeField] private Item inputItemType;
    [SerializeField] private Item outputItemPrefab;
    [SerializeField] private float productionTime = 3f;
    [SerializeField] private bool requiresInputItem = true;
    [SerializeField] private int outputMultiplier = 1;

    private Storage storage;
    private bool isProducing;
    private int pendingOrders;

    public int OutputMultiplier => outputMultiplier;
    public bool IsProducing => isProducing;

    private void Start()
    {
        storage = GetComponent<Storage>();

        if (!requiresInputItem)
        {
            StartProduction();
        }
    }

    public bool CanAcceptItem(Item item)
    {
        if (!requiresInputItem) return false;
        if (storage == null) return false;
        if (item == null) return false;
        if (item.GetType() != inputItemType.GetType()) return false;
        if (storage.CurrentCount >= storage.MaxCapacity) return false;

        return true;
    }

    public void AcceptItem()
    {
        if (!requiresInputItem) return;
        if (storage == null) return;

        pendingOrders++;

        if (!isProducing)
        {
            StartProduction();
        }
    }

    private void StartProduction()
    {
        if (storage == null) return;

        if (requiresInputItem && pendingOrders <= 0) return;
        if (storage.CurrentCount >= storage.MaxCapacity) return;

        isProducing = true;
        StartCoroutine(ProductionRoutine());
    }

    private IEnumerator ProductionRoutine()
    {
        yield return new WaitForSeconds(productionTime);

        if (storage != null)
        {
            int freeSpace = storage.MaxCapacity - storage.CurrentCount;
            int itemsToCreate = Mathf.Min(outputMultiplier, freeSpace);

            for (int i = 0; i < itemsToCreate; i++)
            {
                Item producedItem = Instantiate(outputItemPrefab, transform.position, Quaternion.identity);
                storage.StoreItem(producedItem);
            }

            if (requiresInputItem)
            {
                pendingOrders--;
            }

            // Если не все предметы поместились из-за нехватки места, добавляем обратно в очередь
            int remainingItems = outputMultiplier - itemsToCreate;
            if (remainingItems > 0)
            {
                pendingOrders += remainingItems;
            }

            // Продолжаем производство
            if ((requiresInputItem && pendingOrders > 0) || (!requiresInputItem))
            {
                if (storage.CurrentCount < storage.MaxCapacity)
                {
                    StartProduction();
                }
                else
                {
                    isProducing = false;
                }
            }
            else
            {
                isProducing = false;
            }
        }
        else
        {
            isProducing = false;
        }
    }
}