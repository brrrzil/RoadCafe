using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public Sprite sprite;
    public bool isHeld = false;
    public bool isOnGround = true;

    protected Transform originalParent;
    protected Collider itemCollider;
    private Outline outline;

    protected virtual void Awake()
    {
        itemCollider = GetComponent<Collider>();
        if (itemCollider == null)
        {
            itemCollider = gameObject.AddComponent<BoxCollider>();
        }

        outline = GetComponent<Outline>();
    }

    public virtual void PickUp(Transform holder)
    {
        isHeld = true;
        isOnGround = false;
        outline.gameObject.SetActive(false);
        originalParent = transform.parent;
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
    }

    public virtual void Drop(Vector3 dropPosition)
    {
        isHeld = false;
        isOnGround = true;
        outline.gameObject.SetActive(true);
        transform.SetParent(originalParent);
        transform.position = dropPosition;

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
    }

    public virtual void Use()
    {
    }

    public virtual bool CanPickUp()
    {
        return !isHeld && isOnGround;
    }

    private void OnMouseDown()
    {
        if (CanPickUp())
        {
            PlayerPickup playerPickup = FindFirstObjectByType<PlayerPickup>();
            if (playerPickup != null)
            {
                if (playerPickup.HasItem())
                {
                    playerPickup.ResetPendingPickup();
                }
                else
                {
                    playerPickup.TryPickupItemFromGround(gameObject);
                }
            }
        }
        else
        {
            PlayerPickup playerPickup = FindFirstObjectByType<PlayerPickup>();
            if (playerPickup != null)
            {
                playerPickup.ResetPendingPickup();
            }
        }
    }
}