# 認知數位遊戲設計期末報告

## 《魔王與勇者：幻術之戰》—— 形狀-顏色 Stroop 任務的遊戲化實作

> **課程**：認知數位遊戲設計
> **開發者**：[你的名字]
> **合作者**：[你朋友 / 認知設計顧問的名字]
> **開發環境**：Unity 2022.3 LTS、C#、Mac M4
> **完成日期**：2026 年 [月] [日]

---

## 摘要

本作品以《Ready Player One》OASIS 的啟發為起點，結合認知心理學的 **Shape-Color Stroop Task**，設計一款訓練 7-10 歲兒童「干擾控制」與「反應抑制」能力的遊戲。玩家扮演勇者，面對會施展幻術的魔王軍，必須抑制顏色的直覺干擾，依照形狀辨識怪物屬性並選擇正確的攻擊。本遊戲採用三階段難度遞增，並收集毫秒級反應時間資料，計算「干擾代價 (Interference Cost)」作為玩家執行功能的量化指標。

---

## 一、設計動機

### 1.1 為什麼選擇認知訓練遊戲

傳統電腦化認知訓練測驗（如 PEBL、E-Prime 程式）介面冷漠、缺乏動機。本作品的設計目標是將嚴謹的 Stroop 範式轉化為兒童能持續投入的奇幻冒險，讓「訓練」變成「遊戲」。

### 1.2 為什麼選擇 Shape-Color Stroop

經典 Stroop 任務（讀字 vs 顏色）依賴文字閱讀自動化，**不適合初學識字的兒童**（Prevor & Diamond, 2005）。本作改用形狀作為目標屬性，顏色作為干擾屬性——人類視覺系統處理顏色的速度通常**快於形狀辨識**，因此這種設計在保持 Stroop 衝突本質的同時，讓 7-10 歲兒童也能參與。

---

## 二、認知心理學依據

### 2.1 核心機制：Shape-Color Stroop Task

| 試驗類型 | 例子 | 認知狀態 |
|----------|------|---------|
| **Congruent (一致)** | 紅色火焰怪、藍色水滴怪 | 顏色與形狀相符，神經處理流暢 |
| **Incongruent (衝突)** | 藍色火焰怪、紅色水滴怪 | 需要抑制顏色直覺，動用執行功能 |

### 2.2 關鍵測量指標：干擾代價

```
Interference Cost = RT_incongruent − RT_congruent
```

此指標是 Stroop 任務的核心輸出，代表大腦「踩煞車排除顏色干擾」所花的時間成本。**數值越小，代表執行功能與抗干擾能力越好**。

### 2.3 暈眩懲罰的理論依據

當玩家因被顏色干擾而誤擊時，遊戲強制 3 秒鎖定按鈕（「暈眩」）。這個機制來自 Metcalfe & Mischel (1999) 提出的 **冷/熱認知系統 (Hot/Cool System)**：

- **熱認知** = 情緒驅動、衝動的快速反應
- **冷認知** = 反思、規則導向的慢速思考

兒童犯錯後常陷入「狂按」的熱認知循環。強制冷卻時間中斷此循環，**逼迫切換回冷認知重新評估規則**。

### 2.4 與認知彈性的關係

雖然本作主要訓練干擾控制 (interference control)，但三階段難度也測試了 Diamond 等學者所定義的執行功能其他面向（Davidson et al., 2006），包括工作記憶與規則切換。

---

## 三、遊戲設計

### 3.1 世界觀

玩家扮演勇者，前往魔王城的途中要面對魔王軍的「幻術師」。幻術會用顏色欺騙玩家——但勇者必須訓練自己**只看本質（形狀），不被表象（顏色）所騙**。

### 3.2 核心玩法

```
螢幕中央：怪物（具有形狀屬性 + 顏色屬性）
螢幕下方：兩個攻擊按鈕（紅火球 / 藍水球）

規則：只看形狀，不看顏色
- 火焰形狀 → 按紅火球
- 水滴形狀 → 按藍水球

衝突情境會出現：
- 藍色的火焰怪（直覺想按藍水球，但要按紅火球）
- 紅色的水滴怪（直覺想按紅火球，但要按藍水球）
```

### 3.3 三階段難度系統

