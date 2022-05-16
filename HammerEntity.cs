using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Nest.Components;
using UnityEngine;
using UnityEngine.Events;

public class HammerEntity : MonoBehaviour
{
	protected void FireOutput(Outputs output)
	{
		this.FireOutput(output, "", float.NaN);
	}

	protected void FireOutput(Outputs output, string parameterOverride)
	{
		this.FireOutput(output, parameterOverride, float.NaN);
	}

	protected void FireOutput(Outputs output, float parameterFloat)
	{
		this.FireOutput(output, "", parameterFloat);
	}

	protected void FireOutput(Outputs output, string parameterString, float parameterFloat)
	{
		if (!this.sorted)
		{
			this.SortOutputsByDelay();
			this.sorted = true;
		}
		for (int i = 0; i < this.expandedConnections.Count; i++)
		{
			if (this.expandedConnections[i].output == output && this.expandedConnections[i].nestInput)
			{
				if (parameterString != "")
				{
					if (this.expandedConnections[i].nestInput.CurrentEventType == NestInput.EventType.String && this.expandedConnections[i].nestInput._eventString.GetPersistentEventCount() > 0)
					{
						object persistentTarget = this.expandedConnections[i].nestInput._eventString.GetPersistentTarget(0);
						MethodInfo method = persistentTarget.GetType().GetMethod(this.expandedConnections[i].nestInput._eventString.GetPersistentMethodName(0), new Type[]
						{
							typeof(string)
						});
						this.expandedConnections[i].nestInput._eventString = new NestInput.StringEvent();
						UnityAction<string> call = Delegate.CreateDelegate(typeof(UnityAction<string>), persistentTarget, method) as UnityAction<string>;
						this.expandedConnections[i].nestInput._eventString.AddListener(call);
					}
					this.expandedConnections[i].nestInput._parameterString = parameterString;
				}
				if (!float.IsNaN(parameterFloat))
				{
					if (this.expandedConnections[i].nestInput.CurrentEventType == NestInput.EventType.Float && this.expandedConnections[i].nestInput._eventValue.GetPersistentEventCount() > 0)
					{
						object persistentTarget2 = this.expandedConnections[i].nestInput._eventValue.GetPersistentTarget(0);
						MethodInfo method2 = persistentTarget2.GetType().GetMethod(this.expandedConnections[i].nestInput._eventValue.GetPersistentMethodName(0), new Type[]
						{
							typeof(float)
						});
						this.expandedConnections[i].nestInput._eventValue = new NestInput.ValueEvent();
						UnityAction<float> call2 = Delegate.CreateDelegate(typeof(UnityAction<float>), persistentTarget2, method2) as UnityAction<float>;
						this.expandedConnections[i].nestInput._eventValue.AddListener(call2);
					}
					this.expandedConnections[i].nestInput._parameterFloat = parameterFloat;
				}
				this.expandedConnections[i].nestInput.Invoke();
			}
		}
	}

	private void SortOutputsByDelay()
	{
		List<HammerConnection> list = new List<HammerConnection>();
		while (this.expandedConnections.Count > 0)
		{
			float num = float.MaxValue;
			int num2 = -1;
			for (int i = 0; i < this.expandedConnections.Count; i++)
			{
				if (this.expandedConnections[i].delay < num)
				{
					num = this.expandedConnections[i].delay;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				list.Add(this.expandedConnections[num2]);
				this.expandedConnections.Remove(this.expandedConnections[num2]);
			}
		}
		this.expandedConnections = list;
	}

	private void Validate()
	{
		int i;
		int j;
		for (i = 0; i < this.expandedConnections.Count; i = j + 1)
		{
			new List<Component>(base.GetComponents<Component>()).FindIndex((Component x) => x == this.expandedConnections[i].nestInput);
			j = i;
		}
	}

	private IEnumerator LogOutput(HammerConnection connection, string output, string filter)
	{
		yield return new WaitForGameSeconds(connection.nestInput.Delay);
		string text = " ::: ";
		string text2 = "                              ";
		string text3 = text + "              OUTPUT              " + text;
		string text4;
		if (connection.recipientObject == null)
		{
			text4 = "DESTROYED";
		}
		else
		{
			text4 = connection.recipientObject.name;
		}
		string text5;
		if (connection.nestInput._parameterString != "")
		{
			text5 = connection.nestInput._parameterString;
		}
		else
		{
			text5 = connection.nestInput._parameterFloat.ToString();
		}
		text3 = text3 + base.name + text2.Substring(base.name.Length) + text;
		text3 = text3 + output + text2.Substring(output.Length) + text;
		text3 = text3 + text4 + text2.Substring(text4.Length) + text;
		text3 = text3 + connection.input.ToString() + text2.Substring(connection.input.ToString().Length) + text;
		text3 = text3 + text5 + text2.Substring(text5.Length) + text;
		text3 += connection.nestInput.Delay.ToString();
		if (!(filter == ""))
		{
			text3.Contains(filter);
		}
		yield break;
	}

	public virtual void Use()
	{
	}

	public virtual void Input_Enable()
	{
		this.isEnabled = true;
	}

	public virtual void Input_Disable()
	{
		this.isEnabled = false;
	}

	public virtual void Input_Kill()
	{
		base.gameObject.SetActive(false);
	}

	public void Input_FireUser1()
	{
		this.FireOutput(Outputs.OnUser1);
	}

	public void Input_FireUser2()
	{
		this.FireOutput(Outputs.OnUser2);
	}

	public void Input_FireUser3()
	{
		this.FireOutput(Outputs.OnUser3);
	}

	public void Input_FireUser4()
	{
		this.FireOutput(Outputs.OnUser4);
	}

	public List<HammerConnection> connections = new List<HammerConnection>();

	public List<HammerConnection> expandedConnections = new List<HammerConnection>();

	[HideInInspector]
	public GameObject parent;

	public bool isEnabled = true;

	private bool sorted;
}
