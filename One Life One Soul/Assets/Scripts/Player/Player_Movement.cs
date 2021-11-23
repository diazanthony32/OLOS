using System;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    //reference to the main player script
    [SerializeField] internal Player playerScript;

    [Header("Locomotion Options: ")]
    [Tooltip("The max speed the physics character can move")]                                                       // how fast the player is allowed to move
    [SerializeField] internal float maxSpeed = 8;

    [Tooltip("How fast the physics character can normally get to the max speed set")]                               // how fast the player can get to maxSpeed
    [SerializeField] internal float acceleration = 50;

    [Tooltip("The fastest the physics character can possibly go get to the max speed")]                             // fastest the player can get to maxSpeed
    [SerializeField] internal float maxAcceleration = 200;

    [Tooltip("The acceleration force curve based on rapid movement. Time is direction, value is speed")]            // adjusts the acceleration force when changing directions rapidly
    [SerializeField] internal AnimationCurve accelerationFactorFromDot;

    [Tooltip("The scale of the force applied to the player on individual axes")]                                    // adjusts the scale of the force applied to the player on each axes
    [SerializeField] internal Vector3 forceScale = new Vector3(1.0f, 0.0f, 1.0f);



    [Header("Jump Options: ")]
    [Tooltip("The force applied to the player when jumping")]                                                       // force applied to the player when jumping
    [SerializeField] internal float jumpForce = 10;

    [Tooltip("How many seconds of forgiveness should the player have for jumping to happen")]                       // jumping after leaving the ground forgiveness
    [SerializeField] internal float coyoteTimeForgiveness = 0.25f;

    [Tooltip("How many seconds of forgiveness should the player have for jumping to happen")]                       // jumping after leaving the ground forgiveness
    [SerializeField] internal float jumpBuffer = 0.25f;

    [Tooltip("The layers the player should expect to be ground")]                                                   // how fast the player is allowed to move
    [SerializeField] internal LayerMask groundMask;

    [Tooltip("The offset of a players feet, in case the feet of the player is not located at Vector3.zero")]        // how fast the player is allowed to move
    [SerializeField] internal Vector3 groundCheckOffest;

    [Tooltip("How big of a radius should the players feet be from the ground to be considered grounded again")]     // how fast the player is allowed to move
    [SerializeField] internal float groundCheckRadius = 0.2f;

    private Collider[] groundColliders = new Collider[1];
    private bool isGrounded => Physics.OverlapSphereNonAlloc(transform.position + groundCheckOffest, groundCheckRadius, groundColliders, groundMask, QueryTriggerInteraction.Ignore) > 0;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool mayJump;

    [Space(10)]

    [Header("OLOS Options: ")]
    [Tooltip("How fast the player must move in a direction to flip around. Aids in preventing random flips")]       // aids in preventing random player flipping 
    [SerializeField] private float flipDeadzone = 0.1f;
    internal bool facingRight = true;

    void Start()
    {
        print("Player_Movement Script Starting");
        playerScript.rb.velocity = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (!playerScript.activePlayer) return;

        SetPlayerOrientation();
        DetermineMayJump();
    }

    // Fixed Update is called once per Physics Update
    void FixedUpdate()
    {
        if (!playerScript.activePlayer) return;

        Move();
    }

    void Move()
    {
        // gets the input from the player
        Vector3 _move = new Vector3(playerScript.inputScript.moveInputX, 0, playerScript.inputScript.moveInputY).normalized;
        Vector3 _translateMoveVector = transform.TransformDirection(_move);

        // determines the target velocity while leaving the players y-velocity intact
        Vector3 _targetVelocity = _translateMoveVector * maxSpeed;
        _targetVelocity.y = playerScript.rb.velocity.y;

        //if (mayJump)
        //    _targetVelocity.y *= jumpForce;

        // gets the direction the Player is moving and adjusts the acceleration force when changing directions rapidly
        Vector3 _velocityNormalized = playerScript.rb.velocity.normalized;
        float _velocityDot = Vector3.Dot(_translateMoveVector, _velocityNormalized);
        float _adjustedAcceleration = acceleration * accelerationFactorFromDot.Evaluate(_velocityDot);

        // calculates the acceleration needed to move towards the goal speed per physics update
        Vector3 _goalVelocity = Vector3.MoveTowards(playerScript.rb.velocity, _targetVelocity, _adjustedAcceleration * Time.fixedDeltaTime);
        Vector3 _neededAcceleration = ((_goalVelocity - playerScript.rb.velocity) / Time.fixedDeltaTime);
        _neededAcceleration = Vector3.ClampMagnitude(_neededAcceleration, maxAcceleration);

        // applys the force to the player
        playerScript.rb.AddForce(Vector3.Scale(_neededAcceleration * playerScript.rb.mass, forceScale));

        playerScript.anim.SetFloat("Walking", (Mathf.Abs(playerScript.rb.velocity.x) + Mathf.Abs(playerScript.rb.velocity.z)));

        if (mayJump)
            Jump();
    }


    // may rewrite this to instead create an opposing force to apply rather than setting velocity to zero...
    void Jump()
    {
        // resets the players y velocity
        var temp = playerScript.rb.velocity;
        temp.y = 0.0f;
        playerScript.rb.velocity = temp;

        playerScript.rb.AddForce(Vector3.up * jumpForce * playerScript.rb.mass, ForceMode.Impulse);
        mayJump = false;

        // resets the timers used for jumping buffering as well as coyote time
        coyoteTimer = 0.0f;
        jumpBufferTimer = 0.0f;
    }

    void DetermineMayJump()
    {
        //used for timing jump Buffering and for coyote time
        jumpBufferTimer -= Time.deltaTime;
        coyoteTimer -= Time.deltaTime;

        // resets the timer for coyote time while grounded
        if (isGrounded)
            coyoteTimer = coyoteTimeForgiveness;

        // if the player presses jump, it checks if they are grounded. If so, player jumps next physics update
        // of if the player is no longer grounded but just barely left the ground, the player is allowed to jump
        // If not, jump is then buffered for a fraction of a second to allow for jump forgiveness
        if (playerScript.inputScript.jump)
        {
            if (isGrounded || (coyoteTimer > 0.0f && !isGrounded))
            {
                mayJump = true;
            }
            else
            {
                jumpBufferTimer = jumpBuffer;
            }
        }
        // if the jump buffer is still up and the player lands, the player jumps
        else if ((jumpBufferTimer > 0.0f && isGrounded))
        {
            mayJump = true;
        }
    }

    void CheckStepHeight()
    {

    }

    void SetPlayerOrientation()
    {
        // gets the players velocity according to the players local space rather than world space
        float playerVelocity = transform.InverseTransformDirection(playerScript.rb.velocity).x;

        // Flips the player according to current facing direction and move direction
        if ((playerVelocity > flipDeadzone && !facingRight) || (playerVelocity < -flipDeadzone && facingRight))
        {
            // Switch the way the player is labeled as facing
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1 to flip the player's gameobject
            Vector3 flippedScale = transform.localScale;
            flippedScale.x *= -1;
            transform.localScale = flippedScale;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + groundCheckOffest, groundCheckRadius);

    }

}