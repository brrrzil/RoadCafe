using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] private GameObject leaves;

    private void SetLeaves(bool isEcologyFine)
    {
        if (isEcologyFine)
        {
            leaves.SetActive(true);
        }
        else
        {
            leaves.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Ecology.OnPollutionChanged += OnPollutionChanged;
    }

    private void OnDisable()
    {
        Ecology.OnPollutionChanged -= OnPollutionChanged;
    }

    private void OnPollutionChanged(float pollution)
    {
        SetLeaves(pollution < 50f);
    }
}