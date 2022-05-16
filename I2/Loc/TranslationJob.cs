using System;

namespace I2.Loc
{
	public class TranslationJob : IDisposable
	{
		public virtual TranslationJob.eJobState GetState()
		{
			return this.mJobState;
		}

		public virtual void Dispose()
		{
		}

		public TranslationJob.eJobState mJobState;

		public enum eJobState
		{
			Running,
			Succeeded,
			Failed
		}
	}
}
