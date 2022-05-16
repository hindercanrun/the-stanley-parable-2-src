using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB2_TexturePacker
	{
		private static void printTree(MB2_TexturePacker.Node r, string spc)
		{
			Debug.Log(string.Concat(new object[]
			{
				spc,
				"Nd img=",
				(r.img != null).ToString(),
				" r=",
				r.r
			}));
			if (r.child[0] != null)
			{
				MB2_TexturePacker.printTree(r.child[0], spc + "      ");
			}
			if (r.child[1] != null)
			{
				MB2_TexturePacker.printTree(r.child[1], spc + "      ");
			}
		}

		private static void flattenTree(MB2_TexturePacker.Node r, List<MB2_TexturePacker.Image> putHere)
		{
			if (r.img != null)
			{
				r.img.x = r.r.x;
				r.img.y = r.r.y;
				putHere.Add(r.img);
			}
			if (r.child[0] != null)
			{
				MB2_TexturePacker.flattenTree(r.child[0], putHere);
			}
			if (r.child[1] != null)
			{
				MB2_TexturePacker.flattenTree(r.child[1], putHere);
			}
		}

		private static void drawGizmosNode(MB2_TexturePacker.Node r)
		{
			Vector3 vector = new Vector3((float)r.r.w, (float)r.r.h, 0f);
			Vector3 center = new Vector3((float)r.r.x + vector.x / 2f, (float)(-(float)r.r.y) - vector.y / 2f, 0f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(center, vector);
			if (r.img != null)
			{
				Gizmos.color = new Color(Random.value, Random.value, Random.value);
				vector = new Vector3((float)r.img.w, (float)r.img.h, 0f);
				Gizmos.DrawCube(new Vector3((float)r.r.x + vector.x / 2f, (float)(-(float)r.r.y) - vector.y / 2f, 0f), vector);
			}
			if (r.child[0] != null)
			{
				Gizmos.color = Color.red;
				MB2_TexturePacker.drawGizmosNode(r.child[0]);
			}
			if (r.child[1] != null)
			{
				Gizmos.color = Color.green;
				MB2_TexturePacker.drawGizmosNode(r.child[1]);
			}
		}

		private static Texture2D createFilledTex(Color c, int w, int h)
		{
			Texture2D texture2D = new Texture2D(w, h);
			for (int i = 0; i < w; i++)
			{
				for (int j = 0; j < h; j++)
				{
					texture2D.SetPixel(i, j, c);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public void DrawGizmos()
		{
			if (this.bestRoot != null)
			{
				MB2_TexturePacker.drawGizmosNode(this.bestRoot.root);
				Gizmos.color = Color.yellow;
				Vector3 vector = new Vector3((float)this.bestRoot.outW, (float)(-(float)this.bestRoot.outH), 0f);
				Gizmos.DrawWireCube(new Vector3(vector.x / 2f, vector.y / 2f, 0f), vector);
			}
		}

		private bool ProbeSingleAtlas(MB2_TexturePacker.Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, MB2_TexturePacker.ProbeResult pr)
		{
			MB2_TexturePacker.Node node = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.maxDim);
			node.r = new MB2_TexturePacker.PixRect(0, 0, idealAtlasW, idealAtlasH);
			for (int i = 0; i < imgsToAdd.Length; i++)
			{
				if (node.Insert(imgsToAdd[i], false) == null)
				{
					return false;
				}
				if (i == imgsToAdd.Length - 1)
				{
					int num = 0;
					int num2 = 0;
					this.GetExtent(node, ref num, ref num2);
					int num3 = num;
					int num4 = num2;
					bool fits;
					float num8;
					float num9;
					if (this.doPowerOfTwoTextures)
					{
						num3 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(num), maxAtlasDim);
						num4 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(num2), maxAtlasDim);
						if (num4 < num3 / 2)
						{
							num4 = num3 / 2;
						}
						if (num3 < num4 / 2)
						{
							num3 = num4 / 2;
						}
						fits = (num <= maxAtlasDim && num2 <= maxAtlasDim);
						float num5 = Mathf.Max(1f, (float)num / (float)maxAtlasDim);
						float num6 = Mathf.Max(1f, (float)num2 / (float)maxAtlasDim);
						float num7 = (float)num3 * num5 * (float)num4 * num6;
						num8 = 1f - (num7 - imgArea) / num7;
						num9 = 1f;
					}
					else
					{
						num8 = 1f - ((float)(num * num2) - imgArea) / (float)(num * num2);
						if (num < num2)
						{
							num9 = (float)num / (float)num2;
						}
						else
						{
							num9 = (float)num2 / (float)num;
						}
						fits = (num <= maxAtlasDim && num2 <= maxAtlasDim);
					}
					pr.Set(num, num2, num3, num4, node, fits, num8, num9);
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new object[]
						{
							"Probe success efficiency w=",
							num,
							" h=",
							num2,
							" e=",
							num8,
							" sq=",
							num9,
							" fits=",
							fits.ToString()
						}), Array.Empty<object>());
					}
					return true;
				}
			}
			Debug.LogError("Should never get here.");
			return false;
		}

		private bool ProbeMultiAtlas(MB2_TexturePacker.Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, MB2_TexturePacker.ProbeResult pr)
		{
			int num = 0;
			MB2_TexturePacker.Node node = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.maxDim);
			node.r = new MB2_TexturePacker.PixRect(0, 0, idealAtlasW, idealAtlasH);
			for (int i = 0; i < imgsToAdd.Length; i++)
			{
				if (node.Insert(imgsToAdd[i], false) == null)
				{
					if (imgsToAdd[i].x > idealAtlasW && imgsToAdd[i].y > idealAtlasH)
					{
						return false;
					}
					MB2_TexturePacker.Node node2 = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.Container);
					node2.r = new MB2_TexturePacker.PixRect(0, 0, node.r.w + idealAtlasW, idealAtlasH);
					MB2_TexturePacker.Node node3 = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.maxDim);
					node3.r = new MB2_TexturePacker.PixRect(node.r.w, 0, idealAtlasW, idealAtlasH);
					node2.child[1] = node3;
					node2.child[0] = node;
					node = node2;
					node.Insert(imgsToAdd[i], false);
					num++;
				}
			}
			pr.numAtlases = num;
			pr.root = node;
			pr.totalAtlasArea = (float)(num * maxAtlasDim * maxAtlasDim);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"Probe success efficiency numAtlases=",
					num,
					" totalArea=",
					pr.totalAtlasArea
				}), Array.Empty<object>());
			}
			return true;
		}

		private void GetExtent(MB2_TexturePacker.Node r, ref int x, ref int y)
		{
			if (r.img != null)
			{
				if (r.r.x + r.img.w > x)
				{
					x = r.r.x + r.img.w;
				}
				if (r.r.y + r.img.h > y)
				{
					y = r.r.y + r.img.h;
				}
			}
			if (r.child[0] != null)
			{
				this.GetExtent(r.child[0], ref x, ref y);
			}
			if (r.child[1] != null)
			{
				this.GetExtent(r.child[1], ref x, ref y);
			}
		}

		private int StepWidthHeight(int oldVal, int step, int maxDim)
		{
			if (this.doPowerOfTwoTextures && oldVal < maxDim)
			{
				return oldVal * 2;
			}
			int num = oldVal + step;
			if (num > maxDim && oldVal < maxDim)
			{
				num = maxDim;
			}
			return num;
		}

		public static int RoundToNearestPositivePowerOfTwo(int x)
		{
			int num = (int)Mathf.Pow(2f, (float)Mathf.RoundToInt(Mathf.Log((float)x) / Mathf.Log(2f)));
			if (num == 0 || num == 1)
			{
				num = 2;
			}
			return num;
		}

		public static int CeilToNearestPowerOfTwo(int x)
		{
			int num = (int)Mathf.Pow(2f, Mathf.Ceil(Mathf.Log((float)x) / Mathf.Log(2f)));
			if (num == 0 || num == 1)
			{
				num = 2;
			}
			return num;
		}

		public AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding)
		{
			return this.GetRects(imgWidthHeights, maxDimension, padding, false);
		}

		public AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, bool doMultiAtlas)
		{
			if (doMultiAtlas)
			{
				return this._GetRectsMultiAtlas(imgWidthHeights, maxDimension, padding, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2);
			}
			AtlasPackingResult atlasPackingResult = this._GetRectsSingleAtlas(imgWidthHeights, maxDimension, padding, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 0);
			if (atlasPackingResult == null)
			{
				return null;
			}
			return new AtlasPackingResult[]
			{
				atlasPackingResult
			};
		}

		private AtlasPackingResult _GetRectsSingleAtlas(List<Vector2> imgWidthHeights, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, int recursionDepth)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}, recursionDepth={7}", new object[]
				{
					imgWidthHeights.Count,
					maxDimension,
					padding,
					minImageSizeX,
					minImageSizeY,
					masterImageSizeX,
					masterImageSizeY,
					recursionDepth
				}));
			}
			if (recursionDepth > 10)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Maximum recursion depth reached. Couldn't find packing for these textures.");
				}
				return null;
			}
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			MB2_TexturePacker.Image[] array = new MB2_TexturePacker.Image[imgWidthHeights.Count];
			for (int i = 0; i < array.Length; i++)
			{
				int tw = (int)imgWidthHeights[i].x;
				int th = (int)imgWidthHeights[i].y;
				MB2_TexturePacker.Image image = array[i] = new MB2_TexturePacker.Image(i, tw, th, padding, minImageSizeX, minImageSizeY);
				num += (float)(image.w * image.h);
				num2 = Mathf.Max(num2, image.w);
				num3 = Mathf.Max(num3, image.h);
			}
			if ((float)num3 / (float)num2 > 2f)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using height Comparer", Array.Empty<object>());
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageHeightComparer());
			}
			else if ((double)((float)num3 / (float)num2) < 0.5)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using width Comparer", Array.Empty<object>());
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageWidthComparer());
			}
			else
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Using area Comparer", Array.Empty<object>());
				}
				Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageAreaComparer());
			}
			int num4 = (int)Mathf.Sqrt(num);
			int num6;
			int num5;
			if (this.doPowerOfTwoTextures)
			{
				num5 = (num6 = MB2_TexturePacker.RoundToNearestPositivePowerOfTwo(num4));
				if (num2 > num6)
				{
					num6 = MB2_TexturePacker.CeilToNearestPowerOfTwo(num6);
				}
				if (num3 > num5)
				{
					num5 = MB2_TexturePacker.CeilToNearestPowerOfTwo(num5);
				}
			}
			else
			{
				num6 = num4;
				num5 = num4;
				if (num2 > num4)
				{
					num6 = num2;
					num5 = Mathf.Max(Mathf.CeilToInt(num / (float)num2), num3);
				}
				if (num3 > num4)
				{
					num6 = Mathf.Max(Mathf.CeilToInt(num / (float)num3), num2);
					num5 = num3;
				}
			}
			if (num6 == 0)
			{
				num6 = 4;
			}
			if (num5 == 0)
			{
				num5 = 4;
			}
			int num7 = (int)((float)num6 * 0.15f);
			int num8 = (int)((float)num5 * 0.15f);
			if (num7 == 0)
			{
				num7 = 1;
			}
			if (num8 == 0)
			{
				num8 = 1;
			}
			int num9 = 2;
			int num10 = num5;
			while (num9 >= 1 && num10 < num4 * 1000)
			{
				bool flag = false;
				num9 = 0;
				int num11 = num6;
				while (!flag && num11 < num4 * 1000)
				{
					MB2_TexturePacker.ProbeResult probeResult = new MB2_TexturePacker.ProbeResult();
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Probing h=",
							num10,
							" w=",
							num11
						}));
					}
					if (this.ProbeSingleAtlas(array, num11, num10, num, maxDimension, probeResult))
					{
						flag = true;
						if (this.bestRoot == null)
						{
							this.bestRoot = probeResult;
						}
						else if (probeResult.GetScore(this.doPowerOfTwoTextures) > this.bestRoot.GetScore(this.doPowerOfTwoTextures))
						{
							this.bestRoot = probeResult;
						}
					}
					else
					{
						num9++;
						num11 = this.StepWidthHeight(num11, num7, maxDimension);
						if (this.LOG_LEVEL >= MB2_LogLevel.trace)
						{
							MB2_Log.LogDebug(string.Concat(new object[]
							{
								"increasing Width h=",
								num10,
								" w=",
								num11
							}), Array.Empty<object>());
						}
					}
				}
				num10 = this.StepWidthHeight(num10, num8, maxDimension);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"increasing Height h=",
						num10,
						" w=",
						num11
					}), Array.Empty<object>());
				}
			}
			if (this.bestRoot == null)
			{
				return null;
			}
			int num12;
			int num13;
			if (this.doPowerOfTwoTextures)
			{
				num12 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(this.bestRoot.w), maxDimension);
				num13 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(this.bestRoot.h), maxDimension);
				if (num13 < num12 / 2)
				{
					num13 = num12 / 2;
				}
				if (num12 < num13 / 2)
				{
					num12 = num13 / 2;
				}
			}
			else
			{
				num12 = Mathf.Min(this.bestRoot.w, maxDimension);
				num13 = Mathf.Min(this.bestRoot.h, maxDimension);
			}
			this.bestRoot.outW = num12;
			this.bestRoot.outH = num13;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Best fit found: atlasW=",
					num12,
					" atlasH",
					num13,
					" w=",
					this.bestRoot.w,
					" h=",
					this.bestRoot.h,
					" efficiency=",
					this.bestRoot.efficiency,
					" squareness=",
					this.bestRoot.squareness,
					" fits in max dimension=",
					this.bestRoot.largerOrEqualToMaxDim.ToString()
				}));
			}
			List<MB2_TexturePacker.Image> list = new List<MB2_TexturePacker.Image>();
			MB2_TexturePacker.flattenTree(this.bestRoot.root, list);
			list.Sort(new MB2_TexturePacker.ImgIDComparer());
			AtlasPackingResult result = this.ScaleAtlasToFitMaxDim(this.bestRoot, imgWidthHeights, list, maxDimension, padding, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY, num12, num13, recursionDepth);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Format("Done GetRects atlasW={0} atlasH={1}", this.bestRoot.w, this.bestRoot.h), Array.Empty<object>());
			}
			return result;
		}

		private AtlasPackingResult ScaleAtlasToFitMaxDim(MB2_TexturePacker.ProbeResult root, List<Vector2> imgWidthHeights, List<MB2_TexturePacker.Image> images, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, int outW, int outH, int recursionDepth)
		{
			int minImageSizeX2 = minImageSizeX;
			int minImageSizeY2 = minImageSizeY;
			bool flag = false;
			float num = (float)padding / (float)outW;
			if (root.w > maxDimension)
			{
				num = (float)padding / (float)maxDimension;
				float num2 = (float)maxDimension / (float)root.w;
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Packing exceeded atlas width shrinking to " + num2);
				}
				for (int i = 0; i < images.Count; i++)
				{
					MB2_TexturePacker.Image image = images[i];
					if ((float)image.w * num2 < (float)masterImageSizeX)
					{
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeX.");
						}
						flag = true;
						minImageSizeX2 = Mathf.CeilToInt((float)minImageSizeX / num2);
					}
					int num3 = (int)((float)(image.x + image.w) * num2);
					image.x = (int)(num2 * (float)image.x);
					image.w = num3 - image.x;
				}
				outW = maxDimension;
			}
			float num4 = (float)padding / (float)outH;
			if (root.h > maxDimension)
			{
				num4 = (float)padding / (float)maxDimension;
				float num5 = (float)maxDimension / (float)root.h;
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Packing exceeded atlas height shrinking to " + num5);
				}
				for (int j = 0; j < images.Count; j++)
				{
					MB2_TexturePacker.Image image2 = images[j];
					if ((float)image2.h * num5 < (float)masterImageSizeY)
					{
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeY.");
						}
						flag = true;
						minImageSizeY2 = Mathf.CeilToInt((float)minImageSizeY / num5);
					}
					int num6 = (int)((float)(image2.y + image2.h) * num5);
					image2.y = (int)(num5 * (float)image2.y);
					image2.h = num6 - image2.y;
				}
				outH = maxDimension;
			}
			if (!flag)
			{
				AtlasPackingResult atlasPackingResult = new AtlasPackingResult();
				atlasPackingResult.rects = new Rect[images.Count];
				atlasPackingResult.srcImgIdxs = new int[images.Count];
				atlasPackingResult.atlasX = outW;
				atlasPackingResult.atlasY = outH;
				atlasPackingResult.usedW = -1;
				atlasPackingResult.usedH = -1;
				for (int k = 0; k < images.Count; k++)
				{
					MB2_TexturePacker.Image image3 = images[k];
					Rect rect = atlasPackingResult.rects[k] = new Rect((float)image3.x / (float)outW + num, (float)image3.y / (float)outH + num4, (float)image3.w / (float)outW - num * 2f, (float)image3.h / (float)outH - num4 * 2f);
					atlasPackingResult.srcImgIdxs[k] = image3.imgId;
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new object[]
						{
							"Image: ",
							k,
							" imgID=",
							image3.imgId,
							" x=",
							rect.x * (float)outW,
							" y=",
							rect.y * (float)outH,
							" w=",
							rect.width * (float)outW,
							" h=",
							rect.height * (float)outH,
							" padding=",
							padding
						}), Array.Empty<object>());
					}
				}
				return atlasPackingResult;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("==================== REDOING PACKING ================");
			}
			root = null;
			return this._GetRectsSingleAtlas(imgWidthHeights, maxDimension, padding, minImageSizeX2, minImageSizeY2, masterImageSizeX, masterImageSizeY, recursionDepth + 1);
		}

		private AtlasPackingResult[] _GetRectsMultiAtlas(List<Vector2> imgWidthHeights, int maxDimensionPassed, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}", new object[]
				{
					imgWidthHeights.Count,
					maxDimensionPassed,
					padding,
					minImageSizeX,
					minImageSizeY,
					masterImageSizeX,
					masterImageSizeY
				}));
			}
			float num = 0f;
			int a = 0;
			int a2 = 0;
			MB2_TexturePacker.Image[] array = new MB2_TexturePacker.Image[imgWidthHeights.Count];
			int num2 = maxDimensionPassed;
			if (this.doPowerOfTwoTextures)
			{
				num2 = MB2_TexturePacker.RoundToNearestPositivePowerOfTwo(num2);
			}
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = (int)imgWidthHeights[i].x;
				int num4 = (int)imgWidthHeights[i].y;
				num3 = Mathf.Min(num3, num2 - padding * 2);
				num4 = Mathf.Min(num4, num2 - padding * 2);
				MB2_TexturePacker.Image image = array[i] = new MB2_TexturePacker.Image(i, num3, num4, padding, minImageSizeX, minImageSizeY);
				num += (float)(image.w * image.h);
				a = Mathf.Max(a, image.w);
				a2 = Mathf.Max(a2, image.h);
			}
			int num5;
			int num6;
			if (this.doPowerOfTwoTextures)
			{
				num5 = MB2_TexturePacker.RoundToNearestPositivePowerOfTwo(num2);
				num6 = MB2_TexturePacker.RoundToNearestPositivePowerOfTwo(num2);
			}
			else
			{
				num5 = num2;
				num6 = num2;
			}
			if (num6 == 0)
			{
				num6 = 4;
			}
			if (num5 == 0)
			{
				num5 = 4;
			}
			MB2_TexturePacker.ProbeResult probeResult = new MB2_TexturePacker.ProbeResult();
			Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageHeightComparer());
			if (this.ProbeMultiAtlas(array, num6, num5, num, num2, probeResult))
			{
				this.bestRoot = probeResult;
			}
			Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageWidthComparer());
			if (this.ProbeMultiAtlas(array, num6, num5, num, num2, probeResult) && probeResult.totalAtlasArea < this.bestRoot.totalAtlasArea)
			{
				this.bestRoot = probeResult;
			}
			Array.Sort<MB2_TexturePacker.Image>(array, new MB2_TexturePacker.ImageAreaComparer());
			if (this.ProbeMultiAtlas(array, num6, num5, num, num2, probeResult) && probeResult.totalAtlasArea < this.bestRoot.totalAtlasArea)
			{
				this.bestRoot = probeResult;
			}
			if (this.bestRoot == null)
			{
				return null;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Best fit found: w=",
					this.bestRoot.w,
					" h=",
					this.bestRoot.h,
					" efficiency=",
					this.bestRoot.efficiency,
					" squareness=",
					this.bestRoot.squareness,
					" fits in max dimension=",
					this.bestRoot.largerOrEqualToMaxDim.ToString()
				}));
			}
			List<AtlasPackingResult> list = new List<AtlasPackingResult>();
			List<MB2_TexturePacker.Node> list2 = new List<MB2_TexturePacker.Node>();
			Stack<MB2_TexturePacker.Node> stack = new Stack<MB2_TexturePacker.Node>();
			for (MB2_TexturePacker.Node node = this.bestRoot.root; node != null; node = node.child[0])
			{
				stack.Push(node);
			}
			while (stack.Count > 0)
			{
				MB2_TexturePacker.Node node = stack.Pop();
				if (node.isFullAtlas == MB2_TexturePacker.NodeType.maxDim)
				{
					list2.Add(node);
				}
				if (node.child[1] != null)
				{
					for (node = node.child[1]; node != null; node = node.child[0])
					{
						stack.Push(node);
					}
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				List<MB2_TexturePacker.Image> list3 = new List<MB2_TexturePacker.Image>();
				MB2_TexturePacker.flattenTree(list2[j], list3);
				Rect[] array2 = new Rect[list3.Count];
				int[] array3 = new int[list3.Count];
				for (int k = 0; k < list3.Count; k++)
				{
					array2[k] = new Rect((float)(list3[k].x - list2[j].r.x), (float)list3[k].y, (float)list3[k].w, (float)list3[k].h);
					array3[k] = list3[k].imgId;
				}
				AtlasPackingResult atlasPackingResult = new AtlasPackingResult();
				this.GetExtent(list2[j], ref atlasPackingResult.usedW, ref atlasPackingResult.usedH);
				atlasPackingResult.usedW -= list2[j].r.x;
				int num7 = list2[j].r.w;
				int num8 = list2[j].r.h;
				if (this.doPowerOfTwoTextures)
				{
					num7 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(atlasPackingResult.usedW), list2[j].r.w);
					num8 = Mathf.Min(MB2_TexturePacker.CeilToNearestPowerOfTwo(atlasPackingResult.usedH), list2[j].r.h);
					if (num8 < num7 / 2)
					{
						num8 = num7 / 2;
					}
					if (num7 < num8 / 2)
					{
						num7 = num8 / 2;
					}
				}
				else
				{
					num7 = atlasPackingResult.usedW;
					num8 = atlasPackingResult.usedH;
				}
				atlasPackingResult.atlasY = num8;
				atlasPackingResult.atlasX = num7;
				atlasPackingResult.rects = array2;
				atlasPackingResult.srcImgIdxs = array3;
				list.Add(atlasPackingResult);
				this.normalizeRects(atlasPackingResult, padding);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Format("Done GetRects ", Array.Empty<object>()), Array.Empty<object>());
				}
			}
			return list.ToArray();
		}

		private void normalizeRects(AtlasPackingResult rr, int padding)
		{
			for (int i = 0; i < rr.rects.Length; i++)
			{
				rr.rects[i].x = (rr.rects[i].x + (float)padding) / (float)rr.atlasX;
				rr.rects[i].y = (rr.rects[i].y + (float)padding) / (float)rr.atlasY;
				rr.rects[i].width = (rr.rects[i].width - (float)(padding * 2)) / (float)rr.atlasX;
				rr.rects[i].height = (rr.rects[i].height - (float)(padding * 2)) / (float)rr.atlasY;
			}
		}

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		private MB2_TexturePacker.ProbeResult bestRoot;

		public int atlasY;

		public bool doPowerOfTwoTextures = true;

		private enum NodeType
		{
			Container,
			maxDim,
			regular
		}

		private class PixRect
		{
			public PixRect()
			{
			}

			public PixRect(int xx, int yy, int ww, int hh)
			{
				this.x = xx;
				this.y = yy;
				this.w = ww;
				this.h = hh;
			}

			public override string ToString()
			{
				return string.Format("x={0},y={1},w={2},h={3}", new object[]
				{
					this.x,
					this.y,
					this.w,
					this.h
				});
			}

			public int x;

			public int y;

			public int w;

			public int h;
		}

		private class Image
		{
			public Image(int id, int tw, int th, int padding, int minImageSizeX, int minImageSizeY)
			{
				this.imgId = id;
				this.w = Mathf.Max(tw + padding * 2, minImageSizeX);
				this.h = Mathf.Max(th + padding * 2, minImageSizeY);
			}

			public Image(MB2_TexturePacker.Image im)
			{
				this.imgId = im.imgId;
				this.w = im.w;
				this.h = im.h;
				this.x = im.x;
				this.y = im.y;
			}

			public int imgId;

			public int w;

			public int h;

			public int x;

			public int y;
		}

		private class ImgIDComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.imgId > y.imgId)
				{
					return 1;
				}
				if (x.imgId == y.imgId)
				{
					return 0;
				}
				return -1;
			}
		}

		private class ImageHeightComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.h > y.h)
				{
					return -1;
				}
				if (x.h == y.h)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageWidthComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				if (x.w > y.w)
				{
					return -1;
				}
				if (x.w == y.w)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageAreaComparer : IComparer<MB2_TexturePacker.Image>
		{
			public int Compare(MB2_TexturePacker.Image x, MB2_TexturePacker.Image y)
			{
				int num = x.w * x.h;
				int num2 = y.w * y.h;
				if (num > num2)
				{
					return -1;
				}
				if (num == num2)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ProbeResult
		{
			public void Set(int ww, int hh, int outw, int outh, MB2_TexturePacker.Node r, bool fits, float e, float sq)
			{
				this.w = ww;
				this.h = hh;
				this.outW = outw;
				this.outH = outh;
				this.root = r;
				this.largerOrEqualToMaxDim = fits;
				this.efficiency = e;
				this.squareness = sq;
			}

			public float GetScore(bool doPowerOfTwoScore)
			{
				float num = this.largerOrEqualToMaxDim ? 1f : 0f;
				if (doPowerOfTwoScore)
				{
					return num * 2f + this.efficiency;
				}
				return this.squareness + 2f * this.efficiency + num;
			}

			public void PrintTree()
			{
				MB2_TexturePacker.printTree(this.root, "  ");
			}

			public int w;

			public int h;

			public int outW;

			public int outH;

			public MB2_TexturePacker.Node root;

			public bool largerOrEqualToMaxDim;

			public float efficiency;

			public float squareness;

			public float totalAtlasArea;

			public int numAtlases;
		}

		private class Node
		{
			public Node(MB2_TexturePacker.NodeType rootType)
			{
				this.isFullAtlas = rootType;
			}

			private bool isLeaf()
			{
				return this.child[0] == null || this.child[1] == null;
			}

			public MB2_TexturePacker.Node Insert(MB2_TexturePacker.Image im, bool handed)
			{
				int num;
				int num2;
				if (handed)
				{
					num = 0;
					num2 = 1;
				}
				else
				{
					num = 1;
					num2 = 0;
				}
				if (!this.isLeaf())
				{
					MB2_TexturePacker.Node node = this.child[num].Insert(im, handed);
					if (node != null)
					{
						return node;
					}
					return this.child[num2].Insert(im, handed);
				}
				else
				{
					if (this.img != null)
					{
						return null;
					}
					if (this.r.w < im.w || this.r.h < im.h)
					{
						return null;
					}
					if (this.r.w == im.w && this.r.h == im.h)
					{
						this.img = im;
						return this;
					}
					this.child[num] = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.regular);
					this.child[num2] = new MB2_TexturePacker.Node(MB2_TexturePacker.NodeType.regular);
					int num3 = this.r.w - im.w;
					int num4 = this.r.h - im.h;
					if (num3 > num4)
					{
						this.child[num].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y, im.w, this.r.h);
						this.child[num2].r = new MB2_TexturePacker.PixRect(this.r.x + im.w, this.r.y, this.r.w - im.w, this.r.h);
					}
					else
					{
						this.child[num].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y, this.r.w, im.h);
						this.child[num2].r = new MB2_TexturePacker.PixRect(this.r.x, this.r.y + im.h, this.r.w, this.r.h - im.h);
					}
					return this.child[num].Insert(im, handed);
				}
			}

			public MB2_TexturePacker.NodeType isFullAtlas;

			public MB2_TexturePacker.Node[] child = new MB2_TexturePacker.Node[2];

			public MB2_TexturePacker.PixRect r;

			public MB2_TexturePacker.Image img;
		}
	}
}
