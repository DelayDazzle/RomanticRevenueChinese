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
            // ===== 分类标题 =====
            ["Downtown_Zuzu_Options"] = "祖祖市中心选项",
            ["East_Scarp_Options"] = "东断崖选项",
            ["Fellow_Clown_Options"] = "小丑同盟选项",
            ["Other_NPCs_Options"] = "其他NPC选项",
            ["Ridgeside_Village_Options"] = "Ridgeside Village选项",
            ["Stardew_Valley_Expanded_Options"] = "Stardew Valley Expanded选项",
            ["Vanilla_Options"] = "原版选项",

            // ===== Downtown Zuzu 祖祖市中心 =====
            ["Cal_Min"] = "卡尔 最小",
            ["Cal_Max"] = "卡尔 最大",
            ["David_Min"] = "大卫 最小",
            ["David_Max"] = "大卫 最大",
            ["Gwen_Min"] = "格温 最小",
            ["Gwen_Max"] = "格温 最大",
            ["Hazel_Min"] = "黑兹尔 最小",
            ["Hazel_Max"] = "黑兹尔 最大",
            ["Kristoff_Min"] = "克里斯托夫 最小",
            ["Kristoff_Max"] = "克里斯托夫 最大",
            ["Max_Min"] = "马克斯 最小",
            ["Max_Max"] = "马克斯 最大",
            ["Sadie_Min"] = "萨迪 最小",
            ["Sadie_Max"] = "萨迪 最大",
            ["Selena_Min"] = "塞莱娜 最小",
            ["Selena_Max"] = "塞莱娜 最大",

            // ===== East Scarp - 东断崖 =====
            ["Aideen_Flowers_Min"] = "艾迪 花店 最小",
            ["Aideen_Flowers_Max"] = "艾迪 花店 最大",
            ["Aideen_Rent_Min"] = "艾迪 租金 最小",
            ["Aideen_Rent_Max"] = "艾迪 租金 最大",
            ["Jasper_Min"] = "贾斯珀 最小",
            ["Jasper_Max"] = "贾斯珀 最大",
            ["Juliet_Min"] = "朱丽叶 最小",
            ["Juliet_Max"] = "朱丽叶 最大",
            ["Kataryna_Min"] = "卡塔琳娜 最小",
            ["Kataryna_Max"] = "卡塔琳娜 最大",
            ["Mateo_Min"] = "马特奥 最小",
            ["Mateo_Max"] = "马特奥 最大",
            ["Sterling_Min"] = "斯特林 最小",
            ["Sterling_Max"] = "斯特林 最大",
            ["Tristan_Min"] = "特里斯坦 最小",
            ["Tristan_Max"] = "特里斯坦 最大",

            // ===== Fellow Clown 小丑同盟 =====
            ["Barron_Min"] = "巴伦 最小",
            ["Barron_Max"] = "巴伦 最大",
            ["Lyell_Farming_Min"] = "莱尔 农业 最小",
            ["Lyell_Farming_Max"] = "莱尔 农业 最大",
            ["Lyell_Festival_Min"] = "莱尔 节日 最小",
            ["Lyell_Festival_Max"] = "莱尔 节日 最大",
            ["Nikolai_Min"] = "尼古拉 最小",
            ["Nikolai_Max"] = "尼古拉 最大",
            ["Nikolai_Festival_Min"] = "尼古拉 节日 最小",
            ["Nikolai_Festival_Max"] = "尼古拉 节日 最大",
            ["Valerie_Min"] = "瓦莱丽 最小",
            ["Valerie_Max"] = "瓦莱丽 最大",
            ["Valerie_Festival_Min"] = "瓦莱丽 节日 最小",
            ["Valerie_Festival_Max"] = "瓦莱丽 节日 最大",

            // ===== Other NPCs 其他NPC =====
            ["Alec_Min"] = "亚历克斯 最小",
            ["Alec_Max"] = "亚历克斯 最大",
            ["Aspen_Min"] = "阿斯彭 最小",
            ["Aspen_Max"] = "阿斯彭 最大",
            ["Aster_Reserve_Min"] = "阿斯特 自然保护区 最小",
            ["Aster_Reserve_Max"] = "阿斯特 自然保护区 最大",
            ["Aster_Seeds_Min"] = "阿斯特 种子 最小",
            ["Aster_Seeds_Max"] = "阿斯特 种子 最大",
            ["Ayeisha_Min"] = "艾伊莎 最小",
            ["Ayeisha_Max"] = "艾伊莎 最大",
            ["Gabriel_Min"] = "加布里埃尔 最小",
            ["Gabriel_Max"] = "加布里埃尔 最大",
            ["Lucikiel_Min"] = "卢西克尔 最小",
            ["Lucikiel_Max"] = "卢西克尔 最大",
            ["Satoru_Min"] = "佐藤 最小",
            ["Satoru_Max"] = "佐藤 最大",
            ["Zinnia_Min"] = "紫菀 最小",
            ["Zinnia_Max"] = "紫菀 最大",

            // ===== Ridgeside Village 里奇赛得村 =====
            ["Alissa_Min"] = "艾莉莎 最小",
            ["Alissa_Max"] = "艾莉莎 最大",
            ["Blair_Min"] = "布莱尔 最小",
            ["Blair_Max"] = "布莱尔 最大",
            ["Corine_Min"] = "科琳 最小",
            ["Corine_Max"] = "科琳 最大",
            ["Flor_Teaching_Min"] = "弗洛尔 最大",
            ["Flor_Teaching_Max"] = "弗洛尔 最大",
            ["Flor_School_Min"] = "弗洛尔 学校 最小",
            ["Flor_School_Max"] = "弗洛尔 学校 最大",
            ["Ian_Handyman_Min"] = "伊恩 杂工 最小",
            ["Ian_Handyman_Max"] = "伊恩 杂工 最大",
            ["Jeric_SeedStore_Min"] = "杰里克 种子店 最小",
            ["Jeric_SeedStore_Max"] = "杰里克 种子店 最大",
            ["June_Pianist_Min"] = "琼 钢琴师 最小",
            ["June_Pianist_Max"] = "琼 钢琴师 最大",
            ["Kenneth_Electrician_Min"] = "肯尼思 电工 最小",
            ["Kenneth_Electrician_Max"] = "肯尼思 电工 最大",
            ["Kiarra_Min"] = "卡亚 拉 设计 最小",
            ["Kiarra_Max"] = "卡亚 拉 设计 最大",
            ["Maddie_Min"] = "玛蒂 实验室工作 最小",
            ["Maddie_Max"] = "玛蒂 实验室工作 最大",
            ["Philip_Therapist_Min"] = "菲利普 治疗师 最小",
            ["Philip_Therapist_Max"] = "菲利普 治疗师 最大",
            ["Sean_Handyman_Min"] = "肖恩 杂工 最小",
            ["Sean_Handyman_Max"] = "肖恩 杂工 最大",
            ["Ysabelle_Min"] = "伊莎贝尔 前台 最小",
            ["Ysabelle_Max"] = "伊莎贝尔 前台 最大",

            // ===== Stardew Valley Expanded =====
            ["Claire_Joja_Min"] = "克莱尔 Joja 最小",
            ["Claire_Joja_Max"] = "克莱尔 Joja 最大",
            ["Claire_Movie_Min"] = "克莱尔 影院 最小",
            ["Claire_Movie_Max"] = "克莱尔 影院 最大",
            ["Lance_Min"] = "兰斯 最小",
            ["Lance_Max"] = "兰斯 最大",
            ["Olivia_Min"] = "奥利维亚 最小",
            ["Olivia_Max"] = "奥利维亚 最大",
            ["Sophia_Min"] = "索菲亚 最小",
            ["Sophia_Max"] = "索菲亚 最大",
            ["Victor_Min"] = "维克托 最小",
            ["Victor_Max"] = "维克托 最大",

            // ===== Vanilla =====
            ["Abigail_Min"] = "阿比盖尔 最小",
            ["Abigail_Max"] = "阿比盖尔 最大",
            ["Elliott_Min"] = "艾略特 最小",
            ["Elliott_Max"] = "艾略特 最大",
            ["Emily_Min"] = "艾米丽 最小",
            ["Emily_Max"] = "艾米丽 最大",
            ["Haley_Min"] = "海莉 最小",
            ["Haley_Max"] = "海莉 最大",
            ["Harvey_Min"] = "哈维 最小",
            ["Harvey_Max"] = "哈维 最大",
            ["Leah_Min"] = "利亚 最小",
            ["Leah_Max"] = "利亚 最大",
            ["Maru_Min"] = "玛鲁 最小",
            ["Maru_Max"] = "玛鲁 最大",
            ["Penny_Teaching_Min"] = "佩妮 最小",
            ["Penny_Teaching_Max"] = "佩妮 最大",
            ["Penny_School_Min"] = "佩妮 学校 最小",
            ["Penny_School_Max"] = "佩妮 学校 最大",
            ["Ram_Min"] = "拉姆 最小",
            ["Ram_Max"] = "拉姆 最大",
            ["Sam_Min"] = "萨姆 最小",
            ["Sam_Max"] = "萨姆 最大",
            ["Sebastian_Min"] = "塞巴斯蒂安 最小",
            ["Sebastian_Max"] = "塞巴斯蒂安 最大",
            ["Shane_Min"] = "谢恩 最小",
            ["Shane_Max"] = "谢恩 最大",
        };

        public override void Entry(IModHelper helper)
        {
            _harmony = new Harmony(ModManifest.UniqueID);

            // 注意 DLL 名称是 "Romantic Revenue"（含空格）
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

            Monitor.Log($"Romantic Revenue 汉化补丁已加载，共修补 {patchedCount} 个方法。", LogLevel.Info);
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
