# GitHub Copilot Instructions for Combat Training (Continued) Mod

## Mod Overview and Purpose

The "Combat Training (Continued)" mod is a continuation and enhancement of the original Combat Training mod by Kriil. This mod introduces a dedicated work type for colonists in RimWorld, enabling them to train their combat skills using combat dummies. The mod aims to enhance the gameplay by providing a realistic and balanced way to improve colonists' melee and shooting skills outside of combat scenarios.

## Key Features and Systems

1. **Combat Work Type**: Colonists can prioritize combat training as a work activity like any other.
2. **Combat Dummies**: Train combat skills by attacking dummies. These can be customized for melee, ranged, or both.
3. **Training Weapons**: Craft non-lethal training weapons to ensure safe training without friendly fire.
4. **Combat Training Room**: Assign rooms as Combat Training Rooms by placing dummies, which provide a mood buff to colonists.
5. **Skill Saturation**: Training ends when the daily XP cap is reached, with adjustments for colonists' passion levels.
6. **Weapon Management**: Automated weapon selection and switching to and from training weapons ensure efficient training sessions.

## Coding Patterns and Conventions

- **Class and Method Naming**: Classes and methods follow PascalCase naming conventions for clarity and consistency.
- **Commented Code**: Comments are used to describe the purpose of classes and methods, particularly in complex logic sections.
- **Disposable Patterns**: The use of private methods for logical checks, like `isDummyUsable()` and `isJobPossible()`, reduces redundancy and enhances readability.
- **Static Classes for Shared Logic**: Utility classes like `CompatibilityUtility` and `CombatTrainingTracker` manage shared logic, keeping the code modular and maintainable.

## XML Integration

XML is used for integrating new items and work types:
- **Defs**: XML files define new training weapons, combat dummies, and job activities. Refer to files like `CombatTrainingDefOf.cs` for examples.
- **Translations**: XML support for translations allows mods to provide multi-language support, as seen with the Chinese Simplified translation addition.

## Harmony Patching

Harmony is a critical tool for modding C# games like RimWorld:
- **Method Patching**: Use Harmony to patch game methods, allowing mod-specific functionality to be either added or modified without directly altering game code.
- **Examples**: Ensure trained pawns revert to their original state post-training or disable features like friendly fire during training sessions.

## Suggestions for Copilot

To further enhance productivity when contributing to the mod:

- **Code Pattern Detection**: Encourage Copilot to suggest code patterns commonly used in this project, such as private methods for logical checks.
- **XML Support**: Enhance Copilot’s suggestions when dealing with XML files related to RimWorld defs and translations.
- **Suggesting Modifications**: When working on Harmony patches, guide Copilot to suggest method prefixes, postfixes, and transpilers as appropriate.
- **Common Bug Fixes**: Enable Copilot to draw from the community-provided bug fixes to suggest known solutions to common issues, such as preventing non-canonical dummies from breaking.

By following these detailed instructions, contributors can maintain consistency and enhance the overall quality and scope of the Combat Training (Continued) Mod.
