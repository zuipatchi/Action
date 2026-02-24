using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.Novel
{

    public sealed class NovelTextBox : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private float _charsPerSecond = 45f;

        private Label _nameLabel;
        private Label _messageLabel;
        private VisualElement _root;

        private readonly Subject<Unit> _onClick = new();

        // 現在の表示処理を止める用
        private CancellationTokenSource _lineCts;

        private void Awake()
        {
            _root = _uiDocument.rootVisualElement;
            _nameLabel = _root.Q<Label>("nameLabel");
            _messageLabel = _root.Q<Label>("messageLabel");

            _root.RegisterCallback<ClickEvent>(_ => _onClick.OnNext(Unit.Default));
        }

        private void OnDestroy()
        {
            _lineCts?.Cancel();
            _lineCts?.Dispose();
            _onClick?.Dispose();
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
                    isTyping = false;           // タイピングを止める合図
                    _messageLabel.text = text;  // 全文表示
                    fullyShown = true;
                    return;
                }

                if (!fullyShown) return; // 念のため
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

                // 途中でクリックされて止まった場合でも、全文は出しておく
                if (!_lineCts.IsCancellationRequested && _messageLabel.text != text)
                {
                    _messageLabel.text = text;
                }

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
    }
}
