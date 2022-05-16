using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public float ButtonWidth
	{
		get
		{
			return this._buttonWidth;
		}
		set
		{
			this._buttonWidth = value;
		}
	}

	public InspectorButtonAttribute(string MethodName, string DisplayName = null)
	{
		this.MethodName = MethodName;
		this.DisplayName = ((DisplayName == null) ? MethodName : DisplayName);
	}

	public static float kDefaultButtonWidth;

	public readonly string MethodName;

	public readonly string DisplayName;

	private float _buttonWidth = InspectorButtonAttribute.kDefaultButtonWidth;
}
