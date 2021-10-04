using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    internal Vector3 moveInput;

    internal bool jumping = false;

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
            //https://docs.unity3d.com/ScriptReference/KeyCode.html

            // Directional Movement
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            // Jumping
            jumping = Input.GetButton("Jump");

            // Camera Rotation
            rotateCamClockwise = Input.GetKeyDown(KeyCode.Q);
            rotateCamCounterClockwise = Input.GetKeyDown(KeyCode.E);

            // Splitting
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

            // Combine
            combine = Input.GetKeyDown(KeyCode.C);
        }
        else
        {
            split = Player.SplitState.None;
            moveInput = Vector3.zero;
            combine = false;
            jumping = false;
            rotateCamClockwise = false;
            rotateCamCounterClockwise = false;
        }
    }
}