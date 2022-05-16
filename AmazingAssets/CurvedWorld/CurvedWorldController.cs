using System;
using UnityEngine;

namespace AmazingAssets.CurvedWorld
{
	[ExecuteAlways]
	public class CurvedWorldController : MonoBehaviour
	{
		private void OnDisable()
		{
			this.DisableBend();
		}

		private void OnDestroy()
		{
			this.DisableBend();
		}

		private void OnEnable()
		{
			this.EnableBend();
		}

		private void Start()
		{
			this.GenerateShaderPropertyIDs();
		}

		private void Update()
		{
			if (this.manualUpdate)
			{
				return;
			}
			this.UpdateShaderdata();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			BEND_TYPE bend_TYPE = this.bendType;
			if (bend_TYPE - BEND_TYPE.TwistedSpiral_X_Positive <= 3)
			{
				Gizmos.DrawRay(this.bendPivotPointPosition, this.bendRotationAxis.normalized * 10f);
			}
		}

		private void UpdateShaderdata()
		{
			this.CheckBendChanging();
			if (base.isActiveAndEnabled)
			{
				if (this.disableInEditor && Application.isEditor && !Application.isPlaying)
				{
					this.UpdateShaderDataDisabled();
					return;
				}
				if (this.bendPivotPoint != null)
				{
					this.bendPivotPointPosition = this.bendPivotPoint.transform.position;
				}
				if (this.bendRotationCenter != null)
				{
					this.bendRotationCenterPosition = this.bendRotationCenter.position;
				}
				if (this.bendRotationCenter2 != null)
				{
					this.bendRotationCenter2Position = this.bendRotationCenter2.position;
				}
				switch (this.bendType)
				{
				case BEND_TYPE.ClassicRunner_X_Positive:
				case BEND_TYPE.ClassicRunner_X_Negative:
				case BEND_TYPE.ClassicRunner_Z_Positive:
				case BEND_TYPE.ClassicRunner_Z_Negative:
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalVector(this.materialPropertyID_BendSize, new Vector2(this.bendVerticalSize, this.bendHorizontalSize));
					Shader.SetGlobalVector(this.materialPropertyID_BendOffset, new Vector2(this.bendVerticalOffset, this.bendHorizontalOffset));
					return;
				case BEND_TYPE.LittlePlanet_X:
				case BEND_TYPE.LittlePlanet_Y:
				case BEND_TYPE.LittlePlanet_Z:
				case BEND_TYPE.CylindricalTower_X:
				case BEND_TYPE.CylindricalTower_Z:
				case BEND_TYPE.CylindricalRolloff_X:
				case BEND_TYPE.CylindricalRolloff_Z:
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalFloat(this.materialPropertyID_BendSize, this.bendCurvatureSize);
					Shader.SetGlobalFloat(this.materialPropertyID_BendOffset, this.bendCurvatureOffset);
					return;
				case BEND_TYPE.SpiralHorizontal_X_Positive:
				case BEND_TYPE.SpiralHorizontal_X_Negative:
				case BEND_TYPE.SpiralHorizontal_Z_Positive:
				case BEND_TYPE.SpiralHorizontal_Z_Negative:
				case BEND_TYPE.SpiralVertical_X_Positive:
				case BEND_TYPE.SpiralVertical_X_Negative:
				case BEND_TYPE.SpiralVertical_Z_Positive:
				case BEND_TYPE.SpiralVertical_Z_Negative:
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalVector(this.materialPropertyID_RotationCenter, this.bendRotationCenterPosition);
					Shader.SetGlobalFloat(this.materialPropertyID_BendAngle, this.bendAngle);
					Shader.SetGlobalFloat(this.materialPropertyID_BendMinimumRadius, this.bendMinimumRadius);
					return;
				case BEND_TYPE.SpiralHorizontalDouble_X:
				case BEND_TYPE.SpiralHorizontalDouble_Z:
				case BEND_TYPE.SpiralVerticalDouble_X:
				case BEND_TYPE.SpiralVerticalDouble_Z:
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalVector(this.materialPropertyID_RotationCenter, this.bendRotationCenterPosition);
					Shader.SetGlobalVector(this.materialPropertyID_RotationCenter2, this.bendRotationCenter2Position);
					Shader.SetGlobalVector(this.materialPropertyID_BendAngle, new Vector2(this.bendAngle, this.bendAngle2));
					Shader.SetGlobalVector(this.materialPropertyID_BendMinimumRadius, new Vector2(this.bendMinimumRadius, this.bendMinimumRadius2));
					return;
				case BEND_TYPE.SpiralHorizontalRolloff_X:
				case BEND_TYPE.SpiralHorizontalRolloff_Z:
				case BEND_TYPE.SpiralVerticalRolloff_X:
				case BEND_TYPE.SpiralVerticalRolloff_Z:
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalVector(this.materialPropertyID_RotationCenter, this.bendRotationCenterPosition);
					Shader.SetGlobalFloat(this.materialPropertyID_BendAngle, this.bendAngle);
					Shader.SetGlobalFloat(this.materialPropertyID_BendMinimumRadius, this.bendMinimumRadius);
					Shader.SetGlobalFloat(this.materialPropertyID_BendRolloff, this.bendRolloff);
					return;
				case BEND_TYPE.TwistedSpiral_X_Positive:
				case BEND_TYPE.TwistedSpiral_X_Negative:
				case BEND_TYPE.TwistedSpiral_Z_Positive:
				case BEND_TYPE.TwistedSpiral_Z_Negative:
					switch (this.bendRotationAxisType)
					{
					case CurvedWorldController.AXIS_TYPE.Transform:
						if (this.bendPivotPoint == null)
						{
							switch (this.bendType)
							{
							case BEND_TYPE.ClassicRunner_X_Positive:
								this.bendRotationAxis = Vector3.left;
								break;
							case BEND_TYPE.ClassicRunner_X_Negative:
								this.bendRotationAxis = Vector3.right;
								break;
							case BEND_TYPE.ClassicRunner_Z_Positive:
								this.bendRotationAxis = Vector3.back;
								break;
							case BEND_TYPE.ClassicRunner_Z_Negative:
								this.bendRotationAxis = Vector3.forward;
								break;
							}
						}
						else
						{
							this.bendRotationAxis = this.bendPivotPoint.forward;
						}
						break;
					case CurvedWorldController.AXIS_TYPE.CustomNormalized:
						this.bendRotationAxis = this.bendRotationAxis.normalized;
						break;
					}
					Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, this.bendPivotPointPosition);
					Shader.SetGlobalVector(this.materialPropertyID_RotationAxis, this.bendRotationAxis);
					Shader.SetGlobalVector(this.materialPropertyID_BendSize, new Vector3(this.bendCurvatureSize, this.bendVerticalSize, this.bendHorizontalSize));
					Shader.SetGlobalVector(this.materialPropertyID_BendOffset, new Vector3(this.bendCurvatureOffset, this.bendVerticalOffset, this.bendHorizontalOffset));
					break;
				default:
					return;
				}
			}
		}

