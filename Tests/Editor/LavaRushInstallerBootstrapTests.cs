#if UNITY_EDITOR
using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ActionFit.LavaRushInstaller.Editor.Tests
{
    public sealed class LavaRushInstallerBootstrapTests
    {
        [TestCase("", "Missing")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.1.96", "Exact")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.1.94", "UpgradeCanonical")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.2.0", "PreserveNewerCanonical")]
        [TestCase("https://github.com/SomeoneElse/Custom_Package_Manager.git#1.1.96", "Conflict")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#main", "Conflict")]
        [TestCase("file:com.actionfit.custompackagemanager", "Conflict")]
        public void ClassifyManagerDependency_PreservesUnsafeValuesAndUpgradesOnlyCanonicalOlderTag(
            string currentValue,
            string expected)
        {
            Assert.That(LavaRushInstallerBootstrap.ClassifyManagerDependency(currentValue, "").ToString(), Is.EqualTo(expected));
        }

        [TestCase("1.1.96", "EmbeddedCompatible")]
        [TestCase("1.2.0", "EmbeddedCompatible")]
        [TestCase("1.1.94", "EmbeddedTooOld")]
        public void ClassifyManagerDependency_PreservesEmbeddedPackage(string version, string expected)
        {
            Assert.That(LavaRushInstallerBootstrap.ClassifyManagerDependency("", version).ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ContentBundleProfile_RequiresCompleteLavaRushSetAndAuthorizedReleaseLogin()
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Packages/com.actionfit.lava-rush.installer/Editor/ContentBundleProfile.json");
            Assert.That(asset, Is.Not.Null);

            Profile profile = JsonUtility.FromJson<Profile>(asset.text);
            Assert.That(profile.bundleId, Is.EqualTo("lava-rush"));
            Assert.That(profile.bootstrapPackageId, Is.EqualTo(LavaRushInstallerBootstrap.InstallerPackageId));
            Assert.That(profile.packages.Select(package => package.packageId), Is.EquivalentTo(new[]
            {
                "com.actionfit.custompackagemanager",
                "com.actionfit.content-core",
                "com.actionfit.time",
                "com.actionfit.lava-rush",
                "com.actionfit.lava-rush.ui",
            }));
            Assert.That(profile.packages.All(package => package.required), Is.True);
            Assert.That(profile.packages.All(package =>
                package.gitUrl.StartsWith("https://github.com/ActionFit-Editor/", StringComparison.Ordinal)), Is.True);
            Assert.That(profile.packages.All(package =>
                package.gitUrl.EndsWith("#" + package.version, StringComparison.Ordinal)), Is.True);
            Package manager = profile.packages.Single(package =>
                package.packageId == "com.actionfit.custompackagemanager");
            Assert.That(manager.version, Is.EqualTo(LavaRushInstallerBootstrap.ManagerVersion));
            Assert.That(manager.gitUrl, Is.EqualTo(LavaRushInstallerBootstrap.ManagerGitUrl));
            Assert.That(manager.removeOnRelease, Is.False);
            Assert.That(profile.packages.Single(package =>
                package.packageId == "com.actionfit.lava-rush.ui").required, Is.True);
            Assert.That(profile.allowedReleaseGitHubLogins, Is.EqualTo(new[] { "JewooSong" }));
        }

        [Serializable]
        private sealed class Profile
        {
            public string bundleId = "";
            public string bootstrapPackageId = "";
            public Package[] packages = Array.Empty<Package>();
            public string[] allowedReleaseGitHubLogins = Array.Empty<string>();
        }

        [Serializable]
        private sealed class Package
        {
            public string packageId = "";
            public string version = "";
            public string gitUrl = "";
            public bool required;
            public bool removeOnRelease;
        }
    }
}
#endif
