using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KriilMod_CD;

public class Designator_BaseTrainCombat : Designator
{
    protected DesignationDef defOf = null;

    /*
    * Default constructor 
    */
    public Designator_BaseTrainCombat()
    {
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Designate_Hunt;
        hotKey = KeyBindingDefOf.Misc9;
    }

    public override int DraggableDimensions => 2;

    /*
     * Designates a thing at the given cell.  Checks are made to ensure that a thing at the given cell can be designated with the TrainCombatDesignation
     */
    public override void DesignateSingleCell(IntVec3 loc)
    {
        var combatDummy = GetDesignatable(loc);
        DesignateThing(combatDummy);
    }

    /*
     * Returns the thing at the given location if it can be designated
     */
    public Thing GetDesignatable(IntVec3 loc)
    {
        var thingList = loc.GetThingList(Map);
        foreach (var thing in thingList)
        {
            if (CanDesignateThing(thing).Accepted)
            {
                return thing;
            }
        }

        return null;
    }

    /*
     * Returns true if the given thing is a combat dummy and doesn't already have the TrainCombatDesignation
     */
    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (t == null)
        {
            return false;
        }

        return IsCombatDummy(t) && !CombatTrainingController.HasDesignation(t, defOf);
    }

    /*
     * Returns true if the given cell is inbounds, not fogged and a thing at the given cell can be designated
     */
    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(Map) || c.Fogged(Map))
        {
            return false;
        }

        if (!CanDesignateThing(GetDesignatable(c)).Accepted)
        {
            return "MessageMustDesignateCombatDummy".Translate();
        }

        return true;
    }

    /*
     * Gives the given thing the TrainCombatDesignation
     */
    public override void DesignateThing(Thing t)
    {
        if (t != null)
        {
            CombatTrainingController.ToggleDesignation(t, defOf, true);
        }
    }


    public override void SelectedUpdate()
    {
        GenUI.RenderMouseoverBracket();
        GenDraw.DrawNoBuildEdgeLines();
    }


    public override void RenderHighlight(List<IntVec3> dragCells)
    {
        DesignatorUtility.RenderHighlightOverSelectableThings(this, dragCells);
    }

    private bool IsCombatDummy(Thing t)
    {
        return typeof(Building_CombatDummy).IsAssignableFrom(t.def.thingClass);
    }
}