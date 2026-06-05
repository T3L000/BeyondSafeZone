using System.Collections.Generic;

namespace BeyondSafeZone.Data
{
    /// <summary>设施数据 —— 对应 Godot facilities.gd</summary>
    public static class FacilityData
    {
        public static Dictionary<string, Model.FacilityState> Defaults() => new()
        {
            ["bed"] = new() { Name = "床铺", Role = "recover", Level = 1, Built = false, UsedToday = false },
            ["workbench"] = new() { Name = "工作台", Role = "craft_repair", Level = 1, Built = false, UsedToday = false },
            ["stove"] = new() { Name = "火炉", Role = "warmth", Level = 1, Built = false, UsedToday = false },
            ["barricade"] = new() { Name = "墙体/窗户", Role = "blood_moon_defense", Level = 1, Built = true, UsedToday = false },
            ["radio"] = new() { Name = "收音机", Role = "broadcast_clues", Level = 1, Built = true, UsedToday = false },
            ["storage"] = new() { Name = "储物区", Role = "preserve_carry", Level = 1, Built = true, UsedToday = false }
        };
    }
}
