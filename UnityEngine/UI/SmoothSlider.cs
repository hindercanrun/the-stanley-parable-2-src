using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Smooth Slider", 33)]
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class SmoothSlider : Selectable, IDragHandler, IEventSystemHandler, IInitializePotentialDragHandler, ICanvasElement, IPointerDownHandler, IPointerUpHandler
	{
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public RectTransform fillRect
		{
			get
			{
				return this.m_FillRect;
			}
			set
			{
				if (SmoothSlider.SetClass<RectTransform>(ref this.m_FillRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		public RectTransform handleRect
		{
			get
			{
				return this.m_HandleRect;
			}
			set
			{
				if (SmoothSlider.SetClass<RectTransform>(ref this.m_HandleRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		public SmoothSlider.Direction direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				if (SmoothSlider.SetStruct<SmoothSlider.Direction>(ref this.m_Direction, value))
				{
					this.UpdateVisuals();
				}
			}
		}

		public float minValue
		{
			get
			{
				return this.m_MinValue;
			}
			set
			{
				if (SmoothSlider.SetStruct<float>(ref this.m_MinValue, value))
				{
					this.Set(this.m_Value, true);
					this.UpdateVisuals();
				}
			}
		}

		public float maxValue
		{
			get
			{
				return this.m_MaxValue;
			}
			set
			{
				if (SmoothSlider.SetStruct<float>(ref this.m_MaxValue, value))
				{
					this.Set(this.m_Value, true);
					this.UpdateVisuals();
				}
			}
		}

		public bool wholeNumbers
		{
			get
			{
				return this.m_WholeNumbers;
			}
			set
			{
				if (SmoothSlider.SetStruct<bool>(ref this.m_WholeNumbers, value))
				{
					this.Set(this.m_Value, true);
					this.UpdateVisuals();
				}
			}
		}

		public virtual float value
		{
			get
			{
				if (this.wholeNumbers)
				{
					return Mathf.Round(this.m_Value);
				}
				return this.m_Value;
			}
			set
			{
				this.Set(value, true);
			}
		}

		public virtual void SetValueWithoutNotify(float input)
		{
			this.Set(input, false);
		}

		public virtual void SetValueWithoutNotify(int input)
		{
			this.Set((float)input, false);
		}

		public float normalizedValue
		{
			get
			{
				if (Mathf.Approximately(this.minValue, this.maxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(this.minValue, this.maxValue, this.value);
			}
			set
			{
				this.value = Mathf.Lerp(this.minValue, this.maxValue, value);
			}
		}

		public SmoothSlider.SliderEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		private float m_StepSize
		{
			get
			{
				return (this.wholeNumbers ? 1f : ((this.maxValue - this.minValue) * this.m_CustomStepSize)) * this.m_StepSizeMultiplier;
			}
		}

		protected SmoothSlider()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateCachedReferences();
			this.Set(this.m_Value, false);
			this.UpdateVisuals();
		}

		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			base.OnDisable();
		}

		protected virtual void Update()
		{
			if (this.m_DelayedUpdateVisuals)
			{
				this.m_DelayedUpdateVisuals = false;
				this.Set(this.m_Value, false);
				this.UpdateVisuals();
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			this.m_Value = this.ClampValue(this.m_Value);
			float num = this.normalizedValue;
			if (this.m_FillContainerRect != null)
			{
				if (this.m_FillImage != null && this.m_FillImage.type == Image.Type.Filled)
				{
					num = this.m_FillImage.fillAmount;
				}
				else
				{
					num = (this.reverseValue ? (1f - this.m_FillRect.anchorMin[(int)this.axis]) : this.m_FillRect.anchorMax[(int)this.axis]);
				}
			}
			else if (this.m_HandleContainerRect != null)
			{
				num = (this.reverseValue ? (1f - this.m_HandleRect.anchorMin[(int)this.axis]) : this.m_HandleRect.anchorMin[(int)this.axis]);
			}
			this.UpdateVisuals();
			if (num != this.normalizedValue)
			{
				UISystemProfilerApi.AddMarker("Slider.value", this);
				this.onValueChanged.Invoke(this.m_Value);
			}
		}

		private void UpdateCachedReferences()
		{
			if (this.m_FillRect && this.m_FillRect != (RectTransform)base.transform)
			{
				this.m_FillTransform = this.m_FillRect.transform;
				this.m_FillImage = this.m_FillRect.GetComponent<Image>();
				if (this.m_FillTransform.parent != null)
				{
					this.m_FillContainerRect = this.m_FillTransform.parent.GetComponent<RectTransform>();
				}
			}
			else
			{
				this.m_FillRect = null;
				this.m_FillContainerRect = null;
				this.m_FillImage = null;
			}
			if (this.m_HandleRect && this.m_HandleRect != (RectTransform)base.transform)
			{
				this.m_HandleTransform = this.m_HandleRect.transform;
				if (this.m_HandleTransform.parent != null)
				{
					this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
					return;
				}
			}
			else
			{
				this.m_HandleRect = null;
				this.m_HandleContainerRect = null;
			}
		}

		private float ClampValue(float input)
		{
			float num = Mathf.Clamp(input, this.minValue, this.maxValue);
			if (this.wholeNumbers)
			{
				num = Mathf.Round(num);
			}
			return num;
		}

		protected virtual void Set(float input, bool sendCallback = true)
		{
			float num = this.ClampValue(input);
			if (this.m_Value == num)
			{
				return;
			}
			this.m_Value = num;
			this.UpdateVisuals();
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("Slider.value", this);
				this.m_OnValueChanged.Invoke(num);
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateVisuals();
		}

		private SmoothSlider.Axis axis
		{
			get
			{
				if (this.m_Direction != SmoothSlider.Direction.LeftToRight && this.m_Direction != SmoothSlider.Direction.RightToLeft)
				{
					return SmoothSlider.Axis.Vertical;
				}
				return SmoothSlider.Axis.Horizontal;
			}
		}

		private bool reverseValue
		{
			get
			{
				return this.m_Direction == SmoothSlider.Direction.RightToLeft || this.m_Direction == SmoothSlider.Direction.TopToBottom;
			}
		}

		private void UpdateVisuals()
		{
			this.m_Tracker.Clear();
			if (this.m_FillContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_FillRect, DrivenTransformProperties.Anchors);
				Vector2 zero = Vector2.zero;
				Vector2 one = Vector2.one;
				if (this.m_FillImage != null && this.m_FillImage.type == Image.Type.Filled)
				{
					this.m_FillImage.fillAmount = this.normalizedValue;
				}
				else if (this.reverseValue)
				{
					zero[(int)this.axis] = 1f - this.normalizedValue;
				}
				else
				{
					one[(int)this.axis] = this.normalizedValue;
				}
				this.m_FillRect.anchorMin = zero;
				this.m_FillRect.anchorMax = one;
			}
			if (this.m_HandleContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 zero2 = Vector2.zero;
				Vector2 one2 = Vector2.one;
				zero2[(int)this.axis] = (one2[(int)this.axis] = (this.reverseValue ? (1f - this.normalizedValue) : this.normalizedValue));
				this.m_HandleRect.anchorMin = zero2;
				this.m_HandleRect.anchorMax = one2;
			}
		}

		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform rectTransform = this.m_HandleContainerRect ?? this.m_FillContainerRect;
			if (rectTransform != null && rectTransform.rect.size[(int)this.axis] > 0f)
			{
				Vector2 a;
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, cam, out a))
				{
					return;
				}
				a -= rectTransform.rect.position;
				float num = Mathf.Clamp01((a - this.m_Offset)[(int)this.axis] / rectTransform.rect.size[(int)this.axis]);
				this.normalizedValue = (this.reverseValue ? (1f - num) : num);
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			SimpleEvent simpleEvent = this.onPointerDownEvent;
			if (simpleEvent != null)
			{
				simpleEvent.Call();
			}
			if (!this.MayDrag(eventData))
			{
				return;
			}
			base.OnPointerDown(eventData);
			this.m_Offset = Vector2.zero;
			if (this.m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
			{
				Vector2 offset;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out offset))
				{
					this.m_Offset = offset;
					return;
				}
			}
			else
			{
				this.UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			SimpleEvent simpleEvent = this.onPointerUpEvent;
			if (simpleEvent == null)
			{
				return;
			}
			simpleEvent.Call();
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				base.OnMove(eventData);
				return;
			}
			if (this.m_SliderDurationCheck == null)
			{
				this.m_SliderDurationCheck = base.StartCoroutine(this.SliderDurationCheck());
			}
			switch (eventData.moveDir)
			{
			case MoveDirection.Left:
				if (this.axis == SmoothSlider.Axis.Horizontal && this.FindSelectableOnLeft() == null)
				{
					this.Set(this.reverseValue ? (this.value + this.m_StepSize) : (this.value - this.m_StepSize), true);
					this.m_NumSteps++;
					return;
				}
				base.OnMove(eventData);
				return;
			case MoveDirection.Up:
				if (this.axis == SmoothSlider.Axis.Vertical && this.FindSelectableOnUp() == null)
				{
					this.Set(this.reverseValue ? (this.value - this.m_StepSize) : (this.value + this.m_StepSize), true);
					return;
				}
				base.OnMove(eventData);
				return;
			case MoveDirection.Right:
				if (this.axis == SmoothSlider.Axis.Horizontal && this.FindSelectableOnRight() == null)
				{
					this.Set(this.reverseValue ? (this.value - this.m_StepSize) : (this.value + this.m_StepSize), true);
					this.m_NumSteps++;
					return;
				}
				base.OnMove(eventData);
				return;
			case MoveDirection.Down:
				if (this.axis == SmoothSlider.Axis.Vertical && this.FindSelectableOnDown() == null)
				{
					this.Set(this.reverseValue ? (this.value + this.m_StepSize) : (this.value - this.m_StepSize), true);
					return;
				}
				base.OnMove(eventData);
				return;
			default:
				return;
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == SmoothSlider.Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnLeft();
		}

		public override Selectable FindSelectableOnRight()
		{
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == SmoothSlider.Axis.Horizontal)
			{
				return null;
			}
			return base.FindSelectableOnRight();
		}

		public override Selectable FindSelectableOnUp()
		{
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == SmoothSlider.Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnUp();
		}

		public override Selectable FindSelectableOnDown()
		{
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == SmoothSlider.Axis.Vertical)
			{
				return null;
			}
			return base.FindSelectableOnDown();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		public void SetDirection(SmoothSlider.Direction direction, bool includeRectLayouts)
		{
			SmoothSlider.Axis axis = this.axis;
			bool reverseValue = this.reverseValue;
			this.direction = direction;
			if (!includeRectLayouts)
			{
				return;
			}
			if (this.axis != axis)
			{
				RectTransformUtility.FlipLayoutAxes(base.transform as RectTransform, true, true);
			}
			if (this.reverseValue != reverseValue)
			{
				RectTransformUtility.FlipLayoutOnAxis(base.transform as RectTransform, (int)this.axis, true, true);
			}
		}

		private IEnumerator SliderDurationCheck()
		{
			while (Singleton<GameMaster>.Instance.stanleyActions.Left.IsPressed || Singleton<GameMaster>.Instance.stanleyActions.Right.IsPressed || Singleton<GameMaster>.Instance.stanleyActions.MoveLeft.IsPressed || Singleton<GameMaster>.Instance.stanleyActions.MoveRight.IsPressed)
			{
				if (this.m_NumSteps >= this.numStepsThreshold)
				{
					this.m_StepSizeMultiplier = (float)this.largeStepSize;
				}
				yield return null;
			}
			this.m_NumSteps = 0;
			this.m_StepSizeMultiplier = 1f;
			this.m_SliderDurationCheck = null;
			yield break;
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		[SerializeField]
		private RectTransform m_FillRect;

		[SerializeField]
		private RectTransform m_HandleRect;

		[Space]
		[SerializeField]
		private SmoothSlider.Direction m_Direction;

		[SerializeField]
		private float m_MinValue;

		[SerializeField]
		private float m_MaxValue = 1f;

		[SerializeField]
		private bool m_WholeNumbers;

		[SerializeField]
		protected float m_Value;

		[Space]
		[SerializeField]
		private SmoothSlider.SliderEvent m_OnValueChanged = new SmoothSlider.SliderEvent();

		private Image m_FillImage;

		private Transform m_FillTransform;

		private RectTransform m_FillContainerRect;

		private Transform m_HandleTransform;

		private RectTransform m_HandleContainerRect;

		private Vector2 m_Offset = Vector2.zero;

		private DrivenRectTransformTracker m_Tracker;

		private bool m_DelayedUpdateVisuals;

		public float m_CustomStepSize = 0.05f;

		[Tooltip("The value that stepSize will be multiplied by when the player has changed the slider for a bit.")]
		public int largeStepSize = 10;

		[Tooltip("How many slider value steps does the player have to change before largeStepSize is used?")]
		public int numStepsThreshold = 15;

		private float m_StepSizeMultiplier = 1f;

		private int m_NumSteps;

		private Coroutine m_SliderDurationCheck;

		[SerializeField]
		private SimpleEvent onPointerDownEvent;

		[SerializeField]
		private SimpleEvent onPointerUpEvent;

		public enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}

		[Serializable]
		public class SliderEvent : UnityEvent<float>
		{
		}

		private enum Axis
		{
			Horizontal,
			Vertical
		}
	}
}
