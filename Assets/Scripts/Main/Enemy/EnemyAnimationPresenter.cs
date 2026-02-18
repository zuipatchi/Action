using R3;
using UnityEngine;

namespace Main.Enemy
{
    public class EnemyAnimationPresenter : MonoBehaviour
    {
        private Animator _animator;
        private EnemyModel _enemyModel;
        private NavMeshAgentLocker _navMeshAgentLocker;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _enemyModel = GetComponentInParent<EnemyModel>();
            _navMeshAgentLocker = GetComponentInParent<NavMeshAgentLocker>();

            _enemyModel.OnDead.Subscribe(_ =>
            {
                _animator.SetTrigger("Dead");
                _navMeshAgentLocker.Lock();
            });
        }
    }
}