		private void UpdateShaderDataDisabled()
		{
			Shader.SetGlobalVector(this.materialPropertyID_PivotPoint, Vector3.zero);
			Shader.SetGlobalVector(this.materialPropertyID_RotationCenter, Vector3.zero);
			Shader.SetGlobalVector(this.materialPropertyID_RotationCenter2, Vector3.zero);
			Shader.SetGlobalVector(this.materialPropertyID_RotationAxis, Vector3.zero);
			Shader.SetGlobalVector(this.materialPropertyID_BendSize, Vector3.zero);
			Shader.SetGlobalFloat(this.materialPropertyID_BendSize, 0f);
			Shader.SetGlobalVector(this.materialPropertyID_BendOffset, Vector3.zero);
			Shader.SetGlobalFloat(this.materialPropertyID_BendOffset, 0f);
			Shader.SetGlobalVector(this.materialPropertyID_BendAngle, Vector2.zero);
			Shader.SetGlobalFloat(this.materialPropertyID_BendAngle, 0f);
			Shader.SetGlobalVector(this.materialPropertyID_BendMinimumRadius, Vector2.zero);
			Shader.SetGlobalFloat(this.materialPropertyID_BendMinimumRadius, 0f);
			Shader.SetGlobalFloat(this.materialPropertyID_BendRolloff, 10f);
		}

		public void DisableBend()
		{
			this.GenerateShaderPropertyIDs();
			this.UpdateShaderDataDisabled();
		}

		public void EnableBend()
		{
			this.GenerateShaderPropertyIDs();
			this.previousBentType = this.bendType;
			this.previousID = this.bendID;
			this.UpdateShaderdata();
		}

		public void ManualUpdate()
		{
			this.UpdateShaderdata();
		}

		private void CheckBendChanging()
		{
			if (this.previousBentType != this.bendType || this.previousID != this.bendID)
			{
				this.DisableBend();
				this.previousBentType = this.bendType;
				if (this.bendID < 1)
				{
					this.bendID = 1;
				}
				this.previousID = this.bendID;
				this.GenerateShaderPropertyIDs();
			}
		}

