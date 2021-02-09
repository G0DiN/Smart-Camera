using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField, Range(1f, 100f)]  
    private float maxSpeed = 10f;
    
    [SerializeField, Range(1f,100f)]
     private float maxAcceleration = 10f,
                   maxAirAcceleration = 1f;
    Vector3 velocity,
            desiredVelocity;
    [SerializeField, Range(0f,10f)] 
    float jumpHeight = 2f;
      
    [SerializeField, Range(0,5)]
    int maxAirJump = 0;

    bool bJump,
         onGround;

    int jumpPhase;
    Rigidbody rigidbody;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update() 
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        bJump |= Input.GetButtonDown("Jump");
    
        desiredVelocity = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;
    }

    private void FixedUpdate() 
    {
        velocity = rigidbody.velocity;
        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (bJump)
        {
            bJump = false;
            Jump();
        }
        UpdateState();
        onGround = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision) 
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
        }
    }

    private void Jump()
    {
        if (onGround || jumpPhase < maxAirJump)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if (velocity.y > 0f)
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            velocity.y +=  jumpSpeed;
        }        
    }
    
    private void UpdateState()
    {
        rigidbody.velocity = velocity;    
        if (onGround)                        
            jumpPhase = 0;
    }
}
