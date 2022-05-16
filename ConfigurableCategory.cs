using System;
using System.Collections.Generic;

[Serializable]
public class ConfigurableCategory
{
	public string CategoryName;

	public List<ConfigurableSubCategory> SubCategories = new List<ConfigurableSubCategory>();
}
