using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformParent : MonoBehaviour
{
	private void Awake()
	{
		this._body = base.GetComponent<Rigidbody>();
		this._collider = base.GetComponent<Collider>();
	}

	private void Start()
	{
		if (this.forceStanleyTriggerEnterOnSceneReady_HACK != null)
		{
			base.StartCoroutine(this.PerformStanleyMoveHack());
		}
		if (this.forceParentingOnStart)
		{
			StanleyController.Instance.ParentTo(base.transform);
		}
	}

	private IEnumerator PerformStanleyMoveHack()
	{
		if (this.forceStanleyTriggerEnterOnSceneReady_HACK != null)
		{
			if (this.performExtraStanleyMoveOnReady_HACK)
			{
				StanleyController.Instance.transform.position = new Vector3(-1000f, -1000f, -1000f);
				yield return null;
			}
			StanleyController.Instance.transform.position = this.forceStanleyTriggerEnterOnSceneReady_HACK.position;
		}
		yield break;
	}

	public void SetForceParent(bool value)
	{
		this.forceParent = value;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.touchingColliders.Contains(col))
		{
			return;
		}
		this.touchingColliders.Add(col);
		StanleyController.Instance.transform.parent = base.transform;
	}

	private void OnTriggerStay(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.touchingColliders.Contains(col))
		{
			return;
		}
		this.touchingColliders.Add(col);
		StanleyController.Instance.ParentTo(base.transform);
	}

	private void OnTriggerExit(Collider col)
	{
		if (this.forceParent)
		{
			return;
		}
		if (this.touchingColliders.Contains(col))
		{
			this.touchingColliders.Remove(col);
			StanleyController.Instance.Deparent(false);
		}
	}

	public void Stop()
	{
		StanleyController.Instance.Deparent(false);
		this.touchingColliders = new List<Collider>();
		base.gameObject.SetActive(false);
	}

	public void GoAway()
	{
		StanleyController.Instance.Deparent(false);
		this.touchingColliders = new List<Collider>();
	}

	[SerializeField]
	private bool forceParent;

	[Header("Moves stanley to this transform on scene ready iff set")]
	[SerializeField]
	private Transform forceStanleyTriggerEnterOnSceneReady_HACK;

	[SerializeField]
	private bool performExtraStanleyMoveOnReady_HACK;

	private List<Collider> touchingColliders = new List<Collider>();

	private Rigidbody _body;

	private Collider _collider;

	[SerializeField]
	public bool forceParentingOnStart;
}
