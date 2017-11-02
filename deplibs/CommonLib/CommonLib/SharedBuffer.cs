using System;

public class SharedBuffer : AbstractSmartObj
{
	public byte[] buffer = new byte[4096];

	public override void OnRelease()
	{
	}

	public void EnsureBufferLength(int length)
	{
		int num = this.buffer.Length;
		bool flag = num < length + 4;
		if (flag)
		{
			this.buffer = new byte[length + 1024];
		}
	}
}
