using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class CustomerMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    public void Initialize(float speed)
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
        agent.stoppingDistance = 0.5f;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void GoTo(Vector3 target)
    {
        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target);
        }
    }

    public void Stop()
    {
        if (agent != null) agent.isStopped = true;
    }

    public bool HasArrived()
    {
        if (agent == null) return false;
        if (agent.pathPending) return false;

        bool arrived = agent.remainingDistance <= agent.stoppingDistance;
        
        return arrived;
    }
}