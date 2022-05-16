using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class EnabledIfAttribute : PropertyAttribute
{
	public EnabledIfAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}

	public readonly string MethodName;
}
