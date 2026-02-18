using UnityEngine;

namespace Main.Enemy
{
    /// <summary>
    /// どういう時に攻撃するかにおいて責務を持つ
    /// </summary>
    public class Attack : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private bool _isAttack;

        private readonly string _animationParameter = "Attack";
        private readonly string _targetTag = "Player";


        private void Awake()
        {
            if (_animator == null) Debug.LogError("Animator が見つかりませんでした。");
        }

        public void EndAttack() => _isAttack = false;

        // トリガー内にいれば攻撃
        private void OnTriggerEnter(Collider other)
        {
            if (_isAttack) return;
            if (!other.CompareTag(_targetTag)) return;

            _animator.SetTrigger(_animationParameter);
            _isAttack = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isAttack) return;
            if (!other.CompareTag(_targetTag)) return;

            _animator.SetTrigger(_animationParameter);
            _isAttack = true;
        }
    }
}