| 階段 | 名稱 | 衝突比例 | 反應時限 | 訓練目標 |
|------|------|---------|---------|---------|
| **Stage 1** | 訓練之地 | 0%（純一致） | 2.5s | 建立直覺與基本反應 |
| **Stage 2** | 幻術森林 | 25% 衝突 | 2.0s | 測試抗干擾能力 |
| **Stage 3** | 魔王城 | 50% 衝突 | 1.0s | 極端時間壓力下的執行功能 |

每階段答對 8 次自動進階，確保玩家**確實學會**才進入下一難度。

### 3.4 回饋系統

| 玩家行為 | 視覺回饋 | 聽覺回饋 | 數據懲罰 |
|---------|---------|---------|---------|
| 答對（一致） | 「✨ 破解幻術！」+ 加 10 分 | 叮叮聲 | 無 |
| 答對（衝突） | 「✨ 破解幻術！」+ 加 15 分（衝突獎勵） | 叮叮聲 | 無 |
| 升級 | 「⭐ 升級！」+ 階段轉場 | 升級號角 | 無 |
| 答錯 | 「💀 被顏色騙了！」+ 紫色暈眩遮罩 + 螢幕抖動 | 叭叭聲 | 扣 5 分 + 鎖按鈕 3 秒 |
| 超時 | 「⏰ 太慢了！」 | 鐘響 | 扣 5 分 |

---

## 四、技術實作

### 4.1 系統架構

```
GameFlowManager (導演)
├── DifficultyManager     ── 三階段難度狀態機
├── DataLogger            ── 試驗紀錄 + CSV 匯出
├── StimulusItem          ── 怪物 Prefab（動態染色）
│
├── Toolkit/
│   ├── ScoreManager      ── 通用記分（可跨專案）
│   ├── ReactionTimeRecorder ── 毫秒級計時
│   ├── TrialCounter      ── 回合管理
│   └── CountdownTimer    ── 每回合倒數
│
└── UI/
    ├── FeedbackText      ── 中央回饋（淡入淡出）
    ├── StunOverlay       ── 暈眩遮罩 + 螢幕抖動
    └── ResultPanel       ── 結算面板
```

### 4.2 借自 Mind-Shuffle 框架的設計
- **Prefab + 動態染色**：一個白色刺激物模板，透過程式隨機染紅或藍。
- **模組化 Toolkit**：ScoreManager / TrialCounter / CountdownTimer 獨立可復用。
- **@DATA Console 過濾**：所有試驗資料以 `@DATA,` 前綴印到 Console，研究員可一鍵過濾。

### 4.3 自製超越 Mind-Shuffle 之處
- **完整 Stroop 認知衝突機制**（Mind-Shuffle 是 task-switching，本作是干擾控制）
- **三階段難度自動進階**（Mind-Shuffle 沒有）
- **暈眩懲罰系統**（含螢幕抖動與遮罩動畫）
- **干擾代價自動計算與解讀**（結算畫面直接呈現數據意義）
- **奇幻風格美術**（非 Mind-Shuffle 的教學用簡圖）

### 4.4 開發環境
- **引擎**：Unity 2022.3 LTS
- **語言**：C# (.NET Standard 2.1)
- **UI**：Unity UI + TextMeshPro
- **AI 協作**：Claude Code（程式碼撰寫與架構建議）
- **美術**：AI 生成（Midjourney / Flux）
- **音樂**：Suno + freesound.org

---

## 五、測量指標與數據分析

### 5.1 自動收集的資料欄位

每個試驗自動記錄：

| 欄位 | 說明 |
|------|------|
| `trial` | 第幾回合 |
| `stage` | 1 / 2 / 3 |
| `shape` | Fire / Water |
| `color` | Red / Blue |
| `condition` | Congruent / Incongruent |
| `response` | 玩家選擇 |
| `correct` | 是否正確 |
| `rt_ms` | 反應時間（毫秒） |
| `timed_out` | 是否超時 |
| `timestamp` | UTC 時間戳 |

### 5.2 結算畫面顯示的衍生指標

- **總分** / **正確率**
- **一致情境平均反應時間**
- **衝突情境平均反應時間**
- **干擾代價** = 衝突 RT − 一致 RT
- **解讀文字**：依干擾代價自動分類為「強大 / 良好 / 成長中 / 需練習」

### 5.3 預期數據模式

根據 Davidson et al. (2006) 對 4-13 歲兒童的研究：

