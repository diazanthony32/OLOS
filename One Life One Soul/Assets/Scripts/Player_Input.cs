using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    internal float x;
    //internal float y;
    internal bool jumping = false;
    internal bool crouching = false;

    internal bool bodyMorph;

    internal Vector3 moveInput;

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

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        jumping = Input.GetButton("Jump");

    }
}
