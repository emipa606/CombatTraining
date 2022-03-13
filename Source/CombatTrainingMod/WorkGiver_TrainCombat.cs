using System.Collections.Generic;
using HugsLib.Utils;
using RimWorld;
using Verse;
using Verse.AI;

namespace KriilMod_CD;

public class WorkGiver_TrainCombat : WorkGiver_Scanner
{
    public bool IsDummyUsable(Thing dummy)
    {
        if (IsDummyBreaking(dummy))
        {
            return false;
        }

        if (dummy.Destroyed)
        {
            return false;
        }

        return true;
    }

    public bool IsDummyBreaking(Thing dummy)
    {
        return dummy.HitPoints <= dummy.MaxHitPoints * 0.5;
    }


    public bool IsValidJobTarget(Thing dummy, Pawn pawn)
    {
        if (pawn == null || dummy == null)
        {
            return false;
        }

        var primary = pawn.equipment.Primary;
        if (!IsDummyUsable(dummy))
        {
            return false;
        }

        if (dummy.HasDesignation(CombatTrainingDefOf.TrainCombatDesignation))
        {
            return true;
        }

        if (dummy.HasDesignation(CombatTrainingDefOf.TrainCombatDesignationMeleeOnly))
        {
            if (primary == null)
            {
                return true;
            }

            return primary.def.IsMeleeWeapon;
        }

        if (!dummy.HasDesignation(CombatTrainingDefOf.TrainCombatDesignationRangedOnly))
        {
            return false;
        }

        if (primary == null)
        {
            return false;
        }

        return primary.def.IsRangedWeapon;
    }

    public bool IsValidDesignation(DesignationDef dummyDef)
    {
        if (dummyDef == CombatTrainingDefOf.TrainCombatDesignationMeleeOnly)
        {
            return true;
        }

        if (dummyDef == CombatTrainingDefOf.TrainCombatDesignationRangedOnly)
        {
            return true;
        }

        if (dummyDef == CombatTrainingDefOf.TrainCombatDesignation)
        {
            return true;
        }

        return false;
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        if (CompatibilityUtility.IsGuest(pawn))
        {
            return true;
        }

        if (forced)
        {
            return pawn.WorkTagIsDisabled(WorkTags.Violent);
        }

        return CombatTrainingTracker.ShouldSkipCombatTraining(pawn);
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (pawn.WorkTagIsDisabled(WorkTags.Violent))
        {
            JobFailReason.Is(null, "IsIncapableOfViolence".Translate());
            return false;
        }

        if (t.IsForbidden(pawn))
        {
            return false;
        }

        LocalTargetInfo target = t;
        if (pawn.CanReserve(target, 1, -1, null, forced))
        {
            return IsValidJobTarget(t, pawn);
        }

        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var verb = pawn.TryGetAttackVerb(t);
        if (verb != null)
        {
            return new Job(CombatTrainingDefOf.TrainOnCombatDummy, t)
            {
                verbToUse = verb
            };
        }

        return null;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        var desList = pawn.Map.designationManager.allDesignations;
        foreach (var des in desList)
        {
            if (IsValidDesignation(des.def))
            {
                yield return des.target.Thing;
            }
        }
    }
}