using Common.Novel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCoordinator : MonoBehaviour
{
    private NovelManager _novelManager;

    private void Start()
    {
        GetNovelManager();
        _novelManager.PlayTutorial();
    }

    private void GetNovelManager()
    {
        var scene = SceneManager.GetSceneByName("Common");
        if (scene.isLoaded)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                if (root.name == "[LOGIC]")
                {
                    _novelManager = root.transform.Find("NovelManager").GetComponent<NovelManager>();
                }
            }
        }
    }
}
