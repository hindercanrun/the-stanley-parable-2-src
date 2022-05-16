using System;
using System.Collections.Generic;

namespace I2.Loc
{
	public class BaseSpecializationManager
	{
		public virtual void InitializeSpecializations()
		{
			this.mSpecializations = new string[]
			{
				"Any",
				"PC",
				"Touch",
				"Controller",
				"VR",
				"XBox",
				"PS4",
				"OculusVR",
				"ViveVR",
				"GearVR",
				"Android",
				"IOS"
			};
			this.mSpecializationsFallbacks = new Dictionary<string, string>
			{
				{
					"XBox",
					"Controller"
				},
				{
					"PS4",
					"Controller"
				},
				{
					"OculusVR",
					"VR"
				},
				{
					"ViveVR",
					"VR"
				},
				{
					"GearVR",
					"VR"
				},
				{
					"Android",
					"Touch"
				},
				{
					"IOS",
					"Touch"
				}
			};
		}

		public virtual string GetCurrentSpecialization()
		{
			if (this.mSpecializations == null)
			{
				this.InitializeSpecializations();
			}
			return "PC";
		}

		public virtual string GetFallbackSpecialization(string specialization)
		{
			if (this.mSpecializationsFallbacks == null)
			{
				this.InitializeSpecializations();
			}
			string result;
			if (this.mSpecializationsFallbacks.TryGetValue(specialization, out result))
			{
				return result;
			}
			return "Any";
		}

		public string[] mSpecializations;

		public Dictionary<string, string> mSpecializationsFallbacks;
	}
}
