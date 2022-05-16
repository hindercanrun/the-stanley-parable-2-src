using System;
using TMPro;
using UnityEngine;

public class InWorldTextualObject : MonoBehaviour
{
	public void OnEnable()
	{
		InWorldLabelManager.Instance.RegisterObject(this);
	}

	public void OnDisable()
	{
		if (InWorldLabelManager.Instance != null)
		{
			InWorldLabelManager.Instance.DeregisterObject(this);
		}
	}

	public string localizationTerm;

	public Color labelColor = Color.white;

	public float activationRadius = 10f;

	public HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center;

	[Header("Debug, ignore these")]
	public float currentRadius_DEBUG = -1f;

	public float currentAngleFromCenter_DEBUG = -1f;

	public Collider obstruction_DEBUG;

	public Vector2 normalizedPosition_DEBUG;
}
