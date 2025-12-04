using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 10f, -10f);

    [Header("Smooth")]
    public float followSpeed = 5f;
    public float rotateSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция
        Vector3 desiredPos = target.position + offset;

        // Плавное движение
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Плавный взгляд на игрока
        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
    }
}