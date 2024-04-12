using System;
using Verse;

namespace KriilMod_CD;

[StaticConstructorOnStartup]
public class CombatTrainingController
{
    public static bool HasDesignation(Thing thing, DesignationDef def)
    {
        return thing.Map?.designationManager?.DesignationOn(thing, def) != null;
    }

    public static void ToggleDesignation(Thing thing, DesignationDef def, bool enable)
    {
        if (thing.Map?.designationManager == null)
        {
            throw new Exception("Thing must belong to a map to toggle designations on it");
        }

        var designation = thing.Map.designationManager.DesignationOn(thing, def);
        switch (enable)
        {
            case true when designation == null:
                thing.Map.designationManager.AddDesignation(new Designation(thing, def));
                break;
            case false when designation != null:
                thing.Map.designationManager.RemoveDesignation(designation);
                break;
        }
    }
}