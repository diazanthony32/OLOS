using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Collision : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    internal CapsuleCollider _playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        print("Player_Collision Script Starting");

        _playerCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
