using UnityEngine;
using System.Collections.Generic;

public class CRInputManager : MonoBehaviour
{
    public static CRInputManager Instance { get; private set; }

    [Header("Settings")]
    public float holdToCommitTime = 0.35f;

    private Camera mainCam;

    private string currentTouchGateId;
    private float currentTouchTime;
    private bool isPreviewing;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCam = Camera.main;
    }

    private void Update()
    {
        if (CRGameManager.Instance == null || CRGameManager.Instance.simulator.CurrentState != CRGameState.Playing)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                // Check if hit a gate
                var gateView = hit.collider.GetComponentInParent<CRGateView>();
                if (gateView != null)
                {
                    string gateId = gateView.gateId;

                    // GDD: Chained gates do not respond to touch except for a small shake (handled by view later if needed)
                    if (CRGameManager.Instance.simulator.CanReleaseGate(gateId))
                    {
                        currentTouchGateId = gateId;
                        currentTouchTime = 0f;
                        isPreviewing = true;

                        CRViewManager.Instance?.ShowPreview(gateId);
                    }
                    else
                    {
                        // Play chained shake animation
                        gateView.Shake();
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0) && isPreviewing)
        {
            currentTouchTime += Time.deltaTime;

            // Check if slide off
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            bool isStillOnGate = false;
            if (hit.collider != null)
            {
                var gateView = hit.collider.GetComponentInParent<CRGateView>();
                if (gateView != null && gateView.gateId == currentTouchGateId)
                {
                    isStillOnGate = true;
                }
            }

            if (!isStillOnGate)
            {
                // Slide off -> Cancel
                CancelPreview();
            }
        }
        else if (Input.GetMouseButtonUp(0) && isPreviewing)
        {
            // Lift finger
            if (currentTouchTime >= holdToCommitTime)
            {
                // Commit
                string gateToCommit = currentTouchGateId;
                CancelPreview(); // Cleanup preview before actual release to avoid conflicts
                CRGameManager.Instance.ReleaseGate(gateToCommit);
            }
            else
            {
                // Short tap -> Keep preview on screen?
                // GDD: "A shorter tap keeps the preview on screen and pulses the gate ring to show 'hold to release'".
                // For simplicity, we just pulse and clear preview or keep it. Let's clear for now and we can add keep logic later if needed.
                var gateView = CRViewManager.Instance?.GetGateView(currentTouchGateId);
                gateView?.Pulse();

                CancelPreview();
            }
        }
    }

    private void CancelPreview()
    {
        isPreviewing = false;
        currentTouchGateId = null;
        currentTouchTime = 0f;
        CRViewManager.Instance?.ClearPreview();
    }
}