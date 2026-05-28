# 🎮 StroopGame — Unity 場景組裝指南

> Unity 2022.3 LTS + VS Code 裝好後，照這份做。
> 第一次跑起來預估 **40-60 分鐘**。

---

## 步驟 1：建立 Unity 專案（5 分鐘）

1. Unity Hub → **Projects** → **New Project**
2. 設定：
   - **Editor Version**：Unity **2022.3.x LTS**
   - **Template**：**2D (Built-In Render Pipeline)**
   - **Project Name**：`StroopGame`
   - **Location**：`/Users/diego/Documents/`（**避開含空格的路徑**）
3. 按 **Create Project**，等 1-3 分鐘

**完成後**：把我已產出的 `/Users/diego/3A game/StroopGame/Assets/Scripts/` 整個資料夾**複製**到新 Unity 專案的 `Assets/` 下。

---

## 步驟 2：匯入 TextMeshPro Essentials

1. 打開任意 Scene
2. Hierarchy 右鍵 → **UI → Text - TextMeshPro**
3. 跳出視窗按 **Import TMP Essentials**
4. 完成後可把剛剛建的 Text 刪掉

---

## 步驟 3：放美術資源

1. 把 `fire_shape.png`、`water_shape.png` 丟進 `Assets/Art/Stimuli/`（先用最小版本，其他可以晚點）
2. 點選每張圖 → Inspector → **Texture Type = Sprite (2D and UI)** → **Apply**

**沒有美術也能跑** —— Stimulus Prefab 的 Sprite 欄位先空著，Unity 會顯示白色框。

---

## 步驟 4：建場景結構

### 4-1：建 Canvas

Hierarchy → 右鍵 → **UI → Canvas**

Canvas 設定（Inspector）：
- **Canvas Scaler** → UI Scale Mode：**Scale With Screen Size**
- Reference Resolution：**1920 × 1080**
- Match：0.5

EventSystem 會自動生成（**不要刪**）。

### 4-2：建 SpawnPoint（生成位置）

Canvas → 右鍵 → **Create Empty**，改名 `SpawnPoint`

- Anchor：置中
- Pos：(0, 100)

### 4-3：建兩個攻擊按鈕

Canvas → 右鍵 → **UI → Button - TextMeshPro**，改名 `RedFireballButton`

Inspector：
- Anchor：底部中央
- Pos: (-300, -300)
- Width: 280, Height: 140
- Button → Image → Color：紅色（#F44C26）
- Button → Text → 字：「🔥 紅火球」、字級 36、白色

複製這個按鈕（Cmd+D），改名 `BlueWaterballButton`：
- Pos: (300, -300)
- 顏色：藍色（#3389F2）
- 字：「💧 藍水球」

### 4-4：建 UI 文字

Canvas 下建 4 個 Text - TextMeshPro：

| 物件名 | 位置 | 字級 | 內容 |
|--------|------|------|------|
| `StageText` | 中上 (0, 400) | 48 | 第一關：訓練之地 |
| `ScoreText` | 左上 (-700, 450) | 36 | 分數：0 |
| `TrialText` | 右上 (700, 450) | 36 | 回合 0 / 30 |
| `TimerText` | 中上偏下 (0, 320) | 60 | 2.5s |
| `FeedbackText` | 中央 (0, -100) | 60 | （空） |

### 4-5：建暈眩遮罩

Canvas → 右鍵 → **UI → Image**，改名 `StunOverlay`
- Anchor：填滿（Stretch all sides）
- Color：紫色 #6A1A99，Alpha 0
- 一開始**先勾「Active」打勾**

### 4-6：建結算面板

Canvas → 右鍵 → **UI → Panel**，改名 `ResultPanel`
- Inspector → 取消 Active 勾選（一開始隱藏）

在 ResultPanel 下建以下 TMP Text（每個都是 TextMeshPro Text）：

| 物件名 | 位置 |
|--------|------|
| `TitleText` | (0, 350) |
| `ScoreText` | (0, 250) |
| `AccuracyText` | (0, 180) |
| `RTCongruentText` | (0, 100) |
| `RTIncongruentText` | (0, 30) |
| `InterferenceCostText` | (0, -50) |
| `InterpretationText` | (0, -130) |
| `ExportPathText` | (0, -250) |

再建兩個按鈕：
- `PlayAgainButton` 位置 (-200, -350)，字「再玩一次」
- `QuitButton` 位置 (200, -350)，字「結束」

