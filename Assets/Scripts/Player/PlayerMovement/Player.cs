using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; 

public class Player : MonoBehaviour
{
    [Header("Refs")]
    Rigidbody rb;
    public FakeGravityBody fgb; 
    PlayerMovement movement; 
    [SerializeField] AttractButton btn; 

    [Header("HookMechanic")]
    [Header("Detection")]
    public Camera mainCam;
    [SerializeField] float detectDistance;  
    [SerializeField] KeyCode hookKey = KeyCode.Q;  
    [SerializeField] Slider chargeSlider; 
    [SerializeField] float targetingSpeed; 
    [SerializeField] float charge; 
    public bool hooking = false;
    public bool onObject = false; 
    float chargeMultiplyTimer = 0f; 
    [SerializeField] float chargeMultiplyTime = 0.1f; 
    [SerializeField] GameObject lastOutlinedObject; 
    
    [Header("Hooking")]
    [SerializeField] float hookingForce; 
    public GameObject currentHookedObj; 
    [SerializeField] float airDrag = 5f; 
    public float colDrag; 
    bool charging; 

    [Header("Jump")]
    [SerializeField] float jumpForce; 
    [SerializeField] KeyCode jumpKey = KeyCode.Space; 
    [Header("UI")]
    [SerializeField] GameObject joystick; 
    [SerializeField] GameObject hookButton; 
    [SerializeField] GameObject jumpButton; 
    [SerializeField] bool isMobile = false; 

    [Header("StateMachine")]
    [SerializeField] PlayerStates currentState; 
    public enum PlayerStates {
        doNothing, 
        hooking, 
        onObject
    };

    [Header("CamSettings")]
    [SerializeField] float hookingFOV; 
    [SerializeField] float stillFOV; 

