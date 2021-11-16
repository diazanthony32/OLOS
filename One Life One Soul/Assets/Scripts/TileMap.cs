using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    private CameraController cameraController;

    private Renderer spriteRenderer;
    private Tilemap tileMap;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        spriteRenderer = GetComponentInChildren<Renderer>();
        tileMap = GetComponentInChildren<Tilemap>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spriteRenderer.receiveShadows = true;
    }

    private void Update()
    {
        OnCameraUpdate();
    }

    void OnCameraUpdate()
    {
        if (tileMap.orientationMatrix.rotation.z != cameraController.gameObject.transform.rotation.y)
        {
            // Set a Quaternion from the specified Euler angles.
            float cameraRotation = cameraController.gameObject.transform.rotation.eulerAngles.y;
            Vector3 fixedTemp = new Vector3(TranslateXRotation(-cameraRotation), TranslateYRotation(-cameraRotation), -cameraRotation);
            Debug.Log(fixedTemp);

            // Set the translation, rotation and scale parameters.
            Matrix4x4 newMatrix = Matrix4x4.TRS(tileMap.transform.localPosition, Quaternion.Euler(fixedTemp), tileMap.transform.localScale);
            tileMap.orientationMatrix = newMatrix;
        }
    }

    float TranslateXRotation(float zRotation)
    {
        return (-Mathf.Sin((zRotation + 90) / (180 / Mathf.PI)) * 45);
    }

    float TranslateYRotation(float zRotation)
    {
        return ( -Mathf.Sin(zRotation / (180/Mathf.PI)) * 45);
    }
}
