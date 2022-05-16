using System;
using UnityEngine;
using UnityEngine.Events;

public class Configurator : MonoBehaviour
{
	protected void Start()
	{
		if (this.configurable != null)
		{
			this.AssignConfigurable(this.configurable);
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

	private void OnEnable()
	{
		if (this.updateOnEnable && this.configurable != null)
		{
			this.configurable.ForceUpdate();
		}
	}

	public void AssignConfigurable(Configurable newConfigurable)
	{
		if (this.configurable != null)
		{
			Configurable configurable = this.configurable;
			configurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(configurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		}
		this.configurable = newConfigurable;
		this.configurable.Init();
		string arg = this.configurable.PrintValue();
		this.OnPrintValue.Invoke(arg);
		Configurable configurable2 = this.configurable;
		configurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(configurable2.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		this.configurable.ForceUpdate();
	}

	protected void UpdateDeviationStatus(bool deviates)
	{
		if (this.configuratorValueEqualsSavedValue && deviates)
		{
			this.OnValueDeviated.Invoke();
			this.configuratorValueEqualsSavedValue = false;
		}
		if (!this.configuratorValueEqualsSavedValue && !deviates)
		{
			this.OnValueMatched.Invoke();
			this.configuratorValueEqualsSavedValue = true;
		}
	}

	public virtual void IncreaseValue()
	{
		Configurator.ConfiguratorTypes configuratorTypes = this.configuratorType;
		if (configuratorTypes != Configurator.ConfiguratorTypes.Int)
		{
			if (configuratorTypes == Configurator.ConfiguratorTypes.Boolean)
			{
				this.BooleanValueToggle();
			}
		}
		else
		{
			this.IntValueIncrease();
		}
		this.SaveValue();
	}

	public virtual void DecreaseValue()
	{
		Configurator.ConfiguratorTypes configuratorTypes = this.configuratorType;
		if (configuratorTypes != Configurator.ConfiguratorTypes.Int)
		{
			if (configuratorTypes == Configurator.ConfiguratorTypes.Boolean)
			{
				this.BooleanValueToggle();
			}
		}
		else
		{
			this.IntValueDecrease();
		}
		this.SaveValue();
	}

	public void IntValueIncrease()
	{
		if (this.configurable is IntConfigurable)
		{
			(this.configurable as IntConfigurable).IncreaseValue();
		}
	}

	public void IntValueDecrease()
	{
		if (this.configurable is IntConfigurable)
		{
			(this.configurable as IntConfigurable).DecreaseValue();
		}
	}

	public void IntValueChange(int value)
	{
		if (this.configurable is IntConfigurable)
		{
			(this.configurable as IntConfigurable).SetValue(value);
		}
	}

	public void IntValueChangeRounded(float value)
	{
		if (this.configurable is IntConfigurable)
		{
			(this.configurable as IntConfigurable).SetValue(Mathf.RoundToInt(value));
		}
	}

	public void FloatValueChange(float value)
	{
		if (this.configurable is FloatConfigurable)
		{
			(this.configurable as FloatConfigurable).SetValue(value);
		}
	}

	public void BooleanValueChange(bool value)
	{
		if (this.configurable is BooleanConfigurable)
		{
			(this.configurable as BooleanConfigurable).SetValue(value);
		}
	}

	public void BooleanValueToggle()
	{
		if (this.configurable is BooleanConfigurable)
		{
			BooleanConfigurable booleanConfigurable = this.configurable as BooleanConfigurable;
			bool booleanValue = booleanConfigurable.GetBooleanValue();
			booleanConfigurable.SetValue(!booleanValue);
		}
	}

	public void StringValueChange(string value)
	{
		if (this.configurable is StringConfigurable)
		{
			(this.configurable as StringConfigurable).SetValue(value);
		}
	}

	public void SaveValue()
	{
		this.configurable.SaveToDiskAll();
		this.configuratorValueEqualsSavedValue = true;
		this.UpdateDeviationStatus(false);
		this.ApplyData();
	}

	public virtual void ApplyData()
	{
	}

	public virtual void PrintValue(Configurable _configurable)
	{
		this.OnPrintValue.Invoke(_configurable.PrintValue());
	}

	public virtual void PrintValueWithThisConfigurable()
	{
		this.OnPrintValue.Invoke(this.configurable.PrintValue());
	}

	protected void OnValueChanged(LiveData arg)
	{
		switch (this.configurable.GetConfigurableType())
		{
		case ConfigurableTypes.Int:
			this.OnIntValueChanged.Invoke(arg.IntValue);
			break;
		case ConfigurableTypes.Float:
			this.OnFloatValueChanged.Invoke(arg.FloatValue);
			break;
		case ConfigurableTypes.Boolean:
			this.OnBooleanValueChanged.Invoke(arg.BooleanValue);
			this.OnBooleanValueChangedInverse.Invoke(!arg.BooleanValue);
			break;
		case ConfigurableTypes.String:
			this.OnStringValueChanged.Invoke(arg.StringValue);
			break;
		}
		if (this.liveChange)
		{
			this.SaveValue();
		}
		else
		{
			this.UpdateDeviationStatus(this.configurable.DeviatesFromSavedValue);
		}
		this.PrintValue(this.configurable);
	}

	[SerializeField]
	private Configurator.ConfiguratorTypes configuratorType;

	private bool liveChange = true;

	[SerializeField]
	private bool updateOnEnable = true;

	[SerializeField]
	protected Configurable configurable;

	[SerializeField]
	protected IntValueChangedEvent OnIntValueChanged;

	[SerializeField]
	protected FloatValueChangedEvent OnFloatValueChanged;

	[SerializeField]
	protected BooleanValueChangedEvent OnBooleanValueChanged;

	[SerializeField]
	protected BooleanValueChangedEvent OnBooleanValueChangedInverse;

	[SerializeField]
	protected StringValueChangedEvent OnStringValueChanged;

	[SerializeField]
	protected PrintValueEvent OnPrintValue;

	[SerializeField]
	protected UnityEvent OnValueDeviated;

	[SerializeField]
	protected UnityEvent OnValueMatched;

	private bool configuratorValueEqualsSavedValue = true;

	public enum ConfiguratorTypes
	{
		Int,
		Float,
		Boolean,
		String,
		Custom
	}
}
