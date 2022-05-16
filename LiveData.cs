using System;

[Serializable]
public class LiveData
{
	public LiveData(string _key, ConfigurableTypes _type)
	{
		this.key = _key;
		this.configureableType = _type;
	}

	public void CopyValuesFrom(LiveData data)
	{
		this.IntValue = data.IntValue;
		this.FloatValue = data.FloatValue;
		this.BooleanValue = data.BooleanValue;
		this.StringValue = data.StringValue;
	}

	public string key;

	public ConfigurableTypes configureableType;

	public int IntValue;

	public float FloatValue;

	public bool BooleanValue;

	public string StringValue;
}
