using UnityEngine;

[CreateAssetMenu(menuName = "Game/Faction")]
public class FactionConfig : ScriptableObject {
    public FactionType FactionType;
    public Color Color;
}

public enum FactionType {
    Blue,
    Red
}