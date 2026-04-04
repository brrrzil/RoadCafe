using UnityEngine;

public class PlayerPointTrigger : MonoBehaviour
{
    private CashRegister cashRegister;

    private void Start()
    {
        cashRegister = GetComponentInParent<CashRegister>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cashRegister != null)
        {
            cashRegister.OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cashRegister != null)
        {
            cashRegister.OnPlayerExit();
        }
    }
}