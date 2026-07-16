#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ActionFit.LavaRushInstaller.Editor
{
    internal static class LavaRushInstallerPackageMenu
    {
        private const string MenuRoot = "Tools/Package/Lava Rush/Installer/";
        private const string ReadmePath = "Packages/com.actionfit.lava-rush.installer/README.md";

        [MenuItem(MenuRoot + "Install or Repair Bundle", false, 201)]
        private static void InstallOrRepair()
        {
            if (!EditorUtility.DisplayDialog(
                    "Lava Rush Content Bundle",
                    "Install or repair the required Lava Rush engine, UI, Content Core, Time, and Custom Package Manager dependencies? Compatible embedded and newer canonical packages are preserved.",
                    "Install or Repair",
                    "Cancel"))
                return;

            LavaRushInstallerBootstrap.InstallOrRepair(true, true);
        }

        [MenuItem(MenuRoot + "README", false, 901)]
        private static void OpenReadme()
        {
            TextAsset readme = AssetDatabase.LoadAssetAtPath<TextAsset>(ReadmePath);
            if (readme == null)
            {
                EditorUtility.DisplayDialog("Lava Rush Installer", $"README was not found.\n{ReadmePath}", "OK");
                return;
            }

            Selection.activeObject = readme;
            AssetDatabase.OpenAsset(readme);
        }
    }
}
#endif
