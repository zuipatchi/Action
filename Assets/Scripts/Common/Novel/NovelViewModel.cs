using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.Novel
{
    [RequireComponent(typeof(UIDocument))]
    public class NovelViewModel : MonoBehaviour
    {
        private readonly float _charsPerSecond = 45f;

        private Label _nameLabel;
        private Label _messageLabel;
        private VisualElement _root;

        private readonly Subject<Unit> _onClick = new();
        private CancellationTokenSource _lineCts;

        private bool _initialized;

        private void OnEnable()
        {
            TryInitialize();
        }

        private void TryInitialize()
        {
            if (_initialized) return;

            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;

            if (_root == null) return; // まだパネルが出来てない可能性

            _nameLabel = _root.Q<Label>("nameLabel");
            _messageLabel = _root.Q<Label>("messageLabel");

            _root.RegisterCallback<ClickEvent>(_ => _onClick.OnNext(Unit.Default));

            _initialized = true;
        }

        public async UniTask ShowLineAsync(string speaker, string text, CancellationToken externalCt = default)
        {
            // text が null だと Length/Substring で落ちる
            text ??= string.Empty;

            CancelCurrentLine();
            _lineCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);
            var ct = _lineCts.Token;

            _nameLabel.text = speaker ?? string.Empty;
            _messageLabel.text = "";

            var isTyping = true;
            var fullyShown = false;

            var nextTcs = new UniTaskCompletionSource();

            var clickDisp = _onClick.Subscribe(_ =>
            {
                if (ct.IsCancellationRequested) return;

                if (isTyping)
                {
                    isTyping = false;
                    _messageLabel.text = text;
                    fullyShown = true;
                    return;
                }

                if (!fullyShown) return;
                nextTcs.TrySetResult();
            });

            try
            {
                var delayMs = Mathf.Max(1, Mathf.RoundToInt(1000f / Mathf.Max(1f, _charsPerSecond)));

                for (int i = 0; i < text.Length; i++)
                {
                    if (!isTyping) break;
                    ct.ThrowIfCancellationRequested();

                    _messageLabel.text = text.Substring(0, i + 1);
                    await UniTask.Delay(delayMs, cancellationToken: ct);
                }

                isTyping = false;
                fullyShown = true;

                await nextTcs.Task.AttachExternalCancellation(ct);
            }
            finally
            {
                clickDisp.Dispose();
            }
        }

        private void CancelCurrentLine()
        {
            if (_lineCts == null) return;
            _lineCts.Cancel();
            _lineCts.Dispose();
            _lineCts = null;
        }

        private void OnDestroy()
        {
            _lineCts?.Cancel();
            _lineCts?.Dispose();
            _onClick?.Dispose();
        }
    }
}
