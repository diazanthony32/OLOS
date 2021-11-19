using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Options: ")]
    [Tooltip("")]
    [Space(5)]
    [SerializeField] internal float rotationDegrees;
    [SerializeField] internal float rotationCooldown;
    internal float mayRotate = 0.0f;

    internal CinemachineVirtualCamera _CMVCamera;
    internal Player _currentPlayerScript;

    // Start is called before the first frame update
    void Start()
    {
        _CMVCamera = GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_currentPlayerScript.inputScript.rotateCamClockwise || _currentPlayerScript.inputScript.rotateCamCounterClockwise) && mayRotate < 0.0f)
        {
            mayRotate = rotationCooldown;
            RotateCam();
        }

        // Used for Coyote Time 
        mayRotate -= Time.deltaTime;

        //transform.localRotation;
        //Debug.Log(transform.localRotation);
    }

    // Rotate Camera and Rotate any Enviroment Sprites
    // Enviroment SPrites will eventually have their own Script to handle any camera changes automatically...
    void RotateCam()
    {
        Vector3 tempRot = _CMVCamera.transform.eulerAngles;

        if (_currentPlayerScript.inputScript.rotateCamClockwise)
        {
            tempRot.y += rotationDegrees;
            //_currentPlayerScript.gameManager.RotateTileMaps(1);
        }
        else if (_currentPlayerScript.inputScript.rotateCamCounterClockwise)
        {
            tempRot.y -= rotationDegrees;
            //_currentPlayerScript.gameManager.RotateTileMaps(-1);
        }

        // To check out the different easing types, go to: https://easings.net/
        LeanTween.rotate(_CMVCamera.gameObject, tempRot, rotationCooldown).setEaseInOutSine();
    }

    public void FollowTarget(Transform targetTransform)
    {
        _CMVCamera.Follow = targetTransform;
    }

    public Quaternion getCMQuaternion()
    {
        return transform.localRotation;
    }
}
