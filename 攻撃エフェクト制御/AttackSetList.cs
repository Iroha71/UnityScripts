using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �U���G�t�F�N�g�̃��X�g
/// </summary>
[CreateAssetMenu(fileName = "AttackSetList", menuName = "AttackSet/Create AttackSetList")]
public class AttackSetList : ScriptableObject
{
    public List<AttackSet> attackSets;
}