    [Header("Effects")]
    [SerializeField] ParticleSystem chargingPs, explosion; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        fgb = GetComponent<FakeGravityBody>(); 
        movement = GetComponent<PlayerMovement>(); 
        rb.freezeRotation = true; 
        chargeMultiplyTimer = chargeMultiplyTime;
        stillFOV = mainCam.fieldOfView; 
        hookingFOV = stillFOV * 1.3f; 
        isMobile = StaticGameManager.isMobile; 
    }

    // Update is called once per frame
    void Update()
    {
        joystick.SetActive(movement.enabled);
        hookButton.SetActive(currentState == PlayerStates.doNothing); 
        jumpButton.SetActive(currentState == PlayerStates.onObject);

        HookRay(); 
        StateMachine(); 
        if(Input.GetKeyDown(jumpKey))
            JumpAway(); 
    }

    void StateMachine() {
        if (hooking) {
            hooking = true; 
            fgb.enabled = false;
            movement.enabled = false; 
            rb.linearDamping = 0;
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, hookingFOV, 0.1f); 
            currentState = PlayerStates.hooking; 
        }
        else if(onObject) {
            hooking = false; 
            onObject = true;  
            if(currentHookedObj.GetComponent<HookObject>().isAttractive) {
                fgb.enabled = true; 
                movement.enabled = true; 
            }
            if(!currentHookedObj.GetComponent<HookObject>().isAttractive)
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, stillFOV, 0.1f);
            currentState = PlayerStates.onObject; 
        }
        else {
            rb.linearDamping = airDrag; 
            fgb.enabled = false;
            movement.enabled = false;
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, stillFOV, 0.1f);
            currentState = PlayerStates.doNothing; 
        }
    }
    void Hook(GameObject target, float forceMultiplier) {
        rb.useGravity = false; 
        GetComponentInChildren<Collider>().isTrigger = true; 
        lastOutlinedObject.GetComponent<Outline>().enabled = false; 
        Vector3 direction = (target.transform.position - transform.position).normalized; 
        rb.AddForce(direction * (hookingForce + forceMultiplier), ForceMode.Impulse); 
        charge = 0; 
        chargeSlider.value = Mathf.MoveTowards(chargeSlider.value, charge, targetingSpeed * 3);
    }
    public void JumpAway() {
        if(onObject)
        {
            rb.useGravity = false; 
            Vector3 direction = (transform.position - currentHookedObj.transform.position).normalized;
            rb.linearDamping = 0;
            rb.AddForce(direction * jumpForce, ForceMode.Impulse); 
            onObject = false; 
            currentHookedObj = null; 
        }
    }
    void HookRay() {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f); 
        RaycastHit hit; 

        bool hitSomething = false; 
        if(Physics.Raycast(ray, out hit, Mathf.Infinity) && !hooking && !onObject) {
            HookObject hookObj = hit.collider.GetComponent<HookObject>(); 

            if(hookObj != null) {
                hitSomething = true; 
                if(lastOutlinedObject != null)
                    lastOutlinedObject.GetComponent<Outline>().enabled = false; 
                lastOutlinedObject = hit.collider.gameObject; 
                lastOutlinedObject.GetComponent<Outline>().enabled = true; 
                lastOutlinedObject.GetComponent<Outline>().OutlineWidth = 5f; 
                // if((Input.GetKey(hookKey) && !isMobile) || (btn.isPressed && isMobile)) {
                //     Charging(); 
                // }
                // if((Input.GetKeyUp(hookKey) && !isMobile) || (!btn.isPressed && isMobile)) {
                //     StartHooking(); 
                // }
            }
            else {
                charge = 0; 
                chargingPs.gameObject.SetActive(false); 
                if(lastOutlinedObject != null) {}
                    lastOutlinedObject.GetComponent<Outline>().enabled = false;
                chargeSlider.gameObject.SetActive(false); 
                chargeSlider.value = Mathf.MoveTowards(chargeSlider.value, charge, targetingSpeed * 3);
            }
        }
        else {
            charge = 0; 
            chargingPs.gameObject.SetActive(false); 
            if(lastOutlinedObject != null) {}
                lastOutlinedObject.GetComponent<Outline>().enabled = false;
            chargeSlider.gameObject.SetActive(false); 
            chargeSlider.value = Mathf.MoveTowards(chargeSlider.value, charge, targetingSpeed * 3);
        }
    }
    public void Charging() {
        if(chargeMultiplyTimer <= 0) {
            if(charge < chargeSlider.maxValue) {
                charge++; 
            }
            chargeMultiplyTimer = 1/targetingSpeed; 
        }
        else 
            chargeMultiplyTimer -= Time.deltaTime; 
        chargeSlider.gameObject.SetActive(true);
        chargingPs.gameObject.SetActive(true); 
        chargeSlider.value = Mathf.MoveTowards(chargeSlider.value, charge, 1/targetingSpeed);
    }
    public void StartHooking() {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f); 
        RaycastHit hit; 
        chargeSlider.gameObject.SetActive(false);
        chargingPs.gameObject.SetActive(false); 
        explosion.Play(); 
        bool hitSomething = false; 
        if(Physics.Raycast(ray, out hit, Mathf.Infinity) && !hooking && !onObject) {
            HookObject hookObj = hit.collider.GetComponent<HookObject>(); 

            if(hookObj != null) {
                hitSomething = true; 
                hookObj.isTargeted = true; 
                GameObject targetObj = hit.collider.gameObject;  
                Hook(targetObj, charge); 
                hooking = true;
            }
        }
    }

    public bool IsLookingAtTargetObject() {
        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit; 
        if(Physics.Raycast(ray, out hit, detectDistance)) {
            if (hit.collider.gameObject == currentHookedObj)
                return true; 
            else 
                return false; 
        }
        else 
            return false; 
    }
    public void StartRotating() {
        StartCoroutine(PlayerRotate()); 
    }
    IEnumerator PlayerRotate() {
        mainCam.GetComponent<PlayerCam>().isRotating = true; 
        float elapsed = 0f;
        Quaternion originalRot = transform.rotation; 
        while (elapsed < 0.25f)
        {
            transform.rotation = Quaternion.Slerp(
                originalRot,
                Quaternion.Euler(transform.rotation.x,transform.rotation.y, 0),
                elapsed / 0.25f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCam.GetComponent<PlayerCam>().isRotating = false;
    }
}
