using UnityEngine;
using VContainer;
using Main.Player.Action.Model;

namespace Main.Player.Action.Dash
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerDash : MonoBehaviour
    {
        private CharacterController _controller;
        private PlayerModel _playerModel;

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            _playerModel.Tick(dt);

            // プレイヤーモデルがダッシュの情報を返してくれたらダッシュする
            if (_playerModel.TryConsumeDashDelta(dt, transform.forward, out var dashDelta))
            {
                _controller.Move(dashDelta);
            }
        }
    }
}
