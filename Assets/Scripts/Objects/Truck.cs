using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private AudioClip reverseMovingSound, honk;

    private AudioSource audioSource;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void StartTruck()
    {
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    public void ReverseBeep()
    {
        if (reverseMovingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reverseMovingSound);
        }
    }

    public void Honk()
    {
        if (honk != null && audioSource != null)
        {
            audioSource.PlayOneShot(honk);
        }
    }
}