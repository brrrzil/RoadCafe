using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Базовые настройки")]
    [SerializeField] private Transform characterStopPoint;
    [SerializeField] private Transform customerStopPoint;

    public Transform GetCharacterStopPoint() => characterStopPoint;
    public Transform GetCustomerStopPoint() => customerStopPoint;
}