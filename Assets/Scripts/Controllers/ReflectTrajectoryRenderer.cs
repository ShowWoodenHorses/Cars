using System.Collections.Generic;
using Assets.Scripts.Intrerface;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ReflectTrajectoryRenderer : MonoBehaviour
{
    [Header("Ray Settings")]
    public float maxDistance = 30f;
    public int maxBounces = 5;
    public LayerMask collisionMask;
    [SerializeField] private float yOffset = 0.5f;

    private LineRenderer lr;
    private List<Vector3> points = new();

    private Transform car;

    public  void Initialize(Transform car)
    {
        this.car = car;
        lr = GetComponent<LineRenderer>();
        lr.alignment = LineAlignment.TransformZ;
        lr.positionCount = 0;
    }

    public void Hide()
    {
        lr.positionCount = 0;
    }

    public void Draw(Vector3 start, Vector3 dir)
    {
        points.Clear();
        points.Add(start);

        Vector3 currentStart = start;
        Vector3 currentDir = dir.normalized;

        float remaining = maxDistance;

        for (int i = 0; i < maxBounces; i++)
        {
            if (Physics.Raycast(currentStart, currentDir, out RaycastHit hit, remaining, collisionMask))
            {
                Vector3 point = hit.point;
                point.y = car.position.y + yOffset; // фиксированная высота
                points.Add(point);

                remaining -= hit.distance;

                if (hit.collider.GetComponent<IReflectable>() == null)
                    break;

                currentDir = Vector3.Reflect(currentDir, hit.normal);
                currentStart = hit.point;
            }
            else
            {
                Vector3 endPoint = currentStart + currentDir * remaining;
                endPoint.y = car.position.y + yOffset; // ровная высота
                points.Add(endPoint);
                break;
            }
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
}
