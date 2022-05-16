using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class DynamicLabelAttribute : PropertyAttribute
{
	public DynamicLabelAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}

	public readonly string MethodName;
}
