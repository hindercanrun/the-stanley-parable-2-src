using System;
using StanleyUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Animator))]
public class UIScreen : MonoBehaviour, ISelectableHolderScreen
{
	public bool active { get; private set; }

	public Selectable DefaultSelectable
	{
		get
		{
			if (!(this.defaultSelectable == null))
			{
				return this.defaultSelectable.GetComponent<Selectable>();
			}
			return null;
		}
	}

	public Selectable LastSelectable { get; set; }

	private void Awake()
	{
		this.cGroup = base.GetComponent<CanvasGroup>();
		this.childCanvases = base.GetComponentsInChildren<Canvas>();
		if (this.startHidden)
		{
			this.cGroup.alpha = 0f;
			this.cGroup.interactable = false;
			this.cGroup.blocksRaycasts = false;
			this.SetChildCanvases(false);
		}
		else
		{
			this.OnCall();
		}
		if (this.reference != null)
		{
			UIScreenReference uiscreenReference = this.reference;
			uiscreenReference.OnCall = (Action)Delegate.Combine(uiscreenReference.OnCall, new Action(this.OnCall));
			UIScreenReference uiscreenReference2 = this.reference;
			uiscreenReference2.OnClose = (Action)Delegate.Combine(uiscreenReference2.OnClose, new Action(this.OnClose));
			UIScreenReference uiscreenReference3 = this.reference;
			uiscreenReference3.OnShow = (Action)Delegate.Combine(uiscreenReference3.OnShow, new Action(this.OnShow));
			UIScreenReference uiscreenReference4 = this.reference;
			uiscreenReference4.OnHide = (Action)Delegate.Combine(uiscreenReference4.OnHide, new Action(this.OnHide));
		}
	}

	private void SetChildCanvases(bool status)
	{
		Canvas[] array = this.childCanvases;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(status);
		}
	}

	private void OnDestroy()
	{
		UIScreenReference uiscreenReference = this.reference;
		uiscreenReference.OnCall = (Action)Delegate.Remove(uiscreenReference.OnCall, new Action(this.OnCall));
		UIScreenReference uiscreenReference2 = this.reference;
		uiscreenReference2.OnClose = (Action)Delegate.Remove(uiscreenReference2.OnClose, new Action(this.OnClose));
		UIScreenReference uiscreenReference3 = this.reference;
		uiscreenReference3.OnShow = (Action)Delegate.Remove(uiscreenReference3.OnShow, new Action(this.OnShow));
		UIScreenReference uiscreenReference4 = this.reference;
		uiscreenReference4.OnHide = (Action)Delegate.Remove(uiscreenReference4.OnHide, new Action(this.OnHide));
	}

	private void Update()
	{
		if (this.active && this.canBeCanceled && this.backButton != null && Singleton<GameMaster>.Instance.stanleyActions.MenuBack.WasPressed)
		{
			ExecuteEvents.Execute<IPointerClickHandler>(this.backButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
		}
	}

	private void OnCall()
	{
		this.SetChildCanvases(true);
		StanleyInputModuleAssistant.RegisterScreenAsNewlyVisible(base.gameObject, true);
		this.OnCallEvent.Invoke();
		this.active = true;
	}

	private void OnClose()
	{
		this.SetChildCanvases(false);
		this.OnCloseEvent.Invoke();
		this.active = false;
	}

	private void OnHide()
	{
		this.SetChildCanvases(false);
		this.OnHideEvent.Invoke();
		this.active = false;
	}

	private void OnShow()
	{
		this.SetChildCanvases(true);
		StanleyInputModuleAssistant.RegisterScreenAsNewlyVisible(base.gameObject, false);
		this.OnShowEvent.Invoke();
		this.active = true;
	}

	public void CallReference()
	{
		this.reference.Call();
	}

	public void CloseReference()
	{
		this.reference.Close();
	}

	public void MoveToScreenWithReference(UIScreenReference nextScreen)
	{
		nextScreen.CallWithPrevious(this.reference);
	}

	public void MoveToScreen(UIScreenReference nextScreen)
	{
		nextScreen.Call();
		this.OnClose();
	}

	public void MoveToPrevious()
	{
		this.reference.CloseAndCallPrevious();
	}

	public void MoveToPreviousIfActive()
	{
		if (this.active)
		{
			this.reference.CloseAndCallPrevious();
		}
	}

	[SerializeField]
	private UIScreenReference reference;

	[SerializeField]
	private bool startHidden = true;

	[SerializeField]
	private bool canBeCanceled;

	[SerializeField]
	private UnityEvent OnCallEvent;

	[SerializeField]
	private UnityEvent OnCloseEvent;

	[SerializeField]
	private UnityEvent OnShowEvent;

	[SerializeField]
	private UnityEvent OnHideEvent;

	[SerializeField]
	private Selectable backButton;

	private CanvasGroup cGroup;

	[Header("Controller Selection Stuff")]
	[SerializeField]
	private GameObject defaultSelectable;

	[SerializeField]
	private Canvas[] childCanvases;
}
