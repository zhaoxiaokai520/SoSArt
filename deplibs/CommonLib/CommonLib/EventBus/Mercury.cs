using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MobaGo.EventBus
{
	public class Mercury : Singleton<Mercury>
	{
		public class MercuryEventBase : AbstractSmartObj
		{
			protected int _eventId;

			public int eventId
			{
				get
				{
					return this._eventId;
				}
				set
				{
					this._eventId = value;
				}
			}

			public MercuryEventBase()
			{
			}

			public MercuryEventBase(int evtId)
			{
				this._eventId = evtId;
			}

			public override void OnRelease()
			{
			}
		}

		public delegate void MecruryEventCallback(object sender, Mercury.MercuryEventBase e);

		private class Session : AbstractSmartObj
		{
			private int _sessionId = -1;

			[method: CompilerGenerated]
			[CompilerGenerated]
			private event Mercury.MecruryEventCallback evts;

			public int sessionId
			{
				get
				{
					return this._sessionId;
				}
			}

			public Session()
			{
				this._sessionId = Guid.NewGuid().GetHashCode();
			}

			public void AddListener(Mercury.MecruryEventCallback cb)
			{
				this.evts += cb;
			}

			public void RemoveListener(Mercury.MecruryEventCallback cb)
			{
				this.evts -= cb;
			}

			public void RemoveAllListeners()
			{
				if (this.evts != null)
				{
					Delegate[] invocationList = this.evts.GetInvocationList();
					for (int i = 0; i < invocationList.Length; i++)
					{
						this.evts -= (Mercury.MecruryEventCallback)invocationList[i];
					}
				}
			}

			public void Invoke(object sender, Mercury.MercuryEventBase e)
			{
				if (this.evts != null)
				{
					this.evts(sender, e);
				}
			}

			public override void OnRelease()
			{
				this.RemoveAllListeners();
			}
		}

		private Dictionary<int, Mercury.Session> _sessionSlots = new Dictionary<int, Mercury.Session>();

		private Mercury.Session _recentSession;

		public int AccquireSession()
		{
			Mercury.Session session = Singleton<SmartReferencePool>.instance.Fetch<Mercury.Session>(16, 4);
			this._sessionSlots.Add(session.sessionId, session);
			return session.sessionId;
		}

		public void ReleaseSession(int token)
		{
			if (this._recentSession != null && this._recentSession.sessionId == token)
			{
				this._recentSession.Release();
				this._recentSession = null;
				this._sessionSlots.Remove(token);
				return;
			}
			if (this._sessionSlots.ContainsKey(token))
			{
				this._sessionSlots[token].Release();
				this._sessionSlots.Remove(token);
			}
		}

		public void AddListener(int token, Mercury.MecruryEventCallback cb)
		{
			if (cb != null)
			{
				if (this._recentSession != null && this._recentSession.sessionId == token)
				{
					this._recentSession.AddListener(cb);
					return;
				}
				this._recentSession = this._sessionSlots[token];
				this._recentSession.AddListener(cb);
			}
		}

		public void RemoveListener(int token, Mercury.MecruryEventCallback cb)
		{
			if (cb != null)
			{
				if (this._recentSession != null && this._recentSession.sessionId == token)
				{
					this._recentSession.RemoveListener(cb);
					return;
				}
				this._recentSession = this._sessionSlots[token];
				this._recentSession.RemoveListener(cb);
			}
		}

		public void RemoveAllListeners(int token)
		{
			if (this._recentSession != null && this._recentSession.sessionId == token)
			{
				this._recentSession.RemoveAllListeners();
				return;
			}
			this._recentSession = this._sessionSlots[token];
			this._recentSession.RemoveAllListeners();
		}

		public void Broadcast(int token, object sender, Mercury.MercuryEventBase e)
		{
			if (this._recentSession == null || this._recentSession.sessionId != token)
			{
				this._recentSession = this._sessionSlots[token];
				if (this._recentSession != null)
				{
					this._recentSession.Invoke(sender, e);
				}
			}
			else
			{
				this._recentSession.Invoke(sender, e);
			}
			e.Release();
		}

		public override void UnInit()
		{
			foreach (KeyValuePair<int, Mercury.Session> current in this._sessionSlots)
			{
				current.Value.RemoveAllListeners();
			}
			this._recentSession = null;
		}
	}
}
