#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using StroopGame;
using StroopGame.Toolkit;
using StroopGame.UI;

namespace StroopGame.EditorTools
{
    /// <summary>
    /// 一鍵建場景：取代 SETUP-GUIDE 手動拖拉的 6-9 步。
    /// 選單列：StroopGame → Build Complete Scene
    /// </summary>
    public static class StroopSceneBuilder
    {
        const string MENU_BUILD = "StroopGame/Build Complete Scene";
        const string MENU_CLEAR = "StroopGame/Clear Scene (keep camera+light)";

        const string FIRE_SPRITE_PATH = "Assets/Art/Stimuli/fire_shape.png";
        const string WATER_SPRITE_PATH = "Assets/Art/Stimuli/water_shape.png";
        const string PREFAB_FOLDER = "Assets/Prefabs/Stimuli";
        const string PREFAB_PATH = "Assets/Prefabs/Stimuli/StimulusTemplate.prefab";

        // 色票（對應 ART-PROMPTS.md）
        static readonly Color RED_FIRE = new Color(0xF4 / 255f, 0x4C / 255f, 0x26 / 255f);
        static readonly Color BLUE_WATER = new Color(0x33 / 255f, 0x89 / 255f, 0xF2 / 255f);
        static readonly Color PURPLE_STUN = new Color(0x6A / 255f, 0x1A / 255f, 0x99 / 255f);
        static readonly Color GREEN_BUTTON = new Color(0.3f, 0.7f, 0.4f);
        static readonly Color GRAY_BUTTON = new Color(0.55f, 0.55f, 0.6f);

        [MenuItem(MENU_BUILD)]
        public static void Build()
        {
            // 0. 拒絕在 Play 模式中執行
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("還在 Play 模式",
                    "請先按螢幕上方 ▶ 鈕停止 Play，再執行 Build。", "OK");
                return;
            }

            // 1. TMP 是否裝好
            if (TMP_Settings.instance == null || TMP_Settings.defaultFontAsset == null)
            {
                EditorUtility.DisplayDialog(
                    "請先 Import TMP Essentials",
                    "先到選單 Window → TextMeshPro → Import TMP Essential Resources，再回來執行。",
                    "OK");
                return;
            }

            // 2. 美術 sprite 是否存在
            var fireSprite = LoadOrConvertSprite(FIRE_SPRITE_PATH);
            var waterSprite = LoadOrConvertSprite(WATER_SPRITE_PATH);
            if (fireSprite == null || waterSprite == null)
            {
                EditorUtility.DisplayDialog(
                    "找不到怪物 sprite",
                    $"請確認以下檔案存在：\n{FIRE_SPRITE_PATH}\n{WATER_SPRITE_PATH}",
                    "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                "Build Stroop Scene",
                "會清掉場景內所有非 Camera/Light 的物件，重新建立完整 UI 與 GameManager。\n\n要繼續嗎？",
                "Yes, build", "Cancel"))
                return;

            ClearScene();
            EnsureFolders();

            // === 建立所有物件 ===
            var stimulusPrefab = BuildStimulusPrefab(fireSprite, waterSprite);

            var canvas = BuildCanvas();
            EnsureEventSystem();
            var spawnPoint = BuildSpawnPoint(canvas.transform);
            var (redBtn, blueBtn) = BuildAttackButtons(canvas.transform);
            var hud = BuildHUDTexts(canvas.transform);
            var stunOverlayGO = BuildStunOverlay(canvas.transform);
            var resultPanel = BuildResultPanel(canvas.transform);

            // === GameManager + 7 Components ===
            var gameManager = new GameObject("GameManager");
            var difficulty = gameManager.AddComponent<DifficultyManager>();
            var logger = gameManager.AddComponent<DataLogger>();
            var scoreManager = gameManager.AddComponent<ScoreManager>();
            scoreManager.scoreText = hud.score;
            var trialCounter = gameManager.AddComponent<TrialCounter>();
            trialCounter.trialText = hud.trial;
            var countdown = gameManager.AddComponent<CountdownTimer>();
            countdown.timerText = hud.timer;
            var rtRecorder = gameManager.AddComponent<ReactionTimeRecorder>();
            var flow = gameManager.AddComponent<GameFlowManager>();

            // === UIController + 2 Components ===
            var uiController = new GameObject("UIController");
            var feedback = uiController.AddComponent<FeedbackText>();
            feedback.text = hud.feedback;
            var stunOverlay = uiController.AddComponent<StunOverlay>();
            stunOverlay.stunImage = stunOverlayGO.GetComponent<Image>();
            stunOverlay.shakeTarget = canvas.GetComponent<RectTransform>();

