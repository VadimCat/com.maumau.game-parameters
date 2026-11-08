# Game parameter tables

Editor-only Unity package, created in Snowball Sweep. No runtime dependency, save state or asset generation on import.

Call `MauMau.GameParameters.SerializedParameterTable.Draw(config, search)` from a game's EditorWindow. Unity handles serialization, Undo, field ranges, arrays and dirty tracking. Public and `[SerializeField]` private fields are displayed; `[Header]` groups and tooltips are retained. Call `ExportCsv(config, path)` to export serialized fields, with quoted cells and invariant scalar/vector/color values.

The game owns its ScriptableObject schema, window menu, domain validation, level/material tabs and explicit authoring actions. Reference `MauMau.GameParameters.Editor` from its Editor asmdef. Copy this package directory or install from the versioned source-game package path. Unity 2022.3 or later; no game namespaces or scene references.

Provenance: authored for Snowball Sweep, 27 October 2026. See the source repository's feature PR and the UnityGameModules catalog for the reviewed revision. Runtime configuration remains game-specific.
