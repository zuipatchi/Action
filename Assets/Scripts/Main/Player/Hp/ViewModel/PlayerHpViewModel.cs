using Main.Player.Hp.Model;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Main.Player.Hp.ViewModel
{
    /// <summary>
    /// PlayerHpModelの値が変化したらHPバーに反映する責務を持つ
    /// </summary>
    public class PlayerHpViewModel : MonoBehaviour
    {
        [SerializeField] private UIDocument _uIDocument;
        private ProgressBar _hpBar;
        private PlayerHpModel _playerHpModel;

        [Inject]
        public void Construct(PlayerHpModel playerHpModel)
        {
            _playerHpModel = playerHpModel;
        }

        private void Start()
        {
            _hpBar = _uIDocument.rootVisualElement.Q<ProgressBar>("PlayerHpBar");
            _hpBar.value = 100;

            _playerHpModel.Current.Subscribe(v =>
            {
                _hpBar.value = v;
            });
        }
    }
}
