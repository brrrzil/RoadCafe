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

    public override void PickUp(Transform holder)
    {
        // Уведомляем белку, что она в руках
        if (squirrel != null)
        {
            squirrel.SetHeld(true);
        }

        // Отключаем компоненты
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

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
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        if (itemCollider != null) itemCollider.enabled = true;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        if (squirrel != null)
        {
            squirrel.SetHeld(false);
        }

        isHeld = false;
    }
}