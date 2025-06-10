using UnityEngine;
using TMPro;

[ExecuteAlways] // Para que funcione en el Editor
public class CanvasSizeToTMP : MonoBehaviour
{
    public TMP_Text targetText;
    public Vector2 padding = new Vector2(50, 50);

    private RectTransform canvasRect;

    private void Awake() => canvasRect = GetComponent<RectTransform>();

    private void Update()
    {
        if (targetText == null) return;
        
        canvasRect.sizeDelta = new Vector2(
            targetText.preferredWidth + padding.x,
            targetText.preferredHeight + padding.y
        );
    }
}