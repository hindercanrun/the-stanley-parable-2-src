using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Float Configurable", menuName = "Configurables/Configurable/Float Configurable")]
[Serializable]
public class FloatConfigurable : Configurable
{
	public float MinValue
	{
		get
		{
			return this.minimumValue;
		}
	}

	public float MaxValue
	{
		get
		{
			return this.maximumValue;
		}
	}

	public override LiveData CreateDefaultLiveData()
	{
		return new LiveData(this.key, ConfigurableTypes.Float)
		{
			FloatValue = this.defaultValue
		};
	}

	public override void SetNewConfiguredValue(LiveData argument)
	{
		if (argument.FloatValue > this.maximumValue)
		{
			argument.FloatValue = this.maximumValue;
		}
		if (argument.FloatValue < this.minimumValue)
		{
			argument.FloatValue = this.minimumValue;
		}
		base.SetNewConfiguredValue(argument);
	}

	public void SetValue(float value)
	{
		this.liveData.FloatValue = value;
		this.SetNewConfiguredValue(this.liveData);
	}

	public float GetNormalizedFloatValue()
	{
		return Mathf.InverseLerp(this.MinValue, this.maximumValue, base.GetFloatValue());
	}

	public override string PrintValue()
	{
		return this.liveData.FloatValue.ToString("F0");
	}

	[SerializeField]
	private float defaultValue;

	[SerializeField]
	private float minimumValue;

	[SerializeField]
	private float maximumValue = 1f;
}
