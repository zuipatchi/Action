using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Common.Novel
{

    public sealed class NovelManager : MonoBehaviour
    {
        [SerializeField] private List<Scenario> _scenarios;
        [SerializeField] private GameObject _prefab;
        private Subject<Unit> _onFinished = new();
        public Observable<Unit> OnFinished => _onFinished;

        private Scenario _currentScenario;
        private int _index = 0;
        private GameObject _instance;

        private Subject<GameObject> _onPlay = new();
        public Observable<GameObject> OnPlay => _onPlay;

        private ReactiveProperty<NovelLine> _currentLines = new();
        public ReadOnlyReactiveProperty<NovelLine> CurrentLines => _currentLines;

        public void PlayTutorial1()
        {
            _currentScenario = _scenarios[0];
            CreateNovelInstance();
        }

        public void PlayTutorial2()
        {
            _currentScenario = _scenarios[1];
            CreateNovelInstance();
        }

        private void CreateNovelInstance()
        {
            if (_instance != null)
            {
                Destroy(_instance);
                _instance = null;
            }

            _index = 0;

            _instance = Instantiate(_prefab);
            _onPlay.OnNext(_instance);

            _currentLines.Value = _currentScenario.Lines[_index];
        }

        public void Next()
        {
            _index++;

            if (_index >= _currentScenario.Lines.Length)
            {
                _index = 0;
                Destroy(_instance);
                _onFinished.OnNext(Unit.Default);
                return;
            }

            _currentLines.Value = _currentScenario.Lines[_index];
        }
    }
}
