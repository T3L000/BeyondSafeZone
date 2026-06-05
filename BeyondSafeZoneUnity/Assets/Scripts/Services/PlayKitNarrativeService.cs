using System;
using System.Collections.Generic;
using BeyondSafeZone.Core;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.Services
{
    /// <summary>
    /// PlayKit.ai 叙事服务 —— 本地兜底占位。
    ///
    /// 等 Unity 安装完成并拉到 PlayKit.ai SDK 后，
    /// 需要读取 SDK 示例代码确认真实的 C# API 签名，
    /// 再用真实调用替换本地的 Fallback* 方法。
    ///
    /// 当前占位不做任何网络请求，不包含任何 API Key/Token。
    /// </summary>
    public static class PlayKitNarrativeService
    {
        /// <summary>是否已接入 PlayKit SDK（当前为 false）</summary>
        public static bool IsSdkAvailable { get; private set; } = false;

        // ============ 本地兜底：每日独白 ============

        /// <summary>
        /// 生成每日叙事独白。
        /// 兜底：返回预设模板文本，结合当前 GameState。
        /// SDK 可用时：调用 PlayKit 文本生成 API 生成动态独白。
        /// </summary>
        public static string FallbackDailyMonologue(GameState state, int day)
        {
            return Core.TextRenderer.DailyMonologue(state, day);
        }

        // ============ 本地兜底：祁眠主观片段 ============

        /// <summary>
        /// 生成祁眠的主观叙述片段。
        /// 兜底：返回预制片段列表。
        /// SDK 可用时：调用 PlayKit 文本生成 API 生成动态叙事。
        /// </summary>
        public static string FallbackQimianFragment(GameState state, int day, string context)
        {
            var fragments = new Dictionary<int, string>
            {
                {5, "他们没有扑上来。我还活着，或者说，变成了另一种东西。"},
                {6, "够路上用就行。拿太多，只会让后来的人死得更快。"},
                {7, "血月从窗缝里渗进来。今晚不出去——出去了也看不见路。"},
                {8, "引擎声音更稳了。今晚可以骑远一点。"},
                {9, "货架空一点，总比我被他们拖去筛查要好。"},
                {10, "桥通了一条路。他们不知道是谁做的，这样最好。"},
                {11, "灯扫过来之前，尸群得先动。"},
                {12, "超市后门的铁丝被人重新别过。有人在跟着我的路线。"},
                {13, "墙上多了一道箭头。不是我画的。是那个往大门走的人。"},
                {14, "趁他们还缩在营地里，把这里最后一扇窗也关好。"},
                {15, "那个人擦肩而过。我没有看清他的脸，也不能停。"},
            };

            if (fragments.TryGetValue(day, out var fragment))
                return fragment;
            return "祁眠在夜色中骑行，摩托引擎的震动是她唯一能确认自己还活着的东西。";
        }

        // ============ 本地兜底：结局叙事 ============

        /// <summary>
        /// 生成结局叙事文本。
        /// 兜底：返回 Controller 预置文本。
        /// SDK 可用时：调用 PlayKit 生成个性化结局。
        /// </summary>
        public static string FallbackEndingNarrative(GameState state)
        {
            // 委托给 NightController 的预设结局文本
            return "";
        }

        // ============ SDK 接入预留接口（不要猜测真实 API 签名） ============

        // 等 PlayKit SDK 拉包后，按以下步骤接入：
        // 1. 打开 Unity Package Manager → PlayKit AI SDK
        // 2. 打开 SDK 附带的 Samples（如有）或文档
        // 3. 阅读 SDK 的 C# API 文档
        // 4. 找到文本生成相关的方法签名
        // 5. 在此处添加真实的 SDK 调用
        //
        // 预期的接入模式（仅示意，非真实 API）：
        //   var client = PlayKit.Client.Create("your-api-key");
        //   var response = await client.GenerateTextAsync(prompt);
        //   return response.Text;
        //
        // 注意：不要将 Developer Token 写入代码文件。

        /// <summary>
        /// 标记 SDK 已可用，启用真实调用路径。
        /// 等 Unity 编译通过且 SDK 加载成功后调用。
        /// </summary>
        public static void MarkSdkAvailable()
        {
            IsSdkAvailable = true;
        }
    }
}
