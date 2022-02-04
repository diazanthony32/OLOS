using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Options: ")]
    [Tooltip("")]
    [Space(5)]
    [SerializeField] internal float rotationDegrees;
    [SerializeField] internal float rotationSpeed;

    [SerializeField] internal AnimationCurve rotationCurve;

    internal float rotateCooldown = 0.0f;

    internal CinemachineVirtualCamera _CMVCamera;
    internal Player _currentPlayerScript;

    // Start is called before the first frame update
    void Start()
    {
        //_CMVCamera = GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _CMVCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((_currentPlayerScript.inputScript.rotateCamClockwise || _currentPlayerScript.inputScript.rotateCamCounterClockwise) && rotateCooldown < 0.0f)
        {
            rotateCooldown = rotationSpeed;
            RotateCam();
        }

        rotateCooldown -= Time.deltaTime;
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
        LeanTween.rotate(_CMVCamera.gameObject, tempRot, rotationSpeed).setEase(rotationCurve);
    }

    public void FollowTarget(Transform targetTransform)
    {
        //Debug.Log(GetComponent<CinemachineBrain>().OutputCamera.name);
        _CMVCamera.Follow = targetTransform;
    }

    public Quaternion getCMQuaternion()
    {
        return transform.localRotation;
    }
}
