using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a StunAction Action", UnityEditor.MessageType.Info)]
#endif
    public class StunAction : vStateAction
    {       
       public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Set Stun"; }
        }
        public bool isEnabled = false;
        private StunWatcher stunWatcher;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            //TO DO
            // スタン用アニメーションを再生（anim.Play + ActionStateでもいいかも）
            fsmBehaviour.aiController.animator.SetBool("IsStun", isEnabled);
            // スタン時に弾かれモーションを再生しないため、トリガーをリセット
            fsmBehaviour.aiController.animator.ResetTrigger("TriggerRecoil");
            if (!stunWatcher)
                stunWatcher = fsmBehaviour.aiController.GetAIComponent<StunWatcher>();
            
            if (!isEnabled) {
                stunWatcher.DisableExecutionTrigger();
                return;
            }

            if (!stunWatcher)
                stunWatcher = fsmBehaviour.aiController.GetAIComponent<StunWatcher>();
            stunWatcher.EnableExecutionTrigger();
        }
    }
}