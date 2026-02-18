using Main.Player.Action.Model;
using UnityEngine;
using VContainer;

namespace Main.Player.Action.Move
{
    /// <summary>
    /// プレイヤーを上下左右に移動させるクラス
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _turnSpeed;
        private PlayerModel _playerModel;

        private CharacterController _controller;

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
            if (_playerModel.IsDamaged) return;

            Vector3 moveDir = new Vector3(_playerModel.GetMoveInput().x, 0f, _playerModel.GetMoveInput().y);
            if (moveDir.sqrMagnitude <= 0f) return;

            moveDir = moveDir.normalized;

            // 1) 移動方向へ回転（水平のみ）
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _turnSpeed * Time.deltaTime
            );

            // 2) 移動
            _controller.Move(moveDir * _moveSpeed * Time.deltaTime);
        }
    }
}
