using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]

public class Footsteps : MonoBehaviour
{
    [Header("Звук в помещении")]
    public AudioClip footstepsIndoor;
    public float stepDurationIndoor = 0.4f;
    public int totalStepsIndoor = 20;

    [Header("Звук на улице")]
    public AudioClip footstepsGround;
    public float stepDurationGround = 0.4f;
    public int totalStepsGround = 20;

    [Header("Настройки скорости")]
    public float minStepInterval = 0.3f;
    public float maxStepInterval = 0.8f;
    public float minSpeedForSteps = 0.1f;

    [Header("Определение поверхности")]
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayerMask = ~0;

    [Header("Опционально")]
    public bool randomizePitch = true;
    public float pitchRange = 0.1f;

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float nextStepTime;
    private bool isMoving;
    private SurfaceType currentSurface;

    private AudioClip currentClip;
    private float currentStepDuration;
    private int currentTotalSteps;
    private float currentStepInterval;

    // Флаги для управления воспроизведением
    private bool isPlayingStep = false;
    private float stepEndTime;

    private enum SurfaceType
    {
        Indoor,
        Ground,
        Unknown
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;

        currentSurface = SurfaceType.Unknown;
        UpdateSurfaceSettings();
    }

    void Update()
    {
        // Проверяем поверхность
        CheckSurface();

        // Проверяем движение
        bool isMovingNow = agent.velocity.magnitude > minSpeedForSteps &&
                           agent.remainingDistance > agent.stoppingDistance;

        // Обработка изменения состояния движения
        if (isMovingNow != isMoving)
        {
            isMoving = isMovingNow;
            if (!isMoving)
            {
                // Останавливаем всё воспроизведение при остановке
                if (audioSource.isPlaying)
                    audioSource.Stop();
                isPlayingStep = false;
            }
        }

        if (!isMoving) return;

        // Динамический интервал между шагами
        float currentSpeed = agent.velocity.magnitude;
        float speedRatio = Mathf.InverseLerp(minSpeedForSteps, agent.speed, currentSpeed);
        currentStepInterval = Mathf.Lerp(maxStepInterval, minStepInterval, speedRatio);

        // Проверяем, не закончился ли текущий шаг
        if (isPlayingStep && Time.time >= stepEndTime)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            isPlayingStep = false;
        }

        // Воспроизводим следующий шаг
        if (!isPlayingStep && Time.time >= nextStepTime)
        {
            PlayRandomStep();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    void CheckSurface()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayerMask))
        {
            SurfaceType detectedSurface = DetectSurfaceType(hit.collider);

            if (detectedSurface != currentSurface && detectedSurface != SurfaceType.Unknown)
            {
                currentSurface = detectedSurface;
                UpdateSurfaceSettings();
            }
        }
    }

    SurfaceType DetectSurfaceType(Collider collider)
    {
        if (collider.CompareTag("Indoor"))
            return SurfaceType.Indoor;
        if (collider.CompareTag("Ground"))
            return SurfaceType.Ground;

        string layerName = LayerMask.LayerToName(collider.gameObject.layer);
        if (layerName == "Indoor")
            return SurfaceType.Indoor;
        if (layerName == "Ground")
            return SurfaceType.Ground;

        return SurfaceType.Unknown;
    }

    void UpdateSurfaceSettings()
    {
        switch (currentSurface)
        {
            case SurfaceType.Indoor:
                currentClip = footstepsIndoor;
                currentStepDuration = stepDurationIndoor;
                currentTotalSteps = totalStepsIndoor;
                break;

            case SurfaceType.Ground:
                currentClip = footstepsGround;
                currentStepDuration = stepDurationGround;
                currentTotalSteps = totalStepsGround;
                break;

            default:
                currentClip = footstepsIndoor;
                currentStepDuration = stepDurationIndoor;
                currentTotalSteps = totalStepsIndoor;
                break;
        }

        audioSource.clip = currentClip;
    }

    void PlayRandomStep()
    {
        // Выбираем случайный шаг
        int stepIndex = Random.Range(0, currentTotalSteps);
        float startTime = stepIndex * currentStepDuration;

        // Проверка границ клипа
        if (startTime + currentStepDuration > currentClip.length)
        {
            startTime = Mathf.Max(0, currentClip.length - currentStepDuration);
        }

        // Настройка AudioSource
        audioSource.time = startTime;

        if (randomizePitch)
        {
            audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        }

        // Воспроизводим
        audioSource.Play();

        // Запоминаем время окончания шага
        isPlayingStep = true;
        stepEndTime = Time.time + currentStepDuration;
    }

    // Опционально: принудительная остановка при отключении объекта
    void OnDisable()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
        isPlayingStep = false;
    }
}