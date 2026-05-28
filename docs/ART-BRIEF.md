# 🎨 美術組工作清單 — StroopGame

> 給美術組的精簡 brief（看這份就好，ART-PROMPTS.md 是原始版可參考）
> 遊戲：**魔王與勇者：幻術之戰**（Stroop 認知訓練）
> 目標族群：7-10 歲兒童
> UI 語言：英文（暫時，之後可改回中文）

---

## 一、共通風格指引

| 項目 | 規範 |
|------|------|
| 美術風格 | 日式手繪 × 童趣魔幻 × 半透明發光元素 |
| 線條 | 乾淨向量風、柔和邊緣、無外框或淺色細邊 |
| 色調 | 高彩度、低對比（讓 UI 文字突出） |
| 主要色票 | **紅 #F44C26**、**藍 #3389F2**（程式用） |
| 輔助色 | 紫 #6A1A99（暈眩用）、金黃（升級用） |
| 禁忌 | 不要恐怖、暗黑、寫實風（這是兒童遊戲） |

---

## 二、素材清單（依優先順序）

### 🔴 必要（沒這個遊戲長相會很醜）

#### 1. 火焰怪物 — `fire_shape.png`

| 規格 | 值 |
|------|---|
| 解析度 | 512 × 512 |
| 格式 | PNG，**透明背景** |
| 顏色 | **純白色填色**（重要！） |
| 構圖 | 置中，怪物約佔畫面 70% |

**為什麼是純白色？**
> 遊戲程式會在執行時動態把這張圖染成紅色或藍色（這是 Stroop 衝突的核心機制）。如果你交「紅色火焰」，程式染色會變奇怪的顏色。**一定要純白色 + 透明背景**。

**AI Prompt 參考：**
```
A cute cartoon fire spirit monster shape, white silhouette on transparent background,
chibi style, friendly round face with big innocent eyes, fire-shaped body with curved flame tongues,
clean vector style, no outline, soft edges, suitable for children's educational game,
centered composition, 512x512, PNG with transparent background
```

#### 2. 水滴怪物 — `water_shape.png`

| 規格 | 同上 |
|------|------|

**重點**：與 fire_shape 同款風格、同表情、同尺寸。**只差在形狀**（水滴 vs 火焰）。這樣動態染色後兩隻怪物看起來才是「同一族但屬性不同」。

**AI Prompt 參考：**
```
A cute cartoon water drop spirit monster shape, white silhouette on transparent background,
chibi style, friendly round face with big innocent eyes, teardrop-shaped body with small wave details,
clean vector style, no outline, soft edges, suitable for children's educational game,
centered composition, 512x512, PNG with transparent background
```

---

### 🟡 重要（讓遊戲看起來像「遊戲」而不是「實驗工具」）

#### 3. 主場景背景 — `bg_main.png`

| 規格 | 值 |
|------|---|
| 解析度 | 1920 × 1080 |
| 格式 | PNG（不需透明） |
| 內容 | 奇幻森林 / 魔法場景 |

**重點**：**對比度要低**，因為文字會疊上去。中間區域（怪物會出現的地方）要乾淨，避免複雜紋路干擾辨識。

```
A magical fantasy forest background for a children's game, painterly style,
soft gradient sky from deep purple to indigo, distant mountain silhouette,
floating glowing orbs and sparkles, ethereal and dreamlike atmosphere,
mystical, low-contrast composition so UI elements stand out,
1920x1080, no characters, landscape orientation
```

#### 4. 紅火球按鈕 — `btn_red_fireball.png`

| 規格 | 值 |
|------|---|
| 解析度 | 512 × 512 |
| 格式 | PNG，透明背景 |
| 顏色 | 紅橘漸層 (#F44C26 為主) |

```
A glowing red fireball icon on circular button, 3D rendered style,
warm orange-red gradient with hot core, soft glow effect,
game UI button, suitable for children, vibrant and exciting,
512x512, PNG transparent background
```

#### 5. 藍水球按鈕 — `btn_blue_waterball.png`

```
A glowing blue water orb icon on circular button, 3D rendered style,
cool cyan-blue gradient with translucent core, soft water reflection,
game UI button, suitable for children, refreshing and energetic,
512x512, PNG transparent background
```

---

### 🟢 加分（行有餘力做）

| 編號 | 檔名 | 用途 |
|------|------|------|
| 6 | `bg_castle.png` | Stage 3 魔王城背景（黑暗 / 高張力但仍適合兒童） |
| 7 | `effect_hit.png` | 命中爆炸特效（給打擊感） |
| 8 | `effect_stun.png` | 暈眩星星環繞 |
| 9 | `logo_title.png` | 主標題（英文：「Stroop Quest」或類似） |
| 10 | `panel_result.png` | 結算面板背景（羊皮紙風） |

詳細 prompt 參考 `ART-PROMPTS.md` 原始版。

---

## 三、技術規格（給美術組對齊）

### 檔案格式
- **PNG**（不要 JPG）
- 透明背景的素材**一定要有 alpha channel**
- 不要嵌入 ICC profile（會被 Unity 警告）

### 檔名規則
- 全小寫 + 底線
- 例：`fire_shape.png` ✅、`Fire Shape.png` ❌

### 解析度
- 怪物 / 按鈕：**512 × 512**
- 背景：**1920 × 1080**
- 寧可大不要小（縮小不失真，放大會糊）

---

## 四、繳交方式

**選一種：**

1. **直接放到專案資料夾**
   ```
   ~/Desktop/StroopGame-handoff/StroopGame/My project (1)/Assets/Art/
   ├── Stimuli/
   │   ├── fire_shape.png        ← 蓋掉舊的
   │   └── water_shape.png       ← 蓋掉舊的
   ├── UI/
   │   ├── btn_red_fireball.png
   │   ├── btn_blue_waterball.png
   │   └── bg_main.png
   └── Effects/
       ├── effect_hit.png
       └── effect_stun.png
   ```

2. **打包傳給技術組**：包成 zip，命名 `art_v1.zip`，內容對照上面清單

---

## 五、進度追蹤 Checklist

複製這個給美術組打勾用：

- [ ] fire_shape.png（**必要**）
- [ ] water_shape.png（**必要**）
- [ ] bg_main.png
- [ ] btn_red_fireball.png
- [ ] btn_blue_waterball.png
- [ ] bg_castle.png
- [ ] effect_hit.png
- [ ] effect_stun.png
- [ ] logo_title.png
- [ ] panel_result.png

---

## 六、AI 生圖建議工具

| 工具 | 月費 | 適合做 |
|------|------|--------|
| **Midjourney** | $10 起 | 風格一致性最強，怪物 / 場景優先選 |
| **Flux** (via fal.ai) | 按次計費 | 速度快、便宜 |
| **Stable Diffusion**（本地） | 免費 | 需自己跑，可控性最高 |
| **Claude / GPT** | 訂閱 | 拆 prompt、調風格細節 |

**強烈建議**：兩張怪物（fire / water）一起生，**用同一條 prompt 改一個關鍵字**（"fire-shaped" ↔ "water-drop-shaped"），這樣風格才會一致。

---

## 七、給美術組的話

- 兩張怪物**形狀差**就好，**表情、風格、構圖完全一致** → 動態染色才好看
- 中間區域（怪物出現的地方）背景**留白 / 簡單**，不要花俏紋路
- 想要可愛但不要太幼稚（目標 7-10 歲，太萌會被覺得是給 3 歲玩的）
- 有問題就問技術組要不要在 Unity 裡先看效果，不要悶頭生一堆才發現方向錯

任何疑問 → 找技術組對齊。
