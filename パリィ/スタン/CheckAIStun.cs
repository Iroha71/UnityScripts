using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a CheckAIStun decision", UnityEditor.MessageType.Info)]
#endif
    public class CheckAIStun : vStateDecision
    {
		public override string categoryName
        {
            get { return "Combat/"; }
        }

        public override string defaultName
        {
            get { return "Check Stun"; }
        }

        private StunWatcher stunWatcher;
        public enum CheckValue { Greater, Less};
        public CheckValue checkValue = CheckValue.Greater;
        private bool isCheckedStunWatcher = false;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            //TO DO
            if (!stunWatcher) {
                stunWatcher = fsmBehaviour.aiController.GetAIComponent<StunWatcher>();
                isCheckedStunWatcher = true;
            }

            if (isCheckedStunWatcher && !stunWatcher)
                return false;
            
            switch (checkValue) {
                case CheckValue.Greater:
                    return stunWatcher.Stun >= stunWatcher.MaxStun;

                case CheckValue.Less:
                    return stunWatcher.Stun < stunWatcher.MaxStun;
            }

            return false;
        }
    }
}
