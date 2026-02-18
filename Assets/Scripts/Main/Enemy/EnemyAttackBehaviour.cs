using UnityEngine;

namespace Main.Enemy
{
    public class EnemyAttackBehaviour : StateMachineBehaviour
    {
        private Attack _attack;
        private NavMeshAgentLocker _navMeshAgentLocker;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_navMeshAgentLocker == null) _navMeshAgentLocker = animator.GetComponentInParent<NavMeshAgentLocker>();
            _navMeshAgentLocker.Lock();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_attack == null) _attack = animator.transform.parent.parent.GetComponentInChildren<Attack>();
            _attack.EndAttack();

            _navMeshAgentLocker.Unlock();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
