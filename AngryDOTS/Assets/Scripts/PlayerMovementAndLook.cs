using UnityEngine;

public class PlayerMovementAndLook : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Movement")]
    public float speed = 4.5f;
    public LayerMask whatIsGround;

    [Header("Life Settings")]
    public float playerHealth = 1f;

    [Header("Animation")]
    public Animator playerAnimator;

    Rigidbody playerRigidbody;
    bool isDead;

    public VariableJoystick leftVariableJoystick;

    public DynamicJoystick dynamicJoystick;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private Vector3 _desiredDirection;
    private Vector3 _rightDesiredDirection;

    void FixedUpdate()
    {
        if (isDead)
            return;

        //Arrow Key Input
#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
#else
        float h = leftVariableJoystick.Horizontal;
        float v = leftVariableJoystick.Vertical;
#endif


        float rh = dynamicJoystick.Horizontal;
        float rv = dynamicJoystick.Vertical;

        Vector3 inputDirection = new Vector3(h, 0, v);
        Vector3 rightInputDirection = new Vector3(rh, 0, rv);

        //Camera Direction
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        //Try not to use var for roadshows or learning code
        if (inputDirection != Vector3.zero)
        {
            _desiredDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
            MoveThePlayer(_desiredDirection);
            AnimateThePlayer(_desiredDirection);
        }
        else
        {
            AnimateThePlayer(Vector3.zero);
        }
        //Why not just pass the vector instead of breaking it up only to remake it on the other side?
        if (rightInputDirection != Vector3.zero)
        {
            _rightDesiredDirection = cameraForward * rightInputDirection.z + cameraRight * rightInputDirection.x;
            TurnThePlayer(_rightDesiredDirection);
        }
    }

    void MoveThePlayer(Vector3 desiredDirection)
    {
        Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        movement = movement.normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void TurnThePlayer(Vector3 desiredDirection)
    {
        // Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        //
        // if (Physics.Raycast(ray, out hit, whatIsGround))
        // {
        // 	Vector3 playerToMouse = hit.point - transform.position;
        // 	playerToMouse.y = 0f;
        // 	playerToMouse.Normalize();
        //
        // 	Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
        // 	playerRigidbody.MoveRotation(newRotation);
        // }

        Quaternion newRotation = Quaternion.LookRotation(desiredDirection);
        playerRigidbody.MoveRotation(newRotation);
    }

    void AnimateThePlayer(Vector3 desiredDirection)
    {
        if (!playerAnimator)
            return;

        Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        float forw = Vector3.Dot(movement, transform.forward);
        float stra = Vector3.Dot(movement, transform.right);

        playerAnimator.SetFloat("Forward", forw);
        playerAnimator.SetFloat("Strafe", stra);
    }
    
    public void PlayerDied()
    {
        if (isDead)
            return;

        isDead = true;

        playerAnimator.SetTrigger("Died");
        playerRigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }
}
