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

    internal bool bodyMorph;


    // Start is called before the first frame update
    void Start()
    {
        print("Player_Input Script Starting");
    }

    // Update is called once per frame
    void Update()
    {
        //if (playerScript != this.GetComponent<PlayerController>().currentPlayerScript) return;

        //This is a Ternary Operation that chooses a message based if the selected Key is Being Pressed
        //isLeftPressed = Input.GetKey(KeyCode.A) ? true : false;
        //isRightPressed = Input.GetKey(KeyCode.D) ? true : false;

        //isUpPressed = Input.GetKey(KeyCode.Space) ? true : false;
        //isDownPressed = Input.GetKey(KeyCode.S) ? true : false;


        //x = (Input.GetAxisRaw("Horizontal") * playerScript.movementScript.movementSpeed);
        //y = Input.GetAxisRaw("Vertical");
        //jumping = Input.GetButton("Jump");
        //crouching = Input.GetKey(KeyCode.LeftControl);

        //bodyMorph = Input.GetKeyDown(KeyCode.M);

        //Debug.Log(jumping);

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

    }
}
