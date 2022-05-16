using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New String Configurable", menuName = "Configurables/Configurable/String Configurable")]
[Serializable]
public class StringConfigurable : Configurable
{
	public override LiveData CreateDefaultLiveData()
	{
		return new LiveData(this.key, ConfigurableTypes.String)
		{
			StringValue = this.defaultValue
		};
	}

	public void SetValue(string value)
	{
		this.liveData.StringValue = value;
		this.SetNewConfiguredValue(this.liveData);
	}

	[SerializeField]
	private string defaultValue = "";
}
