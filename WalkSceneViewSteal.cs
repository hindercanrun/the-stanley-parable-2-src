using System;
using System.Collections;
using UnityEngine;

public class WalkSceneViewSteal : MonoBehaviour
{
	private void Start()
	{
	}

	public void StealView()
	{
		StanleyController.Instance.FreezeMotionAndView();
		if (this.stolen)
		{
			return;
		}
		this.stolen = true;
		this.anim.SetTrigger("WalkStart");
		StanleyController.Instance.currentCam.GetComponentInChildren<BucketController>();
		this.childCam = StanleyController.Instance.currentCam.gameObject;
		this.childCam.transform.position = StanleyController.Instance.currentCam.gameObject.transform.position;
		this.childCam.transform.rotation = StanleyController.Instance.currentCam.gameObject.transform.rotation;
		StanleyController.Instance.FreezeMotionAndView();
		base.StartCoroutine(this.SlowWalkingSpeedMultiplier(0.8f));
		base.StartCoroutine(this.LerpCamera());
	}

	private IEnumerator SlowWalkingSpeedMultiplier(float time)
	{
		float minMovementSpeed = 0.2f;
		float timeRemaining = time;
		while (timeRemaining > 0f)
		{
			float num = timeRemaining / time;
			num = Mathf.InverseLerp(minMovementSpeed, 1f, num);
			StanleyController.Instance.SetMovementSpeedMultiplier(1f / num);
			StanleyController.Instance.Bucket.SetWalkingSpeed(num);
			StanleyController.Instance.Bucket.SetAnimationSpeed(num);
			timeRemaining -= Singleton<GameMaster>.Instance.GameDeltaTime;
			yield return null;
		}
		float num2 = minMovementSpeed;
		StanleyController.Instance.SetMovementSpeedMultiplier(1f / num2);
		StanleyController.Instance.Bucket.SetWalkingSpeed(num2);
		StanleyController.Instance.Bucket.SetAnimationSpeed(num2);
		yield break;
	}

	private IEnumerator LerpCamera()
	{
		Vector3 startPos = this.childCam.transform.position;
		Quaternion startRot = this.childCam.transform.rotation;
		float startTime = Singleton<GameMaster>.Instance.GameTime;
		float endTime = startTime + 2.5f;
		while (Singleton<GameMaster>.Instance.GameTime < endTime)
		{
			float num = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			num *= num;
			this.childCam.transform.position = Vector3.Lerp(startPos, base.transform.TransformPoint(Vector3.zero), num);
			this.childCam.transform.rotation = Quaternion.Slerp(startRot, base.transform.rotation, num);
			yield return null;
		}
		base.StartCoroutine(this.FollowCam());
		yield break;
	}

	private IEnumerator FollowCam()
	{
		for (;;)
		{
			this.childCam.transform.position = base.transform.position;
			this.childCam.transform.rotation = base.transform.rotation;
			yield return null;
		}
		yield break;
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
	}

	public Animator anim;

	private bool stolen;

	private GameObject childCam;
}
