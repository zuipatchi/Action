using Common.Novel;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialCoordinator : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    private enum State { Novel, Battle }
    private NovelManager _novelManager;
    private State _currentState;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _novelManager = GetNovelManager();
        EnterState(State.Novel);
    }

    private void EnterState(State nextState)
    {
        _currentState = nextState;

        switch (_currentState)
        {
            case State.Novel:
                _novelManager.OnFinished
                    .Take(1)
                    .Subscribe(_ => EnterState(State.Battle))
                    .AddTo(_disposables);
                _novelManager.PlayTutorial();
                break;

            case State.Battle:
                _enemySpawner.Spawn();
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
}
