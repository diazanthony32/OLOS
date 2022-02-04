using System;
using System.Collections;
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

    [Tooltip("The force applied to the player when jumping")]                                                       // force applied to the player when jumping
    [SerializeField] internal float jumpDuration = 0.2f;

    [Tooltip("How many seconds of forgiveness should the player have for jumping to happen")]                       // jumping after leaving the ground forgiveness
    [SerializeField] internal float coyoteTimeForgiveness = 0.25f;

    [Tooltip("How many seconds of forgiveness should the player have for jumping to happen")]                       // jumping after leaving the ground forgiveness
    [SerializeField] internal float jumpBuffer = 0.25f;

    //[SerializeField] private float fallMultiplier = 2.5f;
    //[SerializeField] private float slowFallMultiplier = 2f;
    //[SerializeField] private float fastFallMultiplier = 2f;

    [Tooltip("The layers the player should expect to be ground")]                                                   // how fast the player is allowed to move
    [SerializeField] internal LayerMask groundMask;

    [Tooltip("The offset of a players feet, in case the feet of the player is not located at Vector3.zero")]        // how fast the player is allowed to move
    [SerializeField] internal Vector3 groundCheckOffest;

    [Tooltip("How big of a radius should the players feet be from the ground to be considered grounded again")]     // how fast the player is allowed to move
    [SerializeField] internal float groundCheckRadius = 0.2f;

    private Collider[] groundColliders = new Collider[1];
    private bool isGrounded => Physics.OverlapSphereNonAlloc(transform.position + groundCheckOffest, groundCheckRadius, groundColliders, groundMask, QueryTriggerInteraction.Ignore) > 0 ;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpTimer;

    private bool mayJump;
    private bool hasJumped;
    private bool stopJump;

    //private JumpState jumpState;

    [Space(10)]

    [Header("OLOS Options: ")]
    [Tooltip("How fast the player must move in a direction to flip around. Aids in preventing random flips")]       // aids in preventing random player flipping 
    [SerializeField] private float flipDeadzone = 0.1f;
    internal bool facingRight = true;

    void Awake()
    {
        print("Player_Movement Script Starting");
        playerScript.rb.velocity = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (!playerScript.activePlayer) return;

        SetPlayerOrientation();
        HandleJump();
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
            StartJump();
    }

    void HandleJump()
    {
        //used for timing jump Buffering and for coyote time
        jumpBufferTimer -= Time.deltaTime;
        coyoteTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        // resets coyote timer to give some leniency if the player leaves the platform a tad too late
        if (isGrounded)
            coyoteTimer = coyoteTimeForgiveness;

        // when the player presses jump, set that jump in the jump buffer 
        if (playerScript.inputScript.jumpPressed)
                jumpBufferTimer = jumpBuffer;

        // if there is a jump in the jump buffer and the player was recently on the ground then begin the jump process
        if (jumpBufferTimer > 0f && coyoteTimer > 0.0f && !hasJumped)
        {
            mayJump = true;
        }

        if ((playerScript.inputScript.jumpReleased || jumpTimer < 0.0f) && playerScript.rb.velocity.y > 0f && !stopJump)
        {
            Debug.Log("Jump Action Ended...");

            //// if the player let go of the jump early
            //if (playerScript.inputScript.jumpReleased && playerScript.rb.velocity.y > 0)
            //{

            //}
            //// if the player holds throughout the entire jump
            //else if (playerScript.inputScript.jumpHeld && playerScript.rb.velocity.y > 0)
            //{

            //}

            EndJump();
        }

        //return;

        ////---------------------------------------------------
        //// Player holds Jump For the entire duration of the Jump
        //// ideally this should be the farthest you can jump
        //if (playerScript.rb.velocity.y > 0 && playerScript.inputScript.jumpPressed)
        //{
        //    //Debug.Log("Player held jump all through the jump");
        //}
        //// Player is jumping but lets go of Jump
        //// ideally the player should descent pretty rapidly
        //else if (playerScript.rb.velocity.y > 0 && !playerScript.inputScript.jumpPressed)
        //{
        //    //Debug.Log("Player let go of jump midway of jumping");
        //}
        //// Player is falling and is holding Jump
        //// ideally the player should still land fast but not as fast as letting go comepletely
        //else if (playerScript.rb.velocity.y < 0 && playerScript.inputScript.jumpPressed)
        //{
        //    //Debug.Log("Player is falling while holding jump");
        //}
        //// Player is falling and lets go of Jump
        //// ideally the player should descent pretty rapidly
        //else if (playerScript.rb.velocity.y < 0 && !playerScript.inputScript.jumpPressed)
        //{
        //    //Debug.Log("Player is falling");
        //}
    }

    void StartJump()
    {
        Debug.LogWarning("begin jump");

        // setup for variable jumping
        //jumpState = JumpState.Rising;

        // will fix eventually
        var temp = playerScript.rb.velocity;
        temp.y = 0;
        playerScript.rb.velocity = temp;

        // resets the timers used for jumping buffering as well as coyote time
        coyoteTimer = -10.0f;
        jumpBufferTimer = -10.0f;

        //starts the jump timer
        jumpTimer = jumpDuration;
        mayJump = false;

        stopJump = false;

        //ADD GRAVITY SCALE HERE
        playerScript.rb.useGravity = false;

        // adds a force to the player in the up direction
        playerScript.rb.AddForce(Vector3.up * jumpForce * playerScript.rb.mass, ForceMode.Impulse);
        StartCoroutine(JumpCooldown());
    }

    void EndJump()
    {
        Debug.LogWarning("stopped jump");

        //jumpState = JumpState.Falling;

        coyoteTimer = -10.0f;

        playerScript.rb.useGravity = true;

        stopJump = true;
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

    private IEnumerator JumpCooldown()
    {
        hasJumped = true;
        yield return new WaitForSeconds(0.4f);
        hasJumped = false;
    }

    public enum JumpState
    {
        Grounded,
        Falling,
        Rising
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + groundCheckOffest, groundCheckRadius);

    }

}