using UnityEngine;
using UnityEngine.EventSystems;

public class AttractButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed = false; 
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        isPressed = true; 
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isPressed = false; 
    }
}
