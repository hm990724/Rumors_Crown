using UnityEngine;
using Rumors.Core;

namespace Rumors.World
{
    /// <summary>
    /// Camera pan/drag/zoom using legacy Input API (activeInputHandler = Both).
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Pan")]
        [SerializeField] private float panSpeed  = 8f;
        [SerializeField] private float dragSpeed = 1f;

        [Header("Zoom")]
        [SerializeField] private float zoomMin   = 2.5f;
        [SerializeField] private float zoomMax   = 7.5f;
        [SerializeField] private float zoomSpeed = 2.5f;

        private Camera _cam;
        private Vector3 _dragOrigin;
        private bool    _dragging;
        private Vector3 _focusTarget;
        private bool    _focusing;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _cam = GetComponent<Camera>() ?? Camera.main;
        }

        private void Update()
        {
            if (_cam == null) return;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C))
                MoveTo(Vector3.zero);
            Pan();
            Drag();
            Zoom();
            Focus();
            Clamp();
        }

        private void Pan()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f) return;
            _focusing = false;
            float mod = _cam.orthographicSize / 6f;
            transform.position += new Vector3(h, v) * (panSpeed * mod * Time.deltaTime);
        }

        private void Drag()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                _dragging   = true;
                _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
                _focusing   = false;
            }
            if ((Input.GetMouseButton(1) || Input.GetMouseButton(2)) && _dragging)
            {
                var cur = _cam.ScreenToWorldPoint(Input.mousePosition);
                transform.position += (_dragOrigin - cur) * dragSpeed;
            }
            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                _dragging = false;
        }

        private void Zoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.01f) return;
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - scroll * zoomSpeed * 2.5f, zoomMin, zoomMax);
        }

        private void Focus()
        {
            if (!_focusing) return;
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(_focusTarget.x, _focusTarget.y, transform.position.z),
                Time.deltaTime * 7f);
            if (Vector2.Distance(transform.position, _focusTarget) < 0.05f)
                _focusing = false;
        }

        private void Clamp()
        {
            float hw = WorldGrid.Width  * 0.5f;
            float hh = WorldGrid.Height * 0.5f;
            float cx = Mathf.Clamp(transform.position.x, -hw, hw);
            float cy = Mathf.Clamp(transform.position.y, -hh, hh);
            transform.position = new Vector3(cx, cy, transform.position.z);
        }

        public void MoveTo(Vector3 worldPos)
        {
            _focusTarget = worldPos;
            _focusing    = true;
        }

        public void MoveTo(Vector2Int gridPos)
        {
            if (WorldGrid.Instance != null)
                MoveTo(WorldGrid.Instance.GridToWorld(gridPos));
        }

        public void ResetToCenter() => MoveTo(WorldGrid.CastleGridPos);
    }
}
