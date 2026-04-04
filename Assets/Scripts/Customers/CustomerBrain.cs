using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]

public class CustomerBrain : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float eatDuration = 5f;
    [SerializeField] private GameObject waitingIcon;
    [SerializeField] private GameObject takingFoodIcon;
    [SerializeField] private GameObject paperBagPrefab;

    private CustomerMovement movement;
    private CustomerPickup pickup;

    private CustomerState currentState;

    private Transform foodPoint;
    private Transform foodStorageParent;
    private Transform cashPoint;
    private Transform tablePoint;
    private Transform exitPoint;
    private Transform trashPoint;

    private static List<Transform> allFoodPoints = new List<Transform>();
    private static List<Transform> allTables = new List<Transform>();
    private static List<Transform> allCashPoints = new List<Transform>();

    private void Start()
    {
        movement = gameObject.AddComponent<CustomerMovement>();
        pickup = gameObject.GetComponent<CustomerPickup>();
        movement.Initialize(speed);
        waitingIcon.SetActive(false);
        takingFoodIcon.SetActive(false);

        FindAllPoints();
        SelectRandomPoints();        

        GoToState(CustomerState.GoingToFood);
    }

    private void Update()
    {
        if (IsMovingState(currentState) && movement.HasArrived())
        {
            OnArrived();
        }

        if (currentState == CustomerState.WaitingForFood)
        {
            TryGetFood();
        }
    }

    private bool IsMovingState(CustomerState state)
    {
        return state == CustomerState.GoingToFood ||
               state == CustomerState.GoingToCash ||
               state == CustomerState.GoingToTable ||
               state == CustomerState.GoingToTrash ||
               state == CustomerState.GoingToExit;
    }

    private void FindAllPoints()
    {
        allFoodPoints.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("FoodPoint"))
            allFoodPoints.Add(obj.transform);

        allTables.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("DinnerTable"))
            allTables.Add(obj.transform);

        allCashPoints.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CashPoint"))
            allCashPoints.Add(obj.transform);

        exitPoint = GameObject.FindGameObjectWithTag("ExitPoint")?.transform;
        trashPoint = GameObject.FindGameObjectWithTag("TrashBin")?.transform;
    }

    private void SelectRandomPoints()
    {
        if (allFoodPoints.Count > 0)
        {
            foodPoint = allFoodPoints[Random.Range(0, allFoodPoints.Count)];
            foodStorageParent = foodPoint.parent != null ? foodPoint.parent : foodPoint;
        }

        if (allCashPoints.Count > 0)
            cashPoint = allCashPoints[Random.Range(0, allCashPoints.Count)];

        if (allTables.Count > 0)
            tablePoint = allTables[Random.Range(0, allTables.Count)];
    }

    private void GoToState(CustomerState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case CustomerState.GoingToFood:
                movement.GoTo(foodPoint.position);
                break;

            case CustomerState.WaitingForFood:
                movement.Stop();
                waitingIcon.SetActive(true);
                break;

            case CustomerState.GoingToCash:
                movement.GoTo(cashPoint.position);
                waitingIcon.SetActive(false);
                break;

            case CustomerState.WaitingForService:
                movement.Stop();
                waitingIcon.SetActive(true);
                break;

            case CustomerState.GoingToTable:
                movement.GoTo(tablePoint.position);
                waitingIcon.SetActive(false);
                break;

            case CustomerState.Eating:
                movement.Stop();
                Invoke(nameof(FinishEating), eatDuration);
                takingFoodIcon.SetActive(true);
                break;

            case CustomerState.GoingToTrash:
                movement.GoTo(trashPoint.position);
                takingFoodIcon.SetActive(false);
                break;

            case CustomerState.GoingToExit:
                movement.GoTo(exitPoint.position);
                takingFoodIcon.SetActive(false);
                break;
        }
    }

    private void OnArrived()
    {
        switch (currentState)
        {
            case CustomerState.GoingToFood:
                GoToState(CustomerState.WaitingForFood);
                break;

            case CustomerState.GoingToCash:
                GoToState(CustomerState.WaitingForService);
                break;

            case CustomerState.GoingToTable:
                GoToState(CustomerState.Eating);
                break;

            case CustomerState.GoingToTrash:
                DropTrash();
                GoToState(CustomerState.GoingToExit);
                break;

            case CustomerState.GoingToExit:
                Destroy(gameObject);
                break;
        }
    }

    private void TryGetFood()
    {
        Storage storage = foodStorageParent.GetComponent<Storage>();
        if (storage == null || storage.CurrentCount == 0) return;

        GameObject foodObject = storage.TakeItem();
        if (foodObject == null) return;

        Item item = foodObject.GetComponent<Item>();
        if (item is CookedFood)
        {
            pickup.Take(foodObject);
            GoToState(CustomerState.GoingToCash);
        }
        else
        {
            storage.StoreItem(item);
        }
    }

    private void DropTrash()
    {
        if (pickup.HasItem && trashPoint != null)
        {
            Storage storage = trashPoint.GetComponent<Storage>();
            Item trash = pickup.GetItem();

            if (storage != null && storage.CanStore())
            {
                storage.StoreItem(trash);
                pickup.DestroyItem();
            }
        }
    }

    private void FinishEating()
    {
        if (paperBagPrefab != null)
        {
            GameObject paperBag = Instantiate(paperBagPrefab, transform.position, Quaternion.identity);
            pickup.Take(paperBag);
            GoToState(CustomerState.GoingToTrash);
        }
        else
        {
            GoToState(CustomerState.GoingToExit);
        }
    }

    public void Serve()
    {
        if (currentState != CustomerState.WaitingForService) return;

        // Проверяем, что клиент действительно у кассы
        if (cashPoint == null) return;

        Item food = pickup.GetItem();
        if (food is CookedFood cookedFood)
        {
            Economy.AddMoney(cookedFood.sellPrice);
        }

        GoToState(CustomerState.GoingToTable);
    }

    public void StopMovement()
    {
        if (movement != null)
        {
            movement.Stop();
        }

        // Останавливаем все корутины
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if (waitingIcon != null)
            Destroy(waitingIcon);

        if (takingFoodIcon != null)
            Destroy(takingFoodIcon);
    }
}