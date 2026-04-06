using UnityEngine;

public class IconClickHandler : MonoBehaviour
{
    private TrashBin trashBin;
    private Filter filter;

    public void Initialize(TrashBin bin)
    {
        trashBin = bin;
    }

    public void Initialize(Filter filterComponent)
    {
        filter = filterComponent;
    }

    private void OnMouseDown()
    {
        if (trashBin != null)
        {
            trashBin.CallTruck();
        }
        else if (filter != null)
        {
            filter.BuildFilter();
        }
    }
}