using System;
using StanleyUI;
using UnityEngine;

public class AntialiasingSelector : MonoBehaviour, ISettingsIntListener
{
	private int IndexToAAValue(int index)
	{
		switch (index)
		{
		default:
			return 0;
		case 1:
			return 2;
		case 2:
			return 4;
		case 3:
			return 8;
		}
	}

	private string IndexToAAOption(int index)
	{
		if (index == 0)
		{
			return "-";
		}
		return this.IndexToAAValue(index) + "x AA";
	}

	public void PrintAll(int index)
	{
		this.PrintAAOption(index);
		this.PrintAAValue(index);
		this.SetValue(index);
	}

	public void PrintAAOption(int index)
	{
		StringValueChangedEvent onPrintAAOption = this.OnPrintAAOption;
		if (onPrintAAOption == null)
		{
			return;
		}
		onPrintAAOption.Invoke(this.IndexToAAOption(index));
	}

	public void PrintAAValue(int index)
	{
		IntValueChangedEvent onPrintAAValue = this.OnPrintAAValue;
		if (onPrintAAValue == null)
		{
			return;
		}
		onPrintAAValue.Invoke(this.IndexToAAValue(index));
	}

	public void SetValue(int val)
	{
		QualitySettings.antiAliasing = this.IndexToAAValue(val);
	}

	[SerializeField]
	private StringValueChangedEvent OnPrintAAOption;

	[SerializeField]
	private IntValueChangedEvent OnPrintAAValue;
}
