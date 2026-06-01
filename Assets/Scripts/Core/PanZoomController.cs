using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanZoomController : MonoBehaviour
{
    [Header("Pan - Drag")]
    [SerializeField] private bool _enableDragPan = true;

    [Header("Pan - Keyboard (WASD + Arrows)")]
    [SerializeField] private bool _enableKeyboardPan = true;
    [SerializeField] private float _keyboardPanSpeed = 5f;

    [Header("Pan - Edge Scroll")]
    [SerializeField] private bool _enableEdgeScroll = true;
    [SerializeField] private float _edgeScrollSpeed = 5f;
    [SerializeField] private float _edgeThickness = 20f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 3f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 12f;

    [Header("Bounds")]
    [SerializeField] private BoxCollider2D _boundsCollider;
    [SerializeField] private bool _useBounds = false;

    [Header("State Persistence")]
    [Tooltip("If set, camera position and zoom are saved/restored via PlayerPrefs under this key.")]
    [SerializeField] private string _saveKey = "";

    private Camera _cam;
    private bool _isDragging;
    private Vector3 _dragOriginWorld;
    private Bounds _bounds;

    void Awake()
    {
        _cam = GetComponent<Camera>();

        if (_useBounds && _boundsCollider != null)
            _bounds = _boundsCollider.bounds;
        else if (_useBounds)
            _useBounds = false;
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(_saveKey))
            LoadCameraState();
    }

    void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_saveKey))
            SaveCameraState();
    }

    void OnDisable()
    {
        _isDragging = false;
    }

    private void SaveCameraState()
    {
        PlayerPrefs.SetFloat(_saveKey + "_PosX", transform.position.x);
        PlayerPrefs.SetFloat(_saveKey + "_PosY", transform.position.y);
        PlayerPrefs.SetFloat(_saveKey + "_Zoom", _cam.orthographicSize);
        PlayerPrefs.Save();
    }

    private void LoadCameraState()
    {
        if (!PlayerPrefs.HasKey(_saveKey + "_PosX")) return;

        Vector3 pos = transform.position;
        pos.x = PlayerPrefs.GetFloat(_saveKey + "_PosX");
        pos.y = PlayerPrefs.GetFloat(_saveKey + "_PosY");
        transform.position = pos;
        _cam.orthographicSize = PlayerPrefs.GetFloat(_saveKey + "_Zoom");
    }

    void Update()
    {
        HandleDragPan();
        HandleKeyboardPan();
        HandleEdgeScroll();
        HandleZoom();
    }

    // ── Drag ──────────────────────────────────────────────────────────────────

    private void HandleDragPan()
    {
        if (!_enableDragPan) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverInteractableUI()) return;

            _dragOriginWorld = _cam.ScreenToWorldPoint(mouse.position.ReadValue());
            _isDragging = true;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
            _isDragging = false;

        if (!_isDragging) return;

        Vector3 currentWorld = _cam.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector3 target = transform.position + (_dragOriginWorld - currentWorld);
        target.z = transform.position.z;

        if (_useBounds)
        {
            Vector3 clamped = ClampToBounds(target);
            clamped.z = target.z;
            _dragOriginWorld += clamped - target;
            target = clamped;
        }

        transform.position = target;
    }

    // ── Keyboard ──────────────────────────────────────────────────────────────

    private void HandleKeyboardPan()
    {
        if (!_enableKeyboardPan) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Vector3 dir = Vector3.zero;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    dir.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  dir.y -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dir.x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  dir.x -= 1f;

        if (dir == Vector3.zero) return;

        MoveCamera(dir.normalized * _keyboardPanSpeed * Time.deltaTime);
    }

    // ── Edge Scroll ───────────────────────────────────────────────────────────

    private void HandleEdgeScroll()
    {
        if (!_enableEdgeScroll) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePos = mouse.position.ReadValue();
        Vector3 dir = Vector3.zero;

        if (mousePos.x < _edgeThickness)                  dir.x -= 1f;
        if (mousePos.x > Screen.width - _edgeThickness)   dir.x += 1f;
        if (mousePos.y < _edgeThickness)                   dir.y -= 1f;
        if (mousePos.y > Screen.height - _edgeThickness)   dir.y += 1f;

        if (dir == Vector3.zero) return;

        MoveCamera(dir.normalized * _edgeScrollSpeed * Time.deltaTime);
    }

    // ── Zoom ──────────────────────────────────────────────────────────────────

    private void HandleZoom()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f)) return;
        if (IsPointerOverInteractableUI()) return;

        _cam.orthographicSize = Mathf.Clamp(
            _cam.orthographicSize - scroll * _zoomSpeed * Time.deltaTime,
            _minZoom,
            _maxZoom
        );

        if (_useBounds)
            ApplyBounds();
    }

    // ── Shared ────────────────────────────────────────────────────────────────

    private void MoveCamera(Vector3 delta)
    {
        Vector3 target = transform.position + delta;
        target.z = transform.position.z;

        if (_useBounds)
        {
            target = ClampToBounds(target);
            target.z = transform.position.z;
        }

        transform.position = target;
    }

    private void ApplyBounds()
    {
        Vector3 clamped = ClampToBounds(transform.position);
        clamped.z = transform.position.z;
        transform.position = clamped;
    }

    private Vector3 ClampToBounds(Vector3 target)
    {
        float halfH = _cam.orthographicSize;
        float halfW = _cam.orthographicSize * _cam.aspect;

        target.x = Mathf.Clamp(target.x, _bounds.min.x + halfW, _bounds.max.x - halfW);
        target.y = Mathf.Clamp(target.y, _bounds.min.y + halfH, _bounds.max.y - halfH);

        return target;
    }

    private bool IsPointerOverInteractableUI()
    {
        if (EventSystem.current == null) return false;

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
                return true;

        return false;
    }
}
