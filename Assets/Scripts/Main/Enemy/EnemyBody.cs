using UnityEngine;

namespace Main.Enemy
{
    /// <summary>
    /// どういう時にダメージを受けるかについて責務を持つ
    /// </summary>
    public class EnemyBody : MonoBehaviour
    {
        [SerializeField] private EnemyModel _enemyModel;
        private readonly string _targetTag = "Weapon";

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_targetTag)) return;
            _enemyModel.Damage(100);
        }
    }
}
