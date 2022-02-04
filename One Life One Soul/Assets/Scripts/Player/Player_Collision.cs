using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Collision : MonoBehaviour
{
    [SerializeField]
    internal Player playerScript;

    internal CapsuleCollider _playerCollider;

    // Start is called before the first frame update
    void Awake()
    {
        print("Player_Collision Script Starting");
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    // 
    private void OnCollisionEnter(Collision collision)
    {
        
    }

    // 
    private void OnCollisionStay(Collision collision)
    {
        
    }

    // 
    private void OnCollisionExit(Collision collision)
    {
        
    }

    // 
    private void OnParticleCollision(GameObject other)
    {
        
    }

}
