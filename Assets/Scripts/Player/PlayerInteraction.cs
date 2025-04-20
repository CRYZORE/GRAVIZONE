using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera mainCam; 
    [SerializeField] float interactionDistance; 

    [SerializeField] GameObject interactionUI; 
    [SerializeField] TMP_Text interactionText; 

    [SerializeField] KeyCode interactionKey = KeyCode.E; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractionRay(); 
    }
    void InteractionRay() {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f); 
        RaycastHit hit; 

        bool hitSomething = false; 
        if(Physics.Raycast(ray, out hit, interactionDistance)) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>(); 

            if(interactable != null) {
                hitSomething = true; 
                interactionText.text = interactable.GetDescription(); 

                if(Input.GetKeyDown(interactionKey)) {
                    Interaction(); 
                }
 
            }
        }
        interactionUI.SetActive(hitSomething);
    }
    public void Interaction() {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f); 
        RaycastHit hit; 

        if(Physics.Raycast(ray, out hit, interactionDistance)) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>(); 

            if(interactable != null) {
                interactable.Interact(); 
            }
        }
    }
}
