using System;

namespace MobaGo.EventBus
{
	public class MEObjDeliver : Mercury.MercuryEventBase
	{
		private const int kArgumentCount = 3;

		private object[] _args = new object[3];

		private int _opcode = 0;

		public object[] args
		{
			get
			{
				return this._args;
			}
		}

		public object obj
		{
			get
			{
				return this._args;
			}
		}

		public int opcode
		{
			get
			{
				return this._opcode;
			}
			set
			{
				this._opcode = value;
			}
		}

		public MEObjDeliver()
		{
			this._eventId = 1;
		}

		public override void Reset()
		{
			base.Reset();
			for (int i = 0; i < 3; i++)
			{
				this._args[i] = null;
			}
		}

		public override void OnRelease()
		{
			this.Reset();
		}
	}
}
