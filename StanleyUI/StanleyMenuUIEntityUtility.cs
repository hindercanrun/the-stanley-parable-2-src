using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace StanleyUI
{
	public class StanleyMenuUIEntityUtility : MonoBehaviour
	{
		private bool IsBoolType()
		{
			return this.type == StanleyMenuUIEntityUtility.UIType.Toggle;
		}

		private bool IsIntType()
		{
			return this.type == StanleyMenuUIEntityUtility.UIType.Selector_IntBased;
		}

		private bool IsFloatType()
		{
			return this.type == StanleyMenuUIEntityUtility.UIType.Slider;
		}

		private bool IsStringType()
		{
			return this.type == StanleyMenuUIEntityUtility.UIType.Selector_StringBased;
		}

		public void RunSettingCodeProxy_bool(bool on)
		{
			ISettingsBoolListener[] components = base.GetComponents<ISettingsBoolListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SetValue(on);
			}
		}

		public void RunSettingCodeProxy_float(float val)
		{
			ISettingsFloatListener[] components = base.GetComponents<ISettingsFloatListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SetValue(val);
			}
		}

		public void RunSettingCodeProxy_int(int val)
		{
			ISettingsIntListener[] components = base.GetComponents<ISettingsIntListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SetValue(val);
			}
		}

		public void RunSettingCodeProxy_string(string val)
		{
			ISettingsStringListener[] components = base.GetComponents<ISettingsStringListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SetValue(val);
			}
		}

		public void RunSettingCodeFromConfigurable()
		{
			if (this.targetConfigurable == null)
			{
				return;
			}
			switch (this.type)
			{
			case StanleyMenuUIEntityUtility.UIType.Slider:
				this.RunSettingCodeProxy_float(this.targetConfigurable.GetFloatValue());
				return;
			case StanleyMenuUIEntityUtility.UIType.Selector:
			case StanleyMenuUIEntityUtility.UIType.SubHeading:
				break;
			case StanleyMenuUIEntityUtility.UIType.Toggle:
				this.RunSettingCodeProxy_bool(this.targetConfigurable.GetBooleanValue());
				return;
			case StanleyMenuUIEntityUtility.UIType.Selector_IntBased:
				this.RunSettingCodeProxy_int(this.targetConfigurable.GetIntValue());
				return;
			case StanleyMenuUIEntityUtility.UIType.Selector_StringBased:
				this.RunSettingCodeProxy_string(this.targetConfigurable.GetStringValue());
				break;
			default:
				return;
			}
		}

		[Header("Hints")]
		[InspectorButton("FindType", "Find Type")]
		public StanleyMenuUIEntityUtility.UIType type;

		public string title;

		[InspectorButton("TryConfigure", "Try Configure Using Hints")]
		public Configurable targetConfigurable;

		[Header("Setter Code Overrides (can be null)")]
		[EnabledIf("IsBoolType")]
		public UnityEvent<bool> settingCodeOverride_bool;

		[EnabledIf("IsIntType")]
		public UnityEvent<int> settingCodeOverride_int;

		[EnabledIf("IsStringType")]
		public UnityEvent<string> settingCodeOverride_string;

		[Header("Set in prefab")]
		public TextMeshProUGUI label;

		[Header("Results:")]
		public bool hasConfigurableEvent;

		public bool hasConfigurableEventConfigurableSet;

		public bool hasConfigurableEventSelfInvoke;

		public bool hasConfigurableEventEventsSetUpCorrectly;

		public bool hasConfigurator;

		public bool hasConfiguratorConfigurableSet;

		public string labelText;

		public bool hasCustomLabel;

		public bool hasISettingsListener;

		[InspectorButton("CheckConfigurable", "Run Checks")]
		public bool noListenerButAllGood;

		[InspectorButton("CorrectConfigurable", "Run Corrections")]
		public bool allGood;

		public enum UIType
		{
			Unknown,
			Slider,
			Selector,
			Toggle,
			SubHeading,
			Selector_IntBased,
			Selector_StringBased
		}
	}
}
