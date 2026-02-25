using R3;
using UnityEngine;

namespace Common.Novel
{

    public sealed class NovelManager : MonoBehaviour
    {
        [SerializeField] private Scenario _tutorialScenario;
        [SerializeField] private GameObject _prefab;

        private Scenario _currentScenario;
        private int _index = 0;
        private GameObject _instance;

        private Subject<GameObject> _onPlay = new();
        public Observable<GameObject> OnPlay => _onPlay;

        private ReactiveProperty<NovelLine> _currentLines = new();
        public ReadOnlyReactiveProperty<NovelLine> CurrentLines => _currentLines;

        public void PlayTutorial()
        {
            _currentScenario = _tutorialScenario;
            _currentLines.Value = _currentScenario.Lines[_index];
            CreateNovelInstance();
        }

        private void CreateNovelInstance()
        {
            _instance = Instantiate(_prefab);
            _onPlay.OnNext(_instance);
        }

        public void Next()
        {
            _index++;

            if (_index >= _currentScenario.Lines.Length)
            {
                Destroy(_instance);
                _index = 0;
                return;
            }

            _currentLines.Value = _currentScenario.Lines[_index];
        }
    }
}
