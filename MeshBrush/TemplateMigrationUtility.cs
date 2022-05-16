using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace MeshBrush
{
	public static class TemplateMigrationUtility
	{
		public static bool TryMigrate(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				Debug.LogError("MeshBrush: The specified template file path is invalid or doesn't exist! Cancelling migration process...");
				return false;
			}
			try
			{
				XContainer xcontainer = XDocument.Load(filePath);
				MeshBrush meshBrush = new GameObject("MeshBrush Template Migration Utility")
				{
					hideFlags = HideFlags.HideAndDontSave
				}.AddComponent<MeshBrush>();
				foreach (XElement xelement in xcontainer.Descendants())
				{
					string localName = xelement.Name.LocalName;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
					if (num <= 2300504609U)
					{
						if (num <= 1045735021U)
						{
							if (num <= 533921558U)
							{
								if (num <= 197295567U)
								{
									if (num != 32242498U)
									{
										if (num != 188229817U)
										{
											if (num != 197295567U)
											{
												continue;
											}
											if (!(localName == "minNrOfMeshes"))
											{
												continue;
											}
											meshBrush.quantityRange.x = float.Parse(xelement.Value);
											continue;
										}
										else
										{
											if (!(localName == "foldoutState_CustomizeKeyboardShortcuts"))
											{
												continue;
											}
											meshBrush.keyBindingsFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
											continue;
										}
									}
									else
									{
										if (!(localName == "slopeReferenceVector"))
										{
											continue;
										}
										meshBrush.slopeReferenceVector = new Vector3(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value), float.Parse(xelement.Element("z").Value));
										continue;
									}
								}
								else if (num != 247161650U)
								{
									if (num != 512611722U)
									{
										if (num != 533921558U)
										{
											continue;
										}
										if (!(localName == "meshBrushTemplate"))
										{
											continue;
										}
										XAttribute xattribute = xelement.Attribute("version");
										if (xattribute != null && 2f <= float.Parse(xattribute.Value))
										{
											Debug.LogWarning("MeshBrush: The template you tried to migrate actually is already up to date with the current format. Cancelling process...");
											return false;
										}
										continue;
									}
									else
									{
										if (!(localName == "paintKey"))
										{
											continue;
										}
										goto IL_9F7;
									}
								}
								else if (!(localName == "brushColor"))
								{
									continue;
								}
							}
							else if (num <= 766757029U)
							{
								if (num != 646499401U)
								{
									if (num != 654835894U)
									{
										if (num != 766757029U)
										{
											continue;
										}
										if (!(localName == "maxSlopeFilterAngle"))
										{
											continue;
										}
										float num2 = float.Parse(xelement.Value);
										meshBrush.angleThresholdRange = new Vector2(num2, num2);
										continue;
									}
									else
									{
										if (!(localName == "combineAreaKey"))
										{
											continue;
										}
										meshBrush.combineKey = (KeyCode)Enum.Parse(typeof(KeyCode), xelement.Value);
										continue;
									}
								}
								else
								{
									if (!(localName == "foldoutState_BrushSettings"))
									{
										continue;
									}
									meshBrush.brushFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
							}
							else if (num <= 1033172063U)
							{
								if (num != 1031692888U)
								{
									if (num != 1033172063U)
									{
										continue;
									}
									if (!(localName == "isActive"))
									{
										continue;
									}
									goto IL_900;
								}
								else if (!(localName == "color"))
								{
									continue;
								}
							}
							else if (num != 1045555937U)
							{
								if (num != 1045735021U)
								{
									continue;
								}
								if (!(localName == "useSlopeFilter"))
								{
									continue;
								}
								meshBrush.useSlopeFilter = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
							else
							{
								if (!(localName == "deleteKey"))
								{
									continue;
								}
								meshBrush.deleteKey = (KeyCode)Enum.Parse(typeof(KeyCode), xelement.Value);
								continue;
							}
							meshBrush.color = new Color(float.Parse(xelement.Element("r").Value), float.Parse(xelement.Element("g").Value), float.Parse(xelement.Element("b").Value), float.Parse(xelement.Element("a").Value));
							continue;
						}
						if (num <= 1401802983U)
						{
							if (num <= 1228985680U)
							{
								if (num != 1085558803U)
								{
									if (num != 1150645771U)
									{
										if (num != 1228985680U)
										{
											continue;
										}
										if (!(localName == "foldoutState_Slopes"))
										{
											continue;
										}
										meshBrush.slopesFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
										continue;
									}
									else
									{
										if (!(localName == "constantUniformScale"))
										{
											continue;
										}
										meshBrush.uniformAdditiveScale = (string.CompareOrdinal(xelement.Value, "true") == 0);
										continue;
									}
								}
								else
								{
									if (!(localName == "foldoutState_Optimize"))
									{
										continue;
									}
									meshBrush.optimizationFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
							}
							else if (num <= 1322381784U)
							{
								if (num != 1243200148U)
								{
									if (num != 1322381784U)
									{
										continue;
									}
									if (!(localName == "delay"))
									{
										continue;
									}
									meshBrush.delay = float.Parse(xelement.Value);
									continue;
								}
								else
								{
									if (!(localName == "autoStatic"))
									{
										continue;
									}
									meshBrush.autoStatic = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
							}
							else if (num != 1339988800U)
							{
								if (num != 1401802983U)
								{
									continue;
								}
								if (!(localName == "groupName"))
								{
									continue;
								}
								meshBrush.groupName = xelement.Value;
								continue;
							}
							else
							{
								if (!(localName == "autoSelectOnCombine"))
								{
									continue;
								}
								meshBrush.autoSelectOnCombine = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num <= 2045852897U)
						{
							if (num != 1571318602U)
							{
								if (num != 1966575161U)
								{
									if (num != 2045852897U)
									{
										continue;
									}
									if (!(localName == "classicUI"))
									{
										continue;
									}
									meshBrush.classicUI = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
								else
								{
									if (!(localName == "trisCounter"))
									{
										continue;
									}
									meshBrush.stats = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
							}
							else
							{
								if (!(localName == "showReferenceVectorInSceneGUI"))
								{
									continue;
								}
								meshBrush.showReferenceVectorInSceneView = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num <= 2175299696U)
						{
							if (num != 2156046578U)
							{
								if (num != 2175299696U)
								{
									continue;
								}
								if (!(localName == "randomRotation"))
								{
									continue;
								}
								float num3 = float.Parse(xelement.Value);
								meshBrush.randomRotationRangeY = new Vector2(num3, num3);
								continue;
							}
							else
							{
								if (!(localName == "slopeReferenceVector_HandleLocation"))
								{
									continue;
								}
								meshBrush.slopeReferenceVectorSampleLocation = new Vector3(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value), float.Parse(xelement.Element("z").Value));
								continue;
							}
						}
						else if (num != 2220598988U)
						{
							if (num != 2300504609U)
							{
								continue;
							}
							if (!(localName == "maxNrOfMeshes"))
							{
								continue;
							}
							meshBrush.quantityRange.y = float.Parse(xelement.Value);
							continue;
						}
						else
						{
							if (!(localName == "inverseSlopeFilter"))
							{
								continue;
							}
							meshBrush.inverseSlopeFilter = (string.CompareOrdinal(xelement.Value, "true") == 0);
							continue;
						}
					}
					else if (num <= 3147311985U)
					{
						if (num <= 2844007268U)
						{
							if (num <= 2680778662U)
							{
								if (num != 2320977079U)
								{
									if (num != 2354551511U)
									{
										if (num != 2680778662U)
										{
											continue;
										}
										if (!(localName == "decreaseRadiusKey"))
										{
											continue;
										}
										meshBrush.decreaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), xelement.Value);
										continue;
									}
									else
									{
										if (!(localName == "previewIconSize"))
										{
											continue;
										}
										meshBrush.previewIconSize = float.Parse(xelement.Value);
										continue;
									}
								}
								else
								{
									if (!(localName == "slopeInfluence"))
									{
										continue;
									}
									float num4 = float.Parse(xelement.Value);
									meshBrush.slopeInfluenceRange = new Vector2(num4, num4);
									continue;
								}
							}
							else if (num <= 2816968833U)
							{
								if (num != 2799640479U)
								{
									if (num != 2816968833U)
									{
										continue;
									}
									if (!(localName == "foldoutState_OverlapFilter"))
									{
										continue;
									}
									meshBrush.overlapFilterFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
								else
								{
									if (!(localName == "scattering"))
									{
										continue;
									}
									float num5 = float.Parse(xelement.Value);
									meshBrush.scatteringRange = new Vector2(num5, num5);
									continue;
								}
							}
							else if (num != 2821424375U)
							{
								if (num != 2844007268U)
								{
									continue;
								}
								if (!(localName == "constantScaleXYZ"))
								{
									continue;
								}
								meshBrush.additiveScaleNonUniform = new Vector3(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value), float.Parse(xelement.Element("z").Value));
								continue;
							}
							else
							{
								if (!(localName == "foldoutState_Templates"))
								{
									continue;
								}
								meshBrush.templatesFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num <= 2956398272U)
						{
							if (num != 2863602374U)
							{
								if (num != 2939628600U)
								{
									if (num != 2956398272U)
									{
										continue;
									}
									if (!(localName == "foldoutState_Randomizers"))
									{
										continue;
									}
									meshBrush.randomizersFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
								else
								{
									if (!(localName == "verticalOffset"))
									{
										continue;
									}
									float num6 = float.Parse(xelement.Value);
									meshBrush.offsetRange = new Vector2(num6, num6);
									continue;
								}
							}
							else
							{
								if (!(localName == "randomAbsMinDist"))
								{
									continue;
								}
								meshBrush.minimumAbsoluteDistanceRange = new Vector2(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value));
								continue;
							}
						}
						else if (num <= 3018570096U)
						{
							if (num != 3000518203U)
							{
								if (num != 3018570096U)
								{
									continue;
								}
								if (!(localName == "maxMeshDensity"))
								{
									continue;
								}
								meshBrush.densityRange.y = float.Parse(xelement.Value);
								continue;
							}
							else
							{
								if (!(localName == "lockSceneView"))
								{
									continue;
								}
								meshBrush.lockSceneView = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num != 3030791712U)
						{
							if (num != 3147311985U)
							{
								continue;
							}
							if (!(localName == "constantAdditiveScale"))
							{
								continue;
							}
							float num7 = float.Parse(xelement.Value);
							meshBrush.additiveScaleRange = new Vector2(num7, num7);
							continue;
						}
						else
						{
							if (!(localName == "randomNonUniformRange"))
							{
								continue;
							}
							meshBrush.randomScaleRangeX = (meshBrush.randomScaleRangeZ = new Vector2(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value)));
							meshBrush.randomScaleRangeY = new Vector2(float.Parse(xelement.Element("z").Value), float.Parse(xelement.Element("w").Value));
							continue;
						}
					}
					else if (num <= 3727491022U)
					{
						if (num <= 3349087890U)
						{
							if (num != 3200568011U)
							{
								if (num != 3332095858U)
								{
									if (num != 3349087890U)
									{
										continue;
									}
									if (!(localName == "yAxisIsTangent"))
									{
										continue;
									}
									meshBrush.yAxisTangent = (string.CompareOrdinal(xelement.Value, "true") == 0);
									continue;
								}
								else
								{
									if (!(localName == "increaseRadiusKey"))
									{
										continue;
									}
									meshBrush.increaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), xelement.Value);
									continue;
								}
							}
							else
							{
								if (!(localName == "uniformScale"))
								{
									continue;
								}
								meshBrush.uniformRandomScale = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num <= 3583504769U)
						{
							if (num != 3379073623U)
							{
								if (num != 3583504769U)
								{
									continue;
								}
								if (!(localName == "useOverlapFilter"))
								{
									continue;
								}
								meshBrush.useOverlapFilter = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
							else
							{
								if (!(localName == "useMeshDensity"))
								{
									continue;
								}
								meshBrush.useDensity = (string.CompareOrdinal(xelement.Value, "true") == 0);
								continue;
							}
						}
						else if (num != 3648362799U)
						{
							if (num != 3727491022U)
							{
								continue;
							}
							if (!(localName == "manualReferenceVectorSampling"))
							{
								continue;
							}
							meshBrush.manualReferenceVectorSampling = (string.CompareOrdinal(xelement.Value, "true") == 0);
							continue;
						}
						else if (!(localName == "active"))
						{
							continue;
						}
					}
					else if (num <= 4004139265U)
					{
						if (num != 3886702991U)
						{
							if (num != 3925186208U)
							{
								if (num != 4004139265U)
								{
									continue;
								}
								if (!(localName == "brushRadius"))
								{
									continue;
								}
								meshBrush.radius = float.Parse(xelement.Value);
								continue;
							}
							else
							{
								if (!(localName == "globalPaintingLayers"))
								{
									continue;
								}
								int num8 = 0;
								using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										XElement xelement2 = enumerator2.Current;
										meshBrush.layerMask[num8] = (string.CompareOrdinal(xelement2.Value, "false") != 0);
										num8++;
									}
									continue;
								}
								goto IL_9F7;
							}
						}
						else
						{
							if (!(localName == "foldoutState_SetOfMeshesToPaint"))
							{
								continue;
							}
							meshBrush.meshesFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
							continue;
						}
					}
					else if (num <= 4117738702U)
					{
						if (num != 4114130774U)
						{
							if (num != 4117738702U)
							{
								continue;
							}
							if (!(localName == "minMeshDensity"))
							{
								continue;
							}
							meshBrush.densityRange.x = float.Parse(xelement.Value);
							continue;
						}
						else
						{
							if (!(localName == "alignWithStroke"))
							{
								continue;
							}
							meshBrush.strokeAlignment = (string.CompareOrdinal(xelement.Value, "true") == 0);
							continue;
						}
					}
					else if (num != 4252346987U)
					{
						if (num != 4263603942U)
						{
							continue;
						}
						if (!(localName == "foldoutState_ApplyAdditiveScale"))
						{
							continue;
						}
						meshBrush.additiveScaleFoldout = (string.CompareOrdinal(xelement.Value, "true") == 0);
						continue;
					}
					else
					{
						if (!(localName == "randomUniformRange"))
						{
							continue;
						}
						meshBrush.randomScaleRange = new Vector2(float.Parse(xelement.Element("x").Value), float.Parse(xelement.Element("y").Value));
						continue;
					}
					IL_900:
					meshBrush.active = (string.CompareOrdinal(xelement.Value, "true") == 0);
					continue;
					IL_9F7:
					meshBrush.paintKey = (KeyCode)Enum.Parse(typeof(KeyCode), xelement.Value);
				}
				meshBrush.SaveTemplate(filePath.Replace(".meshbrush", "__migrated.xml"));
			}
			catch (Exception ex)
			{
				Debug.LogError("MeshBrush: Failed to migrate template file \"" + filePath + "\". Perhaps the file is corrupted? " + ex.ToString());
				return false;
			}
			return true;
		}
	}
}