            // === 拉 GameFlowManager 所有 References ===
            flow.stimulusPrefab = stimulusPrefab;
            flow.spawnPoint = spawnPoint;
            flow.difficulty = difficulty;
            flow.logger = logger;
            flow.scoreManager = scoreManager;
            flow.trialCounter = trialCounter;
            flow.countdown = countdown;
            flow.rtRecorder = rtRecorder;
            flow.redFireballButton = redBtn;
            flow.blueWaterballButton = blueBtn;
            flow.stageText = hud.stage;
            flow.feedback = feedback;
            flow.stunOverlay = stunOverlay;
            flow.resultPanel = resultPanel;

            EditorUtility.SetDirty(gameManager);
            EditorUtility.SetDirty(uiController);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("[StroopSceneBuilder] ✅ Scene built. Press Cmd+S to save, then Play.");
            EditorUtility.DisplayDialog(
                "Build Complete",
                "場景已建好！\n\n下一步：\n1. Cmd+S 存檔\n2. 按 Play 測試\n\n" +
                "⚠️ 中文若顯示成方框：\n要到 Window → TextMeshPro → Settings\n→ Fallback Font Assets 加 CJK 字型。",
                "OK");
        }

        [MenuItem(MENU_CLEAR)]
        public static void ClearOnly()
        {
            if (!EditorUtility.DisplayDialog("Clear Scene",
                "清掉所有非 Camera/Light 的物件，要繼續嗎？", "Yes", "Cancel"))
                return;
            ClearScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // ===================================================================
        // 子建構函式
        // ===================================================================

        static void ClearScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Main Camera" || root.name == "Directional Light")
                    continue;
                Object.DestroyImmediate(root);
            }
        }

        static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PREFAB_FOLDER))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Stimuli");
        }

        static Sprite LoadOrConvertSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null) return sprite;

            // 嘗試把 Texture2D 轉成 Sprite
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return null;
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        static StimulusItem BuildStimulusPrefab(Sprite fire, Sprite water)
        {
            // 臨時建在場景上 → 存成 prefab → 從場景刪除
            var temp = new GameObject("StimulusTemplate", typeof(RectTransform));
            var rt = temp.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 300);
            var img = temp.AddComponent<Image>();
            img.sprite = fire;
            img.color = Color.white;
            img.raycastTarget = false;

            var stim = temp.AddComponent<StimulusItem>();
            stim.fireSprite = fire;
            stim.waterSprite = water;

            var prefab = PrefabUtility.SaveAsPrefabAsset(temp, PREFAB_PATH);
            Object.DestroyImmediate(temp);
            return prefab.GetComponent<StimulusItem>();
        }

        static Canvas BuildCanvas()
        {
            var go = new GameObject("Canvas");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        static RectTransform BuildSpawnPoint(Transform parent)
        {
            var rt = CreateUIObject("SpawnPoint", parent, AnchorPreset.MiddleCenter,
                new Vector2(0, 100), Vector2.zero);
            return rt;
        }

        static (Button red, Button blue) BuildAttackButtons(Transform parent)
        {
            // 用 middle-center 而非 bottom-center，這樣 Pos Y=-300 才會在畫面內
            var red = CreateButton("RedFireballButton", parent, RED_FIRE, "FIRE",
                new Vector2(-300, -300), new Vector2(280, 140), AnchorPreset.MiddleCenter);
            var blue = CreateButton("BlueWaterballButton", parent, BLUE_WATER, "WATER",
                new Vector2(300, -300), new Vector2(280, 140), AnchorPreset.MiddleCenter);
            return (red, blue);
        }

        struct HUDTexts
        {
            public TextMeshProUGUI stage, score, trial, timer, feedback;
        }

        static HUDTexts BuildHUDTexts(Transform parent)
        {
            return new HUDTexts
            {
                stage = CreateTMP("StageText", parent, "Stage 1: Training", 48,
                    new Vector2(0, 400), new Vector2(900, 100)),
                score = CreateTMP("ScoreText", parent, "Score: 0", 36,
                    new Vector2(-700, 450), new Vector2(400, 80)),
                trial = CreateTMP("TrialText", parent, "Round 0 / 30", 36,
                    new Vector2(700, 450), new Vector2(400, 80)),
                timer = CreateTMP("TimerText", parent, "2.5s", 60,
                    new Vector2(0, 320), new Vector2(300, 100)),
                feedback = CreateTMP("FeedbackText", parent, "", 60,
                    new Vector2(0, -100), new Vector2(900, 100)),
            };
        }

        static GameObject BuildStunOverlay(Transform parent)
        {
            var go = new GameObject("StunOverlay", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            SetAnchor(rt, AnchorPreset.StretchAll);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = new Color(PURPLE_STUN.r, PURPLE_STUN.g, PURPLE_STUN.b, 0); // alpha 0
            img.raycastTarget = false; // 不擋按鈕點擊
            return go;
        }

        static ResultPanel BuildResultPanel(Transform parent)
        {
            var go = new GameObject("ResultPanel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            SetAnchor(rt, AnchorPreset.StretchAll);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.85f);

            // 8 個 Text
            var title = CreateTMP("TitleText", go.transform, "Result", 64,
                new Vector2(0, 350), new Vector2(1000, 100));
            var score = CreateTMP("ScoreText", go.transform, "", 40,
                new Vector2(0, 250), new Vector2(1000, 80));
            var accuracy = CreateTMP("AccuracyText", go.transform, "", 36,
                new Vector2(0, 180), new Vector2(1000, 70));
            var rtCong = CreateTMP("RTCongruentText", go.transform, "", 32,
                new Vector2(0, 100), new Vector2(1000, 60));
            var rtIncong = CreateTMP("RTIncongruentText", go.transform, "", 32,
                new Vector2(0, 30), new Vector2(1000, 60));
            var cost = CreateTMP("InterferenceCostText", go.transform, "", 40,
                new Vector2(0, -50), new Vector2(1000, 80));
            var interp = CreateTMP("InterpretationText", go.transform, "", 32,
                new Vector2(0, -130), new Vector2(1200, 60));
            var exportPath = CreateTMP("ExportPathText", go.transform, "", 24,
                new Vector2(0, -250), new Vector2(1200, 50));

            // 2 個 Button
            var playAgain = CreateButton("PlayAgainButton", go.transform, GREEN_BUTTON, "Play Again",
                new Vector2(-200, -380), new Vector2(280, 120), AnchorPreset.MiddleCenter);
            var quit = CreateButton("QuitButton", go.transform, GRAY_BUTTON, "Quit",
                new Vector2(200, -380), new Vector2(280, 120), AnchorPreset.MiddleCenter);

            var panel = go.AddComponent<ResultPanel>();
            panel.titleText = title;
            panel.scoreText = score;
            panel.accuracyText = accuracy;
            panel.rtCongruentText = rtCong;
            panel.rtIncongruentText = rtIncong;
            panel.interferenceCostText = cost;
            panel.interpretationText = interp;
            panel.exportPathText = exportPath;
            panel.playAgainButton = playAgain;
            panel.quitButton = quit;

            go.SetActive(false);
            return panel;
        }

        // ===================================================================
        // 通用工廠
        // ===================================================================

        static RectTransform CreateUIObject(string name, Transform parent, AnchorPreset anchor,
            Vector2 anchoredPos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            SetAnchor(rt, anchor);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            return rt;
        }

        static TextMeshProUGUI CreateTMP(string name, Transform parent, string text, float fontSize,
            Vector2 pos, Vector2 size, AnchorPreset anchor = AnchorPreset.MiddleCenter)
        {
            var rt = CreateUIObject(name, parent, anchor, pos, size);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            return tmp;
        }

        static Button CreateButton(string name, Transform parent, Color color, string label,
            Vector2 pos, Vector2 size, AnchorPreset anchor)
        {
            var rt = CreateUIObject(name, parent, anchor, pos, size);
            var go = rt.gameObject;

            var img = go.AddComponent<Image>();
            img.color = color;
            // 試著用 Unity 內建 UISprite（圓角），找不到就純色方框
            var sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
            }
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelRT = new GameObject("Label", typeof(RectTransform)).GetComponent<RectTransform>();
            labelRT.SetParent(go.transform, false);
            SetAnchor(labelRT, AnchorPreset.StretchAll);
            labelRT.offsetMin = Vector2.zero;
            labelRT.offsetMax = Vector2.zero;
            var tmp = labelRT.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 36;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;

            return btn;
        }

        // ===================================================================
        // Anchor preset helper
        // ===================================================================

        enum AnchorPreset
        {
            MiddleCenter,
            TopCenter,
            BottomCenter,
            TopLeft,
            TopRight,
            StretchAll,
        }

        static void SetAnchor(RectTransform rt, AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.MiddleCenter:
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.TopCenter:
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
                    rt.pivot = new Vector2(0.5f, 1f);
                    break;
                case AnchorPreset.BottomCenter:
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
                    rt.pivot = new Vector2(0.5f, 0f);
                    break;
                case AnchorPreset.TopLeft:
                    rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
                    rt.pivot = new Vector2(0f, 1f);
                    break;
                case AnchorPreset.TopRight:
                    rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
                    rt.pivot = new Vector2(1f, 1f);
                    break;
                case AnchorPreset.StretchAll:
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }
    }
}
#endif
