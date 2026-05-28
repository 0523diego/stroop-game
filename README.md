# 🎮 StroopGame — 魔王與勇者：幻術之戰

> 期末作品：基於 Shape-Color Stroop Task 的兒童認知訓練遊戲
> Unity 2022.3 LTS · C# · Mac M4

---

## 🚀 Quick Start（組員看這）

1. **安裝 Unity Hub** + **Unity 2022.3.62f3 LTS**
   - Unity Hub：[unity.com/download](https://unity.com/download)
   - 在 Unity Hub 點 **Installs → Install Editor → 選 2022.3.62f3**
2. **Clone 這個 repo**：
   ```bash
   git clone https://github.com/0523diego/stroop-game.git
   ```
3. **Unity Hub → Open** → 選 `stroop-game/My project (1)/` 資料夾
4. 首次開啟 Unity 會花 **5-10 分鐘** 重建 Library/（吃 CPU 是正常的）
5. Project 面板 → 雙擊 `Assets/Scenes/SampleScene`
6. **如果場景看起來空空的**：上方選單 → **StroopGame → Build Complete Scene** → Yes
7. 按 ▶ Play 開始玩

> 💡 **重要**：選單 `StroopGame` 的內容只能在「不是 Play 模式」時點。不然會出錯。

---

## 快速導覽

| 你想做的事 | 看哪份文件 |
|----------|----------|
| 第一次組裝遊戲 | [`SETUP-GUIDE.md`](SETUP-GUIDE.md) |
| 寫期末報告 | [`docs/FINAL-REPORT.md`](docs/FINAL-REPORT.md) |
| 生成美術 | [`docs/ART-PROMPTS.md`](docs/ART-PROMPTS.md) |
| 看程式碼架構 | `Assets/Scripts/` |

---

## 已產出檔案清單

### 程式碼（9 個 C# 腳本）
```
Assets/Scripts/
├── Core/
│   ├── GameTypes.cs         — 列舉與資料結構
│   ├── StimulusItem.cs      — 怪物 Prefab（動態染色）
│   ├── DifficultyManager.cs — 三階段難度系統
│   ├── DataLogger.cs        — CSV 匯出 + @DATA Console 過濾
│   └── GameFlowManager.cs   — 主控制器（導演）
├── Toolkit/
│   ├── ScoreManager.cs      — 通用記分（可跨專案）
│   ├── ReactionTimeRecorder.cs — 毫秒級計時
│   ├── TrialCounter.cs      — 回合管理
│   └── CountdownTimer.cs    — 倒數計時
└── UI/
    ├── FeedbackText.cs      — 中央回饋淡入淡出
    ├── StunOverlay.cs       — 暈眩遮罩 + 螢幕抖動
    └── ResultPanel.cs       — 結算 + 干擾代價解讀
```

### 文件
- `README.md`（本檔）
- `SETUP-GUIDE.md` — Unity 場景組裝步驟
- `docs/FINAL-REPORT.md` — 完整期末報告（含學術文獻）
- `docs/ART-PROMPTS.md` — AI 美術生成指令

### 待你產出
- `Assets/Art/Stimuli/fire_shape.png`（必要）
- `Assets/Art/Stimuli/water_shape.png`（必要）
- 其他美術 / 音效（選做）

---

## 環境準備

- ✅ Unity Hub（已裝）
- ⏳ Unity 2022.3 LTS（記得不是 Unity 6）
- ⏳ VS Code + C# Dev Kit
- ⏳ 將 `Assets/Scripts/` 整個複製到新 Unity 專案

---

## 核心數據指標

| 指標 | 公式 | 意義 |
|------|------|------|
| **干擾代價** | RT_incongruent − RT_congruent | 抗干擾能力（越小越好） |
| 一致 RT | 平均反應時間（顏色形狀一致） | 基線速度 |
| 衝突 RT | 平均反應時間（顏色形狀衝突） | 受干擾時的速度 |
| 正確率 | 正確 / 總 trials | 整體準確度 |
| 衝突錯誤率 | 衝突情境的錯誤比例 | 被干擾誤擊率 |

---

## 借自 Mind-Shuffle 的設計
- Prefab + 動態染色
- 模組化 Toolkit（Score / Timer / RT / TrialCounter）
- `@DATA,` 前綴的 Console CSV 匯出

## 自製超越之處
- 完整三階段難度遞增
- 暈眩懲罰（含螢幕抖動）
- 自動計算與解讀干擾代價
- 奇幻風格遊戲化體驗

---

## 接下來你要做的事

1. **裝 Unity 2022.3 LTS**（Unity Hub 裡）
2. **裝 VS Code**（[code.visualstudio.com](https://code.visualstudio.com)）
3. **新建 Unity 2D 專案**，把 `Assets/Scripts/` 整個資料夾複製進去
4. **生兩張美術**（fire_shape、water_shape，照 ART-PROMPTS.md）
5. **跟著 SETUP-GUIDE.md** 組裝場景
6. **按 Play 測試**
7. **匯出 CSV 數據**
8. **寫期末報告**（FINAL-REPORT.md 已是完整骨架）

完成度估計：**3-5 天可達期末可交付狀態**。
