using System.Data.Common;
using Unity.Cinemachine;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

namespace Sonar
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private GameObject cameraBoundaries;
        [SerializeField] private Vector2EventChannelSO moveCameraEvent;
        [SerializeField] private BoolEventChannelSO dragEvent;

        [SerializeField] private float moveSpeed = 10.0f;


        private Vector2 mousePosition;
        private bool isDragging;
        private Vector2 lastMousePosition;

        private Vector2 cameraMovement = Vector2.zero;


        // Parameters
        [SerializeField] private bool useEdgeScrolling = false;
        [SerializeField] private int edgeScrollSize = 30;
        [SerializeField] private bool useDragPan = false;
        [SerializeField] private bool useCameraMovement = false;

        [SerializeField] private bool isFrozen = false;
        // Pan behaviour
        private bool dragPanMoveActive;


        // Zoom behaviour
        private Vector2 mouseScrollDelta;
        private float targetFieldOfView = 15f;

        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float fieldOfViewIncreaseAmount = 5f;
        [SerializeField] private float fieldOfViewStart = 15f;
        [SerializeField] private float fieldOfViewMin = 5f;
        [SerializeField] private float fieldOfViewMax = 60f;

        [SerializeField] private Vector2EventChannelSO mousePositionEvent;
        [SerializeField] private Vector2EventChannelSO mouseScrollEvent;

        private void OnEnable()
        {
            if (dragEvent != null)
            {
                dragEvent.OnEventRaised += HandleDragEvent;
            }
            if (mousePositionEvent)
            {
                mousePositionEvent.OnEventRaised += HandleMousePositionEvent;
            }
            if (mouseScrollEvent)
            {
                mouseScrollEvent.OnEventRaised += HandleMouseScrollEvent;
            }
            if (moveCameraEvent)
            {
                moveCameraEvent.OnEventRaised += HandleCameraMovementKeyboardEvent;
            }
        }

        private void OnDisable()
        {
            if (dragEvent != null)
            {
                dragEvent.OnEventRaised -= HandleDragEvent;
            }
            if (mousePositionEvent)
            {
                mousePositionEvent.OnEventRaised -= HandleMousePositionEvent;
            }
            if (mouseScrollEvent)
            {
                mouseScrollEvent.OnEventRaised -= HandleMouseScrollEvent;
            }
            if (moveCameraEvent)
            {
                moveCameraEvent.OnEventRaised -= HandleCameraMovementKeyboardEvent;
            }
        }

        private void Start()
        {
            cinemachineCamera.Lens.FieldOfView = fieldOfViewStart;
            targetFieldOfView = fieldOfViewStart;
        }

        private void Update()
        {
            if (isFrozen)
            {
                return;
            }
            if (useEdgeScrolling)
            {
                HandleCameraMovementEdgeScrolling();
            }

            if (useCameraMovement)
            {
                Debug.Log($"CameraController: cameraMovenet = {cameraMovement}");
                HandleCameraMovement();
            }

            if (useDragPan)
            {
                HandleCameraMovementDragPan();
            }

            HandleCameraZoom_FieldOfView();
        }

        private void HandleCameraMovement()
        {
            Vector3 inputDir = Vector3.zero;
            if (cameraMovement.x < 0f)
            {
                inputDir.x = -1f;
            }

            if (cameraMovement.x > 0f)
            {
                inputDir.x = 1f;
            }

            if (cameraMovement.y < 0f)
            {
                inputDir.z = -1f;
            }

            if (cameraMovement.y > 0f)
            {
                inputDir.z = 1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
            Vector3 size = cameraBoundaries.GetComponent<Collider>().bounds.size;
            Debug.Log($"CameraController: Boundaries size : {size}");
            newPosition.x = Mathf.Clamp(newPosition.x, -size.x / 2, size.x / 2);
            newPosition.z = Mathf.Clamp(newPosition.z, -size.z / 2, size.z / 2);
            transform.position = newPosition;            
        }

        private void HandleCameraMovementKeyboardEvent(Vector2 movement)
        {
            cameraMovement = movement;
        }

        private void HandleCameraMovementEdgeScrolling()
        {
            Vector3 inputDir = Vector3.zero;

            if (mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -1f;
            }

            if (mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -1f;
            }

            if (mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = +1f;
            }

            if (mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = +1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
            Vector3 size = cameraBoundaries.GetComponent<Collider>().bounds.size;
            Debug.Log($"CameraController: Boundaries size : {size}");
            newPosition.x = Mathf.Clamp(newPosition.x, -size.x / 2, size.x / 2);
            newPosition.z = Mathf.Clamp(newPosition.z, -size.z / 2, size.z / 2);
            transform.position = newPosition;
        }
        
        /// <summary>
        /// Checks if target point is inside cuboid.
        /// </summary>
        /// <param name="cuboidCentre"> centre of the cuboid</param>
        /// <param name="cuboidSize"> cuboid size vector</param>
        /// <param name="targetPosition"> target point transform</param>
        /// <returns></returns>
        public bool IsInsideCuboid(Transform cuboidCentre, Vector3 cuboidSize, Vector3 targetPosition)
        {
        Vector3 cubeCentreLocalPos = cuboidCentre.localPosition;
        Vector3 P1 = new Vector3(cubeCentreLocalPos.x - cuboidSize.x / 2, cubeCentreLocalPos.y - cuboidSize.y / 2, cubeCentreLocalPos.z - cuboidSize.z / 2);
        Vector3 P1Local = cuboidCentre.TransformPoint(P1);
        Vector3 P2 = new Vector3(cubeCentreLocalPos.x - cuboidSize.x / 2, cubeCentreLocalPos.y - cuboidSize.y / 2, cubeCentreLocalPos.z + cuboidSize.z / 2);
        Vector3 P2Local = cuboidCentre.TransformPoint(P2);
        Vector3 P4 = new Vector3(cubeCentreLocalPos.x + cuboidSize.x / 2, cubeCentreLocalPos.y - cuboidSize.y / 2, cubeCentreLocalPos.z - cuboidSize.z / 2);
        Vector3 P4Local = cuboidCentre.TransformPoint(P4);
        Vector3 P5 = new Vector3(cubeCentreLocalPos.x - cuboidSize.x / 2, cubeCentreLocalPos.y + cuboidSize.y / 2, cubeCentreLocalPos.z - cuboidSize.z / 2);
        Vector3 P5Local = cuboidCentre.TransformPoint(P5);

        Vector3 i = P2Local - P1Local;
        Vector3 j = P4Local - P1Local;
        Vector3 k = P5Local - P1Local;
        Vector3 v = targetPosition - P1Local;

        float vdoti = Vector3.Dot(v, i);
        float vdotj = Vector3.Dot(v, j);
        float vdotk = Vector3.Dot(v, k);
        float idoti = Vector3.Dot(i, i);
        float jdotj = Vector3.Dot(j, j);
        float kdotk = Vector3.Dot(k, k);

        if (vdoti > 0 && vdoti < idoti && vdotj > 0 && vdotj < jdotj && vdotk > 0 && vdotk < kdotk)
        {
            // inside cuboid
            return true;
        }

        // outside cuboid
        return false;
        }

        private void HandleCameraMovementDragPan()
        {
            Vector3 inputDir = Vector3.zero;

            if (isDragging)
            {
                dragPanMoveActive = true;
                lastMousePosition = mousePosition;
            }
            else
            {
                dragPanMoveActive = false;
            }

            if (dragPanMoveActive)
            {
                Vector2 mouseMovementDelta = (Vector2)mousePosition - lastMousePosition;

                float dragPanSpeed = 1f;
                inputDir.x = mouseMovementDelta.x * dragPanSpeed;
                inputDir.z = mouseMovementDelta.y * dragPanSpeed;

                lastMousePosition = mousePosition;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        private void HandleDragEvent(bool isDragging)
        {
            this.isDragging = isDragging;
        }

        private void HandleMousePositionEvent(Vector2 mousePosition)
        {
            this.mousePosition = mousePosition;
            //Debug.Log($"CameraController: mouse position = {mousePosition}");
        }

        private void HandleMouseScrollEvent(Vector2 mouseScroll)
        {
            this.mouseScrollDelta = mouseScroll;
            Debug.Log($"CameraController: mouse scroll = {mouseScrollDelta}");
            if (mouseScrollDelta.y > 0)
            {
                targetFieldOfView -= fieldOfViewIncreaseAmount;
            }
            if (mouseScrollDelta.y < 0)
            {
                targetFieldOfView += fieldOfViewIncreaseAmount;
            }

            Debug.Log($"CameraController: new field of view before clamp: {targetFieldOfView}");
            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

            this.mouseScrollDelta = Vector2.zero;
        }

        private void HandleCameraZoom_FieldOfView()
        {
            //Debug.Log($"CameraController: applying zoom with mouseScrollDelta: {mouseScrollDelta}");

            Debug.Log($"CameraController: applying new field of view: {targetFieldOfView}");
            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }
    }
}

