using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Put this on each TMP Input Field that has an icon drawn in its background sprite
/// (UsernameField, PasswordField). It moves the left/right edges of the field's Text Area,
/// so text that scrolls sideways is clipped BEFORE it reaches the icon instead of drawing over it.
///
/// The values are in pixels of the background sprite (your field sprite is 256x32), so they stay
/// correct whatever the canvas scale is.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class InputFieldTextInset : MonoBehaviour
{
    [Tooltip("Space on the left that stays free of text (icon width + a small gap). Sprite pixels.")]
    [SerializeField] private float leftPixels = 36f;

    [Tooltip("Space on the right that stays free of text. Use a bigger value (about 40) for the " +
             "password field so text also stays clear of the eye button. Sprite pixels.")]
    [SerializeField] private float rightPixels = 8f;

    [Tooltip("Width of the background sprite in pixels. 0 = read it from the field's Image.")]
    [SerializeField] private float spriteWidthPixels = 0f;

    private TMP_InputField field;
    private RectTransform fieldRect;

    private void Awake()
    {
        field = GetComponent<TMP_InputField>();
        fieldRect = (RectTransform)transform;
        Apply();
    }

    private void Start()
    {
        Apply();   // again once the layout is final
    }

    private void OnRectTransformDimensionsChange()
    {
        if (field != null) Apply();
    }

    private void Apply()
    {
        RectTransform area = field.textViewport;
        if (area == null) return;

        // Convert sprite pixels to canvas units
        float spriteWidth = spriteWidthPixels;
        if (spriteWidth <= 0f)
        {
            Image image = field.targetGraphic as Image;
            spriteWidth = (image != null && image.sprite != null) ? image.sprite.rect.width : fieldRect.rect.width;
        }
        float scale = spriteWidth > 0f ? fieldRect.rect.width / spriteWidth : 1f;

        // The Text Area stretches across the field, with the free space left and right
        area.anchorMin = new Vector2(0f, area.anchorMin.y);
        area.anchorMax = new Vector2(1f, area.anchorMax.y);
        area.offsetMin = new Vector2(leftPixels * scale, area.offsetMin.y);
        area.offsetMax = new Vector2(-rightPixels * scale, area.offsetMax.y);

        // The text and placeholder must sit flush inside the Text Area, or they'd be shifted twice
        ZeroHorizontalOffsets(field.textComponent != null ? field.textComponent.rectTransform : null);
        ZeroHorizontalOffsets(field.placeholder != null ? field.placeholder.rectTransform : null);

        // This mask is what actually hides the scrolled-away text
        if (area.GetComponent<RectMask2D>() == null) area.gameObject.AddComponent<RectMask2D>();
    }

    private static void ZeroHorizontalOffsets(RectTransform rt)
    {
        if (rt == null) return;
        rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
        rt.anchorMax = new Vector2(1f, rt.anchorMax.y);
        rt.offsetMin = new Vector2(0f, rt.offsetMin.y);
        rt.offsetMax = new Vector2(0f, rt.offsetMax.y);
    }
}
