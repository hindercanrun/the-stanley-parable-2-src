using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VolumeBase : MonoBehaviour
{
	public virtual ProfileBase GetProfile()
	{
		return null;
	}

	private void Awake()
	{
		if (this.BoxCollider == null)
		{
			this.UpdateBoxCollider();
		}
		this.UpdateFeatheredBounds();
	}

	private void OnValidate()
	{
		if (this.BoxCollider == null)
		{
			this.UpdateBoxCollider();
		}
		this.UpdateFeatheredBounds();
	}

	private void UpdateBoxCollider()
	{
		this.BoxCollider = base.GetComponent<BoxCollider>();
		this.BoxCollider.isTrigger = true;
	}

	private void UpdateFeatheredBounds()
	{
		Vector3 center = base.transform.TransformPoint(this.BoxCollider.center);
		Vector3 vector = this.BoxCollider.size;
		vector = base.transform.TransformVector(vector);
		this.FeatheredBounds = new Bounds(center, new Vector3(vector.x * this.FeatherValue, vector.y, vector.z * this.FeatherValue));
	}

	private void OnDrawGizmos()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
		if (component != null || component2 != null)
		{
			Vector3 center;
			Vector3 vector;
			if (component != null)
			{
				center = component.center;
				vector = component.size;
			}
			else
			{
				center = component2.offset;
				vector = component2.size;
			}
			if (this.Preservation != VolumeBase.VolumePreservation.FeatheredVolume && this.Preservation != VolumeBase.VolumePreservation.FeatheredVolumePreserve)
			{
				if (this.Preservation == VolumeBase.VolumePreservation.DiscardWhenLeaving)
				{
					Gizmos.color = this.GetVolumeBaseColor();
				}
				else
				{
					Gizmos.color = this.GetVolumeBaseColor() * Color.gray;
				}
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawCube(center, vector);
				return;
			}
			if (this.Preservation == VolumeBase.VolumePreservation.FeatheredVolumePreserve || this.Preservation == VolumeBase.VolumePreservation.FeatheredVolume)
			{
				if (this.Preservation == VolumeBase.VolumePreservation.FeatheredVolume)
				{
					Gizmos.color = this.GetVolumeBaseColor();
				}
				else
				{
					Gizmos.color = this.GetVolumeBaseColor() * Color.gray;
				}
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawCube(center, vector);
				Gizmos.color = this.GetInnerCubeColor();
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawCube(center, new Vector3(vector.x * this.FeatherValue, vector.y, vector.z * this.FeatherValue));
			}
		}
	}

	protected virtual Color GetVolumeBaseColor()
	{
		return Color.green * new Color(1f, 1f, 1f, 0.25f);
	}

	protected virtual Color GetInnerCubeColor()
	{
		return Color.yellow * new Color(1f, 1f, 1f, 0.25f);
	}

	public int Priority;

	public VolumeBase.VolumePreservation Preservation;

	public AnimationCurve LerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float Duration = 1f;

	[Range(0f, 1f)]
	public float FeatherValue;

	[SerializeField]
	[HideInInspector]
	public BoxCollider BoxCollider;

	[HideInInspector]
	public Bounds FeatheredBounds;

	public enum VolumePreservation
	{
		DiscardWhenLeaving,
		Preserve,
		FeatheredVolume,
		FeatheredVolumePreserve
	}
}
