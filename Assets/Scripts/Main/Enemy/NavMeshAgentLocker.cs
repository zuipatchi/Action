using UnityEngine;
using UnityEngine.AI;

namespace Main.Enemy
{
    /// <summary>
    /// NavMeshAgent の動きを止めたり、再開することに責務を持つ
    /// </summary>
    public class NavMeshAgentLocker : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;

        private float _savedSpeed;
        private float _savedAngularSpeed;
        private bool _savedAutoBraking;

        public void Lock()
        {
            if (_agent == null || !_agent.enabled) return;

            _savedSpeed = _agent.speed;
            _savedAngularSpeed = _agent.angularSpeed;
            _savedAutoBraking = _agent.autoBraking;

            _agent.isStopped = true;
            _agent.speed = 0f;
            _agent.angularSpeed = 0f;
            _agent.autoBraking = true;
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }

        public void Unlock()
        {
            if (_agent == null || !_agent.enabled) return;

            _agent.speed = _savedSpeed;
            _agent.angularSpeed = _savedAngularSpeed;
            _agent.autoBraking = _savedAutoBraking;
            _agent.isStopped = false;
        }
    }
}
