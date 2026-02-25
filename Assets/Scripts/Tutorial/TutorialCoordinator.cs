using Common.Novel;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialCoordinator : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    private NovelManager _novelManager;


    private void Start()
    {
        _novelManager = GetNovelManager();
        _novelManager.PlayTutorial();
        _enemySpawner.Spawn();
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
