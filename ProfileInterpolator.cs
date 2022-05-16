using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileInterpolator : MonoBehaviour
{
	protected void SetupResetOnLevelLoad(bool status)
	{
		if (status)
		{
			SceneManager.sceneLoaded += this.SceneLoaded;
			return;
		}
		SceneManager.sceneLoaded -= this.SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.ClearActiveElements();
	}

	protected void ClearActiveElements()
	{
		this.enteredVolumes.Clear();
		this.activeVolume = null;
		if (this.interpolationRoutine != null)
		{
			base.StopCoroutine(this.interpolationRoutine);
			this.interpolationRoutine = null;
		}
	}

	protected VolumeBase GetPriorityVolume()
	{
		int num = -1;
		int num2 = -1;
		for (int i = this.enteredVolumes.Count - 1; i > -1; i--)
		{
			if (this.enteredVolumes[i].Priority > num)
			{
				num = this.enteredVolumes[i].Priority;
				num2 = i;
			}
		}
		if (num2 > -1)
		{
			return this.enteredVolumes[num2];
		}
		return null;
	}

	protected bool VolumeIsHigherPriority(VolumeBase volume)
	{
		int priority = volume.Priority;
		for (int i = 0; i < this.enteredVolumes.Count; i++)
		{
			if (this.enteredVolumes[i].Priority >= priority)
			{
				return false;
			}
		}
		return true;
	}

	public virtual void SetProfileInstant(ProfileBase profile)
	{
	}

	public virtual void InterpolateToProfile(ProfileBase profile, float duration, AnimationCurve curve)
	{
	}

	public virtual void Interpolate(float lerpValue)
	{
	}

	public virtual void FeatherToProfile(ProfileBase profile, VolumeBase volume, AnimationCurve curve)
	{
	}

	public virtual void LinearInterpolationComplete()
	{
	}

	protected void OnEnterVolume(VolumeBase volume)
	{
		if (this.activeVolume == null || this.VolumeIsHigherPriority(volume))
		{
			if (this.currentProfile != null)
			{
				this.previousProfile = this.currentProfile;
			}
			this.currentProfile = volume.GetProfile();
			this.activeVolume = volume;
			VolumeBase.VolumePreservation preservation = volume.Preservation;
			if (preservation > VolumeBase.VolumePreservation.Preserve)
			{
				if (preservation - VolumeBase.VolumePreservation.FeatheredVolume <= 1)
				{
					this.FeatherToProfile(this.currentProfile, volume, volume.LerpCurve);
				}
			}
			else
			{
				this.InterpolateToProfile(this.currentProfile, volume.Duration, volume.LerpCurve);
			}
		}
		if (!this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Add(volume);
		}
	}

	protected void OnExitVolume(VolumeBase volume)
	{
		this.enteredVolumes.Remove(volume);
		if (this.activeVolume == volume)
		{
			if (this.activeVolume.Preservation == VolumeBase.VolumePreservation.DiscardWhenLeaving || this.activeVolume.Preservation == VolumeBase.VolumePreservation.Preserve)
			{
				VolumeBase priorityVolume = this.GetPriorityVolume();
				if (priorityVolume != null)
				{
					this.previousProfile = this.currentProfile;
					this.currentProfile = priorityVolume.GetProfile();
					this.activeVolume = priorityVolume;
					if (priorityVolume.Preservation == VolumeBase.VolumePreservation.DiscardWhenLeaving || priorityVolume.Preservation == VolumeBase.VolumePreservation.Preserve)
					{
						this.InterpolateToProfile(priorityVolume.GetProfile(), priorityVolume.Duration, priorityVolume.LerpCurve);
						return;
					}
					this.FeatherToProfile(priorityVolume.GetProfile(), priorityVolume, priorityVolume.LerpCurve);
					return;
				}
				else if (this.activeVolume.Preservation != VolumeBase.VolumePreservation.Preserve && this.defaultProfile != null)
				{
					this.currentProfile = this.defaultProfile;
					this.previousProfile = null;
					this.activeVolume = null;
					this.InterpolateToProfile(this.currentProfile, 1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
					return;
				}
			}
			else if (this.activeVolume.Preservation == VolumeBase.VolumePreservation.FeatheredVolume || this.activeVolume.Preservation == VolumeBase.VolumePreservation.FeatheredVolumePreserve)
			{
				VolumeBase priorityVolume2 = this.GetPriorityVolume();
				if (priorityVolume2 != null)
				{
					this.previousProfile = this.currentProfile;
					this.currentProfile = priorityVolume2.GetProfile();
					this.activeVolume = priorityVolume2;
					if (priorityVolume2.Preservation == VolumeBase.VolumePreservation.DiscardWhenLeaving || priorityVolume2.Preservation == VolumeBase.VolumePreservation.Preserve)
					{
						this.InterpolateToProfile(priorityVolume2.GetProfile(), priorityVolume2.Duration, priorityVolume2.LerpCurve);
						return;
					}
					this.FeatherToProfile(priorityVolume2.GetProfile(), priorityVolume2, priorityVolume2.LerpCurve);
					return;
				}
				else if (this.defaultProfile != null)
				{
					this.currentProfile = this.defaultProfile;
					this.previousProfile = null;
					this.activeVolume = null;
					this.SetProfileInstant(this.currentProfile);
				}
			}
		}
	}

	protected IEnumerator LinearInterpolation(float duration, AnimationCurve curve)
	{
		float durationTimer = 0f;
		if (duration > 0f)
		{
			while (durationTimer <= duration)
			{
				durationTimer += Time.deltaTime;
				durationTimer = Mathf.Clamp(durationTimer, 0f, duration);
				float lerpValue = curve.Evaluate(durationTimer / duration);
				this.Interpolate(lerpValue);
				yield return null;
			}
		}
		else
		{
			this.Interpolate(1f);
		}
		this.LinearInterpolationComplete();
		yield break;
	}

	protected IEnumerator DistanceBasedInterpolation(VolumeBase volume, AnimationCurve curve)
	{
		while (volume != null)
		{
			Vector3 size = volume.BoxCollider.bounds.size;
			Vector3 vector = volume.transform.TransformPoint(volume.BoxCollider.center);
			Ray ray = new Ray(vector, (base.transform.position - vector).normalized);
			ray.origin = ray.GetPoint(100f);
			ray.direction = -ray.direction;
			Debug.DrawRay(ray.origin, ray.direction.normalized * 100f, Color.red);
			RaycastHit raycastHit;
			if (volume.BoxCollider.Raycast(ray, out raycastHit, 100f))
			{
				Vector3 point = raycastHit.point;
				Vector3 vector2 = volume.FeatheredBounds.ClosestPoint(base.transform.position);
				float a = Vector3.Distance(vector2, point);
				float value = Vector3.Distance(vector2, base.transform.position);
				float num = Mathf.InverseLerp(a, 0f, value);
				Debug.DrawRay(vector2, Vector3.up, Color.cyan);
				Debug.DrawRay(point, Vector3.up, Color.yellow);
				this.Interpolate(curve.Evaluate(num));
				if (volume.Preservation == VolumeBase.VolumePreservation.FeatheredVolumePreserve && num >= 1f)
				{
					this.activeVolume = null;
					if (this.interpolationRoutine != null)
					{
						base.StopCoroutine(this.interpolationRoutine);
					}
					this.interpolationRoutine = null;
				}
			}
			yield return null;
		}
		yield break;
	}

	[SerializeField]
	protected ProfileBase defaultProfile;

	[SerializeField]
	[HideInInspector]
	protected ProfileBase previousProfile;

	[SerializeField]
	[HideInInspector]
	protected ProfileBase currentProfile;

	[SerializeField]
	[HideInInspector]
	protected List<VolumeBase> enteredVolumes;

	[SerializeField]
	[HideInInspector]
	protected VolumeBase activeVolume;

	protected Coroutine interpolationRoutine;
}
