using Cysharp.Threading.Tasks;
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
            Publish().Forget();
        }

        private async UniTask Publish()
        {
            await UniTask.Delay(2000);
            _onDefeat.OnNext(Unit.Default);
        }
    }
}
