using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    float moveSpeed; 
    public float walkSpeed;
    public float sprintSpeed; 
    public float airMulti; 
    public float groundDrag; 
    public float speedMultiplier = 1f; 
    public bool canMove = true; 
    [SerializeField] Joystick myJoystick; 
    [SerializeField] bool isMobile = true; 

    [Header("GroundCheck")]
    public float playerHeight; 
    public LayerMask whatIsGround; 
    public bool grounded; 

    [Header("KeyBinds")]
    public KeyCode sprintKey = KeyCode.LeftShift; 

    [Header("Refs")]
    public Transform orientation; 
    [SerializeField] Camera mainCam; 
    [SerializeField] float defFOV, sprintFOV; 
    [SerializeField] Player player; 

    float horizontalInput;
    float verticalInput; 

    Vector3 moveDir; 
    Rigidbody rb; 
    public MovementStateWalking state; 

    public enum MovementStateWalking {
        walking,
        sprinting,
        air
    };
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        player = GetComponent<Player>();
        rb.freezeRotation = true; 
        isMobile = StaticGameManager.isMobile; 
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, playerHeight * 0.5f + 0.2f, whatIsGround); 
        moveSpeed = moveSpeed * speedMultiplier; 
        MyInput(); 

        if(grounded) 
            rb.linearDamping = groundDrag;  

        SpeedControl(); 
        StateMachine(); 
    }
    void FixedUpdate() {
        MovePlayer(); 
    }
    void MyInput() {
        horizontalInput = myJoystick.Horizontal; 
        verticalInput = myJoystick.Vertical;
    }
    void StateMachine() {
        if(grounded && Input.GetKey(sprintKey)) {
            state = MovementStateWalking.sprinting;
            moveSpeed = sprintSpeed;  
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, sprintFOV, 0.1f); 
        }
        else if(grounded) {
            state = MovementStateWalking.walking; 
            moveSpeed = walkSpeed; 
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, defFOV, 0.1f);
        }
        else {
            state = MovementStateWalking.air; 
            moveSpeed = 0; 
        }
    }
    void MovePlayer() {
        if(!canMove) return; 
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput; 
        if(!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMulti, ForceMode.Force); 
        else
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force); 

    }
    private void SpeedControl() {
        if(rb.linearVelocity.magnitude > moveSpeed) {
            Vector3 limitedVel = rb.linearVelocity.normalized * moveSpeed; 
            rb.linearVelocity = limitedVel; 
        }
    }
}
