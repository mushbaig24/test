using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class Card : MonoBehaviour, IPointerClickHandler
{
    private CardData data;
    [SerializeField] private Image cardImage;
    private Sprite backSprite;
    
    public int Id => data != null ? data.id : -1;
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }

    public event Action<Card> OnCardClicked;

    private Coroutine flipCoroutine;

    public void Initialize(CardData cardData, Sprite back)
    {
        data = cardData;
        backSprite = back;
        IsFlipped = false;
        IsMatched = false;
        
        if (cardImage == null) cardImage = GetComponent<Image>();
        cardImage.sprite = backSprite;
        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsFlipped || IsMatched) return;
        OnCardClicked?.Invoke(this);
    }

    public void Flip(bool showFace, float duration = 0.2f)
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(FlipRoutine(showFace, duration));
    }

    private IEnumerator FlipRoutine(bool showFace, float duration)
    {
        IsFlipped = showFace;
        
        // Phase 1: Shrink to 0 width
        float elapsed = 0;
        Vector3 startScale = transform.localScale;
        Vector3 midScale = new Vector3(0, startScale.y, startScale.z);

        while (elapsed < duration / 2f)
        {
            transform.localScale = Vector3.Lerp(startScale, midScale, elapsed / (duration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = midScale;
        cardImage.sprite = showFace ? data.faceSprite : backSprite;

        // Phase 2: Expand back to 1 width
        elapsed = 0;
        Vector3 endScale = new Vector3(1, startScale.y, startScale.z);

        while (elapsed < duration / 2f)
        {
            transform.localScale = Vector3.Lerp(midScale, endScale, elapsed / (duration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        flipCoroutine = null;
    }

    public void SetMatched()
    {
        IsMatched = true;
        // Visual feedback for match can be added here
        StartCoroutine(MatchAnimation());
    }

    private IEnumerator MatchAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0;
        Color startColor = cardImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);

        while (elapsed < duration)
        {
            cardImage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        cardImage.color = endColor;
    }
}
