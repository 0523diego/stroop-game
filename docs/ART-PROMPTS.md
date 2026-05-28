# 🎨 美術資源生成 Prompts

> 全部用 **Midjourney**、**Flux**、**Stable Diffusion** 或 **Claude design** 都能生。
> **核心美術風格**：日式手繪 × 童趣魔幻 × 半透明發光元素
> **目標年齡**：7-10 歲兒童（可愛但不幼稚）
> **色彩規範**：紅色 = #F44C26，藍色 = #3389F2（在 Unity 裡動態染色）

---

## 一、刺激物（最重要，必須生）

### 火焰怪物 — `fire_shape.png`
> **白色形狀，透明背景，512×512 PNG**
> 在 Unity 裡程式會自動染成紅或藍

```
A cute cartoon fire spirit monster shape, white silhouette on transparent background,
chibi style, friendly round face with big innocent eyes, fire-shaped body with curved flame tongues,
clean vector style, no outline, soft edges, suitable for children's educational game,
centered composition, 512x512, PNG with transparent background
```

### 水滴怪物 — `water_shape.png`
```
A cute cartoon water drop spirit monster shape, white silhouette on transparent background,
chibi style, friendly round face with big innocent eyes, teardrop-shaped body with small wave details,
clean vector style, no outline, soft edges, suitable for children's educational game,
centered composition, 512x512, PNG with transparent background
```

**重點**：兩張圖**只有形狀差**（火焰 vs 水滴），其他全部一致——表情、線條風格、白色填色。
這樣動態染色才會看起來自然。

---

## 二、攻擊按鈕（4 張，或共用底圖）

### 紅火球按鈕 — `btn_red_fireball.png`
```
A glowing red fireball icon on circular button, 3D rendered style,
warm orange-red gradient with hot core, soft glow effect,
game UI button, suitable for children, vibrant and exciting,
512x512, PNG transparent background
```

### 藍水球按鈕 — `btn_blue_waterball.png`
```
A glowing blue water orb icon on circular button, 3D rendered style,
cool cyan-blue gradient with translucent core, soft water reflection,
game UI button, suitable for children, refreshing and energetic,
512x512, PNG transparent background
```

---

## 三、背景（1 張）

### 主場景背景 — `bg_main.png`
```
A magical fantasy forest background for a children's game, painterly style,
soft gradient sky from deep purple to indigo, distant mountain silhouette,
floating glowing orbs and sparkles, ethereal and dreamlike atmosphere,
mystical, low-contrast composition so UI elements stand out,
1920x1080, no characters, landscape orientation
```

### Stage 3 魔王城背景 — `bg_castle.png`（選做）
```
A dark fantasy demon castle silhouette in the distance, painterly style,
ominous red and purple sky, lightning in clouds, fog at the base,
high tension atmosphere for children's game, still appropriate for ages 7-10,
1920x1080, landscape, no characters
```

---

## 四、特效

### 命中爆炸 — `effect_hit.png`
```
A burst star effect for game feedback, white center with rainbow rays,
sparkly cartoon explosion, comic book pow-style burst,
512x512, transparent background, centered
```

### 暈眩星星 — `effect_stun.png`（環繞角色頭部用）
```
Cartoon dizzy stars circling effect, golden yellow spinning stars,
playful and exaggerated, small sparkles around them,
512x512, transparent background, suitable for stun visual feedback
```

---

## 五、UI 元素

### 主標題 — `logo_title.png`
```
A magical fantasy game title logo with the text "魔王與勇者：幻術之戰"
in stylized Chinese calligraphy with magical sparkles,
golden glow on the characters, ornamental flourishes,
game logo style for children, premium feel,
1024x512, transparent background
```

如果 AI 不支援中文，改用：
```
Title text "Stroop Quest" in magical glowing fantasy game font,
golden gradient letters with sparkle particles, ornate but readable,
1024x512, transparent background
```

### 結算面板 — `panel_result.png`
```
A scroll-style result panel background for fantasy game,
old parchment texture with gold border, slightly translucent,
suitable for displaying game stats, centered design,
800x1000, transparent background
```

---

## 六、音樂 / 音效（用 Suno + freesound.org）

### Suno Prompt（BGM）
```
Magical adventure children's game background music,
medieval fantasy theme with light orchestral elements,
flute and harp lead, gentle tension but encouraging,
loopable, 2 minutes, instrumental
```

### freesound.org 搜尋關鍵字
- 正確音效：`magic chime`、`positive ding`、`pickup star`
- 錯誤音效：`error buzz`、`stun magic`、`fail trumpet short`
- 升級音效：`level up fanfare`、`magical sparkle`
- 怪物出現：`monster appear`、`whoosh magic`

---

## 七、執行順序

**最小可玩版本只需這 2 張**：
1. `fire_shape.png`
2. `water_shape.png`

兩張圖生完，丟進 `Assets/Art/Stimuli/`，遊戲就能跑起來看到怪物。

**Polish 階段再做**：
3. 兩個按鈕圖
4. 背景
5. 特效
6. 標題 logo
7. 結算面板
8. 音樂音效

---

## 八、檔案放哪

```
StroopGame/Assets/Art/
├── Stimuli/
│   ├── fire_shape.png        ← 必要
│   └── water_shape.png       ← 必要
├── UI/
│   ├── btn_red_fireball.png
│   ├── btn_blue_waterball.png
│   ├── logo_title.png
│   ├── panel_result.png
│   └── bg_main.png
└── Effects/
    ├── effect_hit.png
    └── effect_stun.png

StroopGame/Assets/Audio/
├── BGM_main.mp3
├── sfx_correct.wav
├── sfx_wrong.wav
├── sfx_stun.wav
└── sfx_levelup.wav
```

**Unity 匯入後記得**：
- 所有 PNG → Inspector → Texture Type 設為 **Sprite (2D and UI)**
- 按 **Apply**
