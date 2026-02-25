using UnityEngine;

[CreateAssetMenu(fileName = "Scenario", menuName = "Scriptable Objects/Scenario")]
public class Scenario : ScriptableObject
{
    [SerializeField] private NovelLine[] _lines;

    public NovelLine[] Lines => _lines;
}
