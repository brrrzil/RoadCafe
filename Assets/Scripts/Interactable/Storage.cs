using UnityEngine;
using System.Collections.Generic;

public class Storage : MonoBehaviour
{
    [Header("Настройки хранения")]
    [SerializeField] private int maxCapacity = 5;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private float iconSpacing = 3.5f;
    [SerializeField] private float displayOffsetX = 0f;
    [SerializeField] private float displayOffsetY = 4f;
    [SerializeField] private float rowSpacingZ = 3.5f;
    [SerializeField] private float iconScale = 1f;

    private List<ItemData> storedItems = new List<ItemData>();
    private List<GameObject> itemIcons = new List<GameObject>();

    private class ItemData
    {
        public Sprite sprite;
        public GameObject gameObject;

        public ItemData(Sprite sprite, GameObject gameObject)
        {
            this.sprite = sprite;
            this.gameObject = gameObject;
        }
    }

    public int CurrentCount => storedItems.Count;
    public int MaxCapacity => maxCapacity;

    public bool CanStore()
    {
        return storedItems.Count < maxCapacity;
    }

    public void StoreItem(Item item)
    {
        if (!CanStore()) return;

        ItemData data = new ItemData(item.sprite, item.gameObject);
        storedItems.Add(data);

        if (item.gameObject != null)
        {
            item.gameObject.SetActive(false);
        }

        UpdateDisplay();
    }

    public GameObject TakeItem()
    {
        while (storedItems.Count > 0)
        {
            ItemData data = storedItems[0];
            storedItems.RemoveAt(0);

            if (data.gameObject == null)
            {
                UpdateDisplay();
                continue;
            }

            data.gameObject.SetActive(true);
            UpdateDisplay();
            return data.gameObject;
        }

        UpdateDisplay();
        return null;
    }

    private void UpdateDisplay()
    {
        foreach (GameObject icon in itemIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }

        itemIcons.Clear();

        if (storedItems.Count == 0) return;

        int itemsPerRow = Mathf.CeilToInt(maxCapacity / 2f);
        int rows = Mathf.CeilToInt((float)storedItems.Count / itemsPerRow);

        for (int row = 0; row < rows; row++)
        {
            int itemsInRow = Mathf.Min(itemsPerRow, storedItems.Count - (row * itemsPerRow));
            float totalWidth = (itemsInRow - 1) * iconSpacing;
            float startX = -totalWidth / 2f + displayOffsetX;
            float zPos = -row * rowSpacingZ;

            for (int i = 0; i < itemsInRow; i++)
            {
                int itemIndex = (row * itemsPerRow) + i;
                if (itemIndex >= storedItems.Count) break;

                if (storedItems[itemIndex].gameObject == null)
                {
                    storedItems.RemoveAt(itemIndex);
                    UpdateDisplay();
                    return;
                }

                GameObject icon = Instantiate(iconPrefab, transform);
                float xPos = startX + (i * iconSpacing);
                icon.transform.localPosition = new Vector3(xPos, displayOffsetY, zPos);
                icon.transform.localScale = Vector3.one * iconScale;

                SpriteRenderer renderer = icon.GetComponent<SpriteRenderer>();
                if (renderer != null && storedItems[itemIndex] != null)
                {
                    renderer.sprite = storedItems[itemIndex].sprite;
                }

                itemIcons.Add(icon);
            }
        }
    }
}