using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace StanleyUI
{
	public class StanleyUIEventTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IMoveHandler, ISelectHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.OnPointerEnterOnlyInvokesOnMouseMove || !Singleton<GameMaster>.Instance.MouseMoved || eventData.dragging)
			{
				return;
			}
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerEnter;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerExit;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerClick;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnMove(AxisEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.move;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnSelect(BaseEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.select;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.deselect;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerDown;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerUp;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnDrop(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.drop;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.endDrag;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.beginDrag;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			StanleyUIEventTrigger.BaseEventDataEvent baseEventDataEvent = this.drag;
			if (baseEventDataEvent == null)
			{
				return;
			}
			baseEventDataEvent.Invoke(eventData);
		}

		[SerializeField]
		private bool OnPointerEnterOnlyInvokesOnMouseMove = true;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent pointerEnter;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent pointerExit;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent pointerClick;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent pointerDown;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent pointerUp;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent drop;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent beginDrag;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent drag;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent endDrag;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent move;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent select;

		[SerializeField]
		private StanleyUIEventTrigger.BaseEventDataEvent deselect;

		[Serializable]
		public class BaseEventDataEvent : UnityEvent<BaseEventData>
		{
		}
	}
}
