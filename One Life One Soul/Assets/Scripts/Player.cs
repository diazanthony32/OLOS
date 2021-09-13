using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Scripts")]
    [Space(5)]
    //Store a reference to all the sub player scripts
    [SerializeField] internal Player_Input inputScript;
    [SerializeField] internal Player_Movement movementScript;
    [SerializeField] internal Player_Collision collisionScript;

    [Header("Player Attributes")]
    [Space(5)]
    [SerializeField] internal HealthState healthState = HealthState.Full;
    [SerializeField] internal PlayerState playerState = PlayerState.Idle;


    //component references
    internal Animator anim;
    internal Rigidbody rb;

    private void Awake()
    {
        print("Main Player Script Awake");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum HealthState
    {
        Dead,
        Quarter,
        Half,
        TwoThirds,
        Full
    }

    public enum PlayerState
    {
        Idle,
        Walking,
        Hurt,
        Dead,
        Jumping,
        Burning
    }
}
