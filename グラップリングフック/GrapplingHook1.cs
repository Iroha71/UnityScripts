using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using UnityEngine.Events;
using Invector.vCharacterController.vActions;
using Invector.vCharacterController.AI;

/// <summary>
/// �O���b�v�����O�t�b�N
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
            
            // �G�����b�N�I�����G�ɑ΂��Ĉړ��i�ŏI���W������邽�߁A�ړ����͓GAI�̓������~�߂�j
            if (lockon.currentTarget != null) {
                //Vector3 goal = transform.parent.forward * targetDistance + transform.parent.position;
                finalGoal = lockon.currentTarget.position - (transform.forward * 1.5f) + Vector3.up * 3f;
                vControlAI ai = lockon.currentTarget.GetComponent<vControlAI>();
                if (ai)
                    ai.Stop();
                lockTarget = lockon.currentTarget;
                lockon.RestartLockon(null);
            // �ړ���ɏ�Q��������ꍇ��Ray���Փ˂����n�_�𒅒n�_�ɐݒ�
            } else if (Physics.Raycast(ropeOrigin.position, (ropeDestination.position - ropeOrigin.position), out hit, Vector3.Distance(ropeOrigin.position, ropeDestination.position), layerMask)) {
                finalGoal = hit.point;
                // �N���C���\�I�u�W�F�N�g�̏ꍇ�̓N���C�~���O�J�n
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

        // �ړ���
        if (attached) {
            elapsedTime += Time.deltaTime;
            Move2Goal(elapsedTime);
        }
    }

    /// <summary>
    /// �O���b�v���n�_�܂�1�t���[�����ƂɈړ�����
    /// </summary>
    /// <param name="positionRate">���n�_�܂ł̈ړ��i������</param>
    private void Move2Goal(float positionRate) {
        float lerpPoint = speedRate.Evaluate(positionRate);
        rb.position = Vector3.Lerp(startPosition, currentGoal, lerpPoint);
        // ������A�ʏ�ړ��ɐ؂�ւ�
        if (elapsedTime >= airEndTime) {
            lockon.RestartLockon(lockTarget);
            lockTarget = null;
            DisableGrappleMode();
        }
    }

    /// <summary>
    /// �O���b�v���𒆎~����
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
    /// �O���b�v���𒆎~����i�A�j���[�V�����C�x���g����Ăяo���j
    /// </summary>
    /// <param name="animEvent"></param>
    public void CancelGrappling(string animEvent) {
        DisableGrappleMode();
        EndGrappling("GrapplingEnd");
    }

    /// <summary>
    /// �O���b�v���I����̓�����`�i�A�j���[�V�����C�x���g����Ăяo���j
    /// </summary>
    /// <param name="animEvent"></param>
    public void EndGrappling(string animEvent) {
        elapsedTime = 0f;
        tpinput.SetLockBasicInput(false);
    }

    /// <summary>
    /// �O���b�v�����J�n����
    /// </summary>
    /// <param name="goalpoint">���n�_</param>
    private void BeginGrappling(Vector3 goalpoint) {
        anim.CrossFadeInFixedTime("Grappling", 0f);
        transform.LookAt(new Vector3(goalpoint.x, transform.position.y, goalpoint.z));
        startPosition = transform.position;
        currentGoal = goalpoint;
        OnLanchRope.Invoke(goalpoint);
    }

    /// <summary>
    /// �O���b�v���ړ����[�h��L���ɂ���i�O���b�v���J�n�A�j���[�V�����̃C�x���g����Ăяo���j
    /// </summary>
    /// <param name="animEvent"></param>
    public void Move(string animEvent) {
        attached = true;
        rb.isKinematic = true;
    }
}
