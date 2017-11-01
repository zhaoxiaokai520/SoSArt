using FlatBuffers;
using MobaGo.FlatBuffer;
using System;
using System.IO;

public class AB_HeroCmdAction : AB_HeroCmdBase
{
	public AB_HeroCmdAction(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FlatBufferBuilder flatBufferBuilder = new FlatBufferBuilder(1024);
		Offset<AgeData>[] array = new Offset<AgeData>[this.mInfo.GetFiles("*.xml").Length];
		int num = 0;
		FileInfo[] files = this.mInfo.GetFiles("*.xml");
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			StringOffset nameOffset = flatBufferBuilder.CreateString(CFileManager.EraseExtension(fileInfo.Name));
			byte[] data = ActionBuilder.ExportAction(fileInfo.FullName);
			VectorOffset dataOffset = AgeData.CreateDataVector(flatBufferBuilder, data);
			array[num++] = AgeData.CreateAgeData(flatBufferBuilder, nameOffset, dataOffset);
		}
		if (num > 0)
		{
			string path = this.mInfo.FullName + Path.DirectorySeparatorChar.ToString() + "skilldata.bytes";
			VectorOffset datasOffset = HeroAgeDatas.CreateDatasVector(flatBufferBuilder, array);
			Offset<HeroAgeDatas> offset = HeroAgeDatas.CreateHeroAgeDatas(flatBufferBuilder, 0, datasOffset);
			flatBufferBuilder.Finish(offset.Value);
			byte[] array2 = flatBufferBuilder.SizedByteArray();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = CFileManager.Encode(array2[j]);
			}
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			using (FileStream fileStream = File.Create(path))
			{
				fileStream.Write(array2, 0, array2.Length);
			}
			string item = AB_Common.Absolute2RelativePath(path);
			this.mABFileList.Add(item);
			this.mAssetGroupFileList.Add(item);
		}
		base.BuildCmd();
	}
}