| 年齡 | 預期干擾代價 |
|------|-------------|
| 4-5 歲 | 400-600 ms |
| 6-7 歲 | 250-400 ms |
| 8-10 歲 | 150-250 ms |
| 11-13 歲 | 100-150 ms |
| 成人 | < 100 ms |

本作的目標族群 (7-10 歲) 應落在 150-400 ms 區間。

---

## 六、預期訓練成效

### 6.1 近遷移（near transfer）

- 在其他需要排除視覺干擾的任務中表現提升
- 注意力測驗成績改善
- 同類認知測驗的反應時間縮短

### 6.2 遠遷移（far transfer）

- **數學應用題**：抑制無效資訊干擾、切換解題策略（Bull & Scerif, 2001）
- **情緒調節**：面對挑釁時能停下來思考，不直接觸發衝動反擊
- **學業表現**：執行功能訓練對國小數學能力有預測性

### 6.3 重複玩的學習曲線

預期玩家在 5-10 次遊玩後：
- 反應時間 ↓ 20-30%
- 衝突情境的錯誤率 ↓ 40-50%
- 干擾代價 ↓ 30%

---

## 七、設計反思

### 7.1 為何不直接用 Mind-Shuffle 框架

老師提供的 Mind-Shuffle 是 **任務切換 (task-switching)** 範式（依規則切換看顏色或看種類），本作則是 **干擾控制 (interference control)** 範式（始終看形狀無視顏色）。兩者測試的是執行功能的不同面向。我選擇自製是為了：

1. 精準對應友人的認知設計文件
2. 增加暈眩懲罰、三階段難度等 Mind-Shuffle 缺乏的元素
3. 探索更高自由度的美術與互動設計

### 7.2 已知限制

- **僅一位玩家測試**（缺乏統計樣本）
- **未做 IRB 倫理審查**（若要在學校部署需補上）
- **WebGL 版本未實裝**（目前只支援 Mac Standalone）

### 7.3 未來工作

- 加入家長 / 老師端的儀表板，顯示學生長期進步曲線
- 與真實小學合作小規模 pilot study
- 加入更多認知範式（Go/No-Go、Flanker、Wisconsin Card Sorting）

---

## 八、參考文獻

Bull, R., & Scerif, G. (2001). Executive Functioning as a Predictor of Children's Mathematics Ability: Inhibition, Switching, and Working Memory. *Developmental Neuropsychology, 19*(3), 273-293. https://doi.org/10.1207/S15326942DN1903_3

Davidson, M. C., Amso, D., Anderson, L. C., & Diamond, A. (2006). Development of cognitive control and executive functions from 4 to 13 years: Evidence from manipulations of memory, inhibition, and task switching. *Neuropsychologia, 44*(11), 2037-2078. https://doi.org/10.1016/j.neuropsychologia.2006.02.006

Jin, T., Zhou, S., Lang, X., He, J., & Wang, W. (2022). Combined effect of color and shape on cognitive performance. *Mathematical Problems in Engineering, 2022*, 1-12. https://doi.org/10.1155/2022/3284313

Metcalfe, J., & Mischel, W. (1999). A hot/cool-system analysis of delay of gratification: Dynamics of willpower. *Psychological Review, 106*(1), 3-19. https://doi.org/10.1037/0033-295X.106.1.3

Prevor, M. B., & Diamond, A. (2005). Color–object interference in young children: A Stroop effect in children 3½–6½ years old. *Cognitive Development, 20*(2), 256-278. https://doi.org/10.1016/j.cogdev.2005.04.001

---

## 附錄

### A. 程式碼倉庫結構
```
StroopGame/
├── Assets/
│   ├── Scripts/Core/         (GameFlowManager, StimulusItem, DataLogger, ...)
│   ├── Scripts/Toolkit/      (ScoreManager, TrialCounter, ...)
│   ├── Scripts/UI/           (FeedbackText, StunOverlay, ResultPanel)
│   ├── Prefabs/Stimuli/      (StimulusTemplate.prefab)
│   ├── Art/Stimuli/          (fire_shape.png, water_shape.png)
│   └── Scenes/MainScene.unity
└── docs/                     (本報告、設計文件、美術 prompt)
```

### B. Demo 影片
[YouTube 或 Drive 連結]

### C. 試玩資料
匯出之 CSV 檔案：`stroop_session_YYYYMMDD_HHMMSS.csv`
