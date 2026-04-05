using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameObject canvasPrefab;

    private void Awake()
    {
        // Проверяем, существует ли уже Canvas
        Canvas existingCanvas = FindFirstObjectByType<Canvas>();

        if (existingCanvas == null && canvasPrefab != null)
        {
            Instantiate(canvasPrefab);
        }
    }
}