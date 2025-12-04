using UnityEngine;
using Assets.Scripts.Controllers;
using Assets.Scripts.Intrerface;

namespace Assets.Scripts.Obstacles
{
    public class BounceSurface : MonoBehaviour, IReflectable
    {
        public float rotateDuration = 0.2f;

        private void OnCollisionEnter(Collision col)
        {
            var car = col.gameObject.GetComponent<CarController>();
            if (car == null) return;

            Vector3 v = car.GetVelocity();

            // нормаль контакта
            Vector3 n = col.contacts[0].normal.normalized;

            // идеальное зеркальное отражение
            Vector3 r = v - 2f * Vector3.Dot(v, n) * n;

            car.SetVelocity(r);
        }
    }
}