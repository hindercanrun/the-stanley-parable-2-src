using System;
using UnityEngine;
using UnityEngine.UI;

public class SetImageAlphaFromConfigurable : MonoBehaviour
{
	private void Start()
	{
		FloatConfigurable floatConfigurable = this.floatConfigurable;
		floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(floatConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		this.OnValueChanged(null);
	}

	private void OnDestroy()
	{
		FloatConfigurable floatConfigurable = this.floatConfigurable;
		floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(floatConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
	}

	private void OnValueChanged(LiveData data)
	{
		Color color = this.targetImage.color;
		color.a = this.floatConfigurable.GetNormalizedFloatValue();
		this.targetImage.color = color;
	}

	public FloatConfigurable floatConfigurable;

	public Image targetImage;
}
