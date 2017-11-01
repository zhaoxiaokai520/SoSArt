using UnityEngine;
using System.Collections.Generic;

public class ParticleManage : MonoSingleton<ParticleManage>
{
    List<ParticleController> mContainer = new List<ParticleController>();
    public int miMaxCount = 30;

    public void AddParticle(ParticleController pp)
    {
        mContainer.Add(pp);
    }

    public void RemoveParticle(ParticleController pp)
    {
        mContainer.Remove(pp);
    }
}
