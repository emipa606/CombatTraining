# GitHub Copilot Instructions for RimWorld Mod: Combat Training (Continued)

## Mod Overview and Purpose

Combat Training (Continued) is an updated version of the original mod by Kriils. It enhances the combat-training mechanics of RimWorld by introducing a work type dedicated to training combat skills. This mod is designed to provide a more immersive and realistic approach to developing your colonists' melee and shooting skills.

## Key Features and Systems

- **Combat Dummies**: Construct combat dummies that colonists can use to train melee and ranged combat skills. Dummies are categorized into melee-only, ranged-only, or both, and are available in the Misc tab.
- **Training Weapons**: Craft non-lethal training weapons that ensure no friendly fire incidents occur during practice.
- **Combat Training Priority**: Training can be prioritized like any other work via the Work tab.
- **Combat Training Room Role**: Enhance rooms with a combat dummy to designate them as Combat Training Rooms, providing colonists with a mood buff.
- **Weapon and Dummy Interactions**: Colonists intuitively switch between real and training weapons based on dummy designations and availability.

## Coding Patterns and Conventions

- **C# Class Structure**: Classes such as `Building_CombatDummy`, `CombatTrainingController`, and `Designator_TrainCombatXXX` encapsulate different aspects and functionalities of the mod.
- **Method Names**: Methods are prefixed with verbs reflecting the action they perform, such as `determineDesignation`, `tryGainCombatTrainingRoomThought`, and `isDummyUsable`.
- **Comments and Documentation**: Code is annotated with comments to improve readability and understanding, especially in classes like `CombatTrainingTracker`.

## XML Integration

- **Def Files**: `TrainCombatDesignatorDef` manages the XML-based definitions relevant to the mod, linking game assets to in-game functionality.
- **Role Integration**: The mod leverages XML for defining new roles and extensions, such as room roles for the Combat Training Room.

## Harmony Patching

- **Harmony Usage**: Mods that require altering base game behavior utilize Harmony patches. This mod may employ Harmony to dynamically adjust or patch existing capabilities or introduce new functionalities seamlessly.

## Suggestions for Copilot

- **Code Completion**: Offer code completion for common patterns such as RimWorld-designated verbs and noun pairs like `CreateDesignation` or `CheckSkillLevel`.
- **Suggestion Context**: Generate suggestions for integrating new training mechanics using existing patterns, ensuring consistency and compatibility.
- **Debugging Assistance**: Provide real-time debugging insights or suggestions to help resolve potential issues promptly, such as logging or exception handling.
- **XML Editing**: Assist with schema-aware XML editing, ensuring accurate and typo-free def entries and references.

## Miscellaneous

- **Combat Mod Balance**: The mod achieves balance by making combat training a labor-intensive task that consumes colonists' time and resources.
- **Community Contributions**: It has integrated various community-made enhancements that improve gameplay and compatibility with other popular RimWorld mods.
- **Support**: For troubleshooting or support, it's recommended to use the mod's dedicated Discord channel for error reporting and to post solutions to the GitHub repository for community benefit.

By following these instructions and utilizing library dependencies effectively, GitHub Copilot can assist in maintaining and enhancing the Combat Training (Continued) mod to deliver an engaging gameplay experience. 

---

*Contributors*: aph3rson, @Menhi, kfish#2446, KongMD#2541, Borattata, Heremeus, AtomicGu

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
