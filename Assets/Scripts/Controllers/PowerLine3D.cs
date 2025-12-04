using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Controllers.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class PowerLine3D : MonoBehaviour
    {
        public LineRenderer lr;
        public Transform pointTransform;   // конец линии (опционально)

        [Header("Settings")]
        public float maxDistance = 20f;
        public float minAlpha = 0.1f;
        public float maxAlpha = 0.9f;
        public float fadeDuration = 0.2f;
        public float yOffset = 0.05f;      // высота линии над машиной/землей

        private Coroutine fadeRoutine;

        public void Begin(Vector3 start)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);

            lr.gameObject.SetActive(true);
            lr.positionCount = 2;

            UpdateLine(start, start);
        }

        public void UpdateLine(Vector3 start, Vector3 end)
        {
            Vector3 delta = end - start;
            float distance = delta.magnitude;

            Debug.Log("UpdateLine - distance" + distance);
            Debug.Log("UpdateLine - delta" + delta);

            if (distance > maxDistance)
                delta = delta.normalized * maxDistance;

            Vector3 targetPos = start + delta;
            targetPos.y = start.y + yOffset;

            lr.SetPosition(0, start + Vector3.up * yOffset);
            lr.SetPosition(1, targetPos);

            // прозрачность зависит от натяжения
            float t = Mathf.Clamp01(delta.magnitude / maxDistance);
            Color c = lr.material.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            lr.material.color = c;

            if (pointTransform != null)
                pointTransform.position = targetPos;
        }

        public void End()
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float time = 0f;
            Color startColor = lr.material.color;

            while (time < fadeDuration)
            {
                float t = time / fadeDuration;
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                lr.material.color = c;

                time += Time.deltaTime;
                yield return null;
            }

            lr.gameObject.SetActive(false);
            fadeRoutine = null;
        }
    }
}
