using System;

public enum kBundleFlag
{
	KeepInResources = 1,
	UnCompress,
	Resident = 4,
    EncryptBundle = 8,
	UnCompleteAsset = 16,
	InExtraIfs = 32,
    PermanentAfterLoad = 64
}
