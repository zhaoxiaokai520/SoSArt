using System;

namespace MobaGo.EventBus
{
	public static class EventTokenTable
	{
		public static int[] EventDefine = new int[7];

		public static int et_connector
		{
			get
			{
				return EventTokenTable.EventDefine[0];
			}
		}

		public static int et_framewindow
		{
			get
			{
				return EventTokenTable.EventDefine[1];
			}
		}

		public static int et_framesynchr
		{
			get
			{
				return EventTokenTable.EventDefine[2];
			}
		}

		public static int et_game_framework
		{
			get
			{
				return EventTokenTable.EventDefine[3];
			}
		}

		public static int et_loading_ui
		{
			get
			{
				return EventTokenTable.EventDefine[4];
			}
		}

		public static int et_error_monitor
		{
			get
			{
				return EventTokenTable.EventDefine[5];
			}
		}

		public static int et_globally
		{
			get
			{
				return EventTokenTable.EventDefine[6];
			}
		}

		public static void PrepareTokenTable()
		{
			for (int i = 0; i < EventTokenTable.EventDefine.Length; i++)
			{
				EventTokenTable.EventDefine[i] = Singleton<Mercury>.instance.AccquireSession();
			}
		}
	}
}
