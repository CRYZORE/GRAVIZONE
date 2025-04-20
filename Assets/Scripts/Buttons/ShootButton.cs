using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isShooting = false; 
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        isShooting = true; 
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isShooting = false; 
    }
}
