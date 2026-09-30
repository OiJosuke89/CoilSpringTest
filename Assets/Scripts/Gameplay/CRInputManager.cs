using UnityEngine;

public class CRInputManager : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleClick(touch.position);
            }
        }
    }

    private void HandleClick(Vector3 screenPos)
    {
        if (mainCamera == null) return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if it's a gate by name or tag. 
            // In CRViewManager we named it "Gate_{gateId}"
            if (hit.collider.gameObject.name.StartsWith("Gate_"))
            {
                string gateId = hit.collider.gameObject.name.Substring(5); // Remove "Gate_"
                CRGameManager.Instance.ReleaseGate(gateId);
            }
        }
    }
}
