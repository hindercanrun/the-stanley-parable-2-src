using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CategoryButton : MonoBehaviour
{
	private void Awake()
	{
		this.toggle = base.GetComponent<Toggle>();
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetCategory));
		this.toggle.onValueChanged.Invoke(this.toggle.isOn);
	}

	private void OnDestroy()
	{
		this.toggle.onValueChanged.RemoveAllListeners();
	}

	private void SetCategory(bool value)
	{
		if (this.categoryRoot != null)
		{
			this.categoryRoot.SetActive(value);
		}
	}

	[SerializeField]
	private GameObject categoryRoot;

	private Toggle toggle;
}
