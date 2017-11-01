// 
//  ParticleController.cs
//  
//  Author:
//    saviosun 
// 
// Purpose:
// 
// 
// 
//  Copyright (c) 16, 3, 2012
// 
//  All rights reserved.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour, IPooledMonoBehaviour
{
    public enum E_LV
    {
        E_HIGH,
        E_NORMAL,
        E_LOW,
    };

    public E_LV meLv = E_LV.E_NORMAL;

    public enum E_TYPE
    {
        E_SCROLL_UV,
        E_ROTATE_UV,
        E_NORMAL,
    };

    private List<E_TYPE> meTypes = new List<E_TYPE>();
    private Renderer[] mRenderer;
    private float mfTime = 0.0f;
    private float mfSpeed = 1.0f;

    void Awake()
    {
        ParticleManage.GetInstance().AddParticle(this);
        Init();
    }

    //void OnEnable()
    //{
    //    Reset();
    //}

    public void Init()
    {
        meTypes.Clear();
        mRenderer = GetComponentsInChildren<Renderer>();
        if (mRenderer == null || mRenderer.Length == 0)
            return;

        string shaderName = "";
        for (int i = 0; i < mRenderer.Length; i++)
        {
            shaderName = mRenderer[i].sharedMaterial.shader.name.ToLower();
            if (shaderName.Contains("scroll"))
            {
                meTypes.Add(E_TYPE.E_SCROLL_UV);
            }
            else if (shaderName.Contains("rotateuv"))
            {
                meTypes.Add(E_TYPE.E_ROTATE_UV);
            }
            else
            {
                meTypes.Add(E_TYPE.E_NORMAL);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < mRenderer.Length; i++)
        {
            if (meTypes[i] == E_TYPE.E_SCROLL_UV || meTypes[i] == E_TYPE.E_ROTATE_UV)
            {
                mRenderer[i].material.SetFloat("_ParticleTime", mfTime);
            }
        }
        mfTime += Time.deltaTime*mfSpeed;
    }

    public float _Time
    {
        get { return mfTime; }
        set { mfTime = value; }
    }

    public float _Speed
    {
        get { return mfSpeed; }
        set { mfSpeed = value; }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(20.0f, 60.0f, 100.0f, 40.0f), "Reset"))
        {
            mfTime = 0.0f;
        }
    }
#endif

    public void OnCreate()
    {
        
    }

    public void OnGet()
    {
        mfTime = 0.0f;
        mfSpeed = 1.0f;
    }

    public void OnRecycle()
    {
        mfTime = 0.0f;
        mfSpeed = 1.0f;
    }
}