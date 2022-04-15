using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vMelee;
using Invector.vItemManager;
using System.Linq;

/// <summary>
/// バフ・デバフを実装する
/// </summary>
public class BuffSystem : MonoBehaviour
{
    private vItemManager itemManager;
    private vMeleeManager mm;
    private const int NOT_ENCHANT_ITEM = -1;
    // Start is called before the first frame update
    void Start()
    {
        mm = GetComponent<vMeleeManager>();
        itemManager = GetComponent<vItemManager>();
        if (!itemManager)
            return;
        itemManager.onUseItem.AddListener(BuffFromItem);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 使用されたアイテムの効果を適用する
    /// </summary>
    /// <param name="item">使用したアイテム</param>
    private void BuffFromItem(vItem item) {
        // 効果時間を取得
        int enchantTime = GetEnchantTime(item);
        if (enchantTime == NOT_ENCHANT_ITEM)
            return;

        // 効果タイプ取得のため、効果時間以外の属性を取得する
        vItemAttribute damageAttribute = item.attributes.First(attr => attr.name != vItemAttributes.EnchantTime);
        string customDamage = "";
        if (damageAttribute.name != vItemAttributes.Damage) {
            customDamage = damageAttribute.name.ToString();
        }
        
        EnchantWeapon(damageAttribute.name.ToString(), enchantTime, damageAttribute.value, customDamage, item.originalObject);
    }

    /// <summary>
    /// 装備している右手武器に対して、バフとエフェクトを付与する
    /// </summary>
    /// <param name="buffName">効果属性名</param>
    /// <param name="effectiveTime">効果時間</param>
    /// <param name="addDamage">追加ダメージ</param>
    /// <param name="customDamage">属性名（物理属性は""になる）</param>
    /// <param name="effect">武器が纏うエフェクトオブジェクト</param>
    private void EnchantWeapon(string buffName, float effectiveTime, int addDamage, string customDamage, GameObject effect) {
        if (!mm.rightWeapon)
            return;
        addDamage = (int)(mm.rightWeapon.damage.damageValue * ((float)addDamage / 100f));
        mm.rightWeapon.GetComponent<EnchantableWeapon>().Enchant(buffName, effectiveTime, addDamage, customDamage, effect);
    }

    /// <summary>
    /// 使用されたアイテムがバフアイテムか確認し、効果時間を取得する
    /// </summary>
    /// <param name="item">使用したアイテム</param>
    /// <returns>バフの効果時間</returns>
    private int GetEnchantTime(vItem item) {
        if (item.attributes.Count <= 0 || item.attributes[0].name != vItemAttributes.EnchantTime)
            return NOT_ENCHANT_ITEM;

        return item.attributes[0].value;
    }
}
