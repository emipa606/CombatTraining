using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace KriilMod_CD;

public class JobDriver_TrainCombat : JobDriver
{
    private static readonly float trainCombatLearningFactor = .15f;
    private int jobStartTick = -1;
    public ThingWithComps startingEquippedWeapon;
    public ThingWithComps trainingWeapon;

    public Thing Dummy => job.GetTarget(TargetIndex.A).Thing;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref jobStartTick, "jobStartTick");
        Scribe_Deep.Look(ref startingEquippedWeapon, "startingEquippedWeapon");
        Scribe_Deep.Look(ref trainingWeapon, "trainingWeapon");
    }

    public override string GetReport()
    {
        return Dummy != null ? job.def.reportString.Replace("TargetA", Dummy.LabelShort) : base.GetReport();
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
    }

    public bool IsDummyBreaking()
    {
        return Dummy.HitPoints <= Dummy.MaxHitPoints * 0.5;
    }

    public bool IsDummyUsable()
    {
        if (IsDummyBreaking())
        {
            return false;
        }

        return !Dummy.Destroyed;
    }

    public bool IsJobPossible()
    {
        if (!IsDummyUsable())
        {
            return false;
        }

        if (LearningSaturated())
        {
            return false;
        }

        if (CombatTrainingController.HasDesignation(TargetThingA, CombatTrainingDefOf.TrainCombatDesignation))
        {
            return true;
        }

        if (pawn.equipment.Primary == null)
        {
            return CombatTrainingController.HasDesignation(TargetThingA,
                CombatTrainingDefOf.TrainCombatDesignationMeleeOnly);
        }

        if (CombatTrainingController.HasDesignation(TargetThingA, CombatTrainingDefOf.TrainCombatDesignationMeleeOnly))
        {
            return pawn.equipment.Primary.def.IsMeleeWeapon;
        }

        return CombatTrainingController.HasDesignation(TargetThingA,
            CombatTrainingDefOf.TrainCombatDesignationRangedOnly) && pawn.equipment.Primary.def.IsRangedWeapon;
    }

    public bool IsTimeLimitReached()
    {
        return Find.TickManager.TicksGame > jobStartTick + 5000;
    }

    public bool HasTrainingEnded()
    {
        if (IsTimeLimitReached())
        {
            return true;
        }

        return !IsJobPossible();
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        //fail if can't do violence
        AddFailCondition(() => pawn.WorkTagIsDisabled(WorkTags.Violent));

        jobStartTick = Find.TickManager.TicksGame;

        // Make sure our dummy isn't already in use
        this.FailOnSomeonePhysicallyInteracting(TargetIndex.A);

        //fail if dummy is despawned null or forbidden
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

        /**** START SWITCH TO TRAINING WEAPON ****/
        //Pick up a training weapon if one is nearby. Remember previous weapon     
        startingEquippedWeapon = pawn.equipment.Primary;
        trainingWeapon = null;

        if (startingEquippedWeapon == null ||
            !startingEquippedWeapon.def.IsWithinCategory(CombatTrainingDefOf.TrainingWeapons))
        {
            trainingWeapon = GetNearestTrainingWeapon(startingEquippedWeapon);
            if (trainingWeapon != null && !trainingWeapon.IsForbidden(pawn))
            {
                //reserve training weapon, goto, and equip
                if (Map.reservationManager.CanReserve(pawn, trainingWeapon))
                {
                    pawn.Reserve(trainingWeapon, job);
                    job.SetTarget(TargetIndex.B, trainingWeapon);
                    yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
                        .FailOnDespawnedNullOrForbidden(TargetIndex.B);
                    yield return CreateEquipToil(TargetIndex.B);
                }

                //reserve previous weapon and set as target c
                if (Map.reservationManager.CanReserve(pawn, startingEquippedWeapon))
                {
                    pawn.Reserve(startingEquippedWeapon, job);
                    job.SetTarget(TargetIndex.C, startingEquippedWeapon);
                }
            }
        }

        var endOfTraining = Toils_General.Label();
        var gotoCastPos = Toils_Combat.GotoCastPosition(TargetIndex.A, TargetIndex.B, true, 0.95f)
            .EndOnDespawnedOrNull(TargetIndex.A);
        var ifTrainingDoneJumpToReequip = Toils_Jump.JumpIf(endOfTraining, HasTrainingEnded);
        var castVerb = Toils_Combat.CastVerb(TargetIndex.A, false);
        castVerb.AddFinishAction(LearnAttackSkill);
        var trainingRoomImpressivenessMoodBoost =
            Toils_General.Do(TryGainCombatTrainingRoomThought);
        var dropTrainingWeapon = Toils_General.Do(delegate
        {
            pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out _, pawn.Position, false);
        });
        var reequipSwappedStartingWeapon = Toils_General.Do(delegate
        {
            pawn.inventory.innerContainer.Remove(startingEquippedWeapon);
            pawn.equipment.AddEquipment(startingEquippedWeapon);
        });
        var jobEndedLabel = Toils_General.Label();


        yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
        yield return gotoCastPos;
        yield return Toils_Jump.JumpIfTargetNotHittable(TargetIndex.A, gotoCastPos);
        yield return trainingRoomImpressivenessMoodBoost;
        yield return ifTrainingDoneJumpToReequip;
        yield return castVerb;
        yield return Toils_Jump.Jump(ifTrainingDoneJumpToReequip);

        yield return endOfTraining;

        if (trainingWeapon != null)
        {
            yield return dropTrainingWeapon;
        }

        yield return Toils_Jump.JumpIf(reequipSwappedStartingWeapon,
            () => pawn.inventory.Contains(startingEquippedWeapon));
        yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.C);
        yield return CreateEquipToil(TargetIndex.C);
        yield return Toils_Jump.Jump(jobEndedLabel);

        yield return reequipSwappedStartingWeapon;

        yield return jobEndedLabel;
    }

    /*
     * Calculates the xp gained. seems like shooting was based off 170f and melee of 200f. Just used 200f for consistency
     */
    private float CalculateXp(Verb verb, Pawn incomingPawn)
    {
        return trainCombatLearningFactor * 200f * verb.verbProps.AdjustedFullCycleTime(verb, incomingPawn);
    }

    /*
     * Causes pawn to get impressive combat training room mood buff
     */
    private void TryGainCombatTrainingRoomThought()
    {
        var room = pawn.GetRoom();
        if (room == null)
        {
            return;
        }

        //get the impressive stage index for the current room
        var scoreStageIndex =
            RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
        //if the stage index exists in the definition (in xml), gain the memory (and buff)
        if (CombatTrainingDefOf.TrainedInImpressiveCombatTrainingRoom.stages[scoreStageIndex] != null)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(
                ThoughtMaker.MakeThought(CombatTrainingDefOf.TrainedInImpressiveCombatTrainingRoom,
                    scoreStageIndex));
        }
    }

    private bool LearningSaturated()
    {
        var verbToUse = pawn.jobs.curJob.verbToUse;
        var saturated = false;

        var skill = pawn.skills.GetSkill(verbToUse.verbProps.IsMeleeAttack ? SkillDefOf.Melee : SkillDefOf.Shooting);

        if (skill.LearningSaturatedToday ||
            skill.Level == 20 && skill.xpSinceLastLevel >= skill.XpRequiredForLevelUp - 1)
        {
            saturated = true;
        }

        return saturated;
    }

    /*
     * Causes pawn to learn a combat skill based on the verb of the current job
     */
    private void LearnAttackSkill()
    {
        var verbToUse = pawn.jobs.curJob.verbToUse;
        var xpGained = CalculateXp(verbToUse, pawn);
        if (verbToUse.verbProps.IsMeleeAttack)
        {
            pawn.skills.Learn(SkillDefOf.Melee, xpGained);
            CombatTrainingTracker.TrackPawnMeleeSkill(pawn, pawn.skills.GetSkill(SkillDefOf.Melee));
        }
        else
        {
            pawn.skills.Learn(SkillDefOf.Shooting, xpGained);
            CombatTrainingTracker.TrackPawnShootingSkill(pawn, pawn.skills.GetSkill(SkillDefOf.Shooting));
        }
    }


    private ThingWithComps GetNearestTrainingWeaponOfType(ThingDef weaponType)
    {
        var req = ThingRequest.ForDef(weaponType);
        return (ThingWithComps)GenClosest.RegionwiseBFSWorker(TargetA.Thing.Position, pawn.Map, req,
            PathEndMode.OnCell, TraverseParms.For(pawn), x => pawn.CanReserve(x), null, 0, 12, 50f,
            out _, RegionType.Set_Passable, true);
    }

    private ThingWithComps GetNearestTrainingWeaponMelee()
    {
        return GetNearestTrainingWeaponOfType(CombatTrainingDefOf.MeleeWeapon_TrainingKnife);
    }

    private ThingWithComps GetNearestTrainingWeaponRanged()
    {
        ThingWithComps thingWithComps = null;
        ThingDef weaponType;
        if (!pawn.Faction.def.techLevel.IsNeolithicOrWorse())
        {
            weaponType = CombatTrainingDefOf.Gun_TrainingBBGun;
            thingWithComps = GetNearestTrainingWeaponOfType(weaponType);
        }

        if (thingWithComps != null)
        {
            return thingWithComps;
        }

        weaponType = CombatTrainingDefOf.Bow_TrainingShort;
        thingWithComps = GetNearestTrainingWeaponOfType(weaponType);

        return thingWithComps;
    }

    /* 
     * Returns the nearest training weapon.  Enforces training weapons of the same type (melee or ranged) of the
     * weapon passed in, unless the pawn is unarmed.
     */
    private ThingWithComps GetNearestTrainingWeapon(Thing currentWeapon)
    {
        ThingWithComps nearestTrainingWeapon = null;

        // If the pawn has a melee weapon, look for a training knife.
        if (currentWeapon != null && currentWeapon.def.IsMeleeWeapon)
        {
            nearestTrainingWeapon = GetNearestTrainingWeaponMelee();
        }

        // If the pawn has a ranged weapon, look for a training ranged weapon.
        if (currentWeapon != null && !currentWeapon.def.IsMeleeWeapon)
        {
            nearestTrainingWeapon = GetNearestTrainingWeaponRanged();
        }

        // If the pawn does not have a weapon, and the dummy is restricted, look for the appropriate weapon type.
        if (currentWeapon == null &&
            !CombatTrainingController.HasDesignation(TargetThingA, CombatTrainingDefOf.TrainCombatDesignation))
        {
            if (CombatTrainingController.HasDesignation(TargetThingA,
                    CombatTrainingDefOf.TrainCombatDesignationMeleeOnly))
            {
                nearestTrainingWeapon = GetNearestTrainingWeaponMelee();
            }
            else if (CombatTrainingController.HasDesignation(TargetThingA,
                         CombatTrainingDefOf.TrainCombatDesignationRangedOnly))
            {
                nearestTrainingWeapon = GetNearestTrainingWeaponRanged();
            }
        }

        // If the pawn does not have a weapon, and the dummy is not restricted, look for the closest training weapon of any kind.
        if (currentWeapon != null ||
            !CombatTrainingController.HasDesignation(TargetThingA, CombatTrainingDefOf.TrainCombatDesignation))
        {
            return nearestTrainingWeapon;
        }

        var request = ThingRequest.ForGroup(ThingRequestGroup.Weapon);
        nearestTrainingWeapon = (ThingWithComps)GenClosest.RegionwiseBFSWorker(TargetA.Thing.Position,
            pawn.Map, request, PathEndMode.OnCell,
            TraverseParms.For(pawn),
            x => CombatTrainingDefOf.TrainingWeapons.DescendantThingDefs.Contains(x.def) &&
                 pawn.CanReserve(x), null, 0, 12, 50f, out _,
            RegionType.Set_Passable, true);

        return nearestTrainingWeapon;
    }

    /*
     * Returns a toil that equips the target index weapon 
     */
    private Toil CreateEquipToil(TargetIndex index)
    {
        var equipment = pawn.jobs.curJob.GetTarget(index);
        var toil = new Toil
        {
            initAction = delegate
            {
                var weaponPile = (ThingWithComps)(Thing)equipment;
                ThingWithComps weapon;
                if (weaponPile.def.stackLimit > 1 && weaponPile.stackCount > 1)
                {
                    weapon = (ThingWithComps)weaponPile.SplitOff(1);
                }
                else
                {
                    weapon = weaponPile;
                    weapon.DeSpawn();
                }

                if (pawn.equipment.Primary != null)
                {
                    pawn.equipment.TryTransferEquipmentToContainer(pawn.equipment.Primary,
                        pawn.inventory.innerContainer);
                }

                pawn.equipment.AddEquipment(weapon);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        toil.FailOnDespawnedNullOrForbidden(index);
        return toil;
    }
}