using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Invector.vMelee;
using Invector.vCharacterController;
using Invector;

/// <summary>
/// パリィ機能を実装する
/// </summary>
public class ParrySystem : MonoBehaviour
{
    /// <summary>
    /// パリィ受付時間
    /// </summary>
    private const float PARRABLE_TIME = 0.5f;
    private vShooterMeleeInput tpinput;
    private vMeleeManager mm;
    /// <summary>
    /// 現在装備している左手装備
    /// </summary>
    private vMeleeWeapon currentShield;
    private float elapsedTime = 0f;
    /// <summary>
    /// パリィ判定可能状態か
    /// </summary>
    private bool isParrable = false;
    public bool IsParrable { get { return isParrable; } }
    private Animator anim;
    [HideInInspector] public UnityEvent<int> OnReceiveParry = new UnityEvent<int>();
    [SerializeField] private ParticleSystem parryEffect;
    // Start is called before the first frame update
    void Start()
    {
        mm = GetComponent<vMeleeManager>();
        mm.onEquipWeapon.AddListener(SetWeaponInfo);
        tpinput = GetComponent<vShooterMeleeInput>();
        anim = GetComponent<Animator>();

        // 攻撃ヒット時に相手のパリィ演出を発動する
        mm.onDamageHit.AddListener(ReceiveParry);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isParrable) {
            // パリィ受付中の場合、パリィ開始からの経過時間を計測する
            CountReceiveTime(Time.deltaTime);    
        }
    }

    /// <summary>
    /// パリィ開始からの経過時間を計測する
    /// </summary>
    /// <param name="time"></param>
    private void CountReceiveTime(float time) {
        elapsedTime += time;
        if (elapsedTime >= PARRABLE_TIME) {
            elapsedTime = 0f;
            isParrable = false;
            currentShield.breakAttack = false;
            anim.SetInteger("ActionState", 0);
        }
    }

    /// <summary>
    /// パリィ可能状態を切り替える
    /// </summary>
    /// <param name="animEvent"></param>
    public void ControlParryState(string animEvent) {
        if (animEvent.Equals("BeginParry")) {
            isParrable = true;
            // 防御アニメーションの代わりにパリィモーションを再生するため、ActionStateで出しわける
            anim.SetInteger("ActionState", -1);
            // 攻撃してきた相手の弾かれモーションを再生するため、breakAttackを有効にする
            currentShield.breakAttack = true;
        } else if (!tpinput.isBlocking) {
            isParrable = false;
            elapsedTime = 0f;
            currentShield.breakAttack = false;
            anim.SetInteger("ActionState", 0);
        }
    }

    /// <summary>
    /// 現在装備している盾の情報を更新する
    /// </summary>
    /// <param name="weapon">更新する盾の情報</param>
    /// <param name="isLeft">左手に装備しているか</param>
    public void SetWeaponInfo(GameObject weapon, bool isLeft) {
        vMeleeWeapon _weapon = weapon.GetComponent<vMeleeWeapon>();
        if (isLeft || _weapon.meleeType != vMeleeType.OnlyAttack) {
            currentShield = _weapon;
            // 防御時にパリィ演出を再生する
            currentShield.onDefense.AddListener(Parry);
        }
    }

    /// <summary>
    /// パリィ演出を実行する
    /// </summary>
    public async void Parry(vDamage damage) {
        if (!currentShield || !isParrable)
            return;
        Instantiate(parryEffect, currentShield.transform.position, currentShield.transform.rotation);
        await UniTask.Delay(250);
        Time.timeScale = 0.3f;
        await UniTask.Delay(200);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 攻撃時に相手のパリィ処理を受け付ける
    /// </summary>
    /// <param name="hitinfo">相手の情報</param>
    public void ReceiveParry(vHitInfo hitinfo) {
        ParrySystem enemy = hitinfo.targetCollider.GetComponent<ParrySystem>();
        if (!enemy || !enemy.IsParrable)
            return;
        OnReceiveParry.Invoke(hitinfo.attackObject.damage.damageValue);
    }
}
