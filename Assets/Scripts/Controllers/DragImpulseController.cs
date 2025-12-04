using Assets.Scripts.Controllers.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class DragImpulseController : MonoBehaviour
    {
        [Header("Impulse Settings")]
        public float maxPower = 10f;
        public float minDistance = 10f;
        public float maxDistance = 500f;

        [Header("References")]
        public Transform car;
        public CarController carController;
        public Camera cam;
        public DragInput dragInput;

        [Header("Trajectory & Power Line")]
        public ReflectTrajectoryRenderer trajectoryRenderer; // рикошеты
        public PowerLine3D powerLine3D;                      // натяжение

        private Vector2 centerPointScreen;
        private Vector2 currentPointerPos;
        private bool dragging;

        private void Awake()
        {
            dragInput.onDown += OnDown;
            dragInput.onDrag += OnDrag;
            dragInput.onUp += OnUp;
        }

        private void OnDown(Vector2 pos)
        {
            if (car == null) return;

            dragging = true;
            centerPointScreen = cam.WorldToScreenPoint(car.position);
            currentPointerPos = pos;

            powerLine3D?.Begin(car.position);
            trajectoryRenderer?.Hide();
        }

        private void OnDrag(Vector2 pos)
        {
            if (!dragging) return;

            currentPointerPos = pos;

            Vector2 deltaScreen = currentPointerPos - centerPointScreen;

            // направление в мире
            Vector3 deltaWorld = new Vector3(deltaScreen.x, 0, deltaScreen.y);
            deltaWorld = cam.transform.TransformDirection(deltaWorld);
            deltaWorld.y = 0;
            deltaWorld.Normalize();
            deltaWorld = -deltaWorld;

            // Preview вращение машины
            if (deltaScreen.sqrMagnitude > 10f)
                carController.PreviewRotation(deltaWorld);

            // Draw натяжение через LineRenderer
            float t = Mathf.InverseLerp(minDistance, maxDistance, deltaScreen.magnitude);
            Vector3 lineEnd = car.position + deltaWorld * (t * maxDistance);
            powerLine3D?.UpdateLine(car.position, -lineEnd);

            // Рикошетная траектория (можно опционально)
            trajectoryRenderer?.Draw(car.position, deltaWorld);
        }

        private void OnUp(Vector2 pos)
        {
            if (!dragging) return;

            dragging = false;
            currentPointerPos = pos;

            powerLine3D?.End();
            trajectoryRenderer?.Hide();

            SendImpulse();
        }

        private void SendImpulse()
        {
            Vector2 delta = currentPointerPos - centerPointScreen;
            float distance = delta.magnitude;
            if (distance < minDistance)
                return;

            float power = Mathf.InverseLerp(minDistance, maxDistance, distance); // 0..1

            Vector3 dir = new Vector3(delta.x, 0, delta.y).normalized;
            dir = cam.transform.TransformDirection(dir);
            dir.y = 0;
            dir.Normalize();
            dir = -dir;

            carController.OnImpulse(dir * power);
        }
    }
}
