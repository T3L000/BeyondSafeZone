using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondSafeZone.World
{
    [Serializable]
    public class ExplorationSiteDefinition
    {
        public string LocationId { get; }
        public string DisplayName { get; }
        public string DangerLabel { get; }
        public string ResourceHint { get; }

        public ExplorationSiteDefinition(string locationId, string displayName, string dangerLabel, string resourceHint)
        {
            LocationId = locationId;
            DisplayName = displayName;
            DangerLabel = dangerLabel;
            ResourceHint = resourceHint;
        }
    }

    [Serializable]
    public class SearchPointDefinition
    {
        public string LocationId { get; }
        public string RoomId { get; }
        public string DisplayName { get; }
        public string Tactic { get; }
        public bool LureBeforeSearch { get; }

        public SearchPointDefinition(string locationId, string roomId, string displayName, string tactic, bool lureBeforeSearch)
        {
            LocationId = locationId;
            RoomId = roomId;
            DisplayName = displayName;
            Tactic = tactic;
            LureBeforeSearch = lureBeforeSearch;
        }
    }

    public static class ExplorationSiteCatalog
    {
        private static readonly ExplorationSiteDefinition[] CoreSites =
        {
            new("clinic", "社区诊所", "中", "药品 / 异常痕迹"),
            new("supermarket", "小区超市", "高", "食物 / 水 / 建材"),
            new("bike_shop", "修理铺+车库", "中", "零件 / 轮胎 / 旧车")
        };

        private static readonly SearchPointDefinition[] SearchPoints =
        {
            new("clinic", "waiting", "候诊室", "careful", false),
            new("clinic", "exam_a", "诊室A", "careful", true),
            new("clinic", "pharmacy", "药房", "careful", true),
            new("supermarket", "checkout", "收银区", "careful", true),
            new("supermarket", "food_aisle", "食品区", "careful", true),
            new("supermarket", "storage", "仓储区", "careful", true),
            new("bike_shop", "storefront", "店面", "careful", false),
            new("bike_shop", "workshop", "工作间", "careful", true),
            new("bike_shop", "garage", "车库", "careful", false)
        };

        public static IReadOnlyList<ExplorationSiteDefinition> GetCoreSites() => CoreSites;

        public static IReadOnlyList<SearchPointDefinition> GetRoomsForLocation(string locationId) =>
            SearchPoints.Where(point => point.LocationId == locationId).ToArray();

        public static ExplorationSiteDefinition GetSite(string locationId)
        {
            foreach (var site in CoreSites)
            {
                if (site.LocationId == locationId)
                    return site;
            }

            return null;
        }
    }
}