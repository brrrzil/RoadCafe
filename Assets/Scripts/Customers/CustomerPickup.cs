using UnityEngine;

public class CustomerPickup : MonoBehaviour
{
    [SerializeField] Transform holdPosition;

    private GameObject currentItem;

    public bool HasItem => currentItem != null;

    public void Take(GameObject item)
    {
        if (item == null) return;

        currentItem = item;
        Item itemComp = currentItem.GetComponent<Item>();

        if (itemComp != null)
        {
            itemComp.transform.SetParent(holdPosition);
            itemComp.transform.position = holdPosition.transform.position;
            itemComp.transform.localScale = Vector3.one;
            itemComp.isHeld = true;
        }
    }

    public void DestroyItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }

    public Item GetItem()
    {
        return currentItem?.GetComponent<Item>();
    }

    public void ReplaceItem(GameObject newItemPrefab)
    {
        // Уничтожаем текущий предмет
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }

        // Создаём новый предмет
        if (newItemPrefab != null)
        {
            GameObject newItem = Instantiate(newItemPrefab, holdPosition.position, Quaternion.identity);
            Take(newItem);
        }
    }
}