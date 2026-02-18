using UnityEngine;

namespace Main.Player.Action.Model
{
    public sealed class DashState
    {
        // ダッシュ距離
        private readonly float _dashDistance = 6f;

        // ダッシュする時間
        private readonly float _dashDuration = 0.75f;

        // クールダウンタイム
        private readonly float _dashCooldown = 1f;

        private float _remainingDistance;
        private float _remainingTime;
        private float _cooldownRemaining;

        private bool _requested;
        private Vector3 _dashDir = Vector3.forward;

        public bool IsDashing => _remainingTime > 0f;
        public bool CanDash => !IsDashing && _cooldownRemaining <= 0f;

        public void Tick(float dt)
        {
            if (_cooldownRemaining <= 0f) return;

            _cooldownRemaining -= dt;
            if (_cooldownRemaining < 0f) _cooldownRemaining = 0f;
        }

        public void Request() => _requested = true;

        public bool TryConsumeDelta(float dt, Vector3 forward, Vector2 moveInput, out Vector3 delta)
        {
            // ダッシュ開始判定（要求があり、開始可能で、方向が作れる）
            if (_requested && CanDash && TryBuildDashDir(forward, moveInput, out var dir))
            {
                _requested = false;
                Set(dir);
                _cooldownRemaining = _dashCooldown;
            }
            else
            {
                _requested = false;
            }

            delta = ConsumeDashDelta(dt);
            return delta.sqrMagnitude > 0f;
        }

        private static bool TryBuildDashDir(Vector3 forward, Vector2 moveInput, out Vector3 dir)
        {
            dir = new Vector3(moveInput.x, 0f, moveInput.y);
            if (dir.sqrMagnitude > 0f) { dir = dir.normalized; return true; }

            dir = forward;
            dir.y = 0f;
            if (dir.sqrMagnitude <= 0f) return false;

            dir = dir.normalized;
            return true;
        }

        private void Set(Vector3 dashDirection)
        {
            _dashDir = dashDirection;
            _remainingDistance = _dashDistance;
            _remainingTime = _dashDuration;
        }

        private Vector3 ConsumeDashDelta(float dt)
        {
            if (!IsDashing) return Vector3.zero;

            _remainingTime -= dt;
            if (_remainingTime < 0f) _remainingTime = 0f;

            float speed = (_dashDuration <= 0f) ? 0f : (_dashDistance / _dashDuration);
            float step = speed * dt;
            if (step > _remainingDistance) step = _remainingDistance;

            _remainingDistance -= step;

            if (_remainingDistance <= 0f || _remainingTime <= 0f)
            {
                _remainingDistance = 0f;
                _remainingTime = 0f;
            }

            return _dashDir * step;
        }
    }
}
