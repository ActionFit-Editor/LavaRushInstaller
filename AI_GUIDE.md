# AI Guide - ActionFit Lava Rush Installer

## Package Identity

- Package ID: `com.actionfit.lava-rush.installer`
- Display name: ActionFit Lava Rush Installer
- Repository: `https://github.com/ActionFit-Editor/LavaRushInstaller.git`
- Repository visibility: Public
- Current package version at generation time: `0.2.0`
- Unity version: `6000.2`

## Purpose

This public bootstrap package installs the complete Lava Rush `0.2.0` content bundle from one Git URL. `com.actionfit.lava-rush.ui@0.2.0`, `com.actionfit.cat.app@0.2.0`, UI Foundation, UI Popup, ReferenceBinding, the four visual-effect packages referenced by the original prefabs, and UILighting's VContainer runtime dependency are mandatory. UI `0.2.0` supplies the single-owner original production prefab/image baseline and restored direct controllers; Cat App supplies the one product composition plus the separately confirmed three-key Addressables registration tool. After the manager verifies every required package and persists ownership state, the bootstrap dependency removes itself from the consuming project manifest.

## Project Router Registration

Requested router entry:

- `Packages/com.actionfit.lava-rush.installer/AI_GUIDE.md` - ActionFit Lava Rush Installer bootstraps the exact mandatory engine, direct UI, Cat App, reusable owners, and immutable visual graph, then self-removes after verified installation without mutating Addressables.

Read this file when changing the installer bootstrap, `Editor/ContentBundleProfile.json`, required Lava Rush package pins, authorized release users, package metadata, or release flow.

## Contracts

- `package.json` dependencies must remain empty because Git dependencies belong in the consuming project's top-level `Packages/manifest.json`.
- The bootstrap assembly must compile without hard references to Custom Package Manager or any Lava Rush runtime/UI assembly.
- It may bootstrap only canonical `com.actionfit.custompackagemanager@1.1.114` when the manager is missing or has an older tag from the same repository.
- The installer must compare the loaded manager API package version as well as the manifest value. It must wait for package registration when an older manager assembly remains loaded after a manifest upgrade.
- ActionFit bundle packages use canonical Public `ActionFit-Editor` repositories. Production prefab effect dependencies use only the explicitly reviewed upstream repositories and immutable revisions listed below.
- Prefer an exact SemVer tag. Only a repository with no version tag may use a full 40-character immutable commit; branches, short commits, and floating revisions are forbidden.
- Preserve embedded packages, local/file dependencies, forks, branches, unparseable revisions, user changes, and equal/newer canonical tags.
- Preserve an already-installed stable registry VContainer version equal to or newer than `1.16.8` through its explicit `allowCompatibleRegistryVersion` profile opt-in; no other package receives this exception.
- Install `com.actionfit.lava-rush.ui@0.2.0` with `com.actionfit.cat.app@0.2.0` and `com.actionfit.ui.popup@0.1.1` so the single-owner production baseline, direct controllers, one Cat product authority, whole-flow queue owner, ReferenceBinding fields, complete `TMPro*.cginc` set, and engine-backed completion evidence remain one verified release graph.
- A consuming project with different project-owned global `UI_*` sources must preserve those sources and use the documented project-local UI Foundation isolation (`Embed for Edit` plus Runtime `autoReferenced: false`). Never delete or migrate project scripts automatically from the installer.
- Failed installation must report every conflicting package with a credential-safe current/required dependency summary instead of reporting only the conflict count.
- Load `ActionFitContentBundleApi` through reflection and pass the package-shipped profile JSON to `InstallJson` or `RepairJson`.
- The bundle profile must keep `com.actionfit.lava-rush.ui` required. Do not introduce an engine-only installation path.
- The bundle profile must keep `com.actionfit.cat.app` required. The Cat Merge theme remains an optional direct install because no final Cat Runtime assembly depends on it.
- Bundle installation, repair, package import, batchmode, and Editor startup must never modify Addressables settings. After installation, the persistent Cat App package owns read-only preview and separately confirmed create-only apply for the three canonical keys.
- Project visual customization is an explicit post-install `Embed for Edit` action for `com.actionfit.lava-rush.ui` only. The installer must never auto-embed packages or replace compatible embedded UI edits; an embedded version below the profile requirement remains an explicit conflict.
- Keep the manager required with `removeOnRelease: false`; it owns reconciliation and the authorized release UI after the bootstrap self-removes.
- Persist durable ownership only through Custom Package Manager at `ProjectSettings/ActionFitContentBundles.json`.
- Preserve interrupted-operation journals at `UserSettings/ActionFitPackageManager/ContentBundleTransactions` until completion or verified rollback.
- Automatic mutation is disabled in batchmode. Embedded development copies skip automatic installation.
- Release authorization is an exact, case-insensitive GitHub login allowlist. Never read, persist, or log credentials or tokens.

## Current Bundle Profile

- `com.actionfit.custompackagemanager@1.1.114`
- `com.actionfit.content-core@0.2.3`
- `com.actionfit.time@1.0.4`
- `com.actionfit.lava-rush@0.1.11`
- `com.actionfit.referencebinding@0.2.1`
- `com.actionfit.ui.foundation@2.0.5`
- `com.actionfit.ui.popup@0.1.1`
- `com.actionfit.lava-rush.ui@0.2.0`
- `com.actionfit.cat.app@0.2.0`
- `com.coffee.ui-effect@5.10.8` — `mob-sakai/UIEffect`, `Packages/src`
- `com.coffee.ui-particle@4.12.1` — `mob-sakai/ParticleEffectForUGUI`
- `com.coffee.softmask-for-ugui@3.5.0` — `mob-sakai/SoftMaskForUGUI`, `Packages/src`
- `com.actionfit.uilighteffector@1.0.0` — `HuiSungz/UILightingEffect-ReShade` full commit `7dab46ec2378209bd1e524c8336b976eccb3df05`
- `jp.hadashikick.vcontainer@1.16.8` — `hadashiA/VContainer`, `VContainer/Assets/VContainer`
- Authorized release GitHub login: `JewooSong`

Every Git URL must use HTTPS and an immutable revision. Do not strip an original prefab effect component to reduce this dependency set; changing or removing one requires an explicit visual-parity decision.

## Release Note Rules

- `ActionFitPackageInfo_SO.ReleaseNote` is Korean and contains only the single version being prepared.
- Package publishing is manual. Source changes do not authorize repository creation, push, tag, catalog append, or publish.
