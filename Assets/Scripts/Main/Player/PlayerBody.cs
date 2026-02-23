using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace Main.Player
{
    /// <summary>
    /// プレイヤーの体が何かにぶつかったときの挙動の制御に責務を持つ
    /// </summary>
    public class PlayerBody : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private readonly string _targetTag = "Enemy";

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        private void Start()
        {
            _playerModel.OnDead.Subscribe(onDead =>
            {
                if (onDead) Destroy(gameObject,2f);
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_targetTag)) return;
            _playerModel.DamageAsync(20).Forget();
        }
    }
}
