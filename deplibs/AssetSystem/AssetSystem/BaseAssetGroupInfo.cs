using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BaseAssetGroupInfo
{
    private eAssetBundleState m_assetBundleState = eAssetBundleState.Unload;   
    private bool m_bUnLoadAssets = false;
    private bool m_bUsed = false;//外部使用
    private eAssetBundleState m_parentsBundleState = eAssetBundleState.Unload;

    internal AssetBundle m_assetBundle = null;
    protected AssetBundleCreateRequest m_AsyncLoadingOpe = null;


    protected CUtilList<BaseAssetGroupInfo> m_parentbundles = new CUtilList<BaseAssetGroupInfo>();
    protected CUtilList<BaseAssetGroupInfo> m_childbundles = new CUtilList<BaseAssetGroupInfo>();

    internal string m_pathInIFS;

    protected int m_iChildRefCount = 0;

   

    internal void AddParentBundle(BaseAssetGroupInfo assetbundle_t)
    {
        m_parentbundles.Add(assetbundle_t);
        assetbundle_t.AddChildBundle(this);
    }
    

    internal void AddChildBundle(BaseAssetGroupInfo assetbundle_t)
    {
        m_childbundles.Add(assetbundle_t);
    }

    protected void Use(string ifsExtractPath,bool bAysnc)
    {
        m_bUsed = true;

        if (bAysnc)
            AsyncLoad(ifsExtractPath);
        else
            Load(ifsExtractPath);
    }

    bool ShouldUnLoad(bool bcheckref)
    {
        return (bcheckref ? m_iChildRefCount == 0:true) && !m_bUsed;
    }

    void Load(string ifsExtractPath)
    {
        for (int i = 0; i < m_parentbundles.Count; i++)
        {
            m_parentbundles[i].Load(ifsExtractPath);           
        }

        if (BundleState == eAssetBundleState.Loaded || BundleState == eAssetBundleState.SelfLoaded)
        {
            return;
        }
#if DEBUG_LOGOUT
        DebugHelper.Log("Bundle Load: " + m_pathInIFS);
#endif
        if (BundleState == eAssetBundleState.Loading)
        {
            DebugHelper.LogError("Bundle on Loading,so load abort: " + m_pathInIFS);
            return;
        }

        m_bUnLoadAssets = false;

        DoLoad(ifsExtractPath);
    }

    void AsyncLoad(string ifsExtractPath)
    {
        for (int i = 0; i < m_parentbundles.Count; i++)
        {
            m_parentbundles[i].AsyncLoad(ifsExtractPath);
        }

        if (BundleState == eAssetBundleState.Loaded || BundleState == eAssetBundleState.SelfLoaded)
        {
            return;
        }

        if (BundleState == eAssetBundleState.Loading)
        {
            return;
        }

        DoAsyncLoad(ifsExtractPath);

    }
    protected void Unload(bool unloadAssets,bool checkref)
    {
        m_bUnLoadAssets = unloadAssets;

        m_bUsed = false;
        Unload(checkref);
    }

    private void Unload(bool checkref)
    {
        if(BundleState == eAssetBundleState.Loaded || BundleState == eAssetBundleState.SelfLoaded)
        {
            if (ShouldUnLoad(checkref))
                DoUnload(m_bUnLoadAssets);
        }else if(BundleState == eAssetBundleState.Loading)
        {
        }
    }

    protected eAssetBundleState ParentBundleState
    {
        get
        {
            return m_parentsBundleState;
        }
    }
    void SetBundleState(eAssetBundleState state)
    {
        if (m_assetBundleState != state)
        {
            m_assetBundleState = state;
            if (m_assetBundleState == eAssetBundleState.Unload)
            {
                m_assetBundle = null;
                m_AsyncLoadingOpe = null;
                OnUnLoad();
            }
            else if (m_assetBundleState == eAssetBundleState.Loaded)
            {
                OnLoaded();
            }
            else if (m_assetBundleState == eAssetBundleState.Loading)
            {
                OnLoading();
            }
        }
    }
    protected eAssetBundleState BundleState
    {
        get
        {
            return m_assetBundleState;
        }      
    }

    private eAssetBundleState ParentLoadState()
    {
        bool AllLoaded = true;
        for (int i = 0; i < m_parentbundles.Count; i++)
        {
            if ( m_parentbundles[i].BundleState == eAssetBundleState.Unload)
            {
                return eAssetBundleState.Unload;
            }
            else if (m_parentbundles[i].BundleState == eAssetBundleState.Loading ||
                m_parentbundles[i].BundleState == eAssetBundleState.SelfLoaded)
            {
                AllLoaded = false;
            }
        }
        if (AllLoaded)
            return eAssetBundleState.Loaded;
        return eAssetBundleState.Loading;
    }

    //private bool IsChildAllUnLoad()
    //{
    //    return m_iChildRefCount == 0;
    //    //for (int i = 0; i < m_childbundles.Count; i++)
    //    //{
    //    //    if (m_childbundles[i].BundleState != eAssetBundleState.Unload)
    //    //    {
    //    //        return false;
    //    //    }
    //    //}
    //    //return true;
    //}

    void AddChildRef()
    {
        m_iChildRefCount++;
    }

    void ReleaseChildRef()
    {
        m_iChildRefCount--;
        if(m_iChildRefCount==0)
        {
            if(!m_bUsed)
            {//auto UnLoad
                if (BundleState == eAssetBundleState.Loaded || BundleState == eAssetBundleState.SelfLoaded)
                    DoUnload(m_bUnLoadAssets);
            }
        }else if(m_iChildRefCount<0)
        {
            DebugHelper.LogError("ChildRef error " + m_pathInIFS + " count:" + m_iChildRefCount);
        }
    }

    void UpdateStateByParentState()
    {
        m_parentsBundleState = ParentLoadState();
        if (m_parentsBundleState == eAssetBundleState.Loaded)
            SetBundleState(eAssetBundleState.Loaded);
    }
    protected void OnBundleUnLoadResult()
    {
        SetBundleState(eAssetBundleState.Unload);
    }
    protected void OnBundleLoadResult(AssetBundle bundle)
    {
        if (bundle != null)
        {
            bool bfromasync = BundleState == eAssetBundleState.Loading;
            SetBundleState(eAssetBundleState.SelfLoaded);
            m_assetBundle = bundle;

            if (bfromasync && ShouldUnLoad(true))
            {
                DoUnload(false);
                return;
            }

            UpdateStateByParentState();
        }
        else
        {
            SetBundleState(eAssetBundleState.Unload);
        }
    }

    protected void OnAsyncBundleLoadResult(AssetBundleCreateRequest loadreq)
    {
        if (loadreq != null)
        {
            SetBundleState(eAssetBundleState.Loading);
            m_AsyncLoadingOpe = loadreq;
        }
        else
        {
            SetBundleState(eAssetBundleState.Unload);
        }
    }

    protected virtual void OnUnLoad()
    {
        m_assetBundle = null;
        m_AsyncLoadingOpe = null;
        for (int i = 0; i < m_childbundles.Count; ++i)
        {
            m_childbundles[i].OnParentUnLoad();
        }

        for (int i = 0; i < m_parentbundles.Count; ++i)
        {
            m_parentbundles[i].OnChildUnLoad();
        }
    }

    protected virtual void OnLoaded()
    {
        for (int i = 0; i < m_childbundles.Count; ++i)
        {
            m_childbundles[i].OnParentLoaded();
        }

        for (int i = 0; i < m_parentbundles.Count; ++i)
        {
            m_parentbundles[i].OnChildLoaded();
        }
    }

    protected virtual void OnLoading()
    {
              
        for (int i = 0; i < m_childbundles.Count; ++i)
        {
            m_childbundles[i].OnParentLoading();
        }

        for (int i = 0; i < m_parentbundles.Count; ++i)
        {
            m_parentbundles[i].OnChildLoading();
        }
    }

    void OnParentLoaded()
    {        
        if (BundleState == eAssetBundleState.SelfLoaded)
        {
            UpdateStateByParentState();
        }
    }

    void OnParentLoading()
    {
        if (BundleState == eAssetBundleState.Loaded)
        {
            if (m_assetBundle != null)
            {
                SetBundleState(eAssetBundleState.SelfLoaded);
            }
            else
            {
                SetBundleState(eAssetBundleState.Unload);
            }
        }
    }

    void OnParentUnLoad()
    {
        if (BundleState == eAssetBundleState.Loaded)
        {
            if (m_assetBundle != null)
            {
                SetBundleState(eAssetBundleState.SelfLoaded);
            }
            else
            {
                SetBundleState(eAssetBundleState.Unload);
            }
        }
    }

    void OnChildUnLoad()
    {
        ReleaseChildRef();       
    }

    void OnChildLoaded()
    {
        AddChildRef();
    }

    void OnChildLoading()
    {
        AddChildRef();
    }

    protected virtual void DoLoad(string ifsExtractPath)
    {

    }

    protected virtual void DoAsyncLoad(string ifsExtractPath)
    {

    }

    protected virtual void DoUnload(bool bunloadAssets)
    {

    }

    internal void UpdateAsyncLoad()
    {
        if(BundleState == eAssetBundleState.Loading)
        {
            if (m_AsyncLoadingOpe != null)
            {
                if (m_AsyncLoadingOpe.isDone)
                {
                    OnBundleLoadResult(m_AsyncLoadingOpe.assetBundle);

#if DEBUG_LOGOUT
                    DebugHelper.LogOutResouceLoad("==PRINT_LOAD== Load Async finish " + m_pathInIFS);
#endif
                }
            }
            else
            {
                m_assetBundleState = eAssetBundleState.Unload;
            }
        }else if(BundleState == eAssetBundleState.SelfLoaded)
        {
            UpdateStateByParentState();
        }
    }

    internal bool CheckDependencyLoop(BaseAssetGroupInfo root)
    {
        if (root == this)
        {
            DebugHelper.LogError("Find loop dependecy bundle " + m_pathInIFS);
            return false;
        }

        if (root == null)
            root = this;


        for (int s = 0; s < m_parentbundles.Count; s++)
        {
            if (!m_parentbundles[s].CheckDependencyLoop(root))
                return false;
        }
        return true;
    }

    internal bool hasCheckStated = false;
    internal void CheckAndFixBundleLoadedState()
    {
        if (hasCheckStated)
            return;
        hasCheckStated = true;
        if (BundleState == eAssetBundleState.Unload)
            return;
        if (BundleState == eAssetBundleState.Loading)
        {
            DebugHelper.LogError("can't reach code " + m_pathInIFS);
            return;
        }
        for (int i = 0; i < m_parentbundles.Count; i++)
        {
            m_parentbundles[i].CheckAndFixBundleLoadedState();
            if (m_parentbundles[i].BundleState != eAssetBundleState.Loaded)
            {
                DebugHelper.LogError("Parent not loaded: " + m_parentbundles[i].m_pathInIFS + "me: " + m_pathInIFS);
                if (m_assetBundle != null)
                {
                    //miRefCnt = 0;
#if DEBUG_LOGOUT
                    DebugHelper.LogOutResouceLoad("===AssetGroupInfo_t UnLoad Asset Bundle at UpdateLoadedState: " + m_pathInIFS);
#endif
                    m_assetBundle.Unload(false);
                }
                SetBundleState(eAssetBundleState.Unload);
                return;
            }
        }
    }
}