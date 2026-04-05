using UnityEngine;

public class CashRegister : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform iconPosition;

    private CustomerBrain currentCustomer;
    private bool isPlayerInZone = false;
    private GameObject currentIcon;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isPlayerInZone && currentCustomer != null && Input.GetMouseButtonDown(0))
        {
            currentCustomer.Serve();
            audioSource.PlayOneShot(audioSource.clip);
            currentCustomer = null;
        }
    }

    public void OnCustomerEnter(CustomerBrain customer)
    {
        if (currentCustomer != null) return;
        currentCustomer = customer;
        UpdateIcon();
    }

    public void OnCustomerExit(CustomerBrain customer)
    {
        if (currentCustomer == customer)
        {
            currentCustomer = null;
        }
        UpdateIcon();
    }

    public void OnPlayerEnter()
    {
        isPlayerInZone = true;
        UpdateIcon();
    }

    public void OnPlayerExit()
    {
        isPlayerInZone = false;
        UpdateIcon();
    }    

    private void UpdateIcon()
    {
        bool show = isPlayerInZone && currentCustomer != null;

        if (show && currentIcon == null && iconPrefab != null)
        {
            Transform targetPos = iconPosition != null ? iconPosition : transform;
            currentIcon = Instantiate(iconPrefab, targetPos);
            currentIcon.transform.localPosition = iconPosition.localPosition;
        }
        else if (!show && currentIcon != null)
        {
            Destroy(currentIcon);
            currentIcon = null;
        }
    }
}