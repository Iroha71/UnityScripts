using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 攻撃アニメーションに合わせてエフェクトを発生させる
/// </summary>
public class AnimEffectController : MonoBehaviour
{
    [SerializeField] private AttackSetList attackSetList;
    private Animator anim;
    private const int FULLBODY_LAYER = 7;
    public bool isDebugging = false;
    public string debugState = "";
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 攻撃エフェクトを生成する
    /// </summary>
    public void CreateSlashEffect() {
        AttackSet attackset = GetCurrentAttackSet();
        GameObject _effect = Instantiate(attackset.effect, transform);
        _effect.transform.localPosition = attackset.offset;
        _effect.transform.localEulerAngles = attackset.rotationOffset;
#if UNITY_EDITOR
        // エフェクト位置調整のため、特定の攻撃時にエディタを停止する
        if (isDebugging && anim.GetCurrentAnimatorStateInfo(FULLBODY_LAYER).IsName(debugState))
            UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    /// <summary>
    /// 再生中のアニメーションをもとに生成対象のエフェクトを取得する
    /// </summary>
    /// <returns>再生予定のエフェクト</returns>
    private AttackSet GetCurrentAttackSet() {
        AttackSet currentSet = attackSetList.attackSets.Find(attackSet => anim.GetCurrentAnimatorStateInfo(FULLBODY_LAYER).IsName(attackSet.animName)
                                                             && anim.GetInteger("AttackID") == (int)attackSet.attackID);

        return currentSet;
    }
}
