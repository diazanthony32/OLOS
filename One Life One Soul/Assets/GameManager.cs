using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public List<Player> playerlist = new List<Player>();
    public List<Tilemap> tileMaps = new List<Tilemap>();


    public List<Vector3> rotationOrientations = new List<Vector3>();
    int index = 0;

    private static Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);
    private static Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateTileMaps(int dir)
    {
        index += dir;
        Debug.Log(index);

        if (index < 0) {
            index = (rotationOrientations.Count - 1);
        }
        else if (index > rotationOrientations.Count-1)
        {
            index = 0;
        }
        Debug.Log(index);


        foreach (Tilemap tileMap in tileMaps)
        {
            SetTileMapOrientation(tileMap,rotationOrientations[index]);
        }
    }

    public void AddTileMap(Tilemap tileMap)
    {
        tileMaps.Add(tileMap);
    }

    void SetTileMapOrientation(Tilemap tileMap, Vector3 newOrientation)
    {
        // Set a Quaternion from the specified Euler angles.
        Quaternion rotation = Quaternion.Euler(newOrientation.x, newOrientation.y, newOrientation.z);

        // Set the translation, rotation and scale parameters.
        Matrix4x4 newMatrix = Matrix4x4.TRS(translation, rotation, scale);
        tileMap.orientationMatrix = newMatrix;
    }
}
