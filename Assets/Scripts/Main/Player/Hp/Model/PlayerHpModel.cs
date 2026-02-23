using System;
using R3;
using UnityEngine;
using VContainer;

namespace Main.Player.Hp.Model
{

    public class PlayerHpModel
    {
        private ReactiveProperty<int> _current = new();
        public ReadOnlyReactiveProperty<int> Current => _current;

        private ReactiveProperty<bool> _onDead = new();
        public ReadOnlyReactiveProperty<bool> OnDead => _onDead;

        private int _max;
        public int Max => _max;

        public bool IsDie => _current.Value == 0;

        [Inject]
        public PlayerHpModel()
        {
            _current.Value = 100;
            _max = 100;
        }

        public void Damage(int value)
        {
            var tmp = _current.Value - value;
            _current.Value = Math.Clamp(tmp, 0, _max);
            if (_current.Value == 0) _onDead.Value = true;
        }
    }
}
