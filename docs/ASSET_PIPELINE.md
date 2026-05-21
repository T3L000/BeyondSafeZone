# Asset Pipeline

更新日期：2026-05-21

## 美术目标

第一版采用 `2D 像素画风`。比赛 Demo 的美术原则是少而统一：先保证所有素材像来自同一个游戏，再逐步提高精度。

当前锁定方向：

- 角色采用 `32x32` 像素格。
- 基础瓦片采用 `16x16` 像素格。
- 大型场景物件可使用 `32x32`、`32x48` 或多个 `16x16` 瓦片组合。
- 第一批素材优先服务 14 天陈醒线 Demo，不追求完整 30 天素材库。

## 像素规格

### 角色

- 单格尺寸：`32x32`
- 格式：PNG，透明背景
- 视角：2D 像素、轻俯视或正侧混合，但所有角色必须保持同一视角
- 描边：1-2 像素深色描边，所有角色统一
- 帧间隔：先按 `8-12 FPS` 预览，进入 Godot 后再按手感调整
- Godot 导入：关闭过滤，使用 nearest-neighbor，避免像素被平滑

### 动作最低规格

陈醒：

- 待机：2 帧
- 四向行走：每方向 4 帧
- 搜索：1 帧
- 受伤：1 帧
- 骑车：1 帧或 2 帧

祁眠：

- 待机：2 帧
- 四向行走：每方向 4 帧
- 搜索：1 帧
- 救援：1 帧
- 隐藏/剪影：1 帧

普通丧尸：

- 待机：2 帧
- 行走：4 帧
- 攻击：1 帧

血月丧尸：

- 待机：2 帧
- 奔跑：4 帧
- 攻击：1 帧
- 可基于普通丧尸改版，但轮廓、颜色和动作节奏必须明显不同

### 场景和瓦片

- 基础瓦片：`16x16`
- 常用物件：`16x16` 或 `32x32`
- 大物件：`32x48`、`48x32` 或瓦片拼接
- 场景优先做可读性，不做复杂透视
- 陈醒家/据点、医疗点、超市、药房和修车铺要共享同一脏旧末日调色倾向

## 调色和风格

基础方向：

- 主色：灰绿、脏黄、暗蓝灰、旧墙白
- 陈醒：偏蓝灰、普通幸存者、背包或外套识别点
- 祁眠：偏冷青或灰绿，轮廓更安静，隐藏状态可用剪影
- 普通丧尸：灰褐、黄绿、低饱和
- 血月丧尸：暗红、黑影、高对比边缘光

血月素材规则：

- 不只做整屏红色滤镜
- 用红色边缘光、拉长阴影、异常月光、快速动作和更尖锐轮廓区分
- 血月丧尸不能直接照搬商业丧尸或 Minecraft / Plants vs. Zombies 造型

## 文件组织

素材源文件放在：

- `assets/source/`

导出到 Godot 可直接使用的素材放在：

- `assets/sprites/`

建议后续扩展目录：

- `assets/source/characters/`
- `assets/source/enemies/`
- `assets/source/tilesets/`
- `assets/source/ui/`
- `assets/sprites/characters/`
- `assets/sprites/enemies/`
- `assets/sprites/tilesets/`
- `assets/sprites/ui/`

命名规则：

- 角色：`char_chenxing_walk_down_32x32.png`
- 祁眠：`char_qimian_idle_32x32.png`
- 敌人：`enemy_zombie_walk_32x32.png`
- 血月敌人：`enemy_blood_zombie_run_32x32.png`
- 场景瓦片：`tileset_hospital_16x16.png`
- UI 图标：`ui_icon_food_16x16.png`
- 预览 GIF：`preview_char_chenxing_walk.gif`

## 第一批素材清单

### Must Have

角色：

- 陈醒：待机、四向行走、搜索、受伤、骑车
- 祁眠：待机、四向行走、搜索、救援、隐藏/剪影

敌人：

- 普通丧尸：待机、行走、攻击
- 血月丧尸：待机、奔跑、攻击

场景：

- 陈醒家/据点
- 社区诊所或医院医疗点
- 超市
- 药房
- 自行车修理铺
- 血月夜据点外景

UI：

- 食物
- 饮水
- 药品
- 建材
- 自行车零件
- 电池
- 情报
- 血月预警

### Should Have

- 保护区外围剪影
- 地铁口入口
- 匿名药包
- 收音机
- 自行车单独物件
- 简单血月天空/远处尸群剪影

### Later

- 祁烬角色素材
- 特殊丧尸
- 更多地点瓦片
- 祁眠日志回放专用插图
- 宣传图和商店页视觉素材

## FrameRonin 流程

FrameRonin 在本项目中作为像素素材处理和 Sprite Sheet 整理工具，而不是唯一绘制工具。

推荐流程：

1. 收集素材来源：手绘草图、AI 初稿、合法授权素材或视频参考。
2. 写入版权记录：任何来源进入项目之前，先在 `docs/ASSET_LICENSE_LOG.md` 记录来源和授权。
3. FrameRonin 处理：视频转序列帧、去背、整理帧序、生成临时 Sprite Sheet 或 GIF。
4. 手工修正：用 Aseprite、LibreSprite 或 Piskel 清理边缘、统一描边、统一调色。
5. 导出 Sprite Sheet：按 `32x32` 角色格或 `16x16` 瓦片格导出。
6. 生成 GIF 预览：确认动作是否清楚、比例是否一致、帧序是否正确。
7. 放入项目目录：源文件进入 `assets/source/`，游戏用 PNG 进入 `assets/sprites/`。
8. Godot 导入：关闭过滤，检查像素边缘，必要时添加 import notes。
9. 更新版权记录：补充最终文件路径、修改记录和参赛可用状态。

## AI 生成素材规则

AI 可以用于生成原始草图、像素角色初稿、场景气氛图和占位素材。

AI 输出进入项目时必须：

- 记录生成工具和日期
- 保存最终 prompt 或简述
- 标记是否经过人工修改
- 确认平台条款允许商用和参赛展示
- 手工检查是否过度接近已有商业角色

建议 prompt 方向：

```text
32x32 top-down pixel art sprite sheet, post-apocalyptic survivor,
muted colors, dark outline, transparent or solid chroma-key background,
four-direction walk cycle, consistent scale, no text, no watermark,
not Minecraft, not Plants vs. Zombies.
```

## 版权规则

- Minecraft 和《植物大战僵尸》素材只允许私人占位。
- 参赛版本必须替换为原创、AI 生成后可商用、CC0 或明确授权素材。
- 没有明确授权的素材不得进入公开参赛包。
- CC-BY 等需要署名的素材必须在参赛说明或素材清单中署名。
- 素材不得只记录“来自网络”，必须记录具体 URL 或生成工具。
- 每个进入 `assets/sprites/` 的外部或 AI 生成素材，都要在 `docs/ASSET_LICENSE_LOG.md` 有对应记录。
