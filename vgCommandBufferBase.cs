using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public abstract class vgCommandBufferBase : MonoBehaviour
{
	protected abstract void RefreshCommandBufferInfo(CommandBuffer buf, Camera cam);

	protected abstract string GetPassCommandBufferName();

	protected abstract CameraEvent GetPassCameraEvent();

	protected abstract int GetPassSortingIndex();

	public virtual void VerifyResources()
	{
		if (vgCommandBufferBase.fullScreenQuadMesh == null)
		{
			vgCommandBufferBase.fullScreenQuadMesh = new Mesh();
			Vector3[] vertices = new Vector3[]
			{
				new Vector3(-1f, -1f, 3f),
				new Vector3(1f, -1f, 2f),
				new Vector3(1f, 1f, 1f),
				new Vector3(-1f, 1f, 0f)
			};
			Vector2[] uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			int[] triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			};
			vgCommandBufferBase.fullScreenQuadMesh.vertices = vertices;
			vgCommandBufferBase.fullScreenQuadMesh.uv = uv;
			vgCommandBufferBase.fullScreenQuadMesh.triangles = triangles;
		}
	}

	public void AddToCommandListIfNeededAndSort(vgCommandBufferBase.CommandBuffersAndEvents cbce)
	{
		for (int i = 0; i < vgCommandBufferBase.cameraCbs.Count; i++)
		{
			if (cbce.cb.name.Equals(vgCommandBufferBase.cameraCbs[i].cb.name))
			{
				return;
			}
		}
		if (vgCommandBufferBase.mainCamera)
		{
			vgCommandBufferBase.mainCamera.RemoveAllCommandBuffers();
		}
		vgCommandBufferBase.cameraCbs.Add(cbce);
		vgCommandBufferBase.cameraCbs.Sort(new vgCommandBufferBase.CBSorter());
		if (vgCommandBufferBase.mainCamera)
		{
			for (int j = 0; j < vgCommandBufferBase.cameraCbs.Count; j++)
			{
				vgCommandBufferBase.mainCamera.AddCommandBuffer(vgCommandBufferBase.cameraCbs[j].ce, vgCommandBufferBase.cameraCbs[j].cb);
			}
		}
	}

	public void OnDisable()
	{
		if (!vgCommandBufferBase.mainCamera)
		{
			return;
		}
		string passCommandBufferName = this.GetPassCommandBufferName();
		int num = -1;
		int num2 = 0;
		while (num2 < vgCommandBufferBase.cameraCbs.Count && num == -1)
		{
			if (vgCommandBufferBase.cameraCbs[num2].cb.name.Equals(passCommandBufferName))
			{
				num = num2;
			}
			num2++;
		}
		if (num == -1)
		{
			return;
		}
		CameraEvent ce = vgCommandBufferBase.cameraCbs[num].ce;
		CommandBuffer[] commandBuffers = vgCommandBufferBase.mainCamera.GetCommandBuffers(ce);
		for (int i = 0; i < commandBuffers.Length; i++)
		{
			if (commandBuffers[i].name.Equals(passCommandBufferName))
			{
				vgCommandBufferBase.mainCamera.RemoveCommandBuffer(ce, commandBuffers[i]);
			}
		}
		vgCommandBufferBase.cameraCbs.RemoveAt(num);
	}

	public void OnEnable()
	{
		if (vgCommandBufferBase.cameraCbs == null)
		{
			vgCommandBufferBase.cameraCbs = new List<vgCommandBufferBase.CommandBuffersAndEvents>();
		}
		vgCommandBufferBase.mainCamera = base.GetComponent<Camera>();
		this.AddToCommandListIfNeededAndSort(new vgCommandBufferBase.CommandBuffersAndEvents
		{
			cb = new CommandBuffer(),
			cb = 
			{
				name = this.GetPassCommandBufferName()
			},
			ce = this.GetPassCameraEvent(),
			index = this.GetPassSortingIndex()
		});
	}

	private void AddCommandBufferToCameraIfNeeded(vgCommandBufferBase.CommandBuffersAndEvents cbce)
	{
		if (!vgCommandBufferBase.mainCamera)
		{
			return;
		}
		if (!cbce.addedToCamera)
		{
			cbce.addedToCamera = true;
			CommandBuffer[] commandBuffers = vgCommandBufferBase.mainCamera.GetCommandBuffers(cbce.ce);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name.Equals(cbce.cb.name))
				{
					return;
				}
			}
			vgCommandBufferBase.mainCamera.AddCommandBuffer(cbce.ce, cbce.cb);
		}
	}

	private void Update()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			this.OnDisable();
			return;
		}
		if (!vgCommandBufferBase.mainCamera)
		{
			return;
		}
		for (int i = 0; i < vgCommandBufferBase.cameraCbs.Count; i++)
		{
			if (vgCommandBufferBase.cameraCbs[i].cb.name.Equals(this.GetPassCommandBufferName()))
			{
				vgCommandBufferBase.cameraCbs[i].cb.Clear();
				this.RefreshCommandBufferInfo(vgCommandBufferBase.cameraCbs[i].cb, vgCommandBufferBase.mainCamera);
				this.AddCommandBufferToCameraIfNeeded(vgCommandBufferBase.cameraCbs[i]);
			}
		}
	}

	protected static Mesh fullScreenQuadMesh;

	protected static List<vgCommandBufferBase.CommandBuffersAndEvents> cameraCbs;

	protected static Camera mainCamera;

	public class CommandBuffersAndEvents
	{
		public CommandBuffer cb;

		public CameraEvent ce;

		public bool addedToCamera;

		public int index;
	}

	public class CBSorter : IComparer<vgCommandBufferBase.CommandBuffersAndEvents>
	{
		public int Compare(vgCommandBufferBase.CommandBuffersAndEvents x, vgCommandBufferBase.CommandBuffersAndEvents y)
		{
			return x.index - y.index;
		}
	}
}
