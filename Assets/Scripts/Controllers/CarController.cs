using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Controllers
{
    public class CarController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float damping = 0.98f;
        [SerializeField] private float rotateDuration = 0.2f;
        [SerializeField] private float rollingMultiplier = 1f; // регулирует длину наката
        [SerializeField] private float minSpeed = 0.5f;
        [SerializeField] private float maxSpeed = 12f;

        [Header("Ground")]
        [SerializeField] private float groundY = 0f;   // высота поверхности

        [Header("Rotate")]
        [SerializeField] private float minDirectionSqrMagnitude = 5f;
        [SerializeField] private float rotationSpeed = 100f;

        private Vector3 velocity;

        public void OnImpulse(Vector3 impulse)
        {
            float speed = Mathf.Lerp(minSpeed, maxSpeed, impulse.magnitude);

            velocity = impulse.normalized * speed;

            if (velocity.magnitude > 0.1f)
                transform.DOLookAt(transform.position + velocity, rotateDuration, AxisConstraint.Y);
        }

        private void Update()
        {
            if (velocity.sqrMagnitude >= 0.0001f)
            {
                transform.position += velocity * Time.deltaTime;

                // контролируем затухание
                velocity *= Mathf.Pow(damping, Time.deltaTime * 60f * rollingMultiplier);

                if (velocity.sqrMagnitude < 0.00005f)
                    velocity = Vector3.zero;
            }


            // фиксируем машину на земле
            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;
        }

        public Vector3 GetVelocity() => velocity;

        public void SetVelocity(Vector3 v)
        {
            velocity = v;

            if (v.magnitude > 0.1f)
                transform.DOLookAt(transform.position + v, rotateDuration, AxisConstraint.Y);
        }

        public void PreviewRotation(Vector3 direction)
        {
            if (direction.sqrMagnitude < minDirectionSqrMagnitude)
                return;

            // плавный поворот к направлению линии
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
