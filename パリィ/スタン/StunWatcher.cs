using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Invector.vCharacterController.AI;
using Cysharp.Threading.Tasks;
using Invector;
using Invector.vCharacterController.vActions;

/// <summary>
/// スタンを実装する, AIStateから呼ぶためにvIAIComponentを実装
/// </summary>
public class StunWatcher : MonoBehaviour, vIAIComponent
{
    /// <summary>
    /// スタン閾値
    /// </summary>
    [SerializeField] private int maxStun = 100;
    /// <summary>
    /// 現在のスタン値
    /// </summary>
    private int stun;
    [HideInInspector] public UnityEvent OnStuned = new UnityEvent();
    [HideInInspector] public UnityEvent<int> OnAddStun = new UnityEvent<int>();
    /// <summary>
    /// StunWatcherの型（vIAIComponentの実装に必要）
    /// </summary>
    public System.Type ComponentType { get { return typeof(StunWatcher); } }
    /// <summary>
    /// スタン時間
    /// </summary>
    private const float STUN_TIME = 3f;
    public int Stun { get { return stun; } }
    public int MaxStun { get { return maxStun; } }
    /// <summary>
    /// パリィシステムの連携
    /// </summary>
    private ParrySystem parrySystem;
    /// <summary>
    /// フィニッシュムーブ用トリガー
    /// </summary>
    [SerializeField] private vTriggerGenericAction exeTrigger;
    // Start is called before the first frame update
    void Start()
    {
        OnStuned.AddListener(CountStunning);
        parrySystem = GetComponent<ParrySystem>();

        // パリィシステムがある場合、パリィ時にスタン値を加算する
        if (parrySystem)
            parrySystem.OnReceiveParry.AddListener(AddStun);
        if (exeTrigger)
            exeTrigger.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// スタン値を加算する
    /// </summary>
    /// <param name="value">加算するスタン値</param>
    public void AddStun(int value) {
        stun = Mathf.Clamp(stun + value, 0, maxStun);
        OnAddStun.Invoke(stun);
        if (stun >= maxStun)
            OnStuned.Invoke();
    }

    /// <summary>
    /// スタン後の経過時間を計測する
    /// </summary>
    private async void CountStunning() {
        await UniTask.Delay((int)(STUN_TIME * 1000f));
        ResetStun();
    }

    /// <summary>
    /// フィニッシュムーブ用トリガーを有効にする
    /// </summary>
    public void EnableExecutionTrigger() {
        if (!exeTrigger)
            return;
        exeTrigger.gameObject.SetActive(true);
    }

    /// <summary>
    /// フィニッシュムーブ用トリガーを無効にする
    /// </summary>
    public void DisableExecutionTrigger() {
        if (!exeTrigger)
            return;
        exeTrigger.gameObject.SetActive(false);
    }

    /// <summary>
    /// スタンを解除する
    /// </summary>
    /// <param name="damage">受けたダメージ</param>
    private void ResetStun(vDamage damage) {
        ResetStun();
    }

    /// <summary>
    /// スタンを解除する
    /// </summary>
    private void ResetStun() {
        AddStun(-maxStun);
    }
}
