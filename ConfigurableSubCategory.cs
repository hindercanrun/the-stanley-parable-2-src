using System;
using System.Collections.Generic;

[Serializable]
public class ConfigurableSubCategory
{
	public string SubCategoryName;

	public List<Configurable> Configurables = new List<Configurable>();

	public List<ConfigurableAvailabilities> ConfigurableAvailability = new List<ConfigurableAvailabilities>();
}
