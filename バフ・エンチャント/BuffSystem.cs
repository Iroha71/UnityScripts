using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vMelee;
using Invector.vItemManager;
using System.Linq;

/// <summary>
/// �o�t�E�f�o�t����������
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
    /// �g�p���ꂽ�A�C�e���̌��ʂ�K�p����
    /// </summary>
    /// <param name="item">�g�p�����A�C�e��</param>
    private void BuffFromItem(vItem item) {
        // ���ʎ��Ԃ��擾
        int enchantTime = GetEnchantTime(item);
        if (enchantTime == NOT_ENCHANT_ITEM)
            return;

        // ���ʃ^�C�v�擾�̂��߁A���ʎ��ԈȊO�̑������擾����
        vItemAttribute damageAttribute = item.attributes.First(attr => attr.name != vItemAttributes.EnchantTime);
        string customDamage = "";
        if (damageAttribute.name != vItemAttributes.Damage) {
            customDamage = damageAttribute.name.ToString();
        }
        
        EnchantWeapon(damageAttribute.name.ToString(), enchantTime, damageAttribute.value, customDamage, item.originalObject);
    }

    /// <summary>
    /// �������Ă���E�蕐��ɑ΂��āA�o�t�ƃG�t�F�N�g��t�^����
    /// </summary>
    /// <param name="buffName">���ʑ�����</param>
    /// <param name="effectiveTime">���ʎ���</param>
    /// <param name="addDamage">�ǉ��_���[�W</param>
    /// <param name="customDamage">�������i����������""�ɂȂ�j</param>
    /// <param name="effect">���킪�Z���G�t�F�N�g�I�u�W�F�N�g</param>
    private void EnchantWeapon(string buffName, float effectiveTime, int addDamage, string customDamage, GameObject effect) {
        if (!mm.rightWeapon)
            return;
        addDamage = (int)(mm.rightWeapon.damage.damageValue * ((float)addDamage / 100f));
        mm.rightWeapon.GetComponent<EnchantableWeapon>().Enchant(buffName, effectiveTime, addDamage, customDamage, effect);
    }

    /// <summary>
    /// �g�p���ꂽ�A�C�e�����o�t�A�C�e�����m�F���A���ʎ��Ԃ��擾����
    /// </summary>
    /// <param name="item">�g�p�����A�C�e��</param>
    /// <returns>�o�t�̌��ʎ���</returns>
    private int GetEnchantTime(vItem item) {
        if (item.attributes.Count <= 0 || item.attributes[0].name != vItemAttributes.EnchantTime)
            return NOT_ENCHANT_ITEM;

        return item.attributes[0].value;
    }
}
