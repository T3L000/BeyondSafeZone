using TMPro;
using UnityEditor;
using UnityEngine;

public static class ChineseTmpAtlasReadableFixer
{
    private const string FontAssetPath = "Assets/Fonts/ChineseTMP.asset";

    [MenuItem("Tools/Beyond Safe Zone/Fix ChineseTMP Atlas Readable")]
    public static void FixChineseTmpAtlasReadable()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fontAsset == null)
        {
            Debug.LogError($"ChineseTMP fixer could not find font asset at {FontAssetPath}.");
            return;
        }

        int changedCount = 0;
        foreach (Texture2D texture in fontAsset.atlasTextures)
        {
            if (SetTextureReadable(texture))
            {
                changedCount++;
            }
        }

        SetTextureReadable(fontAsset.atlasTexture);

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ChineseTMP fixer finished. Readable atlas textures touched: {changedCount}.");
    }

    private static bool SetTextureReadable(Texture2D texture)
    {
        if (texture == null)
        {
            return false;
        }

        SerializedObject serializedTexture = new SerializedObject(texture);
        SerializedProperty readableProperty = serializedTexture.FindProperty("m_IsReadable");
        if (readableProperty == null)
        {
            Debug.LogWarning($"ChineseTMP fixer could not find m_IsReadable on texture {texture.name}.");
            return false;
        }

        bool wasReadable = readableProperty.boolValue;
        readableProperty.boolValue = true;
        serializedTexture.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(texture);

        return !wasReadable;
    }
}