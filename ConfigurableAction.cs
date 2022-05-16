using System;
using UnityEngine;

public class ConfigurableAction : MonoBehaviour
{
	public void IntConfigurable_Increment(IntConfigurable intConfigurable)
	{
		intConfigurable.IncreaseValue();
	}

	public void IntConfigurable_Decrement(IntConfigurable intConfigurable)
	{
		intConfigurable.DecreaseValue();
	}

	public void InitAndSave(Configurable configurable)
	{
		configurable.Init();
		configurable.SaveToDiskAll();
	}
}
