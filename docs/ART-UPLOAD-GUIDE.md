# 美術組上傳指南

> 給美術組：你們的圖要放對位置、用對檔名，技術組才能無痛接進遊戲。

---

## ⚠️ 最重要的事：放對資料夾

repo 裡有**兩個** `Assets` 資料夾，**只有一個是對的**：

```
stroop-game/
├── Assets/                      ❌ 不要放這裡（這是舊的備份）
└── My project (1)/
    └── Assets/
        └── Art/                 ✅ 放這裡！全部圖都放這底下
            ├── Stimuli/         ← 怪物
            ├── UI/              ← 標題、按鈕、武器圖示
            ├── Backgrounds/     ← 三關背景（城堡等）
            └── Effects/         ← 特效
```

**完整正確路徑**：`My project (1)/Assets/Art/`

放錯到外層的 `Assets/` Unity 會讀不到，等於白做。

---

## 檔名規則（一定要照這個命名）

技術組的程式會**自動依檔名抓圖**，檔名打錯就要手動接，很麻煩。請嚴格照下表：

### 怪物（Stimuli/）

| 檔名 | 說明 | ⚠️ 特別注意 |
|------|------|-----------|
| `fire_shape.png` | 火焰怪 | **純白色！** 程式會自動染成紅 / 藍 |
| `water_shape.png` | 水滴怪 | **純白色！** 同上 |

> 怪物**一定要純白色 + 透明背景**。如果交「紅色火焰」會壞掉，因為遊戲的核心機制就是程式動態染色（藍火、紅水才是 Stroop 衝突）。
> 用**完全一樣的檔名**蓋掉現有的，技術組的所有設定才會自動沿用。

### UI（UI/）

| 檔名 | 說明 |
|------|------|
| `logo_title.png` | 遊戲標題 logo |
| `btn_fire.png` | 火球攻擊按鈕圖示 |
| `btn_water.png` | 水球攻擊按鈕圖示 |

### 背景（Backgrounds/）

| 檔名 | 說明 |
|------|------|
| `bg_stage1.png` | 第一關：訓練之地 |
| `bg_stage2.png` | 第二關：幻術森林 |
| `bg_stage3.png` | 第三關：魔王城（城堡） |

### 特效（Effects/）

| 檔名 | 說明 |
|------|------|
| `effect_hit.png` | 命中爆炸 |
| `effect_stun.png` | 暈眩星星 |

---

## 格式規範（所有圖都要遵守）

1. **格式 = PNG**（不要 JPG，會失去透明背景）
2. **怪物 / 按鈕 / 特效 = 透明背景**（要有 alpha channel）
3. **背景 = 不需透明**，但中間區域要乾淨（怪物會出現在那）
4. **解析度**：
   - 怪物 / 按鈕 / 特效：**512 × 512**
   - 背景：**1920 × 1080**
5. **檔名全小寫 + 底線**，不要空格、不要中文（`fire_shape.png` ✅，`火焰怪.png` ❌）

---

## 怎麼上傳到 GitHub（不用裝 Git，用網頁就好）

### 方法 A：GitHub 網頁直接拖拉（最簡單）

1. 打開 https://github.com/0523diego/stroop-game
2. 點進資料夾：`My project (1)` → `Assets` → `Art` → 你要放的子資料夾（如 `Stimuli`）
3. 點右上角 **Add file** → **Upload files**
4. 把 PNG 拖進去
5. 下面填一句說明（如「加上火焰怪 v2」）
6. 點綠色 **Commit changes**

### 方法 B：你們有人會用 Git

```bash
git clone https://github.com/0523diego/stroop-game.git
# 把圖放進 My project (1)/Assets/Art/ 對應資料夾
git add .
git commit -m "加美術素材 v1"
git push
```

---

## ✅ 上傳後通知技術組

上傳完在群組講一聲：「圖放上去了，在 Art/Stimuli」之類的。
技術組會 `git pull` 接下來、在 Unity 裡設定好、接進遊戲。

---

## 進度 Checklist（複製去打勾）

**怪物**
- [ ] fire_shape.png
- [ ] water_shape.png

**UI**
- [ ] logo_title.png
- [ ] btn_fire.png
- [ ] btn_water.png

**背景**
- [ ] bg_stage1.png
- [ ] bg_stage2.png
- [ ] bg_stage3.png

**特效**
- [ ] effect_hit.png
- [ ] effect_stun.png
