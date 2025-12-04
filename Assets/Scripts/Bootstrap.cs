using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private ReflectTrajectoryRenderer reflectTrajectoryRenderer;
        [SerializeField] private Transform playerTransform;

        private void Awake()
        {
            reflectTrajectoryRenderer.Initialize(playerTransform);
        }

    }
}
