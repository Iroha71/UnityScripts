using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// フィニッシュムーブを実装する
/// </summary>
public class Executer : MonoBehaviour
{
    private Animator anim;
    private StunWatcher stunWatcher;
    private vShooterMeleeInput tpinput;
    // Start is called before the first frame update
    void Start()
    {
        tpinput = GameObject.FindGameObjectWithTag("Player").GetComponent<vShooterMeleeInput>();
        stunWatcher = GetComponent<StunWatcher>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// フィニッシュムーブを発動する
    /// </summary>
    public void Execute() {
        ControllPlayerCamera(isExecute: true);
        anim.SetBool("IsStun", false);
        anim.SetInteger("ActionState", -1);
        anim.Play("Executed");
        stunWatcher.DisableExecutionTrigger();
    }

    /// <summary>
    /// カメラを通常時とフィニッシュムーブ用に切り替える
    /// </summary>
    /// <param name="isExecute">フィニッシュムーブ用に切り替えるか</param>
    public void ControllPlayerCamera(bool isExecute) {
        tpinput.LockCamera = isExecute;
        tpinput.ChangeCameraStateWithLerp(isExecute ? "Execute" : "Default");
    }
}
