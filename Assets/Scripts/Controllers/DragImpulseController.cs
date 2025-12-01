using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Controllers
{
    public class DragImpulseController : MonoBehaviour
    {
        [Header("Line")]
        public LineRenderer line;
        public float lineFadeDuration = 0.2f;

        [Header("Power")]
        public float maxPower = 10f;

        public System.Action<Vector3> onImpulse;

        private bool isDragging;
        private Vector2 centerScreen;
        private Vector2 currentPointerPos;

        private void Start()
        {
            centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);

            if (line != null)
            {
                line.positionCount = 2;
                line.enabled = false;
            }
        }

        private void Update()
        {
            var touch = Touchscreen.current?.primaryTouch;

            if (touch != null && touch.press.isPressed)
            {
                Vector2 pos = touch.position.ReadValue();

                if (!isDragging)
                {
                    isDragging = true;
                    line.enabled = true;
                    //line.material.DOFade(1f, 0.1f);
                }

                currentPointerPos = pos;

                DrawLine();
            }
            else if (isDragging)
            {
                isDragging = false;
                //line.material.DOFade(0f, lineFadeDuration).OnComplete(() => line.enabled = false);

                SendImpulseToCar();
            }
        }

        private void DrawLine()
        {
            Vector3 worldStart = ScreenToWorld(centerScreen);
            Vector3 worldEnd = ScreenToWorld(currentPointerPos);

            line.SetPosition(0, worldStart);
            line.SetPosition(1, worldEnd);
        }

        private Vector3 ScreenToWorld(Vector2 screenPos)
        {
            // расстояние до поверхности, на которой стоит машина
            float distance = 10f;
            return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
        }

        private void SendImpulseToCar()
        {
            Vector2 swipe = currentPointerPos - centerScreen;
            swipe = Vector2.ClampMagnitude(swipe, maxPower);

            // переводим направление в 3D вектор
            Vector3 worldCenter = ScreenToWorld(centerScreen);
            Vector3 worldTouch = ScreenToWorld(centerScreen + swipe);

            Vector3 direction = (worldTouch - worldCenter).normalized;

            onImpulse?.Invoke(direction * swipe.magnitude);
        }
    }
}