		private void GenerateShaderPropertyIDs()
		{
			this.materialPropertyID_PivotPoint = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_PivotPoint", this.bendType, this.bendID));
			this.materialPropertyID_RotationCenter = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_RotationCenter", this.bendType, this.bendID));
			this.materialPropertyID_RotationCenter2 = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_RotationCenter2", this.bendType, this.bendID));
			this.materialPropertyID_RotationAxis = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_RotationAxis", this.bendType, this.bendID));
			this.materialPropertyID_BendSize = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_BendSize", this.bendType, this.bendID));
			this.materialPropertyID_BendOffset = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_BendOffset", this.bendType, this.bendID));
			this.materialPropertyID_BendAngle = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_BendAngle", this.bendType, this.bendID));
			this.materialPropertyID_BendMinimumRadius = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_BendMinimumRadius", this.bendType, this.bendID));
			this.materialPropertyID_BendRolloff = Shader.PropertyToID(string.Format("CurvedWorld_{0}_ID{1}_BendRolloff", this.bendType, this.bendID));
		}

		public Vector3 TransformPosition(Vector3 vertex)
		{
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				return vertex;
			}
			return CurvedWorldUtilities.TransformPosition(vertex, this);
		}

		public Quaternion TransformRotation(Vector3 vertex, Vector3 forwardVector, Vector3 rightVector)
		{
			Vector3 b = this.TransformPosition(vertex);
			Vector3 a = this.TransformPosition(vertex + forwardVector);
			Vector3 a2 = this.TransformPosition(vertex + rightVector);
			Vector3 vector = Vector3.Normalize(a - b);
			Vector3 rhs = Vector3.Normalize(a2 - b);
			Vector3 upwards = Vector3.Cross(vector, rhs);
			if (vector.sqrMagnitude < 0.01f && upwards.sqrMagnitude < 0.01f)
			{
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(vector, upwards);
		}

		public void SetBendVerticalSize(float value)
		{
			this.bendVerticalSize = value;
		}

		public void SetBendVerticalOffset(float value)
		{
			this.bendVerticalOffset = value;
		}

		public void SetBendHorizontalSize(float value)
		{
			this.bendHorizontalSize = value;
		}

		public void SetBendHorizontalOffset(float value)
		{
			this.bendHorizontalOffset = value;
		}

		public void SetBendCurvatureSize(float value)
		{
			this.bendCurvatureSize = value;
		}

		public void SetBendCurvatureOffset(float value)
		{
			this.bendCurvatureOffset = value;
		}

		public void SetBendAngle(float value)
		{
			this.bendAngle = value;
		}

		public void SetBendAngle2(float value)
		{
			this.bendAngle2 = value;
		}

		public void SetBendMinimumRadius(float value)
		{
			this.bendMinimumRadius = value;
		}

		public void SetBendMinimumRadius2(float value)
		{
			this.bendMinimumRadius2 = value;
		}

		public void SetBendRolloff(float value)
		{
			this.bendRolloff = value;
		}

		public BEND_TYPE bendType;

		[Range(1f, 32f)]
		public int bendID = 1;

		public Transform bendPivotPoint;

		public Vector3 bendPivotPointPosition;

		public Transform bendRotationCenter;

		public Vector3 bendRotationCenterPosition;

		public Vector3 bendRotationAxis;

		public CurvedWorldController.AXIS_TYPE bendRotationAxisType;

		public Transform bendRotationCenter2;

		public Vector3 bendRotationCenter2Position;

		public float bendVerticalSize;

		public float bendVerticalOffset;

		public float bendHorizontalSize;

		public float bendHorizontalOffset;

		public float bendCurvatureSize;

		public float bendCurvatureOffset;

		public float bendAngle;

		public float bendAngle2;

		public float bendMinimumRadius;

		public float bendMinimumRadius2;

		public float bendRolloff;

		public bool disableInEditor;

		public bool manualUpdate;

		private BEND_TYPE previousBentType;

		private int previousID;

		private int materialPropertyID_PivotPoint;

		private int materialPropertyID_RotationCenter;

		private int materialPropertyID_RotationCenter2;

		private int materialPropertyID_RotationAxis;

		private int materialPropertyID_BendSize;

		private int materialPropertyID_BendOffset;

		private int materialPropertyID_BendAngle;

		private int materialPropertyID_BendMinimumRadius;

		private int materialPropertyID_BendRolloff;

		public enum AXIS_TYPE
		{
			Transform,
			Custom,
			CustomNormalized
		}
	}
}
