using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Invector.vMelee;
using Invector.vCharacterController;
using Cysharp.Threading.Tasks;

/// <summary>
/// 攻撃時の距離調整と演出を行う
/// </summary>
public class AttackAssistant : MonoBehaviour
{
    private vMeleeManager meleeManager;
    private vLockOn lockon;
    private Animator anim;
    private float preAnimSpeed = 0f;
    private CameraShaker cameraShaker;
    private const int HITSTOP_MS = 200;
    [SerializeField] private float attackDistance = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        meleeManager = GetComponentInParent<vMeleeManager>();
        lockon = GetComponentInParent<vLockOn>();
        meleeManager.onDamageHit.AddListener(PlayHitEnphasis);
        anim = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 攻撃時に適切な距離に移動する
    /// </summary>
    /// <param name="animEvent">アニメーションイベント => FirstAttack</param>
    public void Move2Target(string animEvent) {
        if (!lockon.currentTarget || Vector3.Distance(transform.parent.position, lockon.currentTarget.position) >= 5f)
            return;

        transform.parent.LookAt(new Vector3(lockon.currentTarget.position.x, transform.parent.position.y, lockon.currentTarget.position.z));
        float distance = (Vector3.Distance(transform.parent.position, lockon.currentTarget.position) - attackDistance);
        Vector3 goal = transform.parent.forward * distance + transform.parent.position;
        transform.parent.DOMove(goal, 0.1f);
    }

    /// <summary>
    /// 攻撃ヒット時に演出を発生させる
    /// </summary>
    /// <param name="hitinfo">ヒット情報</param>
    private void PlayHitEnphasis(vHitInfo hitinfo) {
        HitStop();
        cameraShaker.ShakeCamera(duration: 0.1f, strength: 0.2f);
    }

    /// <summary>
    /// ヒットストップを発生させる
    /// </summary>
    private async void HitStop() {
        if (anim.speed == 0f)
            return;
        preAnimSpeed = anim.speed;
        anim.speed = 0f;
        await UniTask.Delay(HITSTOP_MS);
        anim.speed = preAnimSpeed;
    }
}
