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

    [Space(10)]

    [Header("OLOS Options: ")]
    [Tooltip("How fast the player must move in a direction to flip around. Aids in preventing random flips")]               // aids in preventing random player flipping 
    [SerializeField] internal float flipDeadzone = 0.1f;
    private bool facingRight = true;

    void Start()
    {
        print("Player_Movement Script Starting");
        playerScript.rb.velocity = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        SetPlayerOrientation();
    }

    // Fixed Update is called once per Physics Update
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // gets the input from the player
        Vector3 _move = new Vector3(playerScript.inputScript.moveInputX, 0, playerScript.inputScript.moveInputY);
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

    }

    void CheckStepHeight()
    {

    }

    void SetPlayerOrientation()
    {
        // gets the players velocity according to the players local space rather than world space
        float playerVelocity = transform.InverseTransformDirection(playerScript.rb.velocity).x;

        // Flips the player according to current facing direction and move direction
        if (playerVelocity > flipDeadzone && !facingRight)
        {
            Flip();
        }
        else if (playerVelocity < -flipDeadzone && facingRight)
        {
            Flip();
        }

    }

    void Flip()
    {
        // Switch the way the player is labeled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1 to flip the player's gameobject
        Vector3 flippedScale = transform.localScale;
        flippedScale.x *= -1;
        transform.localScale = flippedScale;
    }

}