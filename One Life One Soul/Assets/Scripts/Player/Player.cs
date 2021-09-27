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
    [SerializeField] internal SplitState splitState = SplitState.Full;
    [SerializeField] internal PlayerState playerState = PlayerState.Idle;

    [Header("Body Sprites")]
    [Space(5)]
    [SerializeField] internal SpriteRenderer[] spriteRenderers;

    //component references
    internal Animator anim;
    internal Rigidbody rb;
    internal SpriteRenderer sr;

    internal bool activePlayer = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        print("Main Player Script Awake");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();
        sr = GetComponentInChildren<SpriteRenderer>();

        foreach (Renderer renderer in spriteRenderers)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
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

        // Used to rotate the player sprites when the player is no longer in control
        OnCameraUpdate();
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

        // To check out the different easing types, go to: https://easings.net/
        LeanTween.rotate(playerCam.gameObject, tempRot, rotationCooldown).setEaseInOutSine();
        tempRot.x = 0.0f; // this tos to offset the camera pointing down angle
        LeanTween.rotate(this.gameObject, tempRot, rotationCooldown).setEaseInOutSine();
    }

    void OnCameraUpdate()
    {
        if (this.transform.eulerAngles.y != playerCam.gameObject.transform.eulerAngles.y && !activePlayer)
        {
            Debug.Log("Player Rotation is not aligned to the camera");
            Vector3 tempRot = this.transform.eulerAngles;
            tempRot.y = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.transform.eulerAngles.y;
            this.transform.eulerAngles = tempRot;
        }
    }

    /*

    Player Conditions:
    (4) Whole
    (3) Three Fourths - Either missing TL, TR, BL, BR
    (2) Half - Either LH, RH, TH, BH
    (1) One Fourth - TL, TR, BL, BR

    And Theoretically any Combination of that (EX. Having TL and BR, but not the others)

    Problem: WAY to many sprite Variations to properly Handle
    Solution: Two Sprites, The Dead Sprite always being there underneath, and having the other Sprites be in a List or Array
     
    */

    public enum SplitState
    {
        None,
        Quarter,
        Half,
        ThreeFourths,
        Full
    }

    public enum PlayerState
    {
        Idle,
        Walking,
        Hurt,
        Dead,
        Jumping,
        Falling,
        Burning
    }
}