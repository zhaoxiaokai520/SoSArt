using System;

public abstract class FrameClassAttribute : Attribute
{
	public abstract int CreatorID
	{
		get;
	}
}
