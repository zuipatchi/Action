using R3;
using UnityEngine;

namespace Turtorial
{
    public class DeadChecker : MonoBehaviour
    {
        private readonly string _targetTag = "Enemy";
        private Subject<Unit> _onDefeat = new();
        public Observable<Unit> OnDefeat => _onDefeat;

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(_targetTag)) return;
            _onDefeat.OnNext(Unit.Default);
        }
    }
}
