
using UnityEditor;

public class GameScoreSettingsIOEditor
{
    [MenuItem("Assets/Create/GameScoreSettingsIO")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GameScoreSettingsIO>();
    }
}