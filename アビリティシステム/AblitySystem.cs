using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Linq;
using Invector.vItemManager;

/// <summary>
/// アビリティ制御システム（アイテムに設定したAbilityIDからアビリティを決定する）
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
        // ガード時にアビリティボタン押下で発動
        if (tpinput.isBlocking && ablityInput.GetButtonDown() && currentAbility != null) {
            IgniteAbility();
        }
    }

    /// <summary>
    /// アビリティアニメーションを再生する
    /// </summary>
    private void IgniteAbility() {
        if (ap <= 0)
            return;
        AddAP(-currentAbility.useAP);
        anim.SetTrigger("AbilityAttack");
    }

    /// <summary>
    /// APを加算する
    /// </summary>
    /// <param name="value">加算するAP</param>
    public void AddAP(int value) {
        int tmp = ap + value;
        ap = Mathf.Clamp(tmp, 0, maxAP);
        OnChangeAP.Invoke(ap);
    }

    /// <summary>
    /// 一定時間スローにする
    /// </summary>
    /// <param name="animEvent">アニメーションイベント名</param>
    public async void SwitchSlowMotion(string animEvent) {
        Time.timeScale = slowRate;
        await UniTask.Delay(slowTimeMs);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 使用中アビリティを更新する
    /// </summary>
    /// <param name="equipArea">アイテムスロット情報</param>
    /// <param name="item">セットされたアビリティアイテム</param>
    private void SetCurrentAbility(vEquipArea equipArea, vItem item) {
        if (item.type != vItemType.Ability)
            return;
        int abilityID = item.GetItemAttribute(vItemAttributes.AbilityID).value;
        currentAbility = abilityList.abilityList[abilityID];
        AbilityID = currentAbility.id;
    }

    /// <summary>
    /// アビリティ用エフェクトを発生させる（アニメーションイベント）
    /// </summary>
    /// <param name="animEvent">アニメーションイベント名</param>
    public void IgniteAbilityEffect(string animEvent) {
        GameObject _effect = Instantiate(currentAbility.effect, abilityEffectPoint.position, abilityEffectPoint.rotation);
    }
}
