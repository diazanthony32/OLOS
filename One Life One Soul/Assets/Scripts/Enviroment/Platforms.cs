using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class Platforms : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotateAroundLocal(this.gameObject, Vector3.up, 360.0f, 10.0f).setLoopClamp();
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.name + " has entered the Trigger!");
        other.transform.parent = this.transform;

        //SetChildRotation(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //SetChildRotation(other);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.transform.name + " has left the Trigger...");
        other.transform.parent = null;

        //SetChildRotation(other);
    }

    //private void SetChildRotation(Collider other)
    //{
    //    Vector3 tempRot = other.transform.eulerAngles;
    //    tempRot.y = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.transform.eulerAngles.y;
    //    other.transform.eulerAngles = tempRot;
    //}
}
