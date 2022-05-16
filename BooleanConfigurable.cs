using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boolean Configurable", menuName = "Configurables/Configurable/Boolean Configurable")]
[Serializable]
public class BooleanConfigurable : Configurable
{
	public override LiveData CreateDefaultLiveData()
	{
		return new LiveData(this.key, ConfigurableTypes.Boolean)
		{
			BooleanValue = this.defaultValue
		};
	}

	public override void SetNewConfiguredValue(LiveData argument)
	{
		base.SetNewConfiguredValue(argument);
	}

	public void SetValue(bool value)
	{
		this.liveData.BooleanValue = value;
		this.SetNewConfiguredValue(this.liveData);
	}

	[SerializeField]
	private bool defaultValue;
}
