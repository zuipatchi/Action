using Cysharp.Threading.Tasks;
using Main.Player.Action.Model;
using Main.Player.Hp.Model;
using R3;
using UnityEngine;
using VContainer;

namespace Main.Player
{
    public sealed class PlayerModel
    {
        private ReactiveProperty<Vector2> _moveInput = new(Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> MoveInput => _moveInput;

        private Subject<Unit> _onDash = new();
        public Observable<Unit> OnDash => _onDash;

        private Subject<Unit> _onDamage = new();
        public Observable<Unit> OnDamage => _onDamage;

        private readonly DashState _dashState;
        private readonly PlayerHpModel _playerHpModel;

        private bool _isDamaged = false;
        public bool IsDamaged => _isDamaged;
        public bool IsStop  {get; set;} = false;

        private readonly float _unableToMoveTime = 1f;

        public ReadOnlyReactiveProperty<int> CurrentHp => _playerHpModel.Current;

        public ReadOnlyReactiveProperty<bool> OnDead => _playerHpModel.OnDead;

        [Inject]
        public PlayerModel(DashState dashState, PlayerHpModel playerHpModel)
        {
            _dashState = dashState;
            _playerHpModel = playerHpModel;
        }

        public void SetMoveInput(Vector2 v) => _moveInput.Value = v;

        public Vector2 GetMoveInput()
        {
            // 死んだら移動不可
            if (OnDead.CurrentValue) return new Vector2(0, 0);

            return _moveInput.Value;
        }

        // 前のフレームからの経過時間を渡している
        public void Tick(float dt) => _dashState.Tick(dt);

        public void RequestDash()
        {
            _dashState.Request();
            if (_dashState.CanDash)
            {
                _onDash.OnNext(Unit.Default);
            }
        }

        public bool IsDashing => _dashState.IsDashing;

        // このフレームの移動量を返す
        public bool TryConsumeDashDelta(float dt, Vector3 forward, out Vector3 delta)
        {
            return _dashState.TryConsumeDelta(dt, forward, _moveInput.Value, out delta);
        }

        public async UniTask DamageAsync(int value)
        {
            if (_isDamaged) return;
            if (IsDashing) return;

            _playerHpModel.Damage(value);
            _onDamage.OnNext(Unit.Default);
            _isDamaged = true;
            // ダメージを食らうとしばらく動けない
            var interval = _unableToMoveTime * 1000;

            await UniTask.Delay((int)interval);

            _isDamaged = false;
        }
    }
}
