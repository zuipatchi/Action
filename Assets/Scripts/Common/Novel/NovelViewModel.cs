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

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;
            _nameLabel = _root.Q<Label>("nameLabel");
            _messageLabel = _root.Q<Label>("messageLabel");

            _root.RegisterCallback<ClickEvent>(_ => _onClick.OnNext(Unit.Default));
        }

        /// <summary>
        /// 1行表示。クリックで全文→次へ。戻りは「次へ」まで待つ。
        /// </summary>
        public async UniTask ShowLineAsync(string speaker, string text, CancellationToken externalCt = default)
        {
            CancelCurrentLine();

            _lineCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);
            var ct = _lineCts.Token;

            _nameLabel.text = speaker;
            _messageLabel.text = "";

            var isTyping = true;
            var fullyShown = false;

            // 「次へ」待ち（全文表示後のクリック）
            var nextTcs = new UniTaskCompletionSource();

            // クリック購読：文字送り中は全文表示、全文後は次へ
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
                // タイプライター本体
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

                // 「次へ」クリックまで待つ
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
