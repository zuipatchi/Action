using R3;
using UnityEngine;
using VContainer;

namespace Main.Player.Animation
{

    public class PlayerAnimationPresenter : MonoBehaviour
    {
        private Animator _animator;
        private PlayerModel _playerModel;

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();

            _playerModel.OnDash.Subscribe(_ =>
            {
               _animator.SetTrigger("Dash"); 
            });

            _playerModel.MoveInput.Subscribe(v =>
            {
                _animator.SetFloat("Speed", v.magnitude);
            });

            _playerModel.OnDamage.Subscribe(_ =>
            {
                _animator.SetTrigger("Damage");
            });
        }
    }
}
