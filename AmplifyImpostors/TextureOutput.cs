using System;
using UnityEngine;

namespace AmplifyImpostors
{
	[Serializable]
	public class TextureOutput
	{
		public TextureOutput()
		{
		}

		public TextureOutput(bool a, string n, TextureScale s, bool sr, TextureChannels c, TextureCompression nc, ImageFormat i)
		{
			this.Active = a;
			this.Name = n;
			this.Scale = s;
			this.SRGB = sr;
			this.Channels = c;
			this.Compression = nc;
			this.ImageFormat = i;
		}

		public TextureOutput Clone()
		{
			return (TextureOutput)base.MemberwiseClone();
		}

		[SerializeField]
		public int Index = -1;

		[SerializeField]
		public OverrideMask OverrideMask;

		public bool Active = true;

		public string Name = string.Empty;

		public TextureScale Scale = TextureScale.Full;

		public bool SRGB;

		public TextureChannels Channels;

		public TextureCompression Compression = TextureCompression.Normal;

		public ImageFormat ImageFormat = ImageFormat.TGA;
	}
}
