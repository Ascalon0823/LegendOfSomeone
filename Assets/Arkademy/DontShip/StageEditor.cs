using System;
using CGS.Grid;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Arkademy.DontShip
{
    public class StageEditor : MonoBehaviour
    {
        private SquareGrid2D<int> Grid2D => StageManager.Curr.Grid;
        private StageBuilder Builder => StageManager.Curr.Builder;

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
            return;
#endif
            brushValue = -1;
        }


        #region InputControl

        public Vector2Int CurrCoord;
        [SerializeField] private GameObject cursorPrefab;
        [SerializeField] private GameObject currCursor;
        [Header("Camera control")] public Camera currCamera;
        public bool cam;
        public Vector2 delta;
        public Vector2 pos;
        public bool interact;
        public float zoom;

        public Action OnInteractBegin;
        public Action OnInteractEnd;

        private void Update()
        {
            var mousePos = currCamera.ScreenToWorldPoint(pos);
            CurrCoord = Grid2D.FromPos(new Vector3(mousePos.x, mousePos.y, 0f));
        }

        private void LateUpdate()
        {
            //ZoomCamera();
            MoveCamera();
            ClampCamPos();
            UpdateCursor();
            if (interact)
            {
                Paint();
            }
        }

        private void UpdateCursor()
        {
            if (!currCursor) currCursor = Instantiate(cursorPrefab, transform);
            currCursor.transform.position = Grid2D.GetPos(CurrCoord);
        }


        public void OnCam(InputAction.CallbackContext ctx)
        {
            if (!EventSystem.current.IsPointerOverGameObject() || cam)
            {
                cam = ctx.ReadValueAsButton();
            }
        }

        public void OnDelta(InputAction.CallbackContext ctx)
        {
            delta = ctx.ReadValue<Vector2>();
        }

        public void OnPos(InputAction.CallbackContext ctx)
        {
            pos = ctx.ReadValue<Vector2>();
        }


        public void OnInteract(InputAction.CallbackContext ctx)
        {
            var wasInteract = interact;
            if (!EventSystem.current.IsPointerOverGameObject() || interact)
            {
                interact = ctx.ReadValueAsButton();
            }

            if (!wasInteract && interact)
            {
                OnBeginInteract();
            }

            if (wasInteract && !interact)
            {
                OnEndInteract();
            }
        }

        public void OnZoom(InputAction.CallbackContext ctx)
        {
            if (!EventSystem.current.IsPointerOverGameObject() || cam || interact)
            {
                zoom = ctx.ReadValue<float>();
            }
        }


        [SerializeField] private Vector3 camWorldPos;
        [SerializeField] private Vector2 camPos;
        [SerializeField] private Vector2 camVelocity;

        [SerializeField] private int decelerateMulti;
        // [SerializeField] private float zoomSpeedMulti;
        // [SerializeField] private float zoomSpeed;
        // [SerializeField] private float zoomDecelMulti;


        private void MoveCamera()
        {
            if (cam)
            {
                var worldDisp = currCamera.ScreenToWorldPoint(pos) - currCamera.ScreenToWorldPoint(camPos);
                camVelocity = (currCamera.ScreenToWorldPoint(delta) -
                               currCamera.ScreenToWorldPoint(Vector2.zero)) / Time.deltaTime;
                currCamera.transform.position = camWorldPos - worldDisp;
                return;
            }

            camVelocity = Vector2.Lerp(camVelocity, Vector2.zero, Time.deltaTime * Mathf.Max(1, decelerateMulti));
            currCamera.transform.position -= new Vector3(camVelocity.x, camVelocity.y, 0) * Time.deltaTime;
        }

        private void ClampCamPos()
        {
            var p = currCamera.transform.position;
            var camPosMin = Vector3.zero;
            var camPosMax = Grid2D.GetPos(
                Grid2D.Width(),
                Grid2D.Height());
            currCamera.transform.position = new Vector3(
                Mathf.Clamp(p.x, camPosMin.x, camPosMax.x),
                Mathf.Clamp(p.y, camPosMin.y, camPosMax.y),
                p.z);
            if (!cam)
            {
                camPos = pos;
                camWorldPos = currCamera.transform.position;
            }
        }

        // private void ZoomCamera()
        // {
        //     if (Mathf.Abs(zoom) > 0)
        //     {
        //         zoomSpeed = Mathf.Sign(zoom) * -Mathf.Max(2f, zoomSpeedMulti);
        //     }
        //
        //     zoomSpeed = Mathf.Lerp(zoomSpeed, 0f, Time.deltaTime * Mathf.Max(1, zoomDecelMulti));
        //     currCamera.orthographicSize += zoomSpeed * Time.deltaTime;
        //     currCamera.orthographicSize = Mathf.Clamp(currCamera.orthographicSize, 3, 10);
        // }

        private void SetCamPos(Vector2 coord)
        {
            camVelocity = Vector2.zero;
            currCamera.transform.position = new Vector3(coord.x, coord.y, currCamera.transform.position.z);
        }

        private void OnBeginInteract()
        {
            OnInteractBegin?.Invoke();
        }

        private void OnEndInteract()
        {
            OnInteractEnd?.Invoke();
        }

        #endregion

        public void ReBuild()
        {
            StageManager.Curr.BuildStage();
        }

        public void Save()
        {
            StageManager.Curr.StageData.mapData = StageManager.Curr.Grid.Data;
            StageManager.Curr.StageData.SaveStage();
        }

        public int brushValue = -1;

        public void SetBrush(int value)
        {
            brushValue = value;
        }

        public void Paint()
        {
            if (!Grid2D.IsValid(CurrCoord)) return;
            if (brushValue < 0)
            {
                switch (brushValue)
                {
                    case -3:
                        Builder.SetExit(CurrCoord.x, CurrCoord.y);
                        break;
                    case -2:
                        Builder.SetEnter(CurrCoord.x, CurrCoord.y);
                        break;
                    default:
                        break;
                }

                return;
            }

            Builder.SetGridDataAndTile(CurrCoord.x, CurrCoord.y, brushValue);
        }
    }
}