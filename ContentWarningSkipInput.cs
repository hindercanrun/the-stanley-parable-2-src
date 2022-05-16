using System;
using UnityEngine;
using UnityEngine.Events;

public class ContentWarningSkipInput : MonoBehaviour
{
	private void Start()
	{
		BooleanConfigurable booleanConfigurable = this.contentWarningsToggle;
		booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConfigurables));
		StringConfigurable stringConfigurable = this.contentWarningsShown;
		stringConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(stringConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConfigurables));
		this.CheckConfigurables(null);
	}

	private void OnDestroy()
	{
		BooleanConfigurable booleanConfigurable = this.contentWarningsToggle;
		booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConfigurables));
		StringConfigurable stringConfigurable = this.contentWarningsShown;
		stringConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(stringConfigurable.OnValueChanged, new Action<LiveData>(this.CheckConfigurables));
	}

	private void CheckConfigurables(LiveData ld)
	{
		this.contentWarningEnabled = (this.contentWarningsToggle.GetBooleanValue() && this.contentWarningsShown.GetStringValue() != "");
	}

	private void Update()
	{
		if (!GameMaster.PAUSEMENUACTIVE && Singleton<GameMaster>.Instance.stanleyActions.ExtraAction(0, false).IsPressed)
		{
			this.timeRemaining -= Time.smoothDeltaTime;
		}
		else
		{
			this.timeRemaining = this.timeToWait;
		}
		if (!this.contentWarningEnabled)
		{
			return;
		}
		if (this.timeRemaining < 0f)
		{
			UnityEvent unityEvent = this.onSkip;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	public float timeToWait = 3f;

	public UnityEvent onSkip;

	private float timeRemaining = -1f;

	public BooleanConfigurable contentWarningsToggle;

	public StringConfigurable contentWarningsShown;

	[Header("DEBUG")]
	public bool contentWarningEnabled;
}
