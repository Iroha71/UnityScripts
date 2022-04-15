using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vMelee;
using System.Linq;
using System;

/// <summary>
/// 武器にエンチャントを可能にする
/// </summary>
public class EnchantableWeapon : MonoBehaviour
{
    private vMeleeWeapon weapon;
    private List<BuffInfo> addedBuffs = new List<BuffInfo>();
    private const int NOT_FOUND_SAME_BUFF = -1;

    private AnimEffectController animEffectController;
    // Start is called before the first frame update
    void Start()
    {
        animEffectController = GetComponentInParent<AnimEffectController>();
        weapon = GetComponent<vMeleeWeapon>();
        if (animEffectController)
            weapon.onEnableDamage.AddListener(animEffectController.CreateSlashEffect);
    }

    // Update is called once per frame
    void Update()
    {
        if (addedBuffs.Count > 0) {
            for (int i = 0; i < addedBuffs.Count; i++) {
                bool isFinishedEffect = addedBuffs[i].AddElapsedTime(Time.deltaTime);
                if (isFinishedEffect) {
                    if (addedBuffs[i].EnchantEffect)
                        Destroy(addedBuffs[i].EnchantEffect);
                    AddModifedDamage(-addedBuffs[i].AddDamage);
                    addedBuffs.Remove(addedBuffs[i]);
                }
            }
        }
    }

    /// <summary>
    /// 武器に追加ダメージを設定
    /// </summary>
    /// <param name="damage">追加ダメージ</param>
    /// <param name="customDamage">カスタム属性</param>
    private void AddModifedDamage (int damage, string customDamage="") {
        weapon.damageModifier += damage;
        weapon.damage.damageType = damage <= 0 ? "" : customDamage;
    }

    /// <summary>
    /// 武器に追加効果を付与する
    /// </summary>
    /// <param name="buffName">追加効果名</param>
    /// <param name="effectiveTime">効果時間</param>
    /// <param name="addDamage">追加ダメージ</param>
    /// <param name="customDamage">カスタム属性</param>
    /// <param name="effect">武器が纏うエフェクト</param>
    public void Enchant(string buffName, float effectiveTime, int addDamage, string customDamage, GameObject effect=null) {
        int sameIndex = GetSameBuffIndex(buffName);
        if (sameIndex == NOT_FOUND_SAME_BUFF) {
            BuffInfo newBuff = new BuffInfo(effectiveTime, addDamage);
            newBuff.EnchantEffect = AddMeshEffect(effect);
            addedBuffs.Add(newBuff);
        } else {
            addedBuffs[sameIndex].EffectiveTime += effectiveTime;
            addedBuffs[sameIndex].AddDamage = addDamage;
        }

        AddModifedDamage(addDamage, customDamage);
    }

    /// <summary>
    /// 武器にすでに同じ効果がついているか確認する
    /// </summary>
    /// <param name="buffName">重複しているか確認したい追加効果名</param>
    /// <returns>重複している効果の要素番号</returns>
    private int GetSameBuffIndex(string buffName) {
        try {
            int sameIndex = addedBuffs.FindIndex(buff => buff.Name.Equals(buffName));

            return sameIndex;
        } catch (InvalidOperationException e) {
            return NOT_FOUND_SAME_BUFF;
        }
    }

    /// <summary>
    /// 武器モデルにエフェクトを付与する
    /// </summary>
    /// <param name="effect">付与するエフェクト</param>
    /// <returns>モデルに生成されたエフェクト</returns>
    private GameObject AddMeshEffect(GameObject effect) {
        if (effect == null)
            return null;
        GameObject _effect = Instantiate(effect, transform);
        PSMeshRendererUpdater meshUpdater = _effect.GetComponent<PSMeshRendererUpdater>();
        if (!meshUpdater)
            return null;
        meshUpdater.UpdateMeshEffect(gameObject);

        return _effect;
    }
}

/// <summary>
/// バフ情報を定義する
/// </summary>
public class BuffInfo {
    public string Name { get; set; }
    public int AddDamage { get; set; }
    public float EffectiveTime { get; set; }
    private float elapsedTime = 0f;
    public string CustomDamage { get; set; }
    public GameObject EnchantEffect { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="effectiveTime">バフ効果時間</param>
    /// <param name="addDamage">追加効果ダメージ</param>
    public BuffInfo(float effectiveTime, int addDamage) {
        EffectiveTime = effectiveTime;
        AddDamage = addDamage;
    }

    /// <summary>
    /// バフの経過時間を加算する
    /// </summary>
    /// <param name="addElapsedTime">加算する経過時間</param>
    /// <returns>効果時間が終了したかどうか</returns>
    public bool AddElapsedTime(float addElapsedTime) {
        elapsedTime += addElapsedTime;
        if (elapsedTime > EffectiveTime) {
            elapsedTime = 0f;
            return true;
        }

        return false;
    }
}
