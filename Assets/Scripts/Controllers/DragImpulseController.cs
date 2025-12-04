using Assets.Scripts.Controllers.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class DragImpulseController : MonoBehaviour
    {
        [Header("Impulse Settings")]
        public float maxPower = 10f;
        public float minDistance = 1f; // минимальная длина натяжения
        public float maxDistance = 20f; // максимальная длина в мире

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
            trajectoryRenderer?.Hide(); // рикошеты скрыты до импульса
        }

        private void OnDrag(Vector2 pos)
        {
            if (!dragging) return;
            currentPointerPos = pos;

            Plane plane = new Plane(Vector3.up, car.position);
            Ray ray = cam.ScreenPointToRay(currentPointerPos);
            if (!plane.Raycast(ray, out float enter)) return;

            Vector3 pointerWorld = ray.GetPoint(enter);
            Vector3 dirToPointer = pointerWorld - car.position;

            // направление движения машины (обратное пальцу)
            Vector3 moveDir = -dirToPointer;
            moveDir.y = 0;
            moveDir.Normalize();

            // Preview поворот машины
            carController.PreviewRotation(moveDir);

            // --- линия натяжения ---
            float distance = dirToPointer.magnitude;
            float lineLength = Mathf.Min(distance, maxDistance);
            Vector3 lineEnd = car.position + dirToPointer.normalized * lineLength;
            powerLine3D?.UpdateLine(car.position, lineEnd);

            // --- рикошетная линия для Preview (опционально) ---
             trajectoryRenderer?.Draw(car.position, moveDir);
        }

        private void OnUp(Vector2 pos)
        {
            if (!dragging) return;

            dragging = false;
            currentPointerPos = pos;

            // Завершение линии натяжения
            powerLine3D?.End();

            // Скрываем рикошеты после импульса
            trajectoryRenderer?.Hide();

            SendImpulse();
        }

        private void SendImpulse()
        {
            Plane plane = new Plane(Vector3.up, car.position);
            Ray ray = cam.ScreenPointToRay(currentPointerPos);
            if (!plane.Raycast(ray, out float enter)) return;

            Vector3 pointerWorld = ray.GetPoint(enter);
            Vector3 dirToPointer = pointerWorld - car.position;

            float distance = dirToPointer.magnitude;
            if (distance < minDistance) return;

            // сила пропорциональна длине натяжения (0..maxPower)
            float power = Mathf.Lerp(0f, maxPower, Mathf.InverseLerp(minDistance, maxDistance, distance));

            Vector3 moveDir = -dirToPointer;
            moveDir.y = 0;
            moveDir.Normalize();

            carController.OnImpulse(moveDir * power);
        }
    }
}
