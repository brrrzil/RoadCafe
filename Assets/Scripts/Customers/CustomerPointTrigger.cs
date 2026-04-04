using UnityEngine;

public class CustomerPointTrigger : MonoBehaviour
{
    private CashRegister cashRegister;

    private void Start()
    {
        cashRegister = GetComponentInParent<CashRegister>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CustomerBrain customer = other.GetComponent<CustomerBrain>();
        if (customer != null && cashRegister != null)
        {
            cashRegister.OnCustomerEnter(customer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CustomerBrain customer = other.GetComponent<CustomerBrain>();
        if (customer != null && cashRegister != null)
        {
            cashRegister.OnCustomerExit(customer);
        }
    }
}