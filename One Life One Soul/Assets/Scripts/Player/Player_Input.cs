using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    [Header("General Options: ")]
    [Tooltip("The max speed the physics character can move")]                   // String for getting a player's horizontal input
    [SerializeField] private string horizontalInputAxis = "Horizontal";

    [Tooltip("The max speed the physics character can move")]                   // String for getting a player's vertical input
    [SerializeField] private string verticalInputAxis = "Vertical";

    [Tooltip("The max speed the physics character can move")]                   // Key for making the player jump
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Tooltip("Bybass Unity's internal input smoothing")]                        // If this is enabled, Unity's internal input smoothing is bypassed
    [SerializeField] private bool useRawInput = false;


    [Header("OLOS Options: ")]
    [Tooltip("Option for splitting/merging to be done automatically based on player body " +                    // Asks the Player if they want splitting to be done automatically
             "proximity and splitting type. Disables manual splitting/merging")]                                // Will have to option to split either 'one-at-a-time' or 'soulCount-1' 
    [SerializeField] internal bool useAutoSplitMerge = true;

    [Tooltip("Input to auto split/merge the player's soul if 'Auto Split Merge' is enabled. " +                 // Key input for auto split merge or manual merging 
             "If not, it is to merge the player's soul")]
    [SerializeField] private KeyCode splitMergeKey = KeyCode.F;

    [Tooltip("Input to swap actively controlled soul")]                                                         // Key for swapping camera follow target
    [SerializeField] internal SplitBy splitBy = SplitBy.Max;

    [Tooltip("Input to swap actively controlled soul")]                                                         // Key for swapping camera follow target
    [SerializeField] private KeyCode swapSoulKey = KeyCode.Tab;

    [Tooltip("Input to rotate the player's camera clockwise")]                                                  // Key for rotating camera clockwise
    [SerializeField] private KeyCode rotateClockwiseKey = KeyCode.Q;

    [Tooltip("Input to rotate the player's camera counter-clockwise")]                                          // Key for rotating camera counter-clockwise
    [SerializeField] private KeyCode rotateCounterClockwiseKey = KeyCode.E;


    // what other scripts will have access to
    internal float moveInputX => useRawInput ? Input.GetAxisRaw(horizontalInputAxis) : Input.GetAxis(horizontalInputAxis);
    internal float moveInputY => useRawInput ? Input.GetAxisRaw(verticalInputAxis) : Input.GetAxis(verticalInputAxis);

    internal bool jump => Input.GetKeyDown(jumpKey);

    internal bool splitMerge => Input.GetKeyDown(splitMergeKey);
    internal bool swapSoul => Input.GetKeyDown(swapSoulKey);

    internal bool rotateCamClockwise => Input.GetKeyDown(rotateClockwiseKey);
    internal bool rotateCamCounterClockwise => Input.GetKeyDown(rotateCounterClockwiseKey);

    internal Player.SplitState split;

    internal bool combine;

    // Start is called before the first frame update
    void Start()
    {
        print("Player_Input Script Starting");
    }

    public int GetPressedNumber()
    {
        for (int number = 1; number <= 3; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }

        return -1;
    }

    public enum SplitBy
    {
        Min,
        Max
    }
}