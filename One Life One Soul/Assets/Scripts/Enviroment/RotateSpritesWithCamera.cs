using UnityEngine;
using UnityEngine.Tilemaps;

public class RotateSpritesWithCamera : MonoBehaviour
{
    private CameraController cameraController;

    private Renderer spriteRenderer;

    private Grid grid;
    private Tilemap tileMap;

    // Start is called before the first frame update
    void Awake()
    {
        // these are used for every attached gameobject
        cameraController = Camera.main.GetComponent<CameraController>();
        spriteRenderer = GetComponentInChildren<Renderer>();

        grid = GetComponentInChildren<Grid>();
        tileMap = GetComponentInChildren<Tilemap>();

        // sets shadows in case I forget to...
        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spriteRenderer.receiveShadows = true;
    }

    // Update is called once per frame
    void Update()
    {
        // this is to see if the attached gameObject is a tileMap in the right orientation (ex. spike pits)
        if (grid && tileMap && grid.cellSwizzle == Grid.CellSwizzle.XZY)
        {
            if (tileMap.orientationMatrix.rotation.eulerAngles.y != cameraController.transform.eulerAngles.y)
            {
                // sets the tile map y-rotation equal to the camera's y-rotation
                var temp = tileMap.orientationMatrix.rotation.eulerAngles;
                temp.y = cameraController.transform.eulerAngles.y;

                // Creates a new tileMap Matrix based the translation, rotation and scale parameters
                Matrix4x4 newMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(temp), tileMap.orientationMatrix.lossyScale);
                tileMap.orientationMatrix = newMatrix;
            }
        }
        // this is used for random objects placed around the level (ex. skulls)
        else if (transform.eulerAngles.y != cameraController.transform.eulerAngles.y)
        {
            // sets the gameobject orientation to the camera's
            var temp = transform.eulerAngles;
            temp.y = cameraController.transform.eulerAngles.y;

            transform.eulerAngles = temp;
        }
    }
}
