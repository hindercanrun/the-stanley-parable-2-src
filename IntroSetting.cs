using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class IntroSetting : MonoBehaviour
{
	public void SetFadeInOnMessageBoxAdvance(bool b)
	{
		this.fadeInOnMessageBoxAdvance = b;
	}

	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.cGroup = base.GetComponent<CanvasGroup>();
		MessageBox messageBox = this.messageBox;
		messageBox.OnAdvanceMessageEvent = (UnityAction)Delegate.Combine(messageBox.OnAdvanceMessageEvent, new UnityAction(this.OnMessageBoxAdvance));
		this.cGroup.interactable = false;
		this.cGroup.blocksRaycasts = false;
		this.ResetToHidden();
		if (this.startsVisible)
		{
			this.FadeIn();
		}
	}

	public void SetMessageBox()
	{
		this.messageBox.SetMessage(this.messageBoxDialog);
	}

	public void SetMessageBoxDialog(MessageBoxDialogue newMessageBoxDialog)
	{
		this.messageBoxDialog = newMessageBoxDialog;
	}

	public void BeginSetting()
	{
		this.BeginSetting(null);
	}

	public void BeginSetting(SimpleEvent completeEvent)
	{
		if (this.delayCoroutine != null)
		{
			return;
		}
		if (this.completeEvent == null && completeEvent != null)
		{
			this.completeEvent = completeEvent;
		}
		EventSystem.current.SetSelectedGameObject(null);
		if (this.defaultSelectable != null)
		{
			EventSystem.current.firstSelectedGameObject = this.defaultSelectable.gameObject;
		}
		UnityEvent unityEvent = this.onBeginSetting;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this.delayCoroutine = base.StartCoroutine(this.DelayedStart());
	}

	public void EndSetting()
	{
		if (this.delayCoroutine != null)
		{
			return;
		}
		this.delayCoroutine = base.StartCoroutine(this.DelayedEnd());
	}

	private void MessageBoxBecomesVisible()
	{
		this.messageBox.InformOfVisibility(true);
		this.messageBox.PlayTalkSound();
	}

	private void ContentBecomesVisible()
	{
		this.cGroup.interactable = true;
		this.cGroup.blocksRaycasts = true;
	}

	private IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(this.startEndDelay);
		this.FadeIn();
		this.SetMessageBox();
		this.messageBox.InformOfVisibility(false);
		this.delayCoroutine = null;
		yield break;
	}

	private IEnumerator DelayedEnd()
	{
		yield return new WaitForSeconds(this.startEndDelay);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
		this.cGroup.interactable = false;
		this.cGroup.blocksRaycasts = false;
		this.ResetToHidden();
		yield return new WaitForSeconds(this.endEndDelay);
		if (this.completeEvent != null)
		{
			this.completeEvent.Call();
		}
		this.completeEvent = null;
		this.delayCoroutine = null;
		yield break;
	}

	private void FadeIn()
	{
		if (this.animator.GetBool("Hidden"))
		{
			this.animator.SetBool("Hidden", false);
			return;
		}
		this.animator.SetTrigger("ReFade");
	}

	private void ResetToHidden()
	{
		this.animator.SetBool("Hidden", true);
	}

	private void OnMessageBoxAdvance()
	{
		if (this.fadeInOnMessageBoxAdvance)
		{
			this.FadeIn();
		}
	}

	[SerializeField]
	private float startEndDelay = 0.15f;

	[SerializeField]
	private float endEndDelay = 0.35f;

	[SerializeField]
	private MessageBoxDialogue messageBoxDialog;

	[SerializeField]
	private MessageBox messageBox;

	[SerializeField]
	private Selectable defaultSelectable;

	[SerializeField]
	private UnityEvent onBeginSetting;

	[SerializeField]
	private bool startsVisible;

	[SerializeField]
	private bool fadeInOnMessageBoxAdvance = true;

	[Header("Null means it should be set by BeginSetting(SimpleEvent)")]
	[SerializeField]
	private SimpleEvent completeEvent;

	private Coroutine delayCoroutine;

	private Animator animator;

	private CanvasGroup cGroup;
}
