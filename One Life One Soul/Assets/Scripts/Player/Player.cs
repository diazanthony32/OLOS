using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Player Scripts")]
    [Space(5)]
    //Store a reference to all the sub player scripts
    [SerializeField] internal Player_Input inputScript;
    [SerializeField] internal Player_Movement movementScript;
    [SerializeField] internal Player_Collision collisionScript;

    [Space(5)]
    [SerializeField] internal CinemachineVirtualCamera playerCam;
    [SerializeField] internal float rotationCooldown = 0.1f;
    internal float mayRotate = 0.0f;
    [SerializeField] internal float rotationDegrees = 45.0f;

    [Header("Player Attributes")]
    [Space(5)]
    [SerializeField] internal HealthState healthState = HealthState.Full;
    [SerializeField] internal PlayerState playerState = PlayerState.Idle;


    //component references
    internal Animator anim;
    internal Rigidbody rb;
    internal SpriteRenderer sr;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        print("Main Player Script Awake");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((inputScript.rotateCamClockwise || inputScript.rotateCamCounterClockwise) && mayRotate < 0.0f)
        {
            mayRotate = rotationCooldown;
            RotateCam();
        }

        // Used for Coyote Time 
        mayRotate -= Time.deltaTime;
    }

    void RotateCam()
    {
        Vector3 tempRot = playerCam.transform.eulerAngles;

        if (inputScript.rotateCamClockwise)
        {
            tempRot.y += rotationDegrees;
        }
        else if (inputScript.rotateCamCounterClockwise)
        {
            tempRot.y -= rotationDegrees;
        }

        //playerCam.transform.eulerAngles = tempRot;
        LeanTween.rotate(playerCam.gameObject, tempRot, rotationCooldown);

        tempRot.x = 0.0f;
        LeanTween.rotate(this.gameObject, tempRot, rotationCooldown);
        //ChangeSpriteRotation();
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
