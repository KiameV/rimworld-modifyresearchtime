using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ModifyResearchTime
{
    class Settings : Mod
    {
        public const string FLOAT_FORMAT = "####.####";
        private static string settingsFactor = "1.00";

        public static float SettingsFactor
        {
            get
            {
                if (ValidateInput())
                {
                    return float.Parse(settingsFactor);
                }
                return 1f;
            }
        }

        public Settings(ModContentPack content) : base(content)
        {
            
        }

        public override string SettingsCategory()
        {
            return "ModifyResearchTime".Translate();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            Scribe_Values.Look<string>(ref settingsFactor, "ModifyResearchTime.Factor", "1.00", false);
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            Rect r = new Rect(0, 60, 600, 85);
            GUI.BeginGroup(r);
            r = new Rect(0, 0, 300, 20);
            GUI.Label(r, "ModifyResearchTime.Factor".Translate() + ":");
            r = new Rect(320, 0, 280, 20);
            settingsFactor = GUI.TextField(r, settingsFactor);
            if (Current.Game != null)
            {
                r = new Rect(320, 25, 200, 25);
                if (GUI.Button(r, new GUIContent("ModifyResearchTime.Apply".Translate())))
                {
                    if (ValidateInput())
                    {
                        WorldComp.UpdateFactor(SettingsFactor);
                        Messages.Message("ModifyResearchTime.ResearchTimesUpdated".Translate(), MessageSound.Benefit);
                    }
                }

            }
            GUI.EndGroup();
        }

        private static bool ValidateInput()
        {
            float f;
            if (float.TryParse(settingsFactor, out f))
            {
                if (f <= 0)
                {
                    Messages.Message("Research Time Factor cannot be less than or equal to 0.", MessageSound.RejectInput);
                    return false;
                }
            }
            else
            {
                Messages.Message("Unable to parse Research Time Factor to a number.", MessageSound.RejectInput);
                return false;
            }
            return true;
        }
    }
}