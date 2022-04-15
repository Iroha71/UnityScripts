using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// �U���A�j���[�V�����ɍ��킹�ăG�t�F�N�g�𔭐�������
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
    /// �U���G�t�F�N�g�𐶐�����
    /// </summary>
    public void CreateSlashEffect() {
        AttackSet attackset = GetCurrentAttackSet();
        GameObject _effect = Instantiate(attackset.effect, transform);
        _effect.transform.localPosition = attackset.offset;
        _effect.transform.localEulerAngles = attackset.rotationOffset;
#if UNITY_EDITOR
        // �G�t�F�N�g�ʒu�����̂��߁A����̍U�����ɃG�f�B�^���~����
        if (isDebugging && anim.GetCurrentAnimatorStateInfo(FULLBODY_LAYER).IsName(debugState))
            UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    /// <summary>
    /// �Đ����̃A�j���[�V���������Ƃɐ����Ώۂ̃G�t�F�N�g���擾����
    /// </summary>
    /// <returns>�Đ��\��̃G�t�F�N�g</returns>
    private AttackSet GetCurrentAttackSet() {
        AttackSet currentSet = attackSetList.attackSets.Find(attackSet => anim.GetCurrentAnimatorStateInfo(FULLBODY_LAYER).IsName(attackSet.animName)
                                                             && anim.GetInteger("AttackID") == (int)attackSet.attackID);

        return currentSet;
    }
}
