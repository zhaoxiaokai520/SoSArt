using System;

namespace MobaGo.EventBus
{
	public enum EObjDeliverOPCode
	{
		E_OP_LACK_FRAMES = 1,
		E_OP_HANDLE_FRAMEPACK,
		E_OP_PROCESS_FRAMEPACK,
		E_OP_UPDATE_GAME_LOGIC,
		E_OP_LOADING_PROGRESS,
		E_OP_RECONNECTION_PROGRESS,
		E_OP_OB_FASTFORWARD_PROGRESS = 15,
		E_OP_UPDATE_TAILS_OF_GAME_LOGIC = 7,
		E_OP_START_RECONNECTION = 17,
		E_OP_END_RECONNECTION,
		E_OP_PROCESS_FRAMEDROP = 20,
		E_OP_FETCH_RECONNECTION_FRAMES,
		E_OP_ON_EMPTY_SCENE_LOADED,
		E_OP_NOTIFY_GAMESTART_CTX,
		E_OP_OB_PLAY_STATE,
		E_OP_CONN_CONNECTING = 9,
		E_OP_CONN_CONNECTED,
		E_OP_CONN_CLOSING,
		E_OP_CONN_CLOSED,
		E_OP_CONN_ERROR,
		E_OP_CONN_CLOSE_BY_REMOTE,
		E_OP_NETMSG_SYSERROR,
		E_OP_NETMSG_GAMEERROR = 25,
		E_OP_ATTEMPT_RECONNECTION_PROCESSING_FINISHED,
		E_OP_NOTIFY_RECONNECT_TO_LOBBY,
		E_OP_NOTIFY_RECONNECT_TO_BATTLE,
		E_OP_NOTIFY_LOST_CONNECTION,
		E_OP_NOTIFY_PLAYER_PREPARE_STATE,
		E_OP_NOTIFY_PLAYER_CONFIRM_STATE,
		E_OP_NO_BATTLE_LASTED_ON_SERVER = 19
	}
}
