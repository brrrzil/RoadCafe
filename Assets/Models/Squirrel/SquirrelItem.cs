using UnityEngine;
using UnityEngine.AI;

public class SquirrelItem : MonoBehaviour
{
    private string itemName = "Белка";
    private bool isHeld = false;

    public void PickUp(Transform holder)
    {
        isHeld = true;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        Squirrel squirrel = GetComponent<Squirrel>();
        if (squirrel != null)
        {
            squirrel.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    public void Drop(Vector3 dropPosition)
    {
        isHeld = false;

        transform.SetParent(null);
        transform.position = dropPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);


        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        Squirrel squirrel = GetComponent<Squirrel>();
        if (squirrel != null)
        {
            squirrel.enabled = true;
            squirrel.ResetAfterDrop();
        }
    }

    public bool CanPickUp()
    {
        return !isHeld;
    }
}