using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using Invector.vCamera;
using MalbersAnimations.HAP;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private MapController map;
    private GenericInput mapOpenInput = new GenericInput("M", "Start", "Start");
    private vShooterMeleeInput tpinput;
    private bool isOpeingMap = false;
    private Animator anim;
    private MapController _map;
    private MRider rider;
    // Start is called before the first frame update
    void Start()
    {
        tpinput = GetComponent<vShooterMeleeInput>();
        anim = GetComponent<Animator>();
        rider = GetComponent<MRider>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMapInput();
    }

    /// <summary>
    /// マップを開く操作を受け付ける
    /// </summary>
    public void HandleMapInput() {
        if (mapOpenInput.GetButtonDown()) {
            if (!isOpeingMap)
                OpenMap();
            else
                HideMap();
        }
    }

    /// <summary>
    /// マップを開く
    /// </summary>
    public void OpenMap() {
        tpinput.SetLockAllInput(true);
        tpinput.LockCamera = true;
        anim.Play("ShowMap");
        if (rider.IsRiding) {
            FindObjectOfType<vThirdPersonCamera>().ChangeState("Map");
        } else {
            tpinput.ChangeCameraStateNoLerp("Map");
        }
        _map = Instantiate(map, anim.GetBoneTransform(HumanBodyBones.UpperChest));
        _map.SetPosition();
        isOpeingMap = true;
    }

    /// <summary>
    /// マップを閉じる
    /// </summary>
    public void HideMap() {
        anim.SetInteger("ActionState", 0);
        tpinput.SetLockAllInput(false);
        tpinput.LockCamera = false;
        if (rider.IsRiding) {
            FindObjectOfType<vThirdPersonCamera>().ChangeState("Ride");
        } else {
            tpinput.ResetCameraState();
        }
        Destroy(_map.gameObject);
        isOpeingMap = false;
    }
}
