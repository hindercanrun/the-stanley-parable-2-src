using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class MessageBox : MonoBehaviour
{
	public void InformOfVisibility(bool visible)
	{
		this.visibility = visible;
	}

	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.audioSource = base.GetComponent<AudioSource>();
		foreach (MessageBox.KeyReplaceItem keyReplaceItem in this.replacementItems)
		{
			keyReplaceItem.replace.Init();
		}
	}

	private string[] DefaultDialogueParser(MessageBoxDialogue dialouge)
	{
		return new List<string>(dialouge.GetMessages()).ConvertAll<string>(delegate(string s)
		{
			if (s == null)
			{
				s = "";
			}
			return s;
		}).ToArray();
	}

	public void SetMessage(MessageBoxDialogue dialogue)
	{
		this.animator.SetBool("Hidden", false);
		IMessageBoxDialogueParser component = base.GetComponent<IMessageBoxDialogueParser>();
		if (component != null)
		{
			this.messageArray = component.ParseDialogue(dialogue);
		}
		else
		{
			this.messageArray = this.DefaultDialogueParser(dialogue);
		}
		for (int i = 0; i < this.messageArray.Length; i++)
		{
			string text = this.messageArray[i];
			foreach (MessageBox.KeyReplaceItem keyReplaceItem in this.replacementItems)
			{
				text = keyReplaceItem.Replace(text);
			}
			IMessageBoxKeyReplacer[] components = base.GetComponents<IMessageBoxKeyReplacer>();
			for (int j = 0; j < components.Length; j++)
			{
				text = components[j].DoReplaceStep(text);
			}
			text = text.Replace("\\n", "\n");
			if (this.lineBreakBehaviour == MessageBox.LineBreakBehaviour.RemoveAllLineBreaks)
			{
				text = text.Replace("\n", " ");
			}
			this.messageArray[i] = text;
		}
		this.currentMessageIndex = 0;
		this.AdvanceMessage();
	}

	private void SetVisibility(bool visible)
	{
		this.animator.SetBool("Hidden", !visible);
	}

	private void Update()
	{
		if (this.complete && this.manualClose && !this.closed && this.GotContinueInput())
		{
			UnityEvent onCompleteEvent = this.OnCompleteEvent;
			if (onCompleteEvent != null)
			{
				onCompleteEvent.Invoke();
			}
			this.closed = true;
		}
		if (this.closed)
		{
			return;
		}
		if (this.messageArray == null)
		{
			return;
		}
		this.inputCooldownTimer += Time.deltaTime;
		if (this.characterIndex <= this.text.text.Length)
		{
			this.FillMessageBox();
			return;
		}
		if (!this.complete && this.currentMessageIndex >= this.messageArray.Length)
		{
			this.complete = true;
			this.messageArray = null;
			return;
		}
		if (this.GotContinueInput() && this.inputCooldownTimer > this.inputCooldown && this.currentMessageIndex < this.messageArray.Length)
		{
			this.AdvanceMessage();
		}
	}

	private bool GotContinueInput()
	{
		return Singleton<GameMaster>.Instance.stanleyActions.UseAction.WasPressed || Singleton<GameMaster>.Instance.stanleyActions.MenuConfirm.WasPressed;
	}

	private void AdvanceMessage()
	{
		this.text.text = this.messageArray[this.currentMessageIndex];
		this.currentMessageIndex++;
		this.text.maxVisibleCharacters = 0;
		this.characterIndex = 0;
		this.text.maxVisibleCharacters = (this.characterIndex = this.text.text.Length);
		this.characterAdvanceTimer = 0f;
		this.inputCooldownTimer = 0f;
		UnityEvent onAdvanceMessage = this.OnAdvanceMessage;
		if (onAdvanceMessage != null)
		{
			onAdvanceMessage.Invoke();
		}
		UnityAction onAdvanceMessageEvent = this.OnAdvanceMessageEvent;
		if (onAdvanceMessageEvent == null)
		{
			return;
		}
		onAdvanceMessageEvent();
	}

	public void PlayTalkSound()
	{
		if (this.talkCollection.SetVolumeAndPitchAndPlayClip(this.audioSource))
		{
			this.talkTimeStamp = Time.realtimeSinceStartup;
		}
	}

	private void FillMessageBox()
	{
		if (this.GotContinueInput())
		{
			this.characterIndex = this.text.text.Length - 1;
		}
		this.characterAdvanceTimer += Time.deltaTime;
		if (this.characterAdvanceTimer >= this.characterAdvanceLimit)
		{
			if (Time.realtimeSinceStartup - this.talkTimeStamp >= this.talkSoundDelay && this.playTalkOnFill && this.visibility)
			{
				this.PlayTalkSound();
			}
			this.characterIndex++;
			this.text.maxVisibleCharacters = this.characterIndex;
			this.characterAdvanceTimer = 0f;
			if (this.characterIndex == this.text.text.Length)
			{
				this.endCollection.SetVolumeAndPitchAndPlayClip(this.audioSource);
			}
		}
	}

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float characterAdvanceLimit = 0.1f;

	[SerializeField]
	private UnityEvent OnCompleteEvent;

	[SerializeField]
	private UnityEvent OnAdvanceMessage;

	public UnityAction OnAdvanceMessageEvent;

	[SerializeField]
	private float inputCooldown = 1.5f;

	[Header("Audio")]
	[SerializeField]
	private bool playTalkOnFill = true;

	[SerializeField]
	private AudioCollection talkCollection;

	[SerializeField]
	private AudioCollection endCollection;

	[SerializeField]
	private bool manualClose;

	[SerializeField]
	private float talkSoundDelay = 0.1f;

	private MessageBox.LineBreakBehaviour lineBreakBehaviour = MessageBox.LineBreakBehaviour.RemoveAllLineBreaks;

	[SerializeField]
	private List<MessageBox.KeyReplaceItem> replacementItems;

	private float talkTimeStamp;

	private float characterAdvanceTimer;

	private float inputCooldownTimer;

	private int characterIndex;

	private int currentMessageIndex;

	private string[] messageArray;

	private AudioSource audioSource;

	private bool complete;

	private bool closed;

	private bool visibility = true;

	public enum LineBreakBehaviour
	{
		UseCharacterLineBreaks,
		RemoveAllLineBreaks
	}

	[Serializable]
	private class KeyReplaceItem
	{
		public string Replace(string orig)
		{
			return orig.Replace(this.key, this.replace.GetStringValue());
		}

		public string key;

		public StringConfigurable replace;
	}
}
