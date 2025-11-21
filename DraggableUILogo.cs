using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUILogo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public LogoGameManager manager; // ← задаётся в инспекторе

    private RectTransform rect;
    private Vector2 offset;
    private Vector2 startPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            manager.canvas,
            eventData.position,
            null,
            out Vector2 localPoint
        );
        offset = rect.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            manager.canvas,
            eventData.position,
            null,
            out Vector2 localPoint
        );
        Vector2 newPos = localPoint + offset;
        rect.anchoredPosition = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Проверка пересечений при завершении перетаскивания (если нужно предотвращать наложение)
        bool hasOverlap = false;
        foreach (var other in manager.allShapes)
        {
            if (other == this) continue;
            if (DoRectsOverlap(rect, other.rect))
            {
                hasOverlap = true;
                break;
            }
        }
        
        if (hasOverlap)
        {
            // Возвращаем в начальную позицию, если есть пересечение
            rect.anchoredPosition = startPos;
        }
        else
        {
            // Сохраняем новую позицию, если нет пересечений
            startPos = rect.anchoredPosition;
        }
        
        // После любого перемещения — проверяем, всё ли окрашено
        manager.CheckCompletion();
    }

    bool DoRectsOverlap(RectTransform a, RectTransform b)
    {
        Vector2 sizeA = a.sizeDelta;
        Vector2 sizeB = b.sizeDelta;
        Vector2 posA = a.anchoredPosition;
        Vector2 posB = b.anchoredPosition;

        Rect rectA = new Rect(posA.x - sizeA.x / 2f, posA.y - sizeA.y / 2f, sizeA.x, sizeA.y);
        Rect rectB = new Rect(posB.x - sizeB.x / 2f, posB.y - sizeB.y / 2f, sizeB.x, sizeB.y);

        return rectA.Overlaps(rectB, true);
    }

    public void OnClick()
    {
        if (manager.selectedColor.HasValue)
        {
            GetComponent<Image>().color = manager.selectedColor.Value;
            manager.CheckCompletion();
        }
    }
}
