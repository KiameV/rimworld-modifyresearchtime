using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ModifyResearchTime
{
    internal static class ResearchTimeUtil
    {
        public static Dictionary<string, float> BaseResearchDefs { get; set; }

        static ResearchTimeUtil()
        {
            BaseResearchDefs = null;
        }

        private static void CreateBaseResearchDefs()
        {
            if (BaseResearchDefs == null)
            {
                BaseResearchDefs = new Dictionary<string, float>();
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
                {
                    BaseResearchDefs.Add(def.defName, def.baseCost);
                }
            }
        }

        public static void ApplyFactor(float oldFactor, float newFactor)
        {
            CreateBaseResearchDefs();
            Dictionary<string, ResearchProjectDef> defsToModify = new Dictionary<string, ResearchProjectDef>();
            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
                defsToModify.Add(def.defName, def);
            }
            
            ResearchManager rm = Find.ResearchManager;
            Dictionary<string, ResearchProjectDef> researchCompleted = new Dictionary<string, ResearchProjectDef>();
            Dictionary<ResearchProjectDef, float> progress = 
                (Dictionary<ResearchProjectDef, float>)rm.GetType().GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(rm);
            foreach (ResearchProjectDef defToModify in defsToModify.Values)
            {
                float baseCost = BaseResearchDefs[defToModify.defName];
                if (Math.Abs(rm.GetProgress(defToModify) - (baseCost * oldFactor)) < 0.01f)
                {
                    researchCompleted.Add(defToModify.defName, null);
                }
                else
                {
                    float p;
                    if (progress.TryGetValue(defToModify, out p))
                    {
                        if (p > 0)
                        {
                            p = (p / oldFactor) * newFactor;
                            progress[defToModify] = p;
                        }
                    }
                }
            }

            foreach (ResearchProjectDef defToModify in defsToModify.Values)
            {
                float baseCost = BaseResearchDefs[defToModify.defName];
                defToModify.baseCost = baseCost * newFactor;
                if (defToModify.baseCost < 1)
                    defToModify.baseCost = 1;

                if (researchCompleted.ContainsKey(defToModify.defName))
                {
                    rm.InstantFinish(defToModify, false);
                }
            }

            defsToModify.Clear();
            researchCompleted.Clear();
        }
    }
}
