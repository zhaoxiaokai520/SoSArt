using System;

namespace FlatBuffers
{
	public struct Offset<T> where T : class
	{
		public int Value;

		public Offset(int value)
		{
			this.Value = value;
		}
	}
}
