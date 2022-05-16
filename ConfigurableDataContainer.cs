using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigurableDataContainer
{
	public static ConfigurableDataContainer LoadContainer(string fileName)
	{
		if (PlatformPlayerPrefs.HasKey(fileName))
		{
			return JsonUtility.FromJson<ConfigurableDataContainer>(PlatformPlayerPrefs.GetString(fileName));
		}
		return new ConfigurableDataContainer();
	}

	public LiveData GetSavedDataFromContainer(string key)
	{
		for (int i = 0; i < this.saveDataCache.Count; i++)
		{
			if (this.saveDataCache[i].key.Equals(key))
			{
				return this.saveDataCache[i];
			}
		}
		return null;
	}

	public void UpdateSaveDataCache(LiveData data)
	{
		bool flag = true;
		for (int i = 0; i < this.saveDataCache.Count; i++)
		{
			if (data.key.Equals(this.saveDataCache[i].key))
			{
				this.saveDataCache[i] = data;
				flag = false;
				break;
			}
		}
		if (flag)
		{
			this.saveDataCache.Add(data);
		}
	}

	public void DeleteEntry(string key)
	{
		for (int i = 0; i < this.saveDataCache.Count; i++)
		{
			if (this.saveDataCache[i].key.Equals(key))
			{
				this.saveDataCache.RemoveAt(i);
			}
		}
		this.SaveToPlatformPrefs("data", true);
	}

	public void SaveToPlatformPrefs(string fileName, bool forceSave = false)
	{
		if (ConfigurableDataContainer.OnSaveValues != null)
		{
			ConfigurableDataContainer.OnSaveValues();
		}
		if (forceSave || Configurable.Dirty)
		{
			string value = JsonUtility.ToJson(this, true);
			PlatformPlayerPrefs.SetString(fileName, value);
			PlatformPlayerPrefs.Save();
			Configurable.Dirty = false;
		}
	}

	public static Action OnSaveValues;

	public static Action OnResetValues;

	public List<LiveData> saveDataCache = new List<LiveData>();
}
