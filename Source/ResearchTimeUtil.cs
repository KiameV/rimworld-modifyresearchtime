﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ChangeResearchSpeed
{
    internal static class ResearchTimeUtil
    {
        private static Dictionary<string, float> baseResearchDefs = null;

        public static void CreateBaseResearchDefs()
        {
            if (baseResearchDefs == null)
            {
#if DEBUG
                Log.Warning("Create Base Research Lookup");
#endif
                baseResearchDefs = new Dictionary<string, float>();
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
                {
                    baseResearchDefs.Add(def.defName, def.baseCost);
                }
            }
        }

        public static void ApplyFactor(float factor, bool applyToProgress, float oldFactor = 1)
        {
#if DEBUG
            Log.Warning("ApplyFactor");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
#endif
            if (factor < 0.01)
            {
                Log.Warning("Limiting research factor to 0.01");
                factor = 0.01f;
            }

            Dictionary<ResearchProjectDef, float> progress =
                (Dictionary<ResearchProjectDef, float>)Find.ResearchManager.GetType().GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Find.ResearchManager);

            CreateBaseResearchDefs();
            ResetResearchFactor();

            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
#if DEBUG
                float orig = def.baseCost;
                bool finsihed = def.IsFinished;
#endif
                if (applyToProgress)
                {
                    float p;
                    if (progress.TryGetValue(def, out p))
                    {
                        p /= oldFactor;
                        progress[def] = p * factor;
                    }
                }
                def.baseCost *= factor;
#if DEBUG
                //sb.Append(def.defName + " Finished Orig: " + finsihed + " New: " + def.IsFinished + " Base Cost Orig: " + (int)orig + " New: " + (int)def.baseCost);
#endif
            }
#if DEBUG
            Log.Warning(sb.ToString());
#endif
        }

        public static void ResetResearchFactor()
        {
#if DEBUG
            Log.Warning("ResetResearchFactor");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
#endif
            if (baseResearchDefs != null)
            {
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
                {
#if DEBUG
                    float orig = def.baseCost;
                    bool finsihed = def.IsFinished;
#endif
                    float value;
                    if (baseResearchDefs.TryGetValue(def.defName, out value))
                    {
                        def.baseCost = value;
                    }
                    else
                    {
                        baseResearchDefs[def.defName] = value;
                    }
#if DEBUG
                    //sb.Append(def.defName + " Finished Orig: " + finsihed + " New: " + def.IsFinished + " Base Cost Orig: " + (int)orig + " New: " + (int)def.baseCost);
#endif
                }
            }
#if DEBUG
            Log.Warning(sb.ToString());
#endif
        }
    }
}
