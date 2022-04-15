using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create Ability")]
public class Ability : ScriptableObject
{
    public string animName;
    public int slotTimeMs = 100;
    public int id;
    public GameObject effect;
    public int useAP;
}