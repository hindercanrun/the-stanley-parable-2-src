using System;
using UnityEngine;
using UnityEngine.UI;

namespace MeshBrush.Examples
{
	public class RuntimeExample : MonoBehaviour
	{
		private void Start()
		{
			this.mainCamera = Camera.main.transform;
		}

		private void Update()
		{
			this.meshbrushInstance.radius = this.radiusSlider.value;
			this.meshbrushInstance.scatteringRange = new Vector2(this.scatteringSlider.value, this.scatteringSlider.value);
			this.meshbrushInstance.densityRange = new Vector2(this.densitySlider.value, this.densitySlider.value);
			RaycastHit brushLocation;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out brushLocation))
			{
				this.circleBrush.position = brushLocation.point;
				this.circleBrush.forward = -brushLocation.normal;
				this.circleBrush.localScale = new Vector3(this.meshbrushInstance.radius, this.meshbrushInstance.radius, 1f);
				if (Input.GetKey(this.meshbrushInstance.paintKey))
				{
					this.meshbrushInstance.PaintMeshes(brushLocation);
				}
				if (Input.GetKey(this.meshbrushInstance.deleteKey))
				{
					this.meshbrushInstance.DeleteMeshes(brushLocation);
				}
				if (Input.GetKey(this.meshbrushInstance.randomizeKey))
				{
					this.meshbrushInstance.RandomizeMeshes(brushLocation);
				}
			}
		}

		[SerializeField]
		private MeshBrush meshbrushInstance;

		[SerializeField]
		private Transform circleBrush;

		[SerializeField]
		private Slider radiusSlider;

		[SerializeField]
		private Slider scatteringSlider;

		[SerializeField]
		private Slider densitySlider;

		private Transform mainCamera;
	}
}
