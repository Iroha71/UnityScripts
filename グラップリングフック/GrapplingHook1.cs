using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using UnityEngine.Events;
using Invector.vCharacterController.vActions;
using Invector.vCharacterController.AI;

/// <summary>
/// グラップリングフック
/// </summary>
public class GrapplingHook1 : MonoBehaviour
{
    private Rigidbody rb;
    public bool attached = false;
    [SerializeField] private LayerMask layerMask;
    private Animator anim;
    [SerializeField] private AnimationCurve speedRate;
    [SerializeField] private float elapsedTime = 0f;
    private Vector3 startPosition;
    [SerializeField] private AnimationCurve heightRate;
    [SerializeField] public Transform ropeDestination;
    private vThirdPersonController player;
    private Vector3 currentGoal;
    private vShooterMeleeInput tpinput;
    [HideInInspector] public UnityEvent<Vector3> OnLanchRope = new UnityEvent<Vector3>();
    [HideInInspector] public UnityEvent OnArrivedRope = new UnityEvent();
    [SerializeField] private float airEndTime = 0.82f;
    private RaycastHit hit;
    private Ray ray;
    public Transform ropeOrigin;
    private vLockOn lockon;
    private Transform lockTarget = null;
    private vFreeClimb freeClimb;
    private bool isClimbEnter = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        player = GetComponent<vThirdPersonController>();
        tpinput = GetComponent<vShooterMeleeInput>();
        lockon = GetComponent<vLockOn>();
        freeClimb = GetComponent<vFreeClimb>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.N)) {
            Vector3 finalGoal = ropeDestination.position;
            
            // 敵をロックオン→敵に対して移動（最終座標がずれるため、移動時は敵AIの動きを止める）
            if (lockon.currentTarget != null) {
                //Vector3 goal = transform.parent.forward * targetDistance + transform.parent.position;
                finalGoal = lockon.currentTarget.position - (transform.forward * 1.5f) + Vector3.up * 3f;
                vControlAI ai = lockon.currentTarget.GetComponent<vControlAI>();
                if (ai)
                    ai.Stop();
                lockTarget = lockon.currentTarget;
                lockon.RestartLockon(null);
            // 移動先に障害物がある場合→Rayが衝突した地点を着地点に設定
            } else if (Physics.Raycast(ropeOrigin.position, (ropeDestination.position - ropeOrigin.position), out hit, Vector3.Distance(ropeOrigin.position, ropeDestination.position), layerMask)) {
                finalGoal = hit.point;
                // クライム可能オブジェクトの場合はクライミング開始
                if (hit.collider.CompareTag("FreeClimb"))
                    isClimbEnter = true;
            }
            if (player.isGrounded) {
                //tpinput.SetLockAllInput(true);
                tpinput.SetLockBasicInput(true);
                player.StopCharacter();
                BeginGrappling(finalGoal);
                freeClimb.enterExitInput.useInput = false;
            }
        }

        // 移動中
        if (attached) {
            elapsedTime += Time.deltaTime;
            Move2Goal(elapsedTime);
        }
    }

    /// <summary>
    /// グラップ着地点まで1フレームごとに移動する
    /// </summary>
    /// <param name="positionRate">着地点までの移動進捗割合</param>
    private void Move2Goal(float positionRate) {
        float lerpPoint = speedRate.Evaluate(positionRate);
        rb.position = Vector3.Lerp(startPosition, currentGoal, lerpPoint);
        // 到着後、通常移動に切り替え
        if (elapsedTime >= airEndTime) {
            lockon.RestartLockon(lockTarget);
            lockTarget = null;
            DisableGrappleMode();
        }
    }

    /// <summary>
    /// グラップルを中止する
    /// </summary>
    private void DisableGrappleMode() {
        rb.isKinematic = false;
        attached = false;
        freeClimb.enterExitInput.useInput = true;
        OnArrivedRope.Invoke();
        if (isClimbEnter) {
            freeClimb.AutoClimb();
            isClimbEnter = false;
        }
            
        if (lockTarget != null) {
            Debug.Log("Lockon");
            // original method
            vControlAI ai = lockTarget.GetComponent<vControlAI>();
            if (ai)
                ai.Walk();
        }
    }

    /// <summary>
    /// グラップルを中止する（アニメーションイベントから呼び出し）
    /// </summary>
    /// <param name="animEvent"></param>
    public void CancelGrappling(string animEvent) {
        DisableGrappleMode();
        EndGrappling("GrapplingEnd");
    }

    /// <summary>
    /// グラップル終了後の動作を定義（アニメーションイベントから呼び出し）
    /// </summary>
    /// <param name="animEvent"></param>
    public void EndGrappling(string animEvent) {
        elapsedTime = 0f;
        tpinput.SetLockBasicInput(false);
    }

    /// <summary>
    /// グラップルを開始する
    /// </summary>
    /// <param name="goalpoint">着地点</param>
    private void BeginGrappling(Vector3 goalpoint) {
        anim.CrossFadeInFixedTime("Grappling", 0f);
        transform.LookAt(new Vector3(goalpoint.x, transform.position.y, goalpoint.z));
        startPosition = transform.position;
        currentGoal = goalpoint;
        OnLanchRope.Invoke(goalpoint);
    }

    /// <summary>
    /// グラップル移動モードを有効にする（グラップル開始アニメーションのイベントから呼び出し）
    /// </summary>
    /// <param name="animEvent"></param>
    public void Move(string animEvent) {
        attached = true;
        rb.isKinematic = true;
    }
}