---

## 步驟 5：建 Stimulus Prefab（怪物模板）

> 借自 Mind-Shuffle 的「白色模板 + 動態染色」概念。

1. Canvas → 右鍵 → **UI → Image**，改名 `StimulusTemplate`
2. Inspector：
   - Width × Height：300 × 300
   - Source Image：暫時空著（之後拉 fire_shape 進去當預設）
3. **Add Component** → 搜 `Stimulus Item` → 加上
4. StimulusItem 元件設定：
   - Fire Sprite：拉 `fire_shape.png`
   - Water Sprite：拉 `water_shape.png`
5. **變成 Prefab**：把 `StimulusTemplate` 從 Hierarchy 拖到 `Assets/Prefabs/Stimuli/`
6. 從 Hierarchy **刪掉** `StimulusTemplate`（場景上不要有，只需要 Prefab）

---

## 步驟 6：建 GameManager 主物件

1. Hierarchy → 右鍵 → **Create Empty**，改名 `GameManager`
2. 加 6 個 Component（全部 Add Component 找）：

   **Core 層：**
   - `GameFlowManager`（主控）
   - `DifficultyManager`
   - `DataLogger`

   **Toolkit 層：**
   - `ScoreManager`
   - `TrialCounter`
   - `CountdownTimer`
   - `ReactionTimeRecorder`

3. 再建一個空物件 `UIController`，加 2 個 Component：
   - `FeedbackText`（拉 FeedbackText 物件到 text 欄位）
   - `StunOverlay`（拉 StunOverlay 物件到 stunImage、Canvas 到 shakeTarget）

4. **ResultPanel** 物件加 `ResultPanel` Component，把面板裡的 TMP 文字一一拉好。

---

## 步驟 7：連 GameFlowManager 的所有 References

點 GameManager，在 GameFlowManager 元件裡把以下欄位都拉好：

| 欄位 | 拉什麼 |
|------|--------|
| Stimulus Prefab | `Assets/Prefabs/Stimuli/StimulusTemplate.prefab` |
| Spawn Point | Hierarchy 的 SpawnPoint |
| Difficulty | GameManager 自己 |
| Logger | GameManager 自己 |
| Score Manager | GameManager 自己 |
| Trial Counter | GameManager 自己 |
| Countdown | GameManager 自己 |
| Rt Recorder | GameManager 自己 |
| Red Fireball Button | RedFireballButton |
| Blue Waterball Button | BlueWaterballButton |
| Stage Text | StageText |
| Feedback | UIController 上的 FeedbackText |
| Stun Overlay | UIController 上的 StunOverlay |
| Result Panel | ResultPanel 物件 |

**ScoreManager** 元件：Score Text → ScoreText
**TrialCounter** 元件：Trial Text → TrialText
**CountdownTimer** 元件：Timer Text → TimerText

---

## 步驟 8：第一次按 Play

如果你看到：
- 怪物會出現
- 按按鈕會有回饋
- 暈眩會發動
- 30 回合後出現結算

**恭喜你已經有期末交件版本了。**

如果有錯誤——

1. Console 有紅字？把錯誤訊息給我
2. References 沒拉？打開 GameManager 看是否有 None 欄位
3. EventSystem 是否在？刪了 Canvas 重建會自動生

---

## 步驟 9：匯出 CSV 數據（給期末報告用）

跑完一局後：
- **方法 A（自動）**：到 `~/Library/Application Support/[公司名]/StroopGame/` 找 `stroop_session_*.csv`
- **方法 B（Mind-Shuffle 風格）**：
  1. Unity Console 右上角取消勾選 **Show Timestamp**
  2. Console 設 **Stack Trace Logging → None**
  3. 搜尋列輸入 `@DATA`
  4. 全選 → 複製 → 貼到記事本另存 `.csv`

---

## 加分項（行有餘力再做）

| 項目 | 工作量 |
|------|-------|
| 加 BGM | 5 分鐘（拉 mp3 到 Audio Source） |
| 加音效 | 10 分鐘 |
| 加背景圖 | 5 分鐘 |
| 加特效粒子 | 30 分鐘 |
| 多個 Stimulus Prefab（不同造型） | 1 小時 |
| 主選單畫面 | 1 小時 |

---

完成這份指南 = **期末作業基本成果完成**。剩下就是寫報告。
