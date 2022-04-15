using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃エフェクトのリスト
/// </summary>
[CreateAssetMenu(fileName = "AttackSetList", menuName = "AttackSet/Create AttackSetList")]
public class AttackSetList : ScriptableObject
{
    public List<AttackSet> attackSets;
}
