using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private Vector3 currentFollowOffset, newFollowOffset;

    Ray ray;
    RaycastHit hit;
    GameObject currentHoveredObject;
    Outline currentOutline;
    [SerializeField] LayerMask outlineLayer;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        currentFollowOffset = cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset;
        newFollowOffset = currentFollowOffset;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) 
        {
            MouseScrollUp();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            MouseScrollDown();
        }

        FindOutline();
    }

    private void MouseScrollUp()
    {
        if (currentFollowOffset.y > 3)
        {
            newFollowOffset = new Vector3(currentFollowOffset.x, currentFollowOffset.y - 0.25f, currentFollowOffset.z);
            cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = newFollowOffset;
            currentFollowOffset = newFollowOffset;
        }
    }

    private void MouseScrollDown()
    {
        if (currentFollowOffset.y < 9)
        {
            newFollowOffset = new Vector3(currentFollowOffset.x, currentFollowOffset.y + 0.25f, currentFollowOffset.z);
            cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = newFollowOffset;
            currentFollowOffset = newFollowOffset;
        }        
    }

    private void FindOutline()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, outlineLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (currentHoveredObject != hitObject)
            {
                // Выключаем Outline на предыдущем объекте
                DisableOutline();

                // Включаем Outline на новом объекте
                currentHoveredObject = hitObject;
                currentOutline = currentHoveredObject.GetComponent<Outline>();

                if (currentOutline != null)
                {
                    currentOutline.enabled = true;
                }                
            }
        }
        else
        {
            // Курсор ни на одном объекте нужного слоя
            DisableOutline();
        }
    }

    void DisableOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
        currentHoveredObject = null;
    }
}