using UnityEngine.InputSystem;

namespace Main.Player.Action
{
    public class PlayerInputCallbacks : PlayerControls.IPlayerActions
    {
        private readonly PlayerInputPresenter _playerInputPresenter;

        public PlayerInputCallbacks(PlayerInputPresenter playerInputPresenter)
        {
            _playerInputPresenter = playerInputPresenter;
        }

        public void OnMove(InputAction.CallbackContext context) => _playerInputPresenter.OnMove(context);

        public void OnDash(InputAction.CallbackContext context) => _playerInputPresenter.OnDash(context);
    }
}
