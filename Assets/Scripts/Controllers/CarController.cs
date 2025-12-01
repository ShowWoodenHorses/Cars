using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Scripts.Controllers
{
    public class CarController : MonoBehaviour
    {
        [Header("Movement")]
        public float powerMultiplier = 0.05f;
        public float damping = 0.98f;
        public float rotateDuration = 0.2f;

        private Vector3 velocity;

        private void Start()
        {
            //FindObjectOfType<DragImpulseController_3D>().onImpulse += OnImpulse;
        }

        private void OnImpulse(Vector3 impulse)
        {
            velocity = impulse * powerMultiplier;

            if (velocity.magnitude > 0.1f)
            {
                // плавный разворот машины в сторону импульса
                //transform.DOLookAt(transform.position + velocity, rotateDuration, AxisConstraint.Y);
            }
        }

        private void Update()
        {
            if (velocity.sqrMagnitude > 0.0001f)
            {
                transform.position += velocity * Time.deltaTime;
                velocity *= damping;
            }
        }

        public Vector3 GetVelocity() => velocity;
        public void SetVelocity(Vector3 v)
        {
            velocity = v;

            if (v.magnitude > 0.1f)
            {
                //transform.DOLookAt(transform.position + v, rotateDuration, AxisConstraint.Y);
            }
        }
    }
}