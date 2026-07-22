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
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.1.113", "Exact")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.1.112", "UpgradeCanonical")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#1.2.0", "PreserveNewerCanonical")]
        [TestCase("https://github.com/SomeoneElse/Custom_Package_Manager.git#1.1.113", "Conflict")]
        [TestCase("https://github.com/ActionFit-Editor/Custom_Package_Manager.git#main", "Conflict")]
        [TestCase("file:com.actionfit.custompackagemanager", "Conflict")]
        public void ClassifyManagerDependency_PreservesUnsafeValuesAndUpgradesOnlyCanonicalOlderTag(
            string currentValue,
            string expected)
        {
            Assert.That(LavaRushInstallerBootstrap.ClassifyManagerDependency(currentValue, "").ToString(), Is.EqualTo(expected));
        }

        [TestCase("1.1.113", "EmbeddedCompatible")]
        [TestCase("1.2.0", "EmbeddedCompatible")]
        [TestCase("1.1.112", "EmbeddedTooOld")]
        public void ClassifyManagerDependency_PreservesEmbeddedPackage(string version, string expected)
        {
            Assert.That(LavaRushInstallerBootstrap.ClassifyManagerDependency("", version).ToString(), Is.EqualTo(expected));
        }

        [TestCase("", false)]
        [TestCase("1.1.112", false)]
        [TestCase("1.1.113", true)]
        [TestCase("1.2.0", true)]
        [TestCase("main", false)]
        public void IsManagerApiVersionCompatible_RejectsStaleLoadedAssembly(string version, bool expected)
        {
            Assert.That(LavaRushInstallerBootstrap.IsManagerApiVersionCompatible(version), Is.EqualTo(expected));
        }

        [Test]
        public void ContentBundleProfile_RequiresCompleteLavaRushSetAndAuthorizedReleaseLogin()
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Packages/com.actionfit.lava-rush.installer/Editor/ContentBundleProfile.json");
            Assert.That(asset, Is.Not.Null);

            Profile profile = JsonUtility.FromJson<Profile>(asset.text);
            Assert.That(profile.bundleId, Is.EqualTo("lava-rush"));
            Assert.That(profile.bundleVersion, Is.EqualTo("0.1.16"));
            Assert.That(profile.bootstrapPackageId, Is.EqualTo(LavaRushInstallerBootstrap.InstallerPackageId));
            Assert.That(profile.packages.Select(package => package.packageId), Is.EquivalentTo(new[]
            {
                "com.actionfit.custompackagemanager",
                "com.actionfit.content-core",
                "com.actionfit.time",
                "com.actionfit.lava-rush",
                "com.actionfit.ui.foundation",
                "com.actionfit.ui.popup",
                "com.actionfit.lava-rush.ui",
                "com.coffee.ui-effect",
                "com.coffee.ui-particle",
                "com.coffee.softmask-for-ugui",
                "com.actionfit.uilighteffector",
                "jp.hadashikick.vcontainer",
            }));
            Assert.That(profile.packages.All(package => package.required), Is.True);
            Assert.That(profile.packages.All(package =>
                package.gitUrl.StartsWith("https://github.com/", StringComparison.Ordinal)), Is.True);
            Assert.That(profile.packages.Single(package => package.packageId == "com.coffee.ui-effect").gitUrl,
                Is.EqualTo("https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.10.8"));
            Assert.That(profile.packages.Single(package => package.packageId == "com.coffee.ui-particle").gitUrl,
                Is.EqualTo("https://github.com/mob-sakai/ParticleEffectForUGUI.git#4.12.1"));
            Assert.That(profile.packages.Single(package => package.packageId == "com.coffee.softmask-for-ugui").gitUrl,
                Is.EqualTo("https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src#3.5.0"));
            Assert.That(profile.packages.Single(package => package.packageId == "com.actionfit.uilighteffector").gitUrl,
                Is.EqualTo("https://github.com/HuiSungz/UILightingEffect-ReShade.git#7dab46ec2378209bd1e524c8336b976eccb3df05"));
            Assert.That(profile.packages.Single(package => package.packageId == "jp.hadashikick.vcontainer").gitUrl,
                Is.EqualTo("https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.16.8"));
            Assert.That(profile.packages.Single(package =>
                package.packageId == "jp.hadashikick.vcontainer").allowCompatibleRegistryVersion, Is.True);
            Assert.That(profile.packages.Where(package =>
                package.packageId != "jp.hadashikick.vcontainer").All(package =>
                !package.allowCompatibleRegistryVersion), Is.True);
            Package manager = profile.packages.Single(package =>
                package.packageId == "com.actionfit.custompackagemanager");
            Assert.That(manager.version, Is.EqualTo(LavaRushInstallerBootstrap.ManagerVersion));
            Assert.That(manager.gitUrl, Is.EqualTo(LavaRushInstallerBootstrap.ManagerGitUrl));
            Assert.That(manager.removeOnRelease, Is.False);
            Assert.That(profile.packages.Single(package =>
                package.packageId == "com.actionfit.lava-rush.ui").required, Is.True);
            Assert.That(Version(profile, "com.actionfit.content-core"), Is.EqualTo("0.2.3"));
            Assert.That(Version(profile, "com.actionfit.time"), Is.EqualTo("1.0.4"));
            Assert.That(Version(profile, "com.actionfit.lava-rush"), Is.EqualTo("0.1.10"));
            Assert.That(Version(profile, "com.actionfit.ui.foundation"), Is.EqualTo("2.0.4"));
            Assert.That(Version(profile, "com.actionfit.ui.popup"), Is.EqualTo("0.1.1"));
            Assert.That(Version(profile, "com.actionfit.lava-rush.ui"), Is.EqualTo("0.1.23"));
            Assert.That(Version(profile, "com.coffee.ui-effect"), Is.EqualTo("5.10.8"));
            Assert.That(Version(profile, "com.coffee.ui-particle"), Is.EqualTo("4.12.1"));
            Assert.That(Version(profile, "com.coffee.softmask-for-ugui"), Is.EqualTo("3.5.0"));
            Assert.That(Version(profile, "com.actionfit.uilighteffector"), Is.EqualTo("1.0.0"));
            Assert.That(Version(profile, "jp.hadashikick.vcontainer"), Is.EqualTo("1.16.8"));
            Assert.That(profile.allowedReleaseGitHubLogins, Is.EqualTo(new[] { "JewooSong" }));
        }

        [Test]
        public void ConflictReport_ListsPackageAndCredentialSafeDependencySummary()
        {
            var result = new FakeResult
            {
                plan = new FakePlan
                {
                    changes = new[]
                    {
                        new FakeChange
                        {
                            packageId = "com.coffee.ui-effect",
                            from = "https://secret@github.com/mob-sakai/UIEffect.git?path=Packages/src",
                            to = "https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.10.8",
                            kind = FakeChangeKind.Conflict,
                        },
                        new FakeChange
                        {
                            packageId = "jp.hadashikick.vcontainer",
                            from = "1.17.0",
                            to = "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.16.8",
                            kind = FakeChangeKind.Conflict,
                        },
                    },
                },
            };

            string[] conflicts = LavaRushInstallerBootstrap.DescribeConflicts(result);

            Assert.That(conflicts, Has.Length.EqualTo(2));
            Assert.That(conflicts[0], Does.Contain("com.coffee.ui-effect"));
            Assert.That(conflicts[0], Does.Contain("github.com/mob-sakai/UIEffect.git#(floating)"));
            Assert.That(conflicts[0], Does.Not.Contain("secret"));
            Assert.That(conflicts[1], Does.Contain("registry@1.17.0"));
            Assert.That(conflicts[1], Does.Contain("github.com/hadashiA/VContainer.git#1.16.8"));
        }

        private static string Version(Profile profile, string packageId)
        {
            return profile.packages.Single(package => package.packageId == packageId).version;
        }

        [Serializable]
        private sealed class Profile
        {
            public string bundleId = "";
            public string bundleVersion = "";
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
            public bool allowCompatibleRegistryVersion;
        }

        private sealed class FakeResult
        {
            public FakePlan plan = new();
        }

        private sealed class FakePlan
        {
            public FakeChange[] changes = Array.Empty<FakeChange>();
        }

        private sealed class FakeChange
        {
            public string packageId = "";
            public string from = "";
            public string to = "";
            public FakeChangeKind kind;
        }

        private enum FakeChangeKind
        {
            Conflict,
        }
    }
}
#endif
