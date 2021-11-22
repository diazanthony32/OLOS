using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    [Header("General Options: ")]
    [Tooltip("The max speed the physics character can move")]                   // String for getting a player's horizontal input
    [SerializeField] internal string horizontalInputAxis = "Horizontal";

    [Tooltip("The max speed the physics character can move")]                   // String for getting a player's vertical input
    [SerializeField] internal string verticalInputAxis = "Vertical";

    [Tooltip("The max speed the physics character can move")]                   // Key for making the player jump
    [SerializeField] internal KeyCode jumpKey = KeyCode.Space;

    [Tooltip("Bybass Unity's internal input smoothing")]                        // If this is enabled, Unity's internal input smoothing is bypassed
    [SerializeField] internal bool useRawInput = true;                          


    [Header("OLOS Options: ")]
    [Tooltip("Option for splitting/merging to be done automatically based on player body " +                    // Asks the Player if they want splitting to be done automatically
             "proximity and splitting type. Disables manual splitting/merging")]                                // Will have to option to split either 'one-at-a-time' or 'soulCount-1' 
    [SerializeField] internal bool useAutoSplitMerge = true;

    [Tooltip("Input to auto split/merge the player's soul if 'Auto Split Merge' is enabled. " +                 // Key input for auto split merge or manual merging 
             "If not, it is to merge the player's soul")]
    [SerializeField] internal KeyCode splitMergeKey = KeyCode.F;                                                      

    [Tooltip("Input to swap actively controlled soul")]                                                         // Key for swapping camera follow target
    [SerializeField] internal KeyCode swapSoulKey = KeyCode.Tab;                                                  

    [Tooltip("Input to rotate the player's camera clockwise")]                                                  // Key for rotating camera clockwise
    [SerializeField] internal KeyCode rotateClockwiseKey = KeyCode.Q;

    [Tooltip("Input to rotate the player's camera counter-clockwise")]                                          // Key for rotating camera counter-clockwise
    [SerializeField] internal KeyCode rotateCounterClockwiseKey = KeyCode.E;

    internal float moveInputX;
    internal float moveInputY;

    internal bool jump;

    internal bool rotateCamClockwise;
    internal bool rotateCamCounterClockwise;

    internal Player.SplitState split;

    internal bool combine;

    // Start is called before the first frame update
    void Start()
    {
        print("Player_Input Script Starting");
    }

    // Update is called once per frame
    void Update()
    {
        //This is a Ternary Operation that chooses a message based if the selected Key is Being Pressed
        //isLeftPressed = Input.GetKey(KeyCode.A) ? true : false;
        //isRightPressed = Input.GetKey(KeyCode.D) ? true : false;

        if (playerScript.activePlayer)
        {
            moveInputX = Input.GetAxisRaw(horizontalInputAxis.ToString());                                  //
            moveInputY = Input.GetAxisRaw(verticalInputAxis.ToString());                                    //

            jump = Input.GetKeyDown(jumpKey);                                                  //

            rotateCamClockwise = Input.GetKeyDown(rotateClockwiseKey);                           //
            rotateCamCounterClockwise = Input.GetKeyDown(rotateCounterClockwiseKey);             // 

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                split = Player.SplitState.Quarter;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                split = Player.SplitState.Half;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                split = Player.SplitState.ThreeFourths;
            }
            else
            {
                split = Player.SplitState.None;
            }

            combine = Input.GetKeyDown(KeyCode.C);
        }
        else
        {
            split = Player.SplitState.None;
            moveInputX = 0;
            moveInputY = 0;
            combine = false;
            jump = false;
            rotateCamClockwise = false;
            rotateCamCounterClockwise = false;
        }
    }

    public float GetHorizontalMovementInput()
    {
        if (useRawInput)
            return Input.GetAxisRaw(horizontalInputAxis);
        else
            return Input.GetAxis(horizontalInputAxis);
    }
}