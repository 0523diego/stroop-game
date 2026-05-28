#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

namespace StroopGame.EditorTools
{
    /// <summary>
    /// 回復用：把所有 TMP_Text 換回預設字型（LiberationSans SDF），
    /// 刪掉壞的 NotoSansTC SDF asset 與清掉 fallback 清單裡的孤兒參照。
    /// 跑完後 Console 紅字會消失，中文會變回方框，遊戲邏輯不受影響。
    /// 選單：StroopGame → Reset Fonts to Default (Recovery)
    /// </summary>
    public static class CJKRecover
    {
        const string MENU_PATH = "StroopGame/Reset Fonts to Default (Recovery)";
        const string FONT_ASSET_PATH = "Assets/Fonts/NotoSansTC-Regular SDF.asset";

        [MenuItem(MENU_PATH)]
        public static void Reset()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("還在 Play 模式", "請先停止 Play。", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                "重設字型",
                "會做這幾件事：\n" +
                "1. 把所有 TMP_Text 換回預設字型（中文變回方框）\n" +
                "2. 刪掉壞掉的 NotoSansTC SDF asset\n" +
                "3. 從 fallback 清單移除孤兒參照\n\n" +
                "遊戲邏輯完全不受影響。",
                "Yes, reset", "Cancel"))
                return;

            var defaultFont = TMP_Settings.defaultFontAsset;
            if (defaultFont == null)
            {
                EditorUtility.DisplayDialog("錯誤", "找不到 TMP 預設字型，請先 Import TMP Essentials。", "OK");
                return;
            }

            // 1. 所有 TMP_Text 還原預設字型
            int updated = 0;
            foreach (var tmp in Object.FindObjectsOfType<TMP_Text>(true))
            {
                tmp.font = defaultFont;
                tmp.SetAllDirty();
                EditorUtility.SetDirty(tmp);
                updated++;
            }

            // 2. 刪掉壞掉的 NotoSansTC SDF
            bool deleted = false;
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_ASSET_PATH) != null)
            {
                AssetDatabase.DeleteAsset(FONT_ASSET_PATH);
                deleted = true;
            }

            // 3. TMP_Settings.fallbackFontAssets 移除孤兒
            var settings = TMP_Settings.instance;
            var so = new SerializedObject(settings);
            var prop = so.FindProperty("m_fallbackFontAssets");
            int orphansRemoved = 0;
            if (prop != null)
            {
                for (int i = prop.arraySize - 1; i >= 0; i--)
                {
                    var elem = prop.GetArrayElementAtIndex(i);
                    if (elem.objectReferenceValue == null)
                    {
                        prop.DeleteArrayElementAtIndex(i);
                        orphansRemoved++;
                    }
                }
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
            }

            // 4. 預設字型的 fallbackFontAssetTable 也清孤兒
            if (defaultFont.fallbackFontAssetTable != null)
            {
                int removed = defaultFont.fallbackFontAssetTable.RemoveAll(f => f == null);
                orphansRemoved += removed;
                EditorUtility.SetDirty(defaultFont);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Recovery Done",
                $"✅ 已重設 {updated} 個 TMP_Text\n" +
                $"✅ {(deleted ? "已刪除壞掉的 SDF asset" : "原本就沒有壞 asset")}\n" +
                $"✅ 移除 {orphansRemoved} 個孤兒 fallback\n\n" +
                "Console 的紅字應該消失了（記得 Clear Console）。\n" +
                "中文會變回方框，但遊戲邏輯正常。",
                "OK");
        }
    }
}
#endif
