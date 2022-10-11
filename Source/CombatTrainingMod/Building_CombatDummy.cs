using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KriilMod_CD;

public class Building_CombatDummy : Building
{
    [Flags]
    public enum TrainingTypes
    {
        None = 0,
        Melee = 1,
        Ranged = 2,
        Any = 3
    }

    public TrainingTypes trainingType = TrainingTypes.None;

    public override void PostMapInit()
    {
        if (CombatTrainingController.HasDesignation(this, CombatTrainingDefOf.TrainCombatDesignation))
        {
            trainingType = TrainingTypes.Any;
        }
        else if (CombatTrainingController.HasDesignation(this, CombatTrainingDefOf.TrainCombatDesignationMeleeOnly))
        {
            trainingType = TrainingTypes.Melee;
        }
        else if (CombatTrainingController.HasDesignation(this, CombatTrainingDefOf.TrainCombatDesignationRangedOnly))
        {
            trainingType = TrainingTypes.Ranged;
        }
    }

    protected void determineDesignation()
    {
        CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignation, false);
        CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignationMeleeOnly, false);
        CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignationRangedOnly, false);
        switch (trainingType)
        {
            case TrainingTypes.None:
                break;
            case TrainingTypes.Melee:
                CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignationMeleeOnly,
                    true);
                break;
            case TrainingTypes.Ranged:
                CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignationRangedOnly,
                    true);
                break;
            case TrainingTypes.Any:
                CombatTrainingController.ToggleDesignation(this, CombatTrainingDefOf.TrainCombatDesignation, true);
                break;
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var g in base.GetGizmos())
        {
            yield return g;
        }

        // Melee toggle
        yield return new Command_Toggle
        {
            isActive = () => ((int)trainingType & (int)TrainingTypes.Melee) != 0,
            toggleAction = delegate
            {
                trainingType ^= TrainingTypes.Melee;
                determineDesignation();
            },
            defaultDesc = "CommandTrainCombatMeleeOnlyDesc".Translate(),
            icon = TexCommand.AttackMelee,
            defaultLabel = "CommandTrainCombatMeleeOnlyLabel".Translate()
        };
        // Ranged toggle
        yield return new Command_Toggle
        {
            isActive = () => ((int)trainingType & (int)TrainingTypes.Ranged) != 0,
            toggleAction = delegate
            {
                trainingType ^= TrainingTypes.Ranged;
                determineDesignation();
            },
            defaultDesc = "CommandTrainCombatRangedOnlyDesc".Translate(),
            icon = TexCommand.Attack,
            defaultLabel = "CommandTrainCombatRangedOnlyLabel".Translate()
        };
    }
}