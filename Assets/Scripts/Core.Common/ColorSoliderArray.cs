using UnityEngine;
using System.Collections.Generic;

public class ColorSoliderArray : MonoBehaviour
{
    public List<int> configList = new List<int>();
    public Material[] SoliderRedArray = new Material[(int)(BattleHeroMaterial.E_TYPE.E_MAX)];
    public Material[] SoliderBlueArray = new Material[(int)(BattleHeroMaterial.E_TYPE.E_MAX)];
}
