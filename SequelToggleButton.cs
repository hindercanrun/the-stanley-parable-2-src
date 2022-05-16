using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class SequelToggleButton : MonoBehaviour
{
	public void SetConfigurableToIndex()
	{
		if (this.fixType == SequelToggleButton.FixType.Prefix)
		{
			this.prefixIndexConfigurable.SetValue(this.index);
		}
		if (this.fixType == SequelToggleButton.FixType.Postfix)
		{
			this.postfixIndexConfigurable.SetValue(this.index);
		}
	}

	private void OnValidate()
	{
		this.SetTerm();
	}

	private void Start()
	{
		this.SetTerm();
		IntConfigurable intConfigurable = this.prefixIndexConfigurable;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		IntConfigurable intConfigurable2 = this.postfixIndexConfigurable;
		intConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable2.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.Instance_OnInputDeviceTypeChanged;
		this.OnValueChanged(null);
	}

	private void Instance_OnInputDeviceTypeChanged(GameMaster.InputDevice input)
	{
	}

	private void OnDestroy()
	{
		IntConfigurable intConfigurable = this.prefixIndexConfigurable;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		IntConfigurable intConfigurable2 = this.postfixIndexConfigurable;
		intConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable2.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		if (Singleton<GameMaster>.Instance != null)
		{
			Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.Instance_OnInputDeviceTypeChanged;
		}
	}

	public void OnValueChanged(LiveData ld)
	{
		if (this.fixType == SequelToggleButton.FixType.Postfix)
		{
			this.toggle.interactable = (this.prefixIndexConfigurable.GetIntValue() != -1);
		}
	}

	public void SetIndex(int i)
	{
		this.index = i;
		this.SetTerm();
	}

	private void SetTerm()
	{
		if (this.fixType == SequelToggleButton.FixType.Prefix)
		{
			this.localize.Term = SequelTools.PrefixTerm(this.index);
		}
		if (this.fixType == SequelToggleButton.FixType.Postfix)
		{
			this.localize.Term = SequelTools.PostfixTerm(this.index);
		}
	}

	public Toggle toggle;

	public SequelToggleButton.FixType fixType;

	[SerializeField]
	private int index;

	[SerializeField]
	private IntConfigurable prefixIndexConfigurable;

	[SerializeField]
	private IntConfigurable postfixIndexConfigurable;

	public Localize localize;

	public enum FixType
	{
		Prefix,
		Postfix
	}
}
