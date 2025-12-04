using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controllers.UI
{
    public class DragInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public System.Action<Vector2> onDown;
        public System.Action<Vector2> onDrag;
        public System.Action<Vector2> onUp;

        public void OnPointerDown(PointerEventData eventData)
            => onDown?.Invoke(eventData.position);

        public void OnDrag(PointerEventData eventData)
            => onDrag?.Invoke(eventData.position);

        public void OnPointerUp(PointerEventData eventData)
            => onUp?.Invoke(eventData.position);
    }
}