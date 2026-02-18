using Main.Player.Action.Model;
using R3;
using UnityEngine;
using VContainer;

namespace Main.Player
{

    public sealed class PlayerWeaponHitbox : MonoBehaviour
    {
        [SerializeField] private Collider _hitbox;
        private PlayerModel _playerModel;

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }
        public void EnableHitbox() => _hitbox.enabled = true;
        public void DisableHitbox() => _hitbox.enabled = false;

        private void Awake()
        {
            _hitbox.enabled = false;
        }

        private void Start()
        {
            _playerModel.OnDash.Subscribe(_ =>
            {
                EnableHitbox();
            });
        }
    }
}
