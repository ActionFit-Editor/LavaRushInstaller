# Lava Rush 0.2.1 Release Candidate Matrix

## Candidate

- Bundle: `lava-rush@0.2.1`
- Installer URL after separate publication: `https://github.com/ActionFit-Editor/LavaRushInstaller.git#0.2.1`
- Candidate selection date: `2026-07-24`
- Publication state: not published by MCC-1632
- Catalog state: unchanged by MCC-1632

Fresh remote inspection selected unused source versions `com.actionfit.referencebinding@0.2.2`, `com.actionfit.lava-rush.ui@0.2.4`, `com.actionfit.cat.app@0.2.2`, `com.actionfit.lava-rush.theme.catmerge@0.2.1`, and `com.actionfit.lava-rush.installer@0.2.1`. Lava Rush UI `0.2.3` became occupied on the target branch by the background physical-input lifetime change before this graph was integrated, so the ReferenceBinding pump closure moved to the next unused patch. Existing shared-owner pins remain engine `0.1.11`, Content Core `0.2.3`, Time `1.0.4`, UI Foundation `2.0.5`, UI Popup `0.1.2`, and Custom Package Manager `1.1.114`.

The authoritative required graph is `Editor/ContentBundleProfile.json`. It contains fourteen all-required entries: the eight ActionFit runtime/owner packages, Cat App, four immutable visual packages, and VContainer. The optional `com.actionfit.lava-rush.theme.catmerge@0.2.1` preset is intentionally absent.

## Automated Evidence Matrix

| Scenario | Required result | Evidence status |
| --- | --- | --- |
| Installer package contract | Empty dependencies; Editor-only reflection boundary; exact metadata | Passed package contract and 18/18 isolated installer EditMode tests |
| Exact profile graph | Canonical HTTPS repositories; SemVer tags/full UILighting commit; one registry compatibility exception | Passed installer profile tests and changed-package contract validation |
| First plan/install and same-version repeat | Atomic plan, durable ownership, repeat unchanged | Passed existing Custom Package Manager regression coverage; published one-URL fixture remains a manual final gate |
| Repair and older canonical upgrade | Restore exact required entries without replacing unsafe values | Passed existing Custom Package Manager and installer regression coverage |
| Newer canonical/local/file/fork/branch/embedded input | Preserve compatible/newer values; block unsafe or older embedded values with credential-safe diagnostics | Passed existing Custom Package Manager and installer regression coverage |
| Cancellation/failure/interruption | No partial manifest; recover journal; rollback incomplete registration | Passed existing Custom Package Manager regression coverage; published interruption fixture remains a manual final gate |
| Self-removal/release/shared dependencies | Remove bootstrap only after registration and ownership reload; preserve manager/shared/user values | Passed existing Custom Package Manager regression coverage; published release fixture remains a manual final gate |
| Addressables preview/apply | Read-only preview; separate confirmation; create-only missing entries; collisions block; failure rolls back | Passed 42/42 isolated Cat App EditMode tests |
| UI-only Embed for Edit | Preserve compatible embedded UI; block older UI; never auto-embed | Covered by Custom Package Manager tests and manual final gate |
| Package graph/content/docs | Package, AI CI, content-package, documentation, and diff validation | Passed package contract, AI CI, 12/12 content-package diagnostics, docs validation, and diff check |
| Clean disposable consumer | Downloaded-first compile, explicit three-key registration, canonical controller flow | Manual final release gate |
| Existing Cat migration fixture | Unchanged saves/keys/GUIDs, one authority, no duplicate rewards | Manual final release gate |
| Mobile render/audio/localization | Original presentation and device behavior | Manual final release gate |

## Rollback Pins

The current `0.2.1` candidate pins are recorded in `Editor/ContentBundleProfile.json` and `com.actionfit.lava-rush.ui/Documentation~/ConsumerMigration.md`. Custom Package Manager owns manifest compensation and bundle release; Addressables rollback uses the consuming project's separate serialized-state snapshot.

The last published one-URL baseline is installer `0.1.16` with manager `1.1.113`, engine `0.1.10`, UI Foundation `2.0.4`, UI `0.1.23`, and the same visual revisions. Returning to that breaking line also requires restoring its matching project Runtime from source control. A package-only downgrade or dual controller architecture is unsupported.

## Remaining Manual Final Gates

- Publish every candidate package in dependency order through a separately approved package workflow, then re-run remote tag and catalog checks.
- Install the published installer URL in a clean Unity `6000.3.9f1` project and run explicit Addressables registration.
- Run the existing-consumer save/reward/interruption fixture with compatible UI edits.
- Verify mobile rendering, localization refresh, audio cue timing, popup flow, Order/EventAccess, and package release/rollback.

## MCC-1632 Automated Run

- Unity: `6000.3.9f1`
- `com.actionfit.cat.app.Editor.Tests`: 42 passed
- `com.actionfit.lava-rush.installer.Editor.Tests`: 18 passed
- `com.actionfit.lava-rush.ui.Editor.Tests`: 61 passed
- `com.actionfit.lava-rush.theme.catmerge.Editor.Tests`: 2 passed
- `com.actionfit.custompackagemanager.Editor.Tests`: 165 passed; package shell tests passed
- Changed-package contract validator: 4 packages, 0 errors, 0 warnings
- LavaRush content-package validator: 12 passed, 0 failed, 0 unverified
- AI documentation validator and changed-package AI CI summary: passed
