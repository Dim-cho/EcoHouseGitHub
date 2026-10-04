using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public float hoverHeight = 10f;
    public float moveSpeed = 8f;

    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    private Image image;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        image = GetComponent<Image>();
        image.color = normalColor;
    }

    void Update()
    {
        Vector2 target = originalPosition;

        // Move slightly upward while hovering
        if (IsPointerOver)
            target += Vector2.up * hoverHeight;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            target,
            Time.deltaTime * moveSpeed
        );
    }

    private bool IsPointerOver { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOver = true;
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOver = false;
        image.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Do something when clicked
        Debug.Log("Button clicked!");
    }
}