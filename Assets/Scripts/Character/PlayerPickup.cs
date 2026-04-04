using UnityEngine;
using UnityEngine.AI;

public class PlayerPickup : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Transform holdPosition;

    private GameObject currentItem;
    private Camera mainCamera;
    private InteractableObject currentInteractable;
    private Rigidbody rb;
    private GameObject pendingPickupItem;
    private bool isMovingToPickup;

    private void Start()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
        {
            playerCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && currentItem != null)
        {
            DropItem();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Item hitItem = hit.collider.GetComponent<Item>();
                if (hitItem == null)
                {
                    ResetPendingPickup();
                }
                else if (hitItem.gameObject != pendingPickupItem)
                {
                    ResetPendingPickup();
                }
            }
            else
            {
                ResetPendingPickup();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractableObject interactable = other.GetComponentInParent<InteractableObject>();
        if (interactable != null)
        {
            currentInteractable = interactable;

            if (currentItem == null)
            {
                Storage storage = interactable.GetComponent<Storage>();
                if (storage != null && storage.CurrentCount > 0)
                {
                    GameObject itemObject = storage.TakeItem();
                    if (itemObject != null)
                    {
                        PickupItem(itemObject);
                    }
                }
            }
        }

        Item item = other.GetComponent<Item>();
        if (item != null && item.CanPickUp())
        {
            if (pendingPickupItem == item.gameObject && isMovingToPickup)
            {
                PickupItem(item.gameObject);
                pendingPickupItem = null;
                isMovingToPickup = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentItem == null) return;

        InteractableObject interactable = other.GetComponentInParent<InteractableObject>();
        if (interactable == null) return;

        Production production = interactable.GetComponent<Production>();
        if (production != null)
        {
            Item heldItem = currentItem.GetComponent<Item>();
            if (heldItem != null && production.CanAcceptItem(heldItem))
            {
                production.AcceptItem();
                Destroy(currentItem);
                currentItem = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableObject interactable = other.GetComponentInParent<InteractableObject>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void PickupItem(GameObject itemObject)
    {
        currentItem = itemObject;
        Item item = currentItem.GetComponent<Item>();

        item.transform.SetParent(holdPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.isHeld = true;
        item.isOnGround = false;
        item.transform.localScale = new Vector3(1f, 1f, 1f);

        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        pendingPickupItem = null;
        isMovingToPickup = false;
    }

    public void CatchSquirrel(GameObject squirrel)
    {
        if (currentItem != null) return;

        currentItem = squirrel;
        SquirrelItem squirrelItem = squirrel.GetComponent<SquirrelItem>();

        if (squirrelItem != null)
        {
            squirrelItem.PickUp(holdPosition);
        }
    }

    public void TryPickupItemFromGround(GameObject itemObject)
    {
        if (currentItem != null) return;

        if (pendingPickupItem != itemObject)
        {
            pendingPickupItem = itemObject;
            isMovingToPickup = true;
        }
    }

    public void ResetPendingPickup()
    {
        pendingPickupItem = null;
        isMovingToPickup = false;
    }

    private void DropItem()
    {
        if (currentItem != null)
        {
            Vector3 dropPosition = transform.position + transform.forward * 1f;
            dropPosition.y = 0.1f;

            SquirrelItem squirrelItem = currentItem.GetComponent<SquirrelItem>();
            if (squirrelItem != null)
            {
                squirrelItem.Drop(dropPosition);
            }

            currentItem = null;
        }
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    public GameObject GetCurrentItem()
    {
        return currentItem;
    }

    public void ClearCurrentItem()
    {
        currentItem = null;
    }
}