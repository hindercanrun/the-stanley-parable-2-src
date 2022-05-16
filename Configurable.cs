using System;
using UnityEngine;

public class Configurable : ScriptableObject
{
	public string Key
	{
		get
		{
			return this.key;
		}
	}

	public bool DeviatesFromSavedValue
	{
		get
		{
			return this.deviatesFromSavedValue;
		}
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		if (Configurable.ConfigurableDataContainer == null)
		{
			Configurable.ConfigurableDataContainer = ConfigurableDataContainer.LoadContainer("data");
		}
		this.liveData = this.LoadOrCreateSaveData();
		this.UpdateSaveDataCache();
		ConfigurableDataContainer.OnSaveValues = (Action)Delegate.Combine(ConfigurableDataContainer.OnSaveValues, new Action(this.OnSaveValues));
		ConfigurableDataContainer.OnResetValues = (Action)Delegate.Combine(ConfigurableDataContainer.OnResetValues, new Action(this.OnResetValues));
		this.initialized = true;
	}

	public void ForceUpdate()
	{
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this.liveData);
		}
	}

	private void OnDestroy()
	{
		ConfigurableDataContainer.OnSaveValues = (Action)Delegate.Remove(ConfigurableDataContainer.OnSaveValues, new Action(this.OnSaveValues));
		ConfigurableDataContainer.OnResetValues = (Action)Delegate.Remove(ConfigurableDataContainer.OnResetValues, new Action(this.OnResetValues));
	}

	private void UpdateSaveDataCache()
	{
		if (this.persistent && this.saveDataCache == null)
		{
			this.saveDataCache = new LiveData(this.liveData.key, this.liveData.configureableType);
		}
		if (this.saveDataCache != null)
		{
			this.saveDataCache.CopyValuesFrom(this.liveData);
		}
	}

	public virtual void SetNewConfiguredValue(LiveData data)
	{
		if (this.saveDataCache == null)
		{
			this.deviatesFromSavedValue = true;
			switch (data.configureableType)
			{
			case ConfigurableTypes.Int:
				this.liveData.IntValue = data.IntValue;
				break;
			case ConfigurableTypes.Float:
				this.liveData.FloatValue = data.FloatValue;
				break;
			case ConfigurableTypes.Boolean:
				this.liveData.BooleanValue = data.BooleanValue;
				break;
			case ConfigurableTypes.String:
				this.liveData.StringValue = data.StringValue;
				break;
			}
			if (this.OnValueChanged != null)
			{
				this.OnValueChanged(this.liveData);
			}
			return;
		}
		switch (data.configureableType)
		{
		case ConfigurableTypes.Int:
			this.deviatesFromSavedValue = (this.saveDataCache.IntValue != data.IntValue);
			this.liveData.IntValue = data.IntValue;
			break;
		case ConfigurableTypes.Float:
			this.deviatesFromSavedValue = (this.saveDataCache.FloatValue != data.FloatValue);
			this.liveData.FloatValue = data.FloatValue;
			break;
		case ConfigurableTypes.Boolean:
			this.deviatesFromSavedValue = (this.saveDataCache.BooleanValue != data.BooleanValue);
			this.liveData.BooleanValue = data.BooleanValue;
			break;
		case ConfigurableTypes.String:
			this.deviatesFromSavedValue = !this.saveDataCache.StringValue.Equals(data.StringValue);
			this.liveData.StringValue = data.StringValue;
			break;
		}
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this.liveData);
		}
	}

	public virtual LiveData LoadOrCreateSaveData()
	{
		LiveData savedDataFromContainer = Configurable.ConfigurableDataContainer.GetSavedDataFromContainer(this.key);
		if (this.persistent && savedDataFromContainer != null && this.IsValidSaveData(savedDataFromContainer))
		{
			return savedDataFromContainer;
		}
		return this.CreateDefaultLiveData();
	}

	public virtual bool IsValidSaveData(LiveData data)
	{
		bool result;
		switch (data.configureableType)
		{
		case ConfigurableTypes.Int:
			result = (this is IntConfigurable);
			break;
		case ConfigurableTypes.Float:
			result = (this is FloatConfigurable);
			break;
		case ConfigurableTypes.Boolean:
			result = (this is BooleanConfigurable);
			break;
		case ConfigurableTypes.String:
			result = (this is StringConfigurable);
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	public virtual LiveData LoadOrCreateSaveData(out bool saveDataExists)
	{
		LiveData savedDataFromContainer = Configurable.ConfigurableDataContainer.GetSavedDataFromContainer(this.key);
		if (this.persistent && savedDataFromContainer != null && this.IsValidSaveData(savedDataFromContainer))
		{
			saveDataExists = true;
			return savedDataFromContainer;
		}
		saveDataExists = false;
		return this.CreateDefaultLiveData();
	}

	private void OnResetValues()
	{
		if (this.deviatesFromSavedValue)
		{
			this.liveData.CopyValuesFrom(this.saveDataCache);
			if (this.OnValueChanged != null)
			{
				this.OnValueChanged(this.liveData);
			}
			this.deviatesFromSavedValue = false;
		}
	}

	private void OnSaveValues()
	{
		if (this.deviatesFromSavedValue)
		{
			if (this.persistent)
			{
				Configurable.ConfigurableDataContainer.UpdateSaveDataCache(this.liveData);
			}
			this.deviatesFromSavedValue = false;
			if (this.persistent)
			{
				this.UpdateSaveDataCache();
			}
			if (this.OnValueChanged != null)
			{
				this.OnValueChanged(this.liveData);
			}
			if (this.persistent)
			{
				Configurable.Dirty = true;
			}
		}
	}

	public virtual LiveData CreateDefaultLiveData()
	{
		return null;
	}

	public virtual void SaveToDiskAll()
	{
		if (Configurable.ConfigurableDataContainer == null)
		{
			Configurable.ConfigurableDataContainer = ConfigurableDataContainer.LoadContainer("data");
		}
		Configurable.ConfigurableDataContainer.SaveToPlatformPrefs("data", false);
	}

	public virtual string PrintValue()
	{
		switch (this.liveData.configureableType)
		{
		case ConfigurableTypes.Int:
			return this.liveData.IntValue.ToString();
		case ConfigurableTypes.Float:
			return this.liveData.FloatValue.ToString();
		case ConfigurableTypes.Boolean:
			return this.liveData.BooleanValue.ToString();
		case ConfigurableTypes.String:
			return this.liveData.StringValue;
		default:
			return string.Empty;
		}
	}

	public bool GetBooleanValue()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		return this.liveData.BooleanValue;
	}

	public int GetIntValue()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		return this.liveData.IntValue;
	}

	public float GetFloatValue()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		return this.liveData.FloatValue;
	}

	public string GetStringValue()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		return this.liveData.StringValue;
	}

	public ConfigurableTypes GetConfigurableType()
	{
		if (!this.initialized)
		{
			this.Init();
		}
		return this.liveData.configureableType;
	}

	public static ConfigurableDataContainer ConfigurableDataContainer;

	public static bool Dirty;

	public Action<LiveData> OnValueChanged;

	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private bool deviatesFromSavedValue;

	[Header("Configuration")]
	[SerializeField]
	protected string key = "ConfigurableKey";

	[SerializeField]
	private string description = "Say something about this configurable";

	[SerializeField]
	private bool persistent = true;

	[Space]
	[Header("Data")]
	[NonSerialized]
	private LiveData saveDataCache;

	[SerializeField]
	[HideInInspector]
	protected LiveData liveData;

	private bool triedToInitBeforePlayerPrefs;
}
