using UnityEngine;
using UnityEngine.AI;

namespace Main.Enemy
{
    /// <summary>
    /// Playerを追従することについて責務を持つ
    /// </summary>
    public class NavMeshAgentController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Transform _target;
        private Animator _animator;
        private readonly string _animationParameter = "Speed";
        private readonly string _targetTag = "Player";


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null) Debug.LogError("NavMeshAgentが見つかりませんでした。");
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null) Debug.LogError("Animator が見つかりませんでした。");
        }

        private void Update()
        {
            _animator.SetFloat(_animationParameter, _agent.velocity.magnitude);
            if (_target == null) return;
            _agent.SetDestination(_target.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_targetTag)) return;
            _target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(_targetTag)) return;
            _target = null;
        }
    }
}
