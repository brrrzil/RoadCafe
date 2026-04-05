using UnityEngine;

public class PlayerPointTrigger : MonoBehaviour
{
    private CashRegister cashRegister;
    private TrashBin trashBin;
    private Filter filter;


    private void Start()
    {
        cashRegister = GetComponentInParent<CashRegister>();
        trashBin = GetComponentInParent<TrashBin>();
        filter = GetComponentInParent<Filter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cashRegister != null) cashRegister.OnPlayerEnter();
            if (trashBin != null) trashBin.IsPlayerNear = true;
            if (filter != null) filter.IsPlayerNear = true;        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cashRegister != null) cashRegister.OnPlayerExit();
            if (trashBin != null) trashBin.IsPlayerNear = false;
            if (filter != null) filter.IsPlayerNear = false;
        }
    }
}