using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Common.SceneManagement
{
    // 共通シーンをロードするクラス
    public class CommonSceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        private static bool _loaded = false;

        private async void Awake()
        {
            // 2重起動させない
            if (_loaded) return;
            _loaded = true;

            var commonScene = SceneManager.GetSceneByBuildIndex(0);

            // 共通シーンが存在しなければAdditiveでロード    
            if (!commonScene.IsValid())
            {
                var token = this.GetCancellationTokenOnDestroy();
                await SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive).WithCancellation(token);
            }

            // 共通シーンがアクティブな場合はタイトルシーンに遷移する
            var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (activeSceneIndex == 0)
            {
                _sceneTransitioner.Transit(Scenes.Title).Forget();
            }

            BuildLifetimeScopeInActiveScene();
        }

        // アクティブなシーンに置いてある LifetimeScope をビルドする
        private void BuildLifetimeScopeInActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            var rootObjects = activeScene.GetRootGameObjects();

            foreach (var root in rootObjects)
            {
                var scopes = root.GetComponentsInChildren<LifetimeScope>(true);
                foreach (var scope in scopes)
                {
                    scope.Build();
                }
            }
        }
    }
}
