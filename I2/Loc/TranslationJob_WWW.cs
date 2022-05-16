using System;
using UnityEngine.Networking;

namespace I2.Loc
{
	public class TranslationJob_WWW : TranslationJob
	{
		public override void Dispose()
		{
			if (this.www != null)
			{
				this.www.Dispose();
			}
			this.www = null;
		}

		public UnityWebRequest www;
	}
}
