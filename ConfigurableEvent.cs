using System;
using UnityEngine;
using UnityEngine.Events;

public class ConfigurableEvent : MonoBehaviour
{
	public Configurable Configurable
	{
		get
		{
			return this.configurable;
		}
		private set
		{
		}
	}

	private bool CanInvokeOnStart
	{
		get
		{
			return this.invokeBehaviour == ConfigurableEvent.InvokeBehaviour.AlwaysOnceStarted || this.invokeBehaviour == ConfigurableEvent.InvokeBehaviour.OnlyIfActiveAndEnabled;
		}
	}

	private bool CanInvoke
	{
		get
		{
			return this.invokeBehaviour != ConfigurableEvent.InvokeBehaviour.OnlyIfActiveAndEnabled || base.isActiveAndEnabled;
		}
	}

	public void SetSelfInvokeOnValueChange(bool val)
	{
		this.selfInvokeOnValueChange = val;
	}

	public bool SelfInvokeOnValueChange
	{
		get
		{
			return this.selfInvokeOnValueChange;
		}
	}

	private void Awake()
	{
		if (this.configurable != null)
		{
			Configurable configurable = this.configurable;
			configurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(configurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
			this.configurable.Init();
		}
	}

	private void Start()
	{
		if (this.configurable != null)
		{
			this.configurable.Init();
		}
		if (this.CanInvoke && this.CanInvokeOnStart && this.selfInvokeOnValueChange)
		{
			this.Invoke();
		}
	}

	private void OnDestroy()
	{
		if (this.configurable != null)
		{
			Configurable configurable = this.configurable;
			configurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(configurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		}
	}

	private void OnValueChanged(LiveData data)
	{
		if (this.CanInvoke && this.selfInvokeOnValueChange)
		{
			this.Invoke();
		}
	}

	public void Invoke()
	{
		if (!base.enabled)
		{
			return;
		}
		bool flag;
		switch (this.configurableType)
		{
		case Configurator.ConfiguratorTypes.Int:
			flag = this.EvaluateIntegerCondition();
			break;
		case Configurator.ConfiguratorTypes.Float:
			flag = this.EvaluateFloatCondition();
			break;
		case Configurator.ConfiguratorTypes.Boolean:
			flag = this.EvaluateBooleanCondition();
			break;
		case Configurator.ConfiguratorTypes.String:
			flag = this.EvaluateStringCondition();
			break;
		case Configurator.ConfiguratorTypes.Custom:
			return;
		default:
			return;
		}
		if (flag)
		{
			this.onConditionMet.Invoke();
			return;
		}
		this.onConditionNotMet.Invoke();
	}

	private bool EvaluateIntegerCondition()
	{
		switch (this.numberCondition)
		{
		case ConfigurableEvent.NumberEventCondition.IsSmallerThan:
			return this.configurable.GetIntValue() < this.testInteger;
		case ConfigurableEvent.NumberEventCondition.IsBiggerThan:
			return this.configurable.GetIntValue() > this.testInteger;
		case ConfigurableEvent.NumberEventCondition.IsEqual:
			return this.configurable.GetIntValue().Equals(this.testInteger);
		case ConfigurableEvent.NumberEventCondition.IsDifferentThan:
			return !this.configurable.GetIntValue().Equals(this.testFloat);
		default:
			return false;
		}
	}

	private bool EvaluateFloatCondition()
	{
		switch (this.numberCondition)
		{
		case ConfigurableEvent.NumberEventCondition.IsSmallerThan:
			return this.configurable.GetFloatValue() < this.testFloat;
		case ConfigurableEvent.NumberEventCondition.IsBiggerThan:
			return this.configurable.GetFloatValue() > this.testFloat;
		case ConfigurableEvent.NumberEventCondition.IsEqual:
			return this.configurable.GetFloatValue().Equals(this.testFloat);
		case ConfigurableEvent.NumberEventCondition.IsDifferentThan:
			return !this.configurable.GetFloatValue().Equals(this.testFloat);
		default:
			return false;
		}
	}

	private bool EvaluateBooleanCondition()
	{
		if (this.configurable == null)
		{
			return false;
		}
		ConfigurableEvent.ToggleEventCondition toggleEventCondition = this.toggleCondition;
		if (toggleEventCondition != ConfigurableEvent.ToggleEventCondition.IsTrue)
		{
			return toggleEventCondition == ConfigurableEvent.ToggleEventCondition.IsFalse && this.configurable.GetBooleanValue().Equals(false);
		}
		return this.configurable.GetBooleanValue().Equals(true);
	}

	private bool EvaluateStringCondition()
	{
		ConfigurableEvent.StringEventCondition stringEventCondition = this.stringCondition;
		if (stringEventCondition != ConfigurableEvent.StringEventCondition.Equals)
		{
			return stringEventCondition == ConfigurableEvent.StringEventCondition.IsDifferentThan && !this.configurable.GetStringValue().Equals(this.testString);
		}
		return this.configurable.GetStringValue().Equals(this.testString);
	}

	[SerializeField]
	private Configurator.ConfiguratorTypes configurableType;

	[SerializeField]
	private Configurable configurable;

	[SerializeField]
	private int testInteger;

	[SerializeField]
	private float testFloat;

	[SerializeField]
	private string testString;

	[SerializeField]
	private ConfigurableEvent.NumberEventCondition numberCondition;

	[SerializeField]
	private ConfigurableEvent.ToggleEventCondition toggleCondition;

	[SerializeField]
	private ConfigurableEvent.StringEventCondition stringCondition;

	[SerializeField]
	private bool selfInvokeOnValueChange;

	[SerializeField]
	public ConfigurableEvent.InvokeBehaviour invokeBehaviour = ConfigurableEvent.InvokeBehaviour.OnlyIfActiveAndEnabled;

	[SerializeField]
	private bool onlyInvokeIfActiveAndEnabled;

	[SerializeField]
	private UnityEvent onConditionMet;

	[SerializeField]
	private UnityEvent onConditionNotMet;

	public enum ToggleEventCondition
	{
		IsTrue,
		IsFalse
	}

	public enum NumberEventCondition
	{
		IsSmallerThan,
		IsBiggerThan,
		IsEqual,
		IsDifferentThan
	}

	public enum StringEventCondition
	{
		Equals,
		IsDifferentThan
	}

	public enum InvokeBehaviour
	{
		AlwaysOnceStarted,
		OnlyIfActiveAndEnabled,
		AlwaysAfterStarted,
		AfterStartedIfActiveAndEnabled
	}
}
