using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;

namespace RomanticRevenueChinese
{
    public class ModEntry : Mod
    {
        private Harmony? _harmony;

        private static readonly Dictionary<string, string> Translations = new()
        {
            // ===== 下划线形式（DLL 里直接可见） =====
            ["Downtown_Zuzu_Options"] = "祖祖市中心选项",
            ["East_Scarp_Options"] = "东斯卡普选项",
            ["Fellow_Clown_Options"] = "小丑同盟选项",
            ["Other_NPCs_Options"] = "其他NPC选项",
            ["Ridgeside_Village_Options"] = "里奇赛得村选项",
            ["Stardew_Valley_Expanded_Options"] = "星露谷扩展选项",
            ["Vanilla_Options"] = "原版选项",

            // ===== 带空格英文形式（万一 GMCM 拿到的是这种） =====
            ["Downtown Zuzu Options"] = "祖祖市中心选项",
            ["East Scarp Options"] = "东斯卡普选项",
            ["Fellow Clown Options"] = "小丑同盟选项",
            ["Other NPCs Options"] = "其他NPC选项",
            ["Ridgeside Village Options"] = "里奇赛得村选项",
            ["Stardew Valley Expanded Options"] = "星露谷扩展选项",
            ["Vanilla Options"] = "原版选项",
        };

        public override void Entry(IModHelper helper)
        {
            _harmony = new Harmony(ModManifest.UniqueID);

            Assembly? targetMod = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Romantic Revenue");

            if (targetMod == null)
            {
                Monitor.Log("未找到 Romantic Revenue 程序集，跳过汉化。", LogLevel.Warn);
                return;
            }

            MethodInfo transpiler = typeof(ModEntry).GetMethod(
                nameof(Transpiler),
                BindingFlags.Static | BindingFlags.NonPublic
            )!;

            var harmonyTranspiler = new HarmonyMethod(transpiler);
            int patchedCount = 0;

            // 遍历所有类型（含编译器生成的嵌套类），修补所有方法体里的 Ldstr
            foreach (Type type in targetMod.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly))
                {
                    try
                    {
                        if (method.GetMethodBody() == null) continue;
                        _harmony.Patch(method, transpiler: harmonyTranspiler);
                        patchedCount++;
                    }
                    catch { }
                }
            }

            Monitor.Log($"Romantic Revenue 分类标题汉化已加载，共修补 {patchedCount} 个方法。", LogLevel.Info);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr &&
                    instruction.operand is string original &&
                    Translations.TryGetValue(original, out string? translated))
                {
                    instruction.operand = translated;
                }
                yield return instruction;
            }
        }
    }
}
