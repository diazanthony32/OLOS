using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    //reference to the main player script
    [SerializeField]
    Player playerScript;

    [Header("Movement Settings")]
    [Space(5)]

    //main player properties
    [SerializeField]
    internal float movementSpeed;

    // Amount of force added when the player jumps.
    [SerializeField]
    private float m_JumpForce = 400f;

    // How much to smooth out the movement
    [Range(0, .3f)]
    [SerializeField]
    private float m_MovementSmoothing = .05f;

    // Whether or not a player can steer while jumping;
    [SerializeField]
    internal bool m_AirControl = false;

    // A mask determining what is ground to the character
    [SerializeField]
    internal LayerMask m_WhatIsGround;

    // A position marking where to check if the player is grounded.
    [SerializeField]
    internal Transform m_GroundCheck;

    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float slowFallMultiplier = 2f;
    [SerializeField] private float fastFallMultiplier = 2f;


    // Buffers a jump for the Player in case the jump a fraction of a second too late
    internal float coyoteTimeForgiveness = 0.25f;
    internal float mayJump;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    internal bool m_FacingRight = true;  // For determining which way the player is currently facing.

    private Vector3 velocity = Vector3.zero;

    internal bool softLand = true;

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
        m_Grounded = false;

        // Checking the player's ground Check to see if the player is currently grounded
        Collider[] hitColliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (var hitCollider in hitColliders)
        {
            m_Grounded = true;
            mayJump = coyoteTimeForgiveness;

            if (!softLand)
            {
                // reset Vertical velocity to prevent falling through the floor slightly?
                Vector3 tmpVel = playerScript.rb.velocity;
                tmpVel.y = 0.0f;
                playerScript.rb.velocity = tmpVel;
                softLand = true;
            }
        }

        Movement();
    }

    void Movement()
    {
        // Move our character
        Move(playerScript.inputScript.moveInput, playerScript.inputScript.jumping);

    }

    public void Move(Vector3 move, bool jump)
    {
        // Move the character by finding the target velocity
        // And then smoothing it out and applying it to the character
        Vector3 targetVelocity = new Vector3(move.x * movementSpeed * Time.fixedDeltaTime, playerScript.rb.velocity.y, move.y * movementSpeed * Time.fixedDeltaTime);
        playerScript.rb.velocity = Vector3.SmoothDamp(playerScript.rb.velocity, targetVelocity, ref velocity, m_MovementSmoothing);


        // If the player is moving to the right and the player is currently facing left...
        if (playerScript.rb.velocity.x > 0 && !m_FacingRight)
        {
            Flip();
        }
        // Otherwise if the player is moving to the left and the player is currently facing right...
        else if (playerScript.rb.velocity.x < 0 && m_FacingRight)
        {
            Flip();
        }

        // If the player should jump while on the ground or during the coyoteTimeForgiveness period, the Player Jumps
        if ((m_Grounded || mayJump > 0.0f) && jump)
        {
            m_Grounded = false;
            mayJump = 0.0f;

            // Add a vertical force to the player.
            Vector2 tempVelocity = playerScript.rb.velocity;
            tempVelocity.y = m_JumpForce;
            playerScript.rb.velocity = tempVelocity;

            softLand = false;

            //playerScript.ChangeState("Jumping");
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
            //playerScript.ChangeState("Idle");
        }

    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        playerScript.sr.flipX = !m_FacingRight;

        //// Multiply the player's x local scale by -1.
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;
        //transform.localScale = theScale;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_GroundCheck.position, k_GroundedRadius);
    }

}
