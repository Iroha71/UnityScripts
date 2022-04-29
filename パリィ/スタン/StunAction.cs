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
            // �X�^���p�A�j���[�V�������Đ��ianim.Play + ActionState�ł����������j
            fsmBehaviour.aiController.animator.SetBool("IsStun", isEnabled);
            // �X�^�����ɒe���ꃂ�[�V�������Đ����Ȃ����߁A�g���K�[�����Z�b�g
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