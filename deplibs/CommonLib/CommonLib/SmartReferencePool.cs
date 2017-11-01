using System;
using System.Collections.Generic;

public class SmartReferencePool : Singleton<SmartReferencePool>
{
	public class PoolHandle
	{
		private int _fetchIndex;

		private ISmartObj[] _dataObjList;

		private byte[] _stateList;

		private int _length;

		private int _allocGran;

		private Type _classT;

		public bool IsValid
		{
			get
			{
				return this._length > 0;
			}
		}

		public PoolHandle(Type classT, int size, int allocGran)
		{
			this._length = size;
			this._allocGran = allocGran;
			this._classT = classT;
			if (this._length > 0)
			{
				this._dataObjList = new ISmartObj[this._length];
				this._stateList = new byte[this._length];
				for (int i = 0; i < this._length; i++)
				{
					ISmartObj smartObj = (ISmartObj)Activator.CreateInstance(classT);
					if (smartObj == null)
					{
						this._dataObjList = null;
						this._length = 0;
						this._stateList = null;
						return;
					}
					this._stateList[i] = 0;
					this._dataObjList[i] = smartObj;
					smartObj.index = i;
					smartObj.handle = this;
				}
			}
		}

		public ISmartObj Fetch()
		{
			if (this._length > 0)
			{
				int num = 0;
				while (num < this._length && this._stateList[this._fetchIndex] != 0)
				{
					this._fetchIndex = (this._fetchIndex + 1) % this._length;
					num++;
				}
				if (this._stateList[this._fetchIndex] == 0)
				{
					int fetchIndex = this._fetchIndex;
					this._stateList[this._fetchIndex] = 1;
					this._fetchIndex = (this._fetchIndex + 1) % this._length;
					this._dataObjList[fetchIndex].Reset();
					return this._dataObjList[fetchIndex];
				}
				this.Grow();
				if (this._stateList[this._fetchIndex] == 0)
				{
					int fetchIndex2 = this._fetchIndex;
					this._stateList[this._fetchIndex] = 1;
					this._fetchIndex = (this._fetchIndex + 1) % this._length;
					this._dataObjList[fetchIndex2].Reset();
					return this._dataObjList[fetchIndex2];
				}
			}
			return null;
		}

		private void Grow()
		{
			int length = this._length;
			this._length += this._allocGran;
			ISmartObj[] array = new ISmartObj[this._length];
			byte[] array2 = new byte[this._length];
			Array.Copy(this._dataObjList, array, length);
			Array.Copy(this._stateList, array2, length);
			this._fetchIndex = length;
			for (int i = length; i < this._length; i++)
			{
				ISmartObj smartObj = (ISmartObj)Activator.CreateInstance(this._classT);
				array[i] = smartObj;
				array2[i] = 0;
				smartObj.index = i;
				smartObj.handle = this;
			}
			this._stateList = array2;
			this._dataObjList = array;
		}

		public void Release(int index)
		{
			if (index >= 0 && index < this._length)
			{
				this._stateList[index] = 0;
			}
		}
	}

	private Dictionary<Type, SmartReferencePool.PoolHandle> _objectPool = new Dictionary<Type, SmartReferencePool.PoolHandle>();

	public void CreatePool<T>(int autoCreateSize = 4, int allocGran = 4) where T : ISmartObj
	{
		Type typeFromHandle = typeof(T);
		this.CreatePool(typeFromHandle, autoCreateSize, allocGran);
	}

	public void CreatePool(Type classT, int autoCreateSize = 32, int allocGran = 4)
	{
		if (classT != null && autoCreateSize > 0 && !this._objectPool.ContainsKey(classT))
		{
			SmartReferencePool.PoolHandle value = new SmartReferencePool.PoolHandle(classT, autoCreateSize, allocGran);
			this._objectPool.Add(classT, value);
		}
	}

	public T Fetch<T>(int autoCreateSize = 32, int allocGran = 4) where T : ISmartObj
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle != null)
		{
			SmartReferencePool.PoolHandle poolHandle;
			if (this._objectPool.ContainsKey(typeFromHandle))
			{
				poolHandle = this._objectPool[typeFromHandle];
			}
			else
			{
				poolHandle = new SmartReferencePool.PoolHandle(typeFromHandle, autoCreateSize, allocGran);
				this._objectPool.Add(typeFromHandle, poolHandle);
			}
			return (T)((object)poolHandle.Fetch());
		}
		return default(T);
	}
}
