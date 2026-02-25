using R3;
using UnityEngine;

namespace Turtorial
{

    public class CheckPoint : MonoBehaviour
    {
        private Subject<Unit> _onEnter = new();
        public Observable<Unit> OnEnter => _onEnter;

        private void OnTriggerEnter(Collider other)
        {
            _onEnter.OnNext(Unit.Default);
        }
    }
}
