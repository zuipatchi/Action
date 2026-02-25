using System;
using R3;
using VContainer;
using VContainer.Unity;

namespace Common.Novel
{

    public class NovelPresenter : IStartable, IDisposable
    {
        private NovelManager _novelManager;
        private NovelViewModel _vm;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public NovelPresenter(NovelManager novelManager)
        {
            _novelManager = novelManager;
        }

        public void Start()
        {
            _novelManager.OnPlay.Subscribe(go =>
            {
                _vm = go.GetComponent<NovelViewModel>();

                _novelManager.CurrentLines.Subscribe(async scenario =>
                {
                    try
                    {
                        await _vm.ShowLineAsync(scenario.Speaker, scenario.Text);
                        _novelManager.Next();
                    }
                    catch (OperationCanceledException) { }
                }).AddTo(_disposables);
            }).AddTo(_disposables);

        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
