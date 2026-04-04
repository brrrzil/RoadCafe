using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        // Убеждаемся, что DayManager существует
        if (DayManager.Instance == null)
        {
            GameObject dayManager = new GameObject("DayManager");
            dayManager.AddComponent<DayManager>();
        }

        // DayManager сам запустит новый день через StartNewDay()
        // Данные Economy и Ecology сохраняются благодаря DontDestroyOnLoad
    }
}