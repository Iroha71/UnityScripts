using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃時のエフェクト発生位置・エフェクトの種類を定義する
/// </summary>
[CreateAssetMenu(fileName = "AttackSet", menuName = "AttackSet/Create AttackSet")]
public class AttackSet : ScriptableObject
{
    public enum AttackID { AXE = 1, KATANA = 2 };
    public AttackID attackID;
    public string animName;
    public Vector3 offset;
    public Vector3 rotationOffset;
    public GameObject effect;
}
