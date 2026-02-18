using System;
using R3;
using UnityEngine;

namespace Main.Enemy
{
    public class EnemyModel : MonoBehaviour
    {
        private int _currentHp = 100;
        private int _maxHp = 100;

        private Subject<Unit> _onDead = new();
        public Observable<Unit> OnDead => _onDead;

        public void Damage(int value)
        {
            var tmp = _currentHp - value;
            _currentHp = Math.Clamp(tmp, 0, _maxHp);
            if (_currentHp == 0)
            {
                _onDead.OnNext(Unit.Default);
                Destroy(gameObject, 1.5f);
            }
        }
    }
}
