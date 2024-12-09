using Foundation.EntitySystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MH
{
    public class ClickableHandler : EntityComponent, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public UnityEvent<PointerEventData> OnEnterMouse;
        public UnityEvent<PointerEventData> OnClickEvent;
        public UnityEvent<PointerEventData> OnExitMouse;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterMouse?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitMouse?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }

}
