using UnityEngine;

public class CRGateView : MonoBehaviour
{
    public string gateId;
    public CRGateData data;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isSpent;
    private bool isChained;

    public void Initialize(string id, CRGateData gateData)
    {
        gateId = id;
        data = gateData;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Ensure there's a collider for raycasting
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        originalColor = gateData.color;
        spriteRenderer.color = originalColor;
    }

    public void UpdateState(bool released, bool canRelease)
    {
        isSpent = released;
        isChained = !released && !canRelease;

        if (isSpent)
        {
            spriteRenderer.color = Color.black; // Spent
            spriteRenderer.enabled = false; // Hide spent gates for now
        }
        else if (isChained)
        {
            var chainedColor = originalColor;
            chainedColor.a = 0.3f; // Transparent when chained
            spriteRenderer.color = chainedColor;
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.color = originalColor; // Active
            spriteRenderer.enabled = true;
        }
    }

    public void Pulse()
    {
        // Simple pulse animation using LeanTween or coroutine
        // For mockup, scale up and down quickly
        StartCoroutine(PulseRoutine());
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private System.Collections.IEnumerator PulseRoutine()
    {
        Vector3 origScale = transform.localScale;
        transform.localScale = origScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = origScale;
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        Vector3 origPos = transform.position;
        transform.position = origPos + Vector3.right * 0.1f;
        yield return new WaitForSeconds(0.05f);
        transform.position = origPos - Vector3.right * 0.1f;
        yield return new WaitForSeconds(0.05f);
        transform.position = origPos;
    }
}