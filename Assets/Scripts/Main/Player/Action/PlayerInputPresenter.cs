using Main.Player.Action.Model;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Main.Player.Action
{

    public class PlayerInputPresenter
    {
        private PlayerModel _playerModel;

        [Inject]
        public PlayerInputPresenter(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        public void OnDash(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            _playerModel.RequestDash();
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _playerModel.SetMoveInput(ctx.ReadValue<Vector2>());
        }
    }
}
