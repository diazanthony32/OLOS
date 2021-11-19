using System;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    //reference to the main player script
    [SerializeField] internal Player playerScript;

    [Header("Movement Options: ")]
    [Tooltip("")]
    //main player properties
    [SerializeField] internal float movementSpeed;

    // Amount of force added when the player jumps.
    [SerializeField] private float jumpForce = 400f;

    // Buffers a jump for the Player in case the jump a fraction of a second too late
    [Tooltip("wow jump")]
    [SerializeField] internal float coyoteTime = 0.25f;
    internal float mayJump;

    [SerializeField] internal bool softLand = true;

    // How much to smooth out the movement
    [Range(0, .3f)]
    [SerializeField] private float movementSmoothing = .05f;

    // A mask determining what is ground to the character
    [SerializeField] internal LayerMask whatIsGround;

    // A position marking where to check if the player is grounded.
    [SerializeField] internal Transform m_GroundCheck;

    [Space(5)]
    // How sensitive player sprite fliping is handled, the higher the less sensitive
    [SerializeField] private float flipDeadzone = 0.1f;

    [Header("Falling Options: ")]
    [Tooltip("")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float slowFallMultiplier = 2f;
    [SerializeField] private float fastFallMultiplier = 2f;

    private float _groundRayDistance;
    private RaycastHit _slopeHit;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool grounded;            // Whether or not the player is grounded.
    internal bool facingRight = true;  // For determining which way the player is currently facing.

    [Header("Uneven Flooring Options: ")]
    [Tooltip("")]
    [SerializeField][Range(10, 80)] internal float slopeLimit = 50.0f;
    [SerializeField] [Range(1, 10)] internal float slopeSlideSpeed = 2.5f;
    //this is temporary
    [SerializeField] internal float antiBump = -Physics.gravity.y/2;

    [SerializeField] [Range(0, 1)] internal float stepHeightRatio = 0.25f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        print("Player_Movement Script Starting");
        playerScript.rb.velocity = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        // Used for Coyote Time 
        mayJump -= Time.deltaTime;
    }

    // Fixed Update is called once per Physics Update
    void FixedUpdate()
    {
        IsGrounded();

        Move(playerScript.inputScript.moveInput, playerScript.inputScript.jumping);
        SlopeMovement();
    }

    public void Move(Vector3 move, bool jump)
    {
        // Smoothly move the character to the target velocity after first adjusting the velocity to the players corect orientation
        // Sets velocity to a slight constant upward force if grounded to prevent weird bumps lifts (anti-bump)
        Vector3 targetVelocity;

        targetVelocity = new Vector3(move.x * movementSpeed * Time.fixedDeltaTime, playerScript.rb.velocity.y, move.y * movementSpeed * Time.fixedDeltaTime);

        Vector3 translatedVelocity = playerScript.transform.TransformDirection(targetVelocity);
        playerScript.rb.velocity = Vector3.SmoothDamp(playerScript.rb.velocity, translatedVelocity, ref velocity, movementSmoothing);


        if (OnSlope())
        {
            var slopeDirection = GetSlopeDirection();
            var velocityOffset = slopeDirection * -Physics.gravity.y * 10;

            playerScript.rb.AddForce(velocityOffset);
            //targetVelocity += velocityOffset;

            //var temp = targetVelocity;
            //temp.y = antiBump;
            //targetVelocity = temp;
        }

        // If there is movement, it will swap to the walking animation
        playerScript.anim.SetFloat("Walking", Math.Abs(move.x) + Math.Abs(move.y));

        // InverseTransformDirection converts the Velocity of the Player from World to Local to Adjust when the Camera Moves
        // If the player is moving to the right and the player is currently facing left...
        if (playerScript.transform.InverseTransformDirection(playerScript.rb.velocity).x > flipDeadzone && !facingRight)
        {
            Flip();
        }
        // Otherwise if the player is moving to the left and the player is currently facing right...
        else if (playerScript.transform.InverseTransformDirection(playerScript.rb.velocity).x < -flipDeadzone && facingRight)
        {
            Flip();
        }

        // If the player should try to jump while on the ground or during the coyoteTimeForgiveness period, the Player Jumps
        if ((grounded || mayJump > 0.0f) && jump)
        {
            grounded = false;
            mayJump = 0.0f;

            // Add a vertical force to the player.
            Vector3 tempVelocity = playerScript.rb.velocity;
            tempVelocity.y = jumpForce;
            playerScript.rb.velocity = tempVelocity;

            //softLand = false;
        }

        // Player holds Jump For the entire duration of the Jump
        // ideally this should be the farthest you can jump
        if (playerScript.rb.velocity.y > 0 && jump)
        {
            //Debug.Log("Player held jump all through the jump");
        }
        // Player is jumping but lets go of Jump
        // ideally the player should descent pretty rapidly
        else if (playerScript.rb.velocity.y > 0 && !jump)
        {
            playerScript.rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier * (fastFallMultiplier)) * Time.deltaTime;
            //Debug.Log("Player let go of jump midway of jumping");
        }
        // Player is falling and is holding Jump
        // ideally the player should still land fast but not as fast as letting go comepletely
        else if (playerScript.rb.velocity.y < 0 && jump)
        {
            playerScript.rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier * slowFallMultiplier) * Time.deltaTime;
            //Debug.Log("Player is falling while holding jump");
        }
        // Player is falling and lets go of Jump
        // ideally the player should descent pretty rapidly
        else if (playerScript.rb.velocity.y < 0 && !jump)
        {
            playerScript.rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier * (fastFallMultiplier)) * Time.deltaTime;
            //Debug.Log("Player is falling");
        }

        // Idle if the player is no longer moving in any direction whatsoever
        if (playerScript.rb.velocity == Vector3.zero)
        {
            //Debug.Log("Player is idle");
            //playerScript.ChangeState(Player.PlayerState.Idle);
        }

    }

    void CheckStepHeight()
    {

    }

    bool OnSlope() {
        if (!IsGrounded())
            return false;

        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, (playerScript.collisionScript._playerCollider.height / 2) + _groundRayDistance))
            if (_slopeHit.normal != Vector3.up)
                return true;
        return false;
    }

    bool OnSteepSlope() {

        if (!OnSlope())
            return false;

        float _slopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        if (_slopeAngle  > slopeLimit)
        {
            return true;
        }
        
        return false;
    }

    void SlopeMovement()
    {
        if (OnSteepSlope())
        {
            Vector3 slopeDirection = GetSlopeDirection();
            float slideSpeed = movementSpeed + slopeSlideSpeed + Time.deltaTime;

            velocity = slopeDirection * -slideSpeed;
            velocity.y += -_slopeHit.point.y;
        }
    }

    Vector3 GetSlopeDirection()
    {
        return Vector3.up - _slopeHit.normal * Vector3.Dot(Vector3.up, _slopeHit.normal);
    }

    bool IsGrounded()
    {
        grounded = false;

        // Checking the player's ground Check to see if the player is currently grounded
        Collider[] hitColliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, whatIsGround);
        foreach (var hitCollider in hitColliders)
        {
            grounded = true;
            mayJump = coyoteTime;

            //// This is to reset the player's vertical velocity to prevent falling through the floor at high speeds
            //if (!softLand)
            //{
            //    Vector3 tmpVel = playerScript.rb.velocity;
            //    tmpVel.y = -1.0f;
            //    playerScript.rb.velocity = tmpVel;
            //    softLand = true;
            //}
        }

        return grounded;
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_GroundCheck.position, k_GroundedRadius);
    }

}