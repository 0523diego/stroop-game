#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace StroopGame.EditorTools
{
    /// <summary>
    /// 一鍵把 Noto Sans TC 變成 TMP Font Asset，並掛到 fallback 清單。
    /// v2：靜態模式預烤所有遊戲會用到的中文字，徹底解決 m_AtlasTextures bug。
    /// 選單：StroopGame → Setup CJK Font (Chinese Fallback)
    /// </summary>
    public static class CJKFontSetup
    {
        const string MENU_PATH = "StroopGame/Setup CJK Font (Chinese Fallback)";
        const string FONT_FILE_PATH = "Assets/Fonts/NotoSansTC-Regular.otf";
        const string FONT_ASSET_PATH = "Assets/Fonts/NotoSansTC-Regular SDF.asset";

        const int ATLAS_PADDING = 5;
        const int ATLAS_WIDTH = 2048;   // 2048 容得下所有中文
        const int ATLAS_HEIGHT = 2048;
        const int SAMPLING_POINT_SIZE = 48;

        /// <summary>
        /// 把遊戲會用到的所有中文 + 標點全部列在這
        /// 預烤進 SDF atlas 一次解決
        /// </summary>
        static readonly string PRELOAD_CHARS =
            // 階段名稱
            "第一二三關訓練之地幻術森林魔王城" +
            // HUD
            "分數回合最終得分正確率" +
            // 回饋
            "破解被顏色騙了升級太慢" +
            // 結算
            "冒險結算順境反應幻術反應干擾代價" +
            "強大幾乎不影響你錯抗干擾能力良好" +
            "還在成長多次練習容易需要更" +
            "資料已存" +
            // 按鈕
            "紅火球藍水球再玩一次結束" +
            // 標點與數字
            "0123456789ms,.:%/() " +
            "：，。？！「」（）" +
            // 也包含主要英文字母（保險）
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [MenuItem(MENU_PATH)]
        public static void Setup()
        {
            // 0. 拒絕在 Play 模式中執行
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("還在 Play 模式",
                    "請先按螢幕上方 ▶ 鈕停止 Play，再執行這個選單。", "OK");
                return;
            }

            if (TMP_Settings.instance == null)
            {
                EditorUtility.DisplayDialog("TMP 沒裝",
                    "請先 Window → TextMeshPro → Import TMP Essential Resources", "OK");
                return;
            }

            var sourceFont = AssetDatabase.LoadAssetAtPath<Font>(FONT_FILE_PATH);
            if (sourceFont == null)
            {
                EditorUtility.DisplayDialog("找不到字型檔",
                    $"預期路徑：{FONT_FILE_PATH}", "OK");
                return;
            }

            // === Step 1: 刪掉舊的損壞 asset（如果有的話）===
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_ASSET_PATH) != null)
            {
                AssetDatabase.DeleteAsset(FONT_ASSET_PATH);
                Debug.Log("[CJKFontSetup] 已刪除舊的損壞 SDF asset");
            }

            // === Step 2: 用 Dynamic 模式建立 asset ===
            var fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                SAMPLING_POINT_SIZE,
                ATLAS_PADDING,
                GlyphRenderMode.SDFAA,
                ATLAS_WIDTH, ATLAS_HEIGHT,
                AtlasPopulationMode.Dynamic,
                enableMultiAtlasSupport: true
            );
            fontAsset.name = "NotoSansTC-Regular SDF";

            AssetDatabase.CreateAsset(fontAsset, FONT_ASSET_PATH);

            // === Step 3: 預烤所有遊戲會用到的字元（同步進 atlas）===
            //   這一步是關鍵：強迫 atlas 立刻填入字元，
            //   填完後我們把 atlas texture 存成 sub-asset 就不會掉了。
            fontAsset.TryAddCharacters(PRELOAD_CHARS, out string missingChars);
            if (!string.IsNullOrEmpty(missingChars))
            {
                Debug.LogWarning($"[CJKFontSetup] 這些字 Noto Sans TC 沒有：{missingChars}");
            }

            // === Step 4: ★ 把 atlas texture 與 material 存成 sub-asset ===
            //   這是修 m_AtlasTextures 失聯 bug 的關鍵
            if (fontAsset.atlasTextures != null)
            {
                foreach (var tex in fontAsset.atlasTextures)
                {
                    if (tex != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(tex)))
                    {
                        tex.name = "Font Atlas";
                        AssetDatabase.AddObjectToAsset(tex, fontAsset);
                    }
                }
            }
            if (fontAsset.material != null
                && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(fontAsset.material)))
            {
                fontAsset.material.name = "NotoSansTC-Regular Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FONT_ASSET_PATH);

            Debug.Log($"[CJKFontSetup] 建立 SDF asset 成功，預烤 {PRELOAD_CHARS.Length} 字元");

            // === Step 5: 加入 TMP_Settings 全域 fallback ===
            var settings = TMP_Settings.instance;
            var so = new SerializedObject(settings);
            var prop = so.FindProperty("m_fallbackFontAssets");
            if (prop != null)
            {
                bool alreadyIn = false;
                for (int i = 0; i < prop.arraySize; i++)
                {
                    if (prop.GetArrayElementAtIndex(i).objectReferenceValue == fontAsset)
                    {
                        alreadyIn = true;
                        break;
                    }
                }
                if (!alreadyIn)
                {
                    prop.arraySize++;
                    prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = fontAsset;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(settings);
                }
            }

            // === Step 6: 把所有場景上的 TMP_Text 直接綁這個字型 ===
            int updatedCount = 0;
            foreach (var tmp in Object.FindObjectsOfType<TMP_Text>(true))
            {
                tmp.font = fontAsset;
                tmp.SetAllDirty();
                EditorUtility.SetDirty(tmp);
                updatedCount++;
            }

            // === Step 7: 也加進 LiberationSans SDF 的 fallback（雙保險）===
            var defaultFont = TMP_Settings.defaultFontAsset;
            if (defaultFont != null && defaultFont != fontAsset)
            {
                if (defaultFont.fallbackFontAssetTable == null)
                    defaultFont.fallbackFontAssetTable = new List<TMP_FontAsset>();
                if (!defaultFont.fallbackFontAssetTable.Contains(fontAsset))
                {
                    defaultFont.fallbackFontAssetTable.Add(fontAsset);
                    EditorUtility.SetDirty(defaultFont);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "中文字型完成",
                $"Noto Sans TC SDF 已建立並預烤 {PRELOAD_CHARS.Length} 個字元。\n\n" +
                $"已更新 {updatedCount} 個 TMP_Text 物件。\n\n" +
                "請：\n" +
                "1. Cmd+S 存場景\n" +
                "2. 看 Game 視窗，中文應該都正常了",
                "OK");
        }
    }
}
#endif
