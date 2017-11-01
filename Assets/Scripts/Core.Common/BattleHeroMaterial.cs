using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleHeroMaterial : MonoBehaviour {
    public enum E_TYPE
    {
        E_NORMAL,
        E_HURT,
        E_HIDE,
        E_FROZEN,
        E_STONE,
        E_INVINCIBLE,
        E_DIED,

        E_MAX,
    };
    public Material[] mMatArray = new Material[(int)(E_TYPE.E_MAX)];
    public Renderer mRenderer;
    //public Dictionary<E_TYPE, Material> mHeroMatsMap = new Dictionary<E_TYPE, Material>(); 
    public Material GetMaterial(E_TYPE matType)
    {
        return mMatArray[(int)(matType)];
    }

    public void SetMaterial(E_TYPE tt, Material mat)
    {
        mMatArray[(int) (tt)] = mat;
    }

    public void Init()
    {
        mRenderer = null;
        mMatArray = null;
        mMatArray = new Material[(int)(E_TYPE.E_MAX)];
    }
}
