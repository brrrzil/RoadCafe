using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        if (DayManager.Instance == null)
        {
            GameObject dayManager = new GameObject("DayManager");
            dayManager.AddComponent<DayManager>();
        }
    }
}