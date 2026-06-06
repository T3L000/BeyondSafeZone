using System;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace BeyondSafeZone.Tests
{
    internal static class OneRunTestHelpers
    {
        public static readonly string[] KnownSceneObjects =
        {
            "OneRunHUD", "WalkableShelterGreybox", "EventSystem", "Main Camera",
            "OneRunController_U1", "OneRunController_U2", "OneRunController_U3", "OneRunController_U4",
            "OneRunController_W1", "OneRunController_W2", "OneRunController_W3", "OneRunController_W4", "OneRunController_W5", "OneRunController_W6", "OneRunController_W7",
            "OneRunController_B1", "OneRunController_B2", "OneRunController_B3"
        };

        public static Component GetTextComponent(Transform transform)
        {
            NUnit.Framework.Assert.IsNotNull(transform, "text transform exists");
            return transform
                .GetComponents<Component>()
                .First(component => component.GetType().FullName == "TMPro.TextMeshProUGUI" ||
                                    component.GetType().FullName == "TMPro.TextMeshPro");
        }

        public static float GetFontSize(Component textComponent)
        {
            object value = textComponent.GetType().GetProperty("fontSize").GetValue(textComponent);
            return Convert.ToSingle(value);
        }

        public static string GetTextValue(Component textComponent)
        {
            return textComponent.GetType().GetProperty("text").GetValue(textComponent) as string;
        }

        public static void DestroyKnownSceneObjects()
        {
            foreach (string name in KnownSceneObjects)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null) UnityObject.DestroyImmediate(obj);
            }
        }
    }
}
