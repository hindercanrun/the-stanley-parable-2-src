using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Int Configurable", menuName = "Configurables/Configurable/Int Configurable")]
[Serializable]
public class IntConfigurable : Configurable
{
	public int MinValue
	{
		get
		{
			return this.minimumValue;
		}
	}

	public int MaxValue
	{
		get
		{
			return this.maximumValue;
		}
	}

	public override LiveData CreateDefaultLiveData()
	{
		return new LiveData(this.key, ConfigurableTypes.Int)
		{
			IntValue = this.defaultValue
		};
	}

	public void IncreaseValue()
	{
		this.liveData.IntValue++;
		if (!this.clampOnOverflow && this.liveData.IntValue > this.maximumValue)
		{
			this.liveData.IntValue = this.minimumValue;
		}
		this.SetNewConfiguredValue(this.liveData);
	}

	public void DecreaseValue()
	{
		this.liveData.IntValue--;
		if (!this.clampOnOverflow && this.liveData.IntValue < this.minimumValue)
		{
			this.liveData.IntValue = this.maximumValue;
		}
		this.SetNewConfiguredValue(this.liveData);
	}

	public void SetValue(int value)
	{
		this.liveData.IntValue = value;
		this.SetNewConfiguredValue(this.liveData);
	}

	public void SetNewMaxValue(int newMaxValue)
	{
		this.maximumValue = newMaxValue;
	}

	public void SetNewMinValue(int newMinValue)
	{
		this.minimumValue = newMinValue;
	}

	public override void SetNewConfiguredValue(LiveData argument)
	{
		if (argument.IntValue > this.maximumValue)
		{
			argument.IntValue = this.maximumValue;
		}
		if (argument.IntValue < this.minimumValue)
		{
			argument.IntValue = this.minimumValue;
		}
		base.SetNewConfiguredValue(argument);
	}

	[SerializeField]
	private int defaultValue;

	[SerializeField]
	private int minimumValue;

	[SerializeField]
	private int maximumValue = 1;

	[SerializeField]
	private bool clampOnOverflow;
}
