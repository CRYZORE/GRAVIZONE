using UnityEngine;

public class HookObject : MonoBehaviour
{
    public bool isTargeted = false; 
    public bool isAttractive; 
    FakeGravity fg; 
    
    void Start() {
        if(isAttractive)
            fg = GetComponent<FakeGravity>(); 
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponentInParent<Player>().hooking == true && isTargeted) {
            isTargeted = false; 
            other.isTrigger = false; 
            Player playerScript = other.gameObject.GetComponentInParent<Player>();  
            playerScript.hooking = false; 
            if(!isAttractive)
                other.gameObject.GetComponentInParent<Rigidbody>().linearDamping = playerScript.colDrag;
            playerScript.onObject = true; 
            playerScript.currentHookedObj = gameObject; 
            if(playerScript.IsLookingAtTargetObject()) {
                if(!playerScript.mainCam.GetComponent<PlayerCam>().isRotating)
                    playerScript.mainCam.GetComponent<PlayerCam>().StartCoroutine("RotateBack"); 
            } 
            if(isAttractive)
                playerScript.fgb.attractor = fg; 
        }
    }
}
