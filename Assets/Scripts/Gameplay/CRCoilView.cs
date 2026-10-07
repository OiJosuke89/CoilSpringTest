using UnityEngine;

public class CRCoilView : MonoBehaviour
{
    public string coilId;
    public SpriteRenderer spriteRenderer;

    public void Initialize(string id, Color color)
    {
        coilId = id;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            
        spriteRenderer.color = color;
    }
}