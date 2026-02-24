using System.Threading;
using Common.Novel;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class AdvDemo : MonoBehaviour
{
    [SerializeField] private NovelTextBox _textBox;

    private async UniTaskVoid Start()
    {
        var ct = this.GetCancellationTokenOnDestroy();

        await _textBox.ShowLineAsync("NPC", "こんにちは。今日はいい天気ですね。", ct);
        await _textBox.ShowLineAsync("主人公", "そうだね。散歩に行こう。", ct);
        await _textBox.ShowLineAsync("NPC", "では、行きましょう！", ct);
    }
}
