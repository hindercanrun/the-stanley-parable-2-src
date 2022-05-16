using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class InWorldLabelManager : MonoBehaviour
{
	public static InWorldLabelManager Instance { get; private set; }

	private void Awake()
	{
		InWorldLabelManager.Instance = this;
		this.rectTransform = this.scaler.GetComponent<RectTransform>();
		LocalizationManager.OnLocalizeEvent += this.LocalizationManager_OnLocalizeEvent;
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= this.LocalizationManager_OnLocalizeEvent;
	}

	private void LocalizationManager_OnLocalizeEvent()
	{
		foreach (KeyValuePair<InWorldTextualObject, InWorldText> keyValuePair in this.inWorldObjects)
		{
			keyValuePair.Value.InitTextLabel(keyValuePair.Key);
		}
	}

	public void RegisterObject(InWorldTextualObject worldObj)
	{
		InWorldText component = Object.Instantiate<GameObject>(this.textPrefab.gameObject).GetComponent<InWorldText>();
		component.transform.parent = base.transform;
		component.transform.localScale = Vector3.one;
		component.transform.localPosition = new Vector2(-1000f, 1000f);
		this.inWorldObjects[worldObj] = component;
		component.InitTextLabel(worldObj);
	}

	public void DeregisterObject(InWorldTextualObject obj)
	{
		if (!this.inWorldObjects.ContainsKey(obj))
		{
			Debug.LogError("Deregistering an in world object that has not been registered :(");
			return;
		}
		if (this.inWorldObjects[obj] != null && this.inWorldObjects[obj].gameObject != null)
		{
			Object.Destroy(this.inWorldObjects[obj].gameObject);
		}
		this.inWorldObjects.Remove(obj);
	}

	private void Update()
	{
		foreach (KeyValuePair<InWorldTextualObject, InWorldText> keyValuePair in this.inWorldObjects)
		{
			InWorldTextualObject key = keyValuePair.Key;
			Vector2 vector = StanleyController.Instance.cam.WorldToViewportPoint(key.transform.position);
			Vector2 screenPosition = (vector - Vector2.one / 2f) * this.rectTransform.sizeDelta;
			key.normalizedPosition_DEBUG = vector;
			bool flag = false;
			if (!this.normalizedIllegalZone.Contains(vector))
			{
				Vector3 vector2 = key.transform.position - StanleyController.Instance.camTransform.position;
				float magnitude = vector2.magnitude;
				key.currentRadius_DEBUG = magnitude;
				float num = Vector3.SignedAngle(vector2, StanleyController.Instance.transform.forward, Vector3.up);
				key.currentAngleFromCenter_DEBUG = num;
				flag = (magnitude < key.activationRadius);
				if (Mathf.Abs(num) > 90f)
				{
					flag = false;
				}
				key.obstruction_DEBUG = null;
				RaycastHit raycastHit;
				if (flag && Physics.SphereCast(StanleyController.Instance.camTransform.position, this.sphereCastRadius, vector2, out raycastHit, magnitude - this.sphereCastRadius, this.raycastMask, QueryTriggerInteraction.Ignore))
				{
					flag = false;
					key.obstruction_DEBUG = raycastHit.collider;
				}
			}
			float alpha = (float)(flag ? 1 : 0);
			this.PlaceTextObject(keyValuePair.Key, screenPosition, alpha);
		}
	}

	private void PlaceTextObject(InWorldTextualObject worldObj, Vector2 screenPosition, float alpha)
	{
		InWorldText inWorldText = this.inWorldObjects[worldObj];
		inWorldText.transform.localPosition = screenPosition;
		inWorldText.UpdateTextLabel(worldObj, alpha, this.fadeTime);
	}

	public InWorldLabelManager.InWorldLabelSizeProfile[] sizeProfiles;

	public float fadeTime = 0.3f;

	public InWorldText textPrefab;

	public CanvasScaler scaler;

	public LayerMask raycastMask;

	public float sphereCastRadius = 0.1f;

	public Rect normalizedIllegalZone = new Rect(0f, 0.8f, 1f, 0.2f);

	private Dictionary<InWorldTextualObject, InWorldText> inWorldObjects = new Dictionary<InWorldTextualObject, InWorldText>();

	private RectTransform rectTransform;

	[Serializable]
	public class InWorldLabelSizeProfile
	{
		public float fontSize = 38f;

		public string i2LocalizationTerm = "None";
	}
}
