#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace StroopGame.EditorTools
{
    /// <summary>
    /// 自動把 Assets/Art/ 底下所有 PNG 匯入成 Sprite (2D and UI)。
    ///
    /// 為什麼需要：美術組沒裝 Unity，他們丟上來的是「裸 PNG」，
    /// Unity 預設會用 Texture Type = Default 匯入，UI 無法當 sprite 用。
    /// 這個 AssetPostprocessor 會在每次匯入時自動把它改成 Sprite，
    /// 所以你 git pull 之後 Unity 一 import，圖就自動可用了，零點擊。
    /// </summary>
    public class ArtAutoImporter : AssetPostprocessor
    {
        // 只處理這個資料夾底下的圖
        const string ART_FOLDER = "Assets/Art/";

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ART_FOLDER)) return;

            var importer = (TextureImporter)assetImporter;

            // 只在「第一次匯入」時自動設定（避免覆蓋你之後手動調的設定）
            if (!importer.importSettingsMissing) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;          // UI 不需要 mipmap
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;

            // 怪物形狀（會被程式染色）建議高品質、不壓縮失真
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            Debug.Log($"[ArtAutoImporter] 自動設為 Sprite：{assetPath}");
        }

        /// <summary>
        /// 手動把 Assets/Art/ 底下所有圖強制重設為 Sprite。
        /// 用在：git pull 進來的圖已被 Unity 用 Default 匯入過、
        /// PostProcessor 沒自動觸發時。
        /// 選單：StroopGame → Reimport All Art as Sprites
        /// </summary>
        [MenuItem("StroopGame/Reimport All Art as Sprites")]
        public static void ReimportAllArt()
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art" });
            int count = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                count++;
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Reimport Art",
                $"已把 {count} 張圖重設為 Sprite (2D and UI)。", "OK");
        }
    }
}
#endif
