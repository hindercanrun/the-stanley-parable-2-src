using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class SequelTitleTextFormatter : MonoBehaviour
{
	private void Start()
	{
		this.UpdateTitleName();
		LocalizationManager.OnLocalizeEvent += this.UpdateTitleName;
		IntConfigurable intConfigurable = this.sequelCountConfigurable;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
		IntConfigurable intConfigurable2 = this.prefixIndexConfigurable;
		intConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable2.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
		IntConfigurable intConfigurable3 = this.postfixIndexConfigurable;
		intConfigurable3.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable3.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= this.UpdateTitleName;
		IntConfigurable intConfigurable = this.sequelCountConfigurable;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
		IntConfigurable intConfigurable2 = this.prefixIndexConfigurable;
		intConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable2.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
		IntConfigurable intConfigurable3 = this.postfixIndexConfigurable;
		intConfigurable3.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable3.OnValueChanged, new Action<LiveData>(this.UpdateTitleName));
	}

	public void ForceUpdateTitleName()
	{
		this.UpdateTitleName();
	}

	private void UpdateTitleName(LiveData ld)
	{
		this.UpdateTitleName();
	}

	private void UpdateTitleName()
	{
		this.titleText.text = SequelTools.DoStandardSequelReplacementStep(this.unformattedString, this.sequelCountConfigurable, this.prefixIndexConfigurable, this.postfixIndexConfigurable).ToUpper();
	}

	public TMP_Text titleText;

	public IntConfigurable sequelCountConfigurable;

	public IntConfigurable prefixIndexConfigurable;

	public IntConfigurable postfixIndexConfigurable;

	[Multiline(2)]
	public string unformattedString = "The Stanley Parable %!N!%:\n%!P!% %!S!%";
}
