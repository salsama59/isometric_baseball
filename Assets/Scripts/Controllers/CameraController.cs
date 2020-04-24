using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform targetTransform;
    [SerializeField]
    private float cameraMovementSpeed;
    [SerializeField]
    private float zoomAmount;
    [SerializeField]
    private float zoomSpeed;
    private Camera cameraComponent;
    private float defaultOrthograficSize;
    private float minimumZoomAmount;
    private float maximumZoomAmount;
    private float targetZoom;
    private bool isFocusedOnPlayerForTurn;
    private GameObject ballGameObject;

    // Start is called before the first frame update
    void Start()
    {
        CameraComponent = CameraUtils.FetchMainCamera();
        DefaultOrthograficSize = CameraComponent.orthographicSize;
        TargetZoom = DefaultOrthograficSize;
        MaximumZoomAmount = DefaultOrthograficSize * 2;
        MinimumZoomAmount = DefaultOrthograficSize / 2;
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateCameraFocusState();
        if (!IsFocusedOnPlayerForTurn)
        {
            this.SwitchToBallTarget();
        }
        this.FollowTarget();
        this.ZoomCamera();
    }

    public void FocusOnPlayer(Transform playerTransform)
    {
        IsFocusedOnPlayerForTurn = true;
        TargetTransform = playerTransform;
    }

    private void UpdateCameraFocusState()
    {
        if (!PlayersTurnManager.IsCommandPhase)
        {
            IsFocusedOnPlayerForTurn = false;
        }
    }

    private void SwitchToBallTarget()
    {
        //Simplified form to check null value instead of a ternary condition
        TargetTransform = BallGameObject?.transform;
    }

    private void ZoomCamera()
    {
        float mouseScrollAmount = Input.GetAxis("Mouse ScrollWheel");
        TargetZoom -= mouseScrollAmount * zoomAmount;
        TargetZoom = Mathf.Clamp(TargetZoom, MinimumZoomAmount, MaximumZoomAmount);
        CameraComponent.orthographicSize = Mathf.Lerp(CameraComponent.orthographicSize, TargetZoom, Time.deltaTime * zoomSpeed);
    }

    private void FollowTarget()
    {
        if (TargetTransform != null)
        {
            Vector3 finalTargetPosition = new Vector3(TargetTransform.position.x, TargetTransform.position.y, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, finalTargetPosition, CameraMovementSpeed);
        }
    }

    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }
    public float CameraMovementSpeed { get => cameraMovementSpeed; set => cameraMovementSpeed = value; }
    public float ZoomAmount { get => zoomAmount; set => zoomAmount = value; }
    public float ZoomSpeed { get => zoomSpeed; set => zoomSpeed = value; }
    public Camera CameraComponent { get => cameraComponent; set => cameraComponent = value; }
    public float DefaultOrthograficSize { get => defaultOrthograficSize; set => defaultOrthograficSize = value; }
    public float MinimumZoomAmount { get => minimumZoomAmount; set => minimumZoomAmount = value; }
    public float MaximumZoomAmount { get => maximumZoomAmount; set => maximumZoomAmount = value; }
    public float TargetZoom { get => targetZoom; set => targetZoom = value; }
    public bool IsFocusedOnPlayerForTurn { get => isFocusedOnPlayerForTurn; set => isFocusedOnPlayerForTurn = value; }
    public GameObject BallGameObject { get => ballGameObject; set => ballGameObject = value; }
}
