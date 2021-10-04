using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RotateEnviromentItems : MonoBehaviour
{
    private Renderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<Renderer>();

        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spriteRenderer.receiveShadows = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.eulerAngles.y != Camera.main.gameObject.transform.eulerAngles.y)
        {
            //Debug.Log("Player Rotation is not aligned to the camera");
            Vector3 tempRot = this.transform.eulerAngles;
            tempRot.y = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.transform.eulerAngles.y;
            this.transform.eulerAngles = tempRot;
        }
    }
}
