using UnityEngine;
using UnityEngine.AI;

public class Squirrel : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;
    [SerializeField] private float radiusAroundTree = 5f;

    [Header("Ссылки")]
    [SerializeField] private Transform treeTransform;

    [Header("Клики")]
    [SerializeField] private AudioClip squeakSound;

    private NavMeshAgent agent;
    private bool isWaiting = false;
    private int clickCount = 0;
    private AudioSource audioSource;

    private void Start()
    {
        if (Ecology.CurrentPollution >= 50f)
        {
            gameObject.SetActive(false);
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && squeakSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        MoveToRandomPoint();
    }

    private void Update()
    {
        if (isWaiting) return;

        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void MoveToRandomPoint()
    {
        if (treeTransform == null || agent == null) return;

        Vector3 randomPoint = GetRandomPointAroundTree();

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(randomPoint, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetDestination(randomPoint);
        }
        else
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, radiusAroundTree, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Invoke(nameof(MoveToRandomPoint), 1f);
            }
        }
    }

    private Vector3 GetRandomPointAroundTree()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(1f, radiusAroundTree);
        float x = treeTransform.position.x + Mathf.Cos(angle) * distance;
        float z = treeTransform.position.z + Mathf.Sin(angle) * distance;
        return new Vector3(x, treeTransform.position.y, z);
    }

    private void StartWaiting()
    {
        isWaiting = true;
        agent.isStopped = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Invoke(nameof(EndWaiting), waitTime);
    }

    private void EndWaiting()
    {
        isWaiting = false;
        agent.isStopped = false;
        MoveToRandomPoint();
    }

    public void ResetAfterDrop()
    {
        clickCount = 0;
        isWaiting = false;

        if (agent != null)
        {
            agent.isStopped = false;
            MoveToRandomPoint();
        }
    }

    private void OnMouseDown()
    {
        clickCount++;

        if (squeakSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(squeakSound);
        }

        if (clickCount == 5)
        {
            Debug.Log("*белка чихнула*");
        }
        else if (clickCount == 10)
        {
            Debug.Log("*белка сердито посмотрела на вас*");
        }
        else if (clickCount == 15)
        {
            Debug.Log("*белка показала язык*");
        }
        else if (clickCount == 20)
        {
            Debug.Log("*белка бросила орешек*");
        }
        else if (clickCount == 30)
        {
            Debug.Log("*белка устала и убежала*");

            PlayerPickup playerPickup = FindFirstObjectByType<PlayerPickup>();
            if (playerPickup != null)
            {
                playerPickup.CatchSquirrel(gameObject);
            }
        }
    }

    //private System.Collections.IEnumerator JumpEffect()
    //{
    //    Vector3 originalPos = transform.position;
    //    Vector3 jumpPos = originalPos + Vector3.up * 0.3f;
    //    float duration = 0.15f;
    //    float elapsed = 0f;

    //    while (elapsed < duration)
    //    {
    //        float t = elapsed / duration;
    //        transform.position = Vector3.Lerp(originalPos, jumpPos, t);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    elapsed = 0f;
    //    while (elapsed < duration)
    //    {
    //        float t = elapsed / duration;
    //        transform.position = Vector3.Lerp(jumpPos, originalPos, t);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    transform.position = originalPos;
    //}

    private void OnEnable()
    {
        Ecology.OnPollutionChanged += OnPollutionChanged;
    }

    private void OnDisable()
    {
        Ecology.OnPollutionChanged -= OnPollutionChanged;
    }

    private void OnPollutionChanged(float pollution)
    {
        gameObject.SetActive(pollution < 50f);
    }
}