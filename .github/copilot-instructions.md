# GitHub Copilot Instructions for Combat Training (Continued) Mod

## Mod Overview and Purpose
Combat Training (Continued) is an update to Kriil's mod, adding Quality of Life (QOL) features from the unupdated Combat Training mod forked by Eltech. This mod introduces a new work type to train combat skills, including both melee and shooting skills. It also includes fixes for pawns trying to train ranged combat with shield-belt-equipped units and ensures that combat dummies no longer break down and require components to repair.

## Key Features and Systems
- **Combat Training Work Type**: Allows colonists to train their combat skills by attacking combat dummies.
- **Designation System**: Colonists can be designated to train with melee-only, ranged-only, or both types of combat dummies.
- **Training Weapons**: Pawns can use either real weapons or crafted training weapons that cause no damage.
- **Combat Dummy Rooms**: Adding a combat dummy to a room gives that room the Combat Training Room role, providing a mood buff for the colonists.
- **Skill Saturation**: Colonists will stop training when their skill level has been saturated for the day (4000xp).
- **Weapon Selection Rules**: Automatically selects appropriate training weapons based on the dummy's designation and the colonist's current weapon.
- **Bug Fixes and Enhancements**: Includes various bug fixes and community enhancements to improve the mod's functionality and balance.

## Coding Patterns and Conventions
- **Namespace and Assembly**: Ensure that all classes and methods are correctly namespaced to avoid conflicts.
- **Documentation**: Use XML documentation comments for classes, methods, and properties to improve code readability and maintainability.
- **Error Handling**: Implement robust error handling to manage unexpected situations gracefully.
- **Performance Optimization**: Optimize code to ensure smooth performance, especially in scenarios with multiple pawns and complex interactions.

## XML Integration
- **Defs Files**: The mod uses XML files to define various aspects such as job definitions, designation definitions, room roles, thing categories, and thing definitions.
- **Def Types**: Ensure that all def types are correctly defined and integrated into the game's data system.
- **Def Exposures**: Use the `ExposeData` method to serialize and deserialize def data, ensuring that mod settings persist across sessions.

## Harmony Patching
- **Patch Targets**: Use Harmony to patch existing RimWorld methods and constructors to introduce new functionality or fix bugs.
- **Patch Naming**: Clearly name patches to identify their purpose and impact.
- **Patch Order**: Ensure that patches are applied in the correct order to avoid conflicts or unintended side effects.

## Suggestions for Copilot
- **Code Completion**: Use Copilot for code completion when implementing new features or fixing bugs. Ensure that the generated code aligns with the coding patterns and conventions.
- **Def Creation**: Utilize Copilot to generate XML def files for new job definitions, designation definitions, room roles, and thing definitions.
- **Patch Creation**: Use Copilot to assist in creating Harmony patches, ensuring that patches are correctly targeted and named.
- **Documentation**: Use Copilot to generate XML documentation comments for classes, methods, and properties to improve code readability and maintainability.
- **Error Handling**: Use Copilot to assist in implementing robust error handling for unexpected situations.

## Example XML File
xml
<Defs>
    <JobDef>
        <defName>TrainCombat</defName>
        <label>Train Combat</label>
        <description>Train your colonist's combat skills by attacking combat dummies.</description>
        <category>Work</category>
        <workType>CombatTraining</workType>
        <doUntilDone>true</doUntilDone>
        <makeTargetPrerotation>false</makeTargetPrerotation>
        <checkTargetWhileActive>true</checkTargetWhileActive>
        <checkOverrideOnJobStart>true</checkOverrideOnJobStart>
        <checkOverrideOnResume>true</checkOverrideOnResume>
        <checkOverrideOnFinish>true</checkOverrideOnFinish>
        <reportString>Training combat at {TargetA}</reportString>
    </JobDef>
</Defs>


## Example Harmony Patch
csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatTrainingMod
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Pawn_JobTracker_StartJob_Patch
    {
        public static void Postfix(Pawn_JobTracker __instance, Job job)
        {
            if (job.def.defName == "TrainCombat")
            {
                // Custom logic for combat training
            }
        }
    }
}


By following these guidelines, you can effectively use GitHub Copilot to enhance the Combat Training (Continued) mod, ensuring that new features are implemented efficiently and existing code is well-maintained.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

