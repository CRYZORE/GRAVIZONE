using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX; 
    public float sensY; 

    public Transform orient; 

    float xRot, yRot; 
    [SerializeField] bool isMobile = false; 
    [SerializeField] float mouseX, mouseY; 
    public bool isRotating;
    public float transitionDuration = 1f;
    public float mobileSensMultiplier = 0.1f; // Добавьте эту строку
    public MobileTouchControl mobileTouchControl; // Добавьте эту ссылку

    void Start() {
        isMobile = StaticGameManager.isMobile; 
        if (!isMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (isRotating) return;

        if (mobileTouchControl != null)
        {
            mouseX = mobileTouchControl.TouchDelta.x * sensX * mobileSensMultiplier * Time.deltaTime;
            mouseY = mobileTouchControl.TouchDelta.y * sensY * mobileSensMultiplier * Time.deltaTime;
        }
        // Остальной код без изменений
        yRot += mouseX;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        if (orient != null)
            orient.rotation = Quaternion.Euler(xRot, yRot, 0);
    }

    IEnumerator RotateBack() {
        isRotating = true; 
        float elapsed = 0f;
        Quaternion originalCamRot = transform.rotation; 
        while (elapsed < transitionDuration)
        {
            transform.rotation = Quaternion.Slerp(
                originalCamRot,
                Quaternion.Euler(xRot, yRot + 180f, 0),
                elapsed / transitionDuration
            );

            elapsed += Time.deltaTime;
            yield return null;
        }
        yRot += 180f; 
        isRotating = false; 
    }
}
