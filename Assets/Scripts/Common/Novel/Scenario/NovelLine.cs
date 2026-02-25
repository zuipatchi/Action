using System;
using UnityEngine;

/// <summary>
/// 1行分のセリフ
/// </summary>
[Serializable]
public struct NovelLine
{
    [SerializeField] private string _speaker;
    [SerializeField, TextArea(2, 6)] private string _text;

    public string Speaker => _speaker;
    public string Text => _text;
}
