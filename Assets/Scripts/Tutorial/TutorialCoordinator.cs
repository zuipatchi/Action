using System;
using Common.Novel;
using Main.Player;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Turtorial
{
    /// <summary>
    /// ゲーム全体の進行に責務を持つ
    /// </summary>
    public class TutorialCoordinator : IStartable, IDisposable
    {
        private enum State { Novel1, Stroll, Novel2, Battle, Novel3 }
        private State _currentState;

        private EnemySpawner _enemySpawner;
        private CheckPoint _checkPoint;
        private NovelManager _novelManager;
        private PlayerModel _playerModel;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public TutorialCoordinator(EnemySpawner enemySpawner, CheckPoint checkPoint, PlayerModel playerModel)
        {
            _enemySpawner = enemySpawner;
            _checkPoint = checkPoint;
            _playerModel = playerModel;
        }

        public void Start()
        {
            _novelManager = GetNovelManager();
            EnterState(State.Novel1);
        }

        private void EnterState(State nextState)
        {
            _currentState = nextState;

            switch (_currentState)
            {
                case State.Novel1:
                    _novelManager.PlayTutorial1();
                    _playerModel.IsStop = true;
                    _novelManager.OnFinished
                        .Take(1)
                        .Subscribe(_ => EnterState(State.Stroll))
                        .AddTo(_disposables);
                    break;

                case State.Stroll:
                    _playerModel.IsStop = false;
                    _checkPoint.OnEnter
                        .Take(1)
                        .Subscribe(_ => EnterState(State.Novel2))
                        .AddTo(_disposables);
                    break;

                case State.Novel2:
                    _novelManager.PlayTutorial2();
                    _novelManager.OnFinished
                        .Take(1)
                        .Subscribe(_ => EnterState(State.Battle))
                        .AddTo(_disposables);
                    _playerModel.IsStop = true;
                    _enemySpawner.Spawn();
                    break;

                case State.Battle:
                    _playerModel.IsStop = false;
                    break;

                case State.Novel3:
                    _playerModel.IsStop = false;
                    break;
            }
        }

        private NovelManager GetNovelManager()
        {
            var scene = SceneManager.GetSceneByName("Common");
            if (scene.isLoaded)
            {
                var rootObjects = scene.GetRootGameObjects();
                foreach (var root in rootObjects)
                {
                    if (root.name == "[LOGIC]")
                    {
                        return root.transform.Find("NovelManager").GetComponent<NovelManager>();
                    }
                }
            }
            Debug.LogError("NovelManagerが見つかりませんでした。");
            return null;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
