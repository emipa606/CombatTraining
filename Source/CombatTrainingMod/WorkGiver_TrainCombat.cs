using System.Collections.Generic;
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

        return !dummy.Destroyed;
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

        if (CombatTrainingController.HasDesignation(dummy, CombatTrainingDefOf.TrainCombatDesignation))
        {
            return true;
        }

        if (CombatTrainingController.HasDesignation(dummy, CombatTrainingDefOf.TrainCombatDesignationMeleeOnly))
        {
            return primary == null || primary.def.IsMeleeWeapon;
        }

        if (!CombatTrainingController.HasDesignation(dummy, CombatTrainingDefOf.TrainCombatDesignationRangedOnly))
        {
            return false;
        }

        if (primary == null)
        {
            return false;
        }

        return pawn.TryGetAttackVerb(dummy)?.ApparelPreventsShooting() != true && primary.def.IsRangedWeapon;
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

        return dummyDef == CombatTrainingDefOf.TrainCombatDesignation;
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        if (CompatibilityUtility.IsGuest(pawn))
        {
            return true;
        }

        return forced ? pawn.WorkTagIsDisabled(WorkTags.Violent) : CombatTrainingTracker.ShouldSkipCombatTraining(pawn);
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
        return pawn.CanReserve(target, 1, -1, null, forced) && IsValidJobTarget(t, pawn);
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
        var desList = pawn.Map.designationManager.AllDesignations;
        foreach (var des in desList)
        {
            if (IsValidDesignation(des.def))
            {
                yield return des.target.Thing;
            }
        }
    }
}