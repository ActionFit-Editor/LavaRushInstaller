# AI Guide - ActionFit Lava Rush Installer

## Package Identity

- Package ID: `com.actionfit.lava-rush.installer`
- Display name: ActionFit Lava Rush Installer
- Repository: `https://github.com/ActionFit-Editor/LavaRushInstaller.git`
- Repository visibility: Public
- Current package version at generation time: `0.1.2`
- Unity version: `6000.2`

## Purpose

This public bootstrap package installs the complete Lava Rush content bundle from one Git URL. `com.actionfit.lava-rush.ui` is mandatory, not optional. After the manager verifies every required package and persists ownership state, the bootstrap dependency removes itself from the consuming project manifest.

## Project Router Registration

Requested router entry:

- `Packages/com.actionfit.lava-rush.installer/AI_GUIDE.md` - ActionFit Lava Rush Installer bootstraps the mandatory engine, UI, Content Core, Time, and Custom Package Manager bundle and self-removes after verified installation.

Read this file when changing the installer bootstrap, `Editor/ContentBundleProfile.json`, required Lava Rush package pins, authorized release users, package metadata, or release flow.

## Contracts

- `package.json` dependencies must remain empty because Git dependencies belong in the consuming project's top-level `Packages/manifest.json`.
- The bootstrap assembly must compile without hard references to Custom Package Manager or any Lava Rush runtime/UI assembly.
- It may bootstrap only canonical `com.actionfit.custompackagemanager@1.1.96` when the manager is missing or has an older tag from the same repository.
- Every bundle Git URL must use the canonical Public `ActionFit-Editor` repository and an exact version tag.
- Preserve embedded packages, local/file dependencies, forks, branches, unparseable revisions, user changes, and equal/newer canonical tags.
- Load `ActionFitContentBundleApi` through reflection and pass the package-shipped profile JSON to `InstallJson` or `RepairJson`.
- The bundle profile must keep `com.actionfit.lava-rush.ui` required. Do not introduce an engine-only installation path.
- Keep the manager required with `removeOnRelease: false`; it owns reconciliation and the authorized release UI after the bootstrap self-removes.
- Persist durable ownership only through Custom Package Manager at `ProjectSettings/ActionFitContentBundles.json`.
- Preserve interrupted-operation journals at `UserSettings/ActionFitPackageManager/ContentBundleTransactions` until completion or verified rollback.
- Automatic mutation is disabled in batchmode. Embedded development copies skip automatic installation.
- Release authorization is an exact, case-insensitive GitHub login allowlist. Never read, persist, or log credentials or tokens.

## Current Bundle Profile

- `com.actionfit.custompackagemanager@1.1.96`
- `com.actionfit.content-core@0.2.1`
- `com.actionfit.time@1.0.3`
- `com.actionfit.lava-rush@0.1.3`
- `com.actionfit.lava-rush.ui@0.1.3`
- Authorized release GitHub login: `JewooSong`

Every Git URL must use HTTPS and an exact version tag matching the declared version.

## Release Note Rules

- `ActionFitPackageInfo_SO.ReleaseNote` is Korean and contains only the single version being prepared.
- Package publishing is manual. Source changes do not authorize repository creation, push, tag, catalog append, or publish.
