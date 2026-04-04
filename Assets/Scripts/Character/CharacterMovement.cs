// CharacterMovement.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class CharacterMovement : MonoBehaviour
{
    private RaycastHit hit;
    private NavMeshAgent agent;
    [SerializeField] private GameObject pointMarkerPrefab;

    public NavMeshAgent Agent => agent;
    public event System.Action OnDestinationReached;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetDestination();
        }

        CheckDestinationReached();
    }

    private void SetDestination()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 targetPoint;

            InteractableObject interactableObject = hit.collider.GetComponent<InteractableObject>();

            if (interactableObject != null && interactableObject.GetCharacterStopPoint() != null)
            {
                targetPoint = interactableObject.GetCharacterStopPoint().position;
            }
            else
            {
                targetPoint = hit.point;
            }

            agent.destination = targetPoint;

            if (pointMarkerPrefab != null)
            {
                GameObject tempPointMarker = Instantiate(pointMarkerPrefab, targetPoint, Quaternion.identity);
                StartCoroutine(DestroyAfterPlay(tempPointMarker));
            }
        }
    }

    private void CheckDestinationReached()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            OnDestinationReached?.Invoke();
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        agent.destination = point;
    }

    public void MoveToInteractable(InteractableObject interactable)
    {
        agent.destination = interactable.GetCharacterStopPoint().position;
    }

    private IEnumerator DestroyAfterPlay(GameObject objectToDestroy)
    {
        if (objectToDestroy == null) yield break;

        ParticleSystem particleSystem = objectToDestroy.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            yield return new WaitWhile(() => particleSystem.IsAlive(true));
        }
        Destroy(objectToDestroy);
    }
}