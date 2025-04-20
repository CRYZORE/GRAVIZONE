using UnityEngine;

public class MobileTouchControl : MonoBehaviour
{
    private RectTransform rectTransform;
    private int touchId = -1;
    private Vector2 previousPosition;
    public Vector2 TouchDelta { get; private set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        TouchDelta = Vector2.zero;
        
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touchId == -1)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, touch.position))
                {
                    touchId = touch.fingerId;
                    previousPosition = touch.position;
                }
            }
            else if (touch.fingerId == touchId)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 currentPosition = touch.position;
                    TouchDelta = currentPosition - previousPosition;
                    previousPosition = currentPosition;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    touchId = -1;
                    TouchDelta = Vector2.zero;
                }
            }
        }
    }
}