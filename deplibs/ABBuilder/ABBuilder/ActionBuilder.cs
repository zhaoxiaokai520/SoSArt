using FlatBuffers;
using MobaGo.FlatBuffer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

public static class ActionBuilder
{
	private static MD5 md5calc;

	private static string GetMD5Str(byte[] data)
	{
		string text = "";
		for (int i = 0; i < data.Length; i++)
		{
			text += string.Format("{0:X2}", data[i]);
		}
		return text;
	}

	private static bool CheckMD5(string actionFile)
	{
		if (ActionBuilder.md5calc == null)
		{
			ActionBuilder.md5calc = MD5.Create();
		}
		if (!CFileManager.IsFileExist(actionFile))
		{
			return false;
		}
		string text = "";
		try
		{
			char[] arg_32_0 = File.ReadAllText(actionFile).ToCharArray();
			List<byte> list = new List<byte>();
			char[] array = arg_32_0;
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				byte[] array2 = new byte[2];
				for (int j = 0; j < array2.Length; j++)
				{
					if (BitConverter.IsLittleEndian)
					{
						array2[j] = (byte)(c >> (j * 8 & 31));
					}
					else
					{
						array2[array2.Length - 1 - j] = (byte)(c >> (j * 8 & 31));
					}
				}
				list.AddRange(array2);
			}
			text = ActionBuilder.GetMD5Str(ActionBuilder.md5calc.ComputeHash(list.ToArray()));
		}
		catch
		{
			Debug.LogError("Action Builder: Can't read '" + actionFile + "' for building CRC checksum.");
			bool result = false;
			return result;
		}
		string text2 = actionFile.Replace(".xml", ".md5");
		string filePath = actionFile.Replace(".xml", ".bytes");
		if (!CFileManager.IsFileExist(text2) || !CFileManager.IsFileExist(filePath))
		{
			ActionBuilder.SaveCRCFile(text2, text);
			return false;
		}
		StreamReader expr_11F = new StreamReader(File.OpenRead(text2), Encoding.UTF8);
		string b = expr_11F.ReadToEnd();
		expr_11F.Close();
		if (text != b)
		{
			ActionBuilder.SaveCRCFile(text2, text);
			return false;
		}
		return true;
	}

	private static void SaveCRCFile(string md5file, string md5str)
	{
		StreamWriter expr_10 = new StreamWriter(File.Create(md5file), Encoding.UTF8);
		expr_10.Write(md5str);
		expr_10.Close();
	}

	public static string CookAction(string actionFile)
	{
		string text = actionFile.Replace(".xml", ".bytes");
		XmlDocument expr_16 = new XmlDocument();
		expr_16.Load(actionFile);
		XmlNode xmlNode = expr_16.SelectSingleNode("Project");
		FlatBufferBuilder flatBufferBuilder = new FlatBufferBuilder(1);
		VectorOffset templateObjsOffset = default(VectorOffset);
		VectorOffset referenceParamsOffset = default(VectorOffset);
		Offset<ActionObj> actionOffset = default(Offset<ActionObj>);
		for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = xmlNode.ChildNodes[i] as XmlElement;
			if (xmlElement != null)
			{
				if (xmlElement.Name == "TemplateObjectList")
				{
					templateObjsOffset = ActionBuilder.CookTemplateObjectList(flatBufferBuilder, xmlElement);
				}
				else if (xmlElement.Name == "RefParamList")
				{
					referenceParamsOffset = ActionBuilder.CookRefParamList(flatBufferBuilder, xmlElement);
				}
				else if (xmlElement.Name == "Action")
				{
					actionOffset = ActionBuilder.CookActionObj(flatBufferBuilder, xmlElement);
				}
			}
		}
		Offset<ActionData> offset = ActionData.CreateActionData(flatBufferBuilder, templateObjsOffset, referenceParamsOffset, actionOffset);
		flatBufferBuilder.Finish(offset.Value);
		FileStream expr_F6 = File.Create(text);
		expr_F6.Write(flatBufferBuilder.SizedByteArray(), 0, flatBufferBuilder.get_SizedArrayLength());
		expr_F6.Close();
		if (!CFileManager.IsFileExist(text))
		{
			return string.Empty;
		}
		return text;
	}

	public static byte[] ExportAction(string actionFile)
	{
		actionFile.Replace(".xml", ".bytes");
		XmlDocument expr_16 = new XmlDocument();
		expr_16.Load(actionFile);
		XmlNode xmlNode = expr_16.SelectSingleNode("Project");
		FlatBufferBuilder flatBufferBuilder = new FlatBufferBuilder(1);
		VectorOffset templateObjsOffset = default(VectorOffset);
		VectorOffset referenceParamsOffset = default(VectorOffset);
		Offset<ActionObj> actionOffset = default(Offset<ActionObj>);
		for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = xmlNode.ChildNodes[i] as XmlElement;
			if (xmlElement != null)
			{
				if (xmlElement.Name == "TemplateObjectList")
				{
					templateObjsOffset = ActionBuilder.CookTemplateObjectList(flatBufferBuilder, xmlElement);
				}
				else if (xmlElement.Name == "RefParamList")
				{
					referenceParamsOffset = ActionBuilder.CookRefParamList(flatBufferBuilder, xmlElement);
				}
				else if (xmlElement.Name == "Action")
				{
					actionOffset = ActionBuilder.CookActionObj(flatBufferBuilder, xmlElement);
				}
			}
		}
		Offset<ActionData> offset = ActionData.CreateActionData(flatBufferBuilder, templateObjsOffset, referenceParamsOffset, actionOffset);
		flatBufferBuilder.Finish(offset.Value);
		return flatBufferBuilder.SizedByteArray();
	}

	private static VectorOffset CookTemplateObjectList(FlatBufferBuilder builder, XmlElement node)
	{
		List<Offset<TemplateObj>> list = new List<Offset<TemplateObj>>();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
			if (xmlElement != null && xmlElement.Name == "TemplateObject")
			{
				StringOffset objectNameOffset = builder.CreateString(xmlElement.GetAttribute("objectName"));
				Offset<TemplateObj> item = TemplateObj.CreateTemplateObj(builder, objectNameOffset, int.Parse(xmlElement.GetAttribute("id")), bool.Parse(xmlElement.GetAttribute("isTemp").ToLower()));
				list.Add(item);
			}
		}
		builder.StartVector(4, list.Count, 4);
		for (int j = list.Count - 1; j >= 0; j--)
		{
			builder.AddOffset(list[j].Value);
		}
		return builder.EndVector();
	}

	private static VectorOffset CookRefParamList(FlatBufferBuilder builder, XmlElement node)
	{
		List<Offset<VarContext>> list = new List<Offset<VarContext>>();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
			if (xmlElement != null && !(xmlElement.Name.ToLower() == "log"))
			{
				Offset<VarContext> item = ActionBuilder.CookVarContext(builder, xmlElement);
				list.Add(item);
			}
		}
		builder.StartVector(4, list.Count, 4);
		for (int j = list.Count - 1; j >= 0; j--)
		{
			builder.AddOffset(list[j].Value);
		}
		return builder.EndVector();
	}

	private static Offset<VarContext> CookVarContext(FlatBufferBuilder builder, XmlElement node)
	{
		byte[] array = null;
		StringOffset nameOffset = builder.CreateString(node.GetAttribute("name"));
		StringOffset refParamNameOffset = builder.CreateString(node.GetAttribute("refParamName"));
		byte dataType = 0;
		string text = node.Name.ToLower();
		if (text == "float")
		{
			dataType = 0;
			float value = 0f;
			if (!float.TryParse(node.GetAttribute("value"), out value))
			{
				Debug.LogError(string.Format(" {1} Convert to {0} Value Error {2}", text, node.GetAttribute("value"), node.ParentNode.ParentNode.BaseURI));
			}
			array = BitConverter.GetBytes(value);
		}
		else if (text == "int")
		{
			dataType = 1;
			int value2 = 0;
			if (!int.TryParse(node.GetAttribute("value"), out value2))
			{
				Debug.LogError(string.Format(" {1} Convert to {0} Value Error {2}", text, node.GetAttribute("value"), node.ParentNode.ParentNode.BaseURI));
			}
			array = BitConverter.GetBytes(value2);
		}
		else if (text == "uint")
		{
			dataType = 3;
			uint value3 = 0u;
			if (!uint.TryParse(node.GetAttribute("value"), out value3))
			{
				Debug.LogError(string.Format(" {1} Convert to {0} Value Error", text, node.GetAttribute("value"), node.ParentNode.ParentNode.BaseURI));
			}
			array = BitConverter.GetBytes(value3);
		}
		else if (text == "bool")
		{
			dataType = 4;
			array = BitConverter.GetBytes(bool.Parse(node.GetAttribute("value").ToLower()));
		}
		else if (text == "string")
		{
			dataType = 5;
			string attribute = node.GetAttribute("value");
			array = Encoding.UTF8.GetBytes(attribute);
		}
		else if (text == "enum")
		{
			dataType = 10;
			array = BitConverter.GetBytes(int.Parse(node.GetAttribute("value")));
		}
		else if (text == "vector3" || text == "eulerangle")
		{
			if (text == "vector3")
			{
				dataType = 6;
			}
			else if (text == "eulerangle")
			{
				dataType = 9;
			}
			FlatBufferBuilder expr_229 = new FlatBufferBuilder(1);
			Offset<vector3> offset = vector3.Createvector3(expr_229, float.Parse(node.GetAttribute("x")), float.Parse(node.GetAttribute("y")), float.Parse(node.GetAttribute("z")));
			expr_229.Finish(offset.Value);
			array = expr_229.SizedByteArray();
		}
		else if (text == "vector3i")
		{
			dataType = 7;
			FlatBufferBuilder expr_28F = new FlatBufferBuilder(1);
			Offset<vector3i> offset2 = vector3i.Createvector3i(expr_28F, int.Parse(node.GetAttribute("x")), int.Parse(node.GetAttribute("y")), int.Parse(node.GetAttribute("z")));
			expr_28F.Finish(offset2.Value);
			array = expr_28F.SizedByteArray();
		}
		else if (text == "quaternion")
		{
			dataType = 8;
			FlatBufferBuilder expr_2F5 = new FlatBufferBuilder(1);
			Offset<quat> offset3 = quat.Createquat(expr_2F5, float.Parse(node.GetAttribute("x")), float.Parse(node.GetAttribute("y")), float.Parse(node.GetAttribute("z")), float.Parse(node.GetAttribute("w")));
			expr_2F5.Finish(offset3.Value);
			array = expr_2F5.SizedByteArray();
		}
		else if (text == "array")
		{
			dataType = 11;
			FlatBufferBuilder flatBufferBuilder = new FlatBufferBuilder(1);
			List<Offset<VarContext>> list = new List<Offset<VarContext>>();
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
				if (xmlElement != null && xmlElement.Name.ToLower() != "log")
				{
					Offset<VarContext> item = ActionBuilder.CookVarContext(flatBufferBuilder, xmlElement);
					list.Add(item);
				}
			}
			VectorOffset dataListOffset = ArrayObj.CreateDataListVector(flatBufferBuilder, list.ToArray());
			Offset<ArrayObj> offset4 = ArrayObj.CreateArrayObj(flatBufferBuilder, dataListOffset);
			flatBufferBuilder.Finish(offset4.Value);
			array = flatBufferBuilder.SizedByteArray();
		}
		else if (text == "templateobject" || text == "trackobject")
		{
			dataType = 2;
			FlatBufferBuilder expr_433 = new FlatBufferBuilder(1);
			StringOffset objectNameOffset = expr_433.CreateString(node.GetAttribute("objectName"));
			bool isTemp = node.HasAttribute("isTemp") && bool.Parse(node.GetAttribute("isTemp"));
			int id = 0;
			if (!int.TryParse(node.GetAttribute("id"), out id))
			{
				Debug.LogError(string.Format(" {1} Convert to {0} Value Error {2}", text, node.GetAttribute("id"), node.ParentNode.ParentNode.BaseURI));
			}
			Offset<TemplateObj> offset5 = TemplateObj.CreateTemplateObj(expr_433, objectNameOffset, id, isTemp);
			expr_433.Finish(offset5.Value);
			array = expr_433.SizedByteArray();
		}
		else if (text == "color")
		{
			dataType = 12;
			FlatBufferBuilder expr_4E8 = new FlatBufferBuilder(1);
			Offset<color> offset6 = color.Createcolor(expr_4E8, float.Parse(node.GetAttribute("r")) / 255f, float.Parse(node.GetAttribute("g")) / 255f, float.Parse(node.GetAttribute("b")) / 255f, float.Parse(node.GetAttribute("a")) / 255f);
			expr_4E8.Finish(offset6.Value);
			array = expr_4E8.SizedByteArray();
		}
		VectorOffset dataOffset = (array != null) ? VarObj.CreateDataVector(builder, array) : default(VectorOffset);
		Offset<VarObj> contextOffset = VarObj.CreateVarObj(builder, dataType, dataOffset);
		return VarContext.CreateVarContext(builder, nameOffset, refParamNameOffset, bool.Parse(node.GetAttribute("useRefParam").ToLower()), contextOffset);
	}

	private static Offset<ActionObj> CookActionObj(FlatBufferBuilder builder, XmlElement node)
	{
		int length = Mathf.RoundToInt(float.Parse(node.GetAttribute("length")) * 1000f);
		bool loop = bool.Parse(node.GetAttribute("loop").ToLower());
		List<Offset<TrackObj>> list = new List<Offset<TrackObj>>();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
			if (xmlElement != null && xmlElement.Name == "Track")
			{
				list.Add(ActionBuilder.CookTrackObj(builder, xmlElement));
			}
		}
		VectorOffset trackOffset = ActionObj.CreateTrackVector(builder, list.ToArray());
		return ActionObj.CreateActionObj(builder, length, loop, trackOffset);
	}

	private static Offset<TrackObj> CookTrackObj(FlatBufferBuilder builder, XmlElement node)
	{
		StringOffset trackNameOffset = builder.CreateString(node.GetAttribute("trackName"));
		StringOffset eventTypeOffset = builder.CreateString(node.GetAttribute("eventType"));
		StringOffset refParamNameOffset = builder.CreateString(node.GetAttribute("refParamName"));
		List<Offset<ConditionObj>> list = new List<Offset<ConditionObj>>();
		List<Offset<EventObj>> list2 = new List<Offset<EventObj>>();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
			if (xmlElement != null)
			{
				if (xmlElement.Name == "Event")
				{
					list2.Add(ActionBuilder.CookEventObj(builder, xmlElement));
				}
				else if (xmlElement.Name == "Condition")
				{
					list.Add(ActionBuilder.CookConditionObj(builder, xmlElement));
				}
			}
		}
		VectorOffset evtsOffset = default(VectorOffset);
		builder.StartVector(4, list2.Count, 4);
		for (int j = list2.Count - 1; j >= 0; j--)
		{
			builder.AddOffset(list2[j].Value);
		}
		evtsOffset = builder.EndVector();
		VectorOffset conditionOffset = default(VectorOffset);
		builder.StartVector(4, list.Count, 4);
		for (int k = list.Count - 1; k >= 0; k--)
		{
			builder.AddOffset(list[k].Value);
		}
		conditionOffset = builder.EndVector();
		bool enabled = true;
		bool useRefParam = false;
		bool execOnActionCompleted = false;
		bool execOnForceStopped = false;
		bool stopAfterLastEvent = true;
		for (int l = 0; l < node.Attributes.Count; l++)
		{
			if (node.Attributes[l].Name.ToLower() == "enabled")
			{
				enabled = bool.Parse(node.Attributes[l].Value.ToLower());
			}
			else if (node.Attributes[l].Name.ToLower() == "userefparam")
			{
				useRefParam = bool.Parse(node.Attributes[l].Value.ToLower());
			}
			else if (node.Attributes[l].Name.ToLower() == "execonactioncompleted")
			{
				execOnActionCompleted = bool.Parse(node.Attributes[l].Value.ToLower());
			}
			else if (node.Attributes[l].Name.ToLower() == "execonforcestopped")
			{
				execOnForceStopped = bool.Parse(node.Attributes[l].Value.ToLower());
			}
			else if (node.Attributes[l].Name.ToLower() == "stopafterlastevent")
			{
				stopAfterLastEvent = bool.Parse(node.Attributes[l].Value.ToLower());
			}
		}
		bool hasCondition = false;
		for (int m = 0; m < node.ChildNodes.Count; m++)
		{
			if (node.ChildNodes[m].Name == "Condition")
			{
				hasCondition = true;
				break;
			}
		}
		return TrackObj.CreateTrackObj(builder, enabled, trackNameOffset, eventTypeOffset, refParamNameOffset, useRefParam, execOnActionCompleted, execOnForceStopped, stopAfterLastEvent, hasCondition, conditionOffset, evtsOffset);
	}

	private static Offset<EventObj> CookEventObj(FlatBufferBuilder builder, XmlElement node)
	{
		List<Offset<VarContext>> list = new List<Offset<VarContext>>();
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			XmlElement xmlElement = node.ChildNodes[i] as XmlElement;
			if (xmlElement != null && xmlElement.Name.ToLower() != "log")
			{
				list.Add(ActionBuilder.CookVarContext(builder, xmlElement));
			}
		}
		VectorOffset varListOffset = EventObj.CreateVarListVector(builder, list.ToArray());
		int time = Mathf.RoundToInt(float.Parse(node.GetAttribute("time")) * 1000f);
		int length = node.HasAttribute("length") ? Mathf.RoundToInt(float.Parse(node.GetAttribute("length")) * 1000f) : 0;
		return EventObj.CreateEventObj(builder, time, length, varListOffset);
	}

	private static Offset<ConditionObj> CookConditionObj(FlatBufferBuilder builder, XmlElement node)
	{
		return ConditionObj.CreateConditionObj(builder, int.Parse(node.GetAttribute("id")), bool.Parse(node.GetAttribute("status").ToLower()));
	}
}
