using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vMelee;
using System.Linq;
using System;

/// <summary>
/// ����ɃG���`�����g���\�ɂ���
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
    /// ����ɒǉ��_���[�W��ݒ�
    /// </summary>
    /// <param name="damage">�ǉ��_���[�W</param>
    /// <param name="customDamage">�J�X�^������</param>
    private void AddModifedDamage (int damage, string customDamage="") {
        weapon.damageModifier += damage;
        weapon.damage.damageType = damage <= 0 ? "" : customDamage;
    }

    /// <summary>
    /// ����ɒǉ����ʂ�t�^����
    /// </summary>
    /// <param name="buffName">�ǉ����ʖ�</param>
    /// <param name="effectiveTime">���ʎ���</param>
    /// <param name="addDamage">�ǉ��_���[�W</param>
    /// <param name="customDamage">�J�X�^������</param>
    /// <param name="effect">���킪�Z���G�t�F�N�g</param>
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
    /// ����ɂ��łɓ������ʂ����Ă��邩�m�F����
    /// </summary>
    /// <param name="buffName">�d�����Ă��邩�m�F�������ǉ����ʖ�</param>
    /// <returns>�d�����Ă�����ʂ̗v�f�ԍ�</returns>
    private int GetSameBuffIndex(string buffName) {
        try {
            int sameIndex = addedBuffs.FindIndex(buff => buff.Name.Equals(buffName));

            return sameIndex;
        } catch (InvalidOperationException e) {
            return NOT_FOUND_SAME_BUFF;
        }
    }

    /// <summary>
    /// ���탂�f���ɃG�t�F�N�g��t�^����
    /// </summary>
    /// <param name="effect">�t�^����G�t�F�N�g</param>
    /// <returns>���f���ɐ������ꂽ�G�t�F�N�g</returns>
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
/// �o�t�����`����
/// </summary>
public class BuffInfo {
    public string Name { get; set; }
    public int AddDamage { get; set; }
    public float EffectiveTime { get; set; }
    private float elapsedTime = 0f;
    public string CustomDamage { get; set; }
    public GameObject EnchantEffect { get; set; }

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="effectiveTime">�o�t���ʎ���</param>
    /// <param name="addDamage">�ǉ����ʃ_���[�W</param>
    public BuffInfo(float effectiveTime, int addDamage) {
        EffectiveTime = effectiveTime;
        AddDamage = addDamage;
    }

    /// <summary>
    /// �o�t�̌o�ߎ��Ԃ����Z����
    /// </summary>
    /// <param name="addElapsedTime">���Z����o�ߎ���</param>
    /// <returns>���ʎ��Ԃ��I���������ǂ���</returns>
    public bool AddElapsedTime(float addElapsedTime) {
        elapsedTime += addElapsedTime;
        if (elapsedTime > EffectiveTime) {
            elapsedTime = 0f;
            return true;
        }

        return false;
    }
}
