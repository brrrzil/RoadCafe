using UnityEngine;
using UnityEngine.AI;

public class SquirrelItem : Item
{
    private Vector3 originalScale;
    private Squirrel squirrel;

    private void Start()
    {
        originalScale = transform.localScale;
        squirrel = GetComponent<Squirrel>();
        itemName = "Белка";
    }

    // Отключаем стандартный OnMouseDown от Item
    private new void OnMouseDown()
    {
        // Пусто - белка ловится только через Squirrel.OnMouseDown
    }

    public override void PickUp(Transform holder)
    {
        // Отключаем компоненты
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        if (squirrel != null) squirrel.enabled = false;

        if (itemCollider != null) itemCollider.enabled = false;

        // Привязываем к рукам
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        isHeld = true;
    }

    public override void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        transform.localScale = new Vector3(0.1f, 0.1f,0.1f);

        if (itemCollider != null) itemCollider.enabled = true;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        if (squirrel != null)
        {
            squirrel.enabled = true;
            squirrel.ResetAfterDrop();
        }

        isHeld = false;
    }
}