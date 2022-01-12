using System.Collections;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Game Manager: ")]
    [Space(5)]
    [SerializeField] internal GameManager gameManager;

    [Header("Player Scripts: ")]
    [Space(5)]
    //Store a reference to all the sub player scripts
    [SerializeField] internal Player_Input inputScript;
    [SerializeField] internal Player_Movement movementScript;
    [SerializeField] internal Player_Collision collisionScript;
    [SerializeField] internal Player_Split splitScript;
    [SerializeField] internal Player_SpriteManager spriteManager;


    internal CameraController cameraController;

    [Header("Player Attributes: ")]
    [Space(5)]
    [SerializeField] internal SplitState splitState = SplitState.Full;
    [SerializeField] internal PlayerState playerState = PlayerState.Idle;

    //component references
    internal Animator anim;
    internal Rigidbody rb;

    internal bool activePlayer = false;

    private void Awake()
    {
        print("Main Player Script Awake");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    // Start is called before the first frame update
    // Gets the Player Camera and Game Manager since the prefab doesnt like doing it
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.playerlist.Add(this);

        SetActive();
    }

    // Update is called once per frame
    void Update()
    {
        OutOfBoundsHandler();
    }

    void OutOfBoundsHandler()
    {
        if (transform.position.y < -50.0f)
            transform.position = Vector3.zero;
    }

    internal void ChangeState(PlayerState newState)
    {
        if (newState != playerState)
        {
            Debug.Log("Triggering: \"" + newState + "\" Animation!");

            //anim.ResetTrigger(currentState);
            anim.SetTrigger(newState.ToString());

            playerState = newState;
        }
    }

    // Takes care of unexpected death
    public void Die()
    {
        Debug.Log("b4: " + gameManager.playerlist.Count);

        activePlayer = false;
        gameManager.playerlist.Remove(this);

        Debug.Log("afr: " + gameManager.playerlist.Count);


        if (gameManager.playerlist.Count > 0)
        {
            cameraController.FollowTarget(gameManager.playerlist[gameManager.playerlist.Count - 1].transform);
            gameManager.playerlist[gameManager.playerlist.Count - 1].SetActive();
        }

        Destroy(this.gameObject);
    }

    // Sets al the needed variables in order to control/disable a player with an optional delay
    public void SetActive(bool state = true, float delay = 1.0f)
    {
        if (state)
        {
            Camera.main.GetComponent<CameraController>()._currentPlayerScript = this;
            cameraController.FollowTarget(this.transform);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Soul");
        }

        StartCoroutine(ExecuteAfterDelay(state, delay));
    }

    IEnumerator ExecuteAfterDelay(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Code to execute after the delay
        activePlayer = state ? true : false;
    }

    /*

    Player Conditions:
    (4) Whole
    (3) Three Fourths - Either missing TL, TR, BL, BR
    (2) Half - Either LH, RH, TH, BH
    (1) One Fourth - TL, TR, BL, BR

    And Theoretically any Combination of that (EX. Having TL and BR, but not the others)

    Problem: WAY to many sprite variations to properly Handle
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