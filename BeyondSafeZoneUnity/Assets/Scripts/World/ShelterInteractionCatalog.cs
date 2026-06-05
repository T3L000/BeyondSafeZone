using System;
using System.Collections.Generic;
using BeyondSafeZone.Model;

namespace BeyondSafeZone.World
{
    [Serializable]
    public class ShelterInteractionDefinition
    {
        public string FacilityId { get; }
        public string DisplayName { get; }
        public string BuiltActionId { get; }
        public string BuildActionId { get; }
        public string Prompt { get; }

        public ShelterInteractionDefinition(string facilityId, string displayName, string builtActionId, string buildActionId, string prompt)
        {
            FacilityId = facilityId;
            DisplayName = displayName;
            BuiltActionId = builtActionId;
            BuildActionId = buildActionId;
            Prompt = prompt;
        }
    }

    public static class ShelterInteractionCatalog
    {
        private static readonly ShelterInteractionDefinition[] Interactions =
        {
            new("bed", "床", "rest_bed", "build_bed", "休息 / 建造床"),
            new("workbench", "工作台", "workbench_repair", "build_workbench", "修理 / 建造工作台"),
            new("stove", "火炉", "quiet", "build_stove", "取暖 / 建造火炉"),
            new("barricade", "墙体/窗户", "barricade_windows", "barricade_windows", "修补墙体和窗户"),
            new("radio", "收音机", "radio_broadcast", "radio_broadcast", "收听广播"),
            new("storage", "储物区", "organize_storage", "organize_storage", "整理物资")
        };

        public static IReadOnlyList<ShelterInteractionDefinition> GetAll() => Interactions;

        public static ShelterInteractionDefinition Get(string facilityId)
        {
            foreach (var interaction in Interactions)
            {
                if (interaction.FacilityId == facilityId)
                    return interaction;
            }

            return null;
        }

        public static string GetActionForFacility(GameState state, string facilityId)
        {
            var interaction = Get(facilityId);
            if (interaction == null)
                return string.Empty;

            if (!state.Shelter.Facilities.TryGetValue(facilityId, out var facility))
                return interaction.BuiltActionId;

            return facility.Built ? interaction.BuiltActionId : interaction.BuildActionId;
        }

        public static string GetPrompt(GameState state, string facilityId)
        {
            var interaction = Get(facilityId);
            if (interaction == null)
                return string.Empty;

            string actionId = GetActionForFacility(state, facilityId);
            return $"{interaction.DisplayName}：{interaction.Prompt} [{actionId}]";
        }
    }
}