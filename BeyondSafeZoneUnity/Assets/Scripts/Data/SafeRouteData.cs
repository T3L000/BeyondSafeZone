using System.Collections.Generic;
using System.Linq;

namespace BeyondSafeZone.Data
{
    /// <summary>安全演示路线 —— 对应 Godot safe_route.gd</summary>
    public static class SafeRouteData
    {
        public static readonly Dictionary<int, string> DAY_LOCATION = new()
        {
            {1, "home"}, {2, "convenience"}, {3, "clinic"}, {4, "bike_shop"}, {5, "bike_shop"},
            {6, "police"}, {7, "school"}, {8, "supermarket"}, {9, "bridge_camp"}, {10, "gas_station"},
            {11, "hardware_store"}, {12, "subway"}, {13, "apartment"}, {14, "quarantine"}, {15, "safezone_edge"}
        };

        public static readonly Dictionary<int, string> DAY_SHELTER_ACTION = new()
        {
            {10, "workbench_car"}, {12, "workbench_car"}, {13, "workbench_car"}, {14, "workbench_car"}
        };

        public const int MAX_ROOMS_PER_LOCATION = 3;
        public const string SEARCH_TACTIC = "careful";

        public static string GetLocationForDay(int day) =>
            DAY_LOCATION.GetValueOrDefault(day, "home");

        public static string GetActionForDay(int day)
        {
            if (DAY_SHELTER_ACTION.TryGetValue(day, out var action))
                return action;
            if (IsBloodMoon(day)) return "fortify";
            if (day % 3 == 0) return "radio";
            if (day % 2 == 0) return "workbench_repair";
            return "quiet";
        }

        private static bool IsBloodMoon(int day) => new[] { 7, 15 }.Contains(day);
    }
}
