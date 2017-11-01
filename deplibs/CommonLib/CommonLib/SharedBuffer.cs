using System;

public class SharedBuffer : AbstractSmartObj
{
	public byte[] buffer = new byte[4096];

	public override void OnRelease()
	{
	}
}
