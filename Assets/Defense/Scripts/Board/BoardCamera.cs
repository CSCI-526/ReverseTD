using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// Keeps the whole board in view of an orthographic camera, at any screen aspect ratio.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class BoardCamera : MonoBehaviour
    {
        [SerializeField, Tooltip("The board to keep in view.")]
        private BoardView board;

        [SerializeField, Min(0f), Tooltip("Empty space around the board, in world units.")]
        private float padding = 0.5f;

        private Camera cachedCamera;
        private float fittedAspect;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            Fit();
        }

        // The Game view or the device can change aspect ratio while running.
        private void LateUpdate()
        {
            if (!Mathf.Approximately(cachedCamera.aspect, fittedAspect))
            {
                Fit();
            }
        }

        private void Fit()
        {
            fittedAspect = cachedCamera.aspect;
            if (board == null || board.Layout == null)
            {
                return;
            }

            Bounds bounds = board.Layout.CalculateBounds();
            cachedCamera.orthographic = true;
            cachedCamera.orthographicSize = Mathf.Max(bounds.extents.y, bounds.extents.x / fittedAspect) + padding;
            transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
        }
    }
}
