using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Linq;
using Invector.vItemManager;

/// <summary>
/// �A�r���e�B����V�X�e���i�A�C�e���ɐݒ肵��AbilityID����A�r���e�B�����肷��j
/// </summary>
public class AblitySystem : MonoBehaviour
{
    [SerializeField] private int ap;
    public int AP { get { return ap; } }
    public int maxAP = 100;
    private GenericInput ablityInput = new GenericInput("Space", "X", "X");
    private vShooterMeleeInput tpinput;
    private Animator anim;
    private ParrySystem parrySystem;
    [HideInInspector] public UnityEvent<int> OnChangeAP = new UnityEvent<int>();
    [SerializeField] private float slowRate = 0.1f;
    [SerializeField] private int slowTimeMs = 200;
    [SerializeField] private AbilityList abilityList;
    private Ability currentAbility;
    private vItemManager itemManager;
    [SerializeField] private Transform abilityEffectPoint;

    public int AbilityID { get { return AbilityID; } set { anim.SetInteger("AbilityID", value); } }
    // Start is called before the first frame update
    void Start()
    {
        
        tpinput = GetComponent<vShooterMeleeInput>();
        anim = GetComponent<Animator>();
        parrySystem = GetComponent<ParrySystem>();
        itemManager = GetComponent<vItemManager>();
        itemManager.onEquipItem.AddListener(SetCurrentAbility);
        OnChangeAP.Invoke(ap);
    }

    // Update is called once per frame
    void Update()
    {
        // �K�[�h���ɃA�r���e�B�{�^�������Ŕ���
        if (tpinput.isBlocking && ablityInput.GetButtonDown() && currentAbility != null) {
            IgniteAbility();
        }
    }

    /// <summary>
    /// �A�r���e�B�A�j���[�V�������Đ�����
    /// </summary>
    private void IgniteAbility() {
        if (ap <= 0)
            return;
        AddAP(-currentAbility.useAP);
        anim.SetTrigger("AbilityAttack");
    }

    /// <summary>
    /// AP�����Z����
    /// </summary>
    /// <param name="value">���Z����AP</param>
    public void AddAP(int value) {
        int tmp = ap + value;
        ap = Mathf.Clamp(tmp, 0, maxAP);
        OnChangeAP.Invoke(ap);
    }

    /// <summary>
    /// ��莞�ԃX���[�ɂ���
    /// </summary>
    /// <param name="animEvent">�A�j���[�V�����C�x���g��</param>
    public async void SwitchSlowMotion(string animEvent) {
        Time.timeScale = slowRate;
        await UniTask.Delay(slowTimeMs);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// �g�p���A�r���e�B���X�V����
    /// </summary>
    /// <param name="equipArea">�A�C�e���X���b�g���</param>
    /// <param name="item">�Z�b�g���ꂽ�A�r���e�B�A�C�e��</param>
    private void SetCurrentAbility(vEquipArea equipArea, vItem item) {
        if (item.type != vItemType.Ability)
            return;
        int abilityID = item.GetItemAttribute(vItemAttributes.AbilityID).value;
        currentAbility = abilityList.abilityList[abilityID];
        AbilityID = currentAbility.id;
    }

    /// <summary>
    /// �A�r���e�B�p�G�t�F�N�g�𔭐�������i�A�j���[�V�����C�x���g�j
    /// </summary>
    /// <param name="animEvent">�A�j���[�V�����C�x���g��</param>
    public void IgniteAbilityEffect(string animEvent) {
        GameObject _effect = Instantiate(currentAbility.effect, abilityEffectPoint.position, abilityEffectPoint.rotation);
    }
}
