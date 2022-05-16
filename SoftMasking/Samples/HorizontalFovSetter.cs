using System;
using UnityEngine;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(Camera))]
	public class HorizontalFovSetter : MonoBehaviour
	{
		public void Awake()
		{
			this._camera = base.GetComponent<Camera>();
		}

		public void Update()
		{
			this._camera.fieldOfView = this.horizontalFov / this._camera.aspect;
		}

		public float horizontalFov;

		private Camera _camera;
	}
}
