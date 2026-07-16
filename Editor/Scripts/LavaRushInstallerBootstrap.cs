#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ActionFit.LavaRushInstaller.Editor
{
    [InitializeOnLoad]
    internal static class LavaRushInstallerBootstrap
    {
        internal const string InstallerPackageId = "com.actionfit.lava-rush.installer";
        internal const string ManagerPackageId = "com.actionfit.custompackagemanager";
        internal const string ManagerVersion = "1.1.95";
        internal const string ManagerRepository = "https://github.com/ActionFit-Editor/Custom_Package_Manager.git";
        internal const string ManagerGitUrl = ManagerRepository + "#" + ManagerVersion;

        private const string ProfileAssetPath = "Packages/com.actionfit.lava-rush.installer/Editor/ContentBundleProfile.json";
        private const string ManifestRelativePath = "Packages/manifest.json";
        private const string ManagerRequestSessionKey = "ActionFit.LavaRushInstaller.ManagerRequest.0.1.1";
        private const string ManagerResolveSessionKey = "ActionFit.LavaRushInstaller.ManagerResolve.0.1.1";

        private static AddRequest _managerRequest;

        static LavaRushInstallerBootstrap()
        {
            EditorApplication.delayCall += AutoInstall;
            Events.registeredPackages += OnRegisteredPackages;
        }

        internal static void InstallOrRepair(bool repair, bool allowEmbedded)
        {
            if (Application.isBatchMode)
            {
                Debug.LogError("[Lava Rush Installer] Bundle mutation is disabled in Unity batchmode.");
                return;
            }

            if (!allowEmbedded && IsDevelopmentEmbedded())
            {
                Debug.Log("[Lava Rush Installer] Embedded development package detected; automatic bundle installation was skipped.");
                return;
            }

            Type apiType = FindManagerApiType();
            if (apiType == null)
            {
                if (repair)
                {
                    SessionState.SetBool(ManagerRequestSessionKey, false);
                    SessionState.SetBool(ManagerResolveSessionKey, false);
                }
                EnsureManagerAvailable();
                return;
            }

            TextAsset profile = AssetDatabase.LoadAssetAtPath<TextAsset>(ProfileAssetPath);
            if (profile == null)
            {
                Debug.LogError($"[Lava Rush Installer] Bundle profile not found: {ProfileAssetPath}");
                return;
            }

            string methodName = repair ? "RepairJson" : "InstallJson";
            MethodInfo method = apiType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                Debug.LogError($"[Lava Rush Installer] Custom Package Manager does not expose {methodName}. Upgrade {ManagerPackageId} to {ManagerVersion} or newer.");
                return;
            }

            try
            {
                object result = method.Invoke(null, new object[] { profile.text });
                ReportResult(result);
            }
            catch (TargetInvocationException exception)
            {
                Debug.LogError($"[Lava Rush Installer] Bundle install failed: {exception.InnerException?.Message ?? exception.Message}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Lava Rush Installer] Bundle install failed: {exception.Message}");
            }
        }

        internal static ManagerDependencyDisposition ClassifyManagerDependency(
            string currentValue,
            string embeddedVersion)
        {
            if (!string.IsNullOrWhiteSpace(embeddedVersion))
            {
                return CompareVersions(embeddedVersion, ManagerVersion) >= 0
                    ? ManagerDependencyDisposition.EmbeddedCompatible
                    : ManagerDependencyDisposition.EmbeddedTooOld;
            }

            if (string.IsNullOrWhiteSpace(currentValue))
                return ManagerDependencyDisposition.Missing;
            if (string.Equals(currentValue, ManagerGitUrl, StringComparison.Ordinal))
                return ManagerDependencyDisposition.Exact;
            if (!TrySplitGitUrl(currentValue, out string repository, out string revision) ||
                !string.Equals(NormalizeRepository(repository), NormalizeRepository(ManagerRepository), StringComparison.OrdinalIgnoreCase) ||
                !IsVersionTag(revision))
            {
                return ManagerDependencyDisposition.Conflict;
            }

            return CompareVersions(revision, ManagerVersion) < 0
                ? ManagerDependencyDisposition.UpgradeCanonical
                : ManagerDependencyDisposition.PreserveNewerCanonical;
        }

        private static void AutoInstall()
        {
            InstallOrRepair(false, false);
        }

        private static void OnRegisteredPackages(PackageRegistrationEventArgs _)
        {
            EditorApplication.delayCall += AutoInstall;
        }

        private static void EnsureManagerAvailable()
        {
            string embeddedVersion = ReadEmbeddedPackageVersion(ManagerPackageId);
            string currentValue = ReadManifestDependency(ManagerPackageId);
            ManagerDependencyDisposition disposition = ClassifyManagerDependency(currentValue, embeddedVersion);
            switch (disposition)
            {
                case ManagerDependencyDisposition.Missing:
                case ManagerDependencyDisposition.UpgradeCanonical:
                    RequestManagerPackage();
                    return;
                case ManagerDependencyDisposition.Exact:
                case ManagerDependencyDisposition.PreserveNewerCanonical:
                case ManagerDependencyDisposition.EmbeddedCompatible:
                    Debug.Log($"[Lava Rush Installer] Waiting for {ManagerPackageId} API registration.");
                    if (!SessionState.GetBool(ManagerResolveSessionKey, false))
                    {
                        SessionState.SetBool(ManagerResolveSessionKey, true);
                        Client.Resolve();
                    }
                    return;
                case ManagerDependencyDisposition.EmbeddedTooOld:
                    Debug.LogError($"[Lava Rush Installer] Embedded {ManagerPackageId} is older than required {ManagerVersion}; it was preserved. Upgrade the embedded package explicitly.");
                    return;
                default:
                    Debug.LogError($"[Lava Rush Installer] Existing {ManagerPackageId} is local, forked, branch-based, or unparseable; it was preserved. Install canonical {ManagerVersion} explicitly.");
                    return;
            }
        }

        private static void RequestManagerPackage()
        {
            if (_managerRequest != null && !_managerRequest.IsCompleted) return;
            if (SessionState.GetBool(ManagerRequestSessionKey, false)) return;
            SessionState.SetBool(ManagerRequestSessionKey, true);
            Debug.Log($"[Lava Rush Installer] Installing {ManagerPackageId}@{ManagerVersion}.");
            _managerRequest = Client.Add(ManagerGitUrl);
            EditorApplication.update -= PollManagerRequest;
            EditorApplication.update += PollManagerRequest;
        }

        private static void PollManagerRequest()
        {
            if (_managerRequest == null || !_managerRequest.IsCompleted) return;
            EditorApplication.update -= PollManagerRequest;
            AddRequest completed = _managerRequest;
            _managerRequest = null;

            if (completed.Status == StatusCode.Success)
            {
                Debug.Log($"[Lava Rush Installer] Installed {ManagerPackageId}; waiting for API registration.");
                return;
            }

            SessionState.SetBool(ManagerRequestSessionKey, false);
            string errorCode = completed.Error == null ? "unknown" : completed.Error.errorCode.ToString();
            Debug.LogError($"[Lava Rush Installer] Failed to install {ManagerPackageId} (UPM error {errorCode}). Fix package access and retry from the installer menu.");
        }

        private static Type FindManagerApiType()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("ActionFitContentBundleApi", false))
                .FirstOrDefault(type => type != null);
        }

        private static void ReportResult(object result)
        {
            if (result == null)
            {
                Debug.LogError("[Lava Rush Installer] Custom Package Manager returned no bundle result.");
                return;
            }

            Type type = result.GetType();
            bool success = ReadBoolField(type, result, "success");
            bool pending = ReadBoolField(type, result, "pending");
            string code = ReadStringField(type, result, "code");
            string message = ReadStringField(type, result, "message");
            string output = $"{code} - {message}";
            if (!success)
                Debug.LogError($"[Lava Rush Installer] {output}");
            else if (pending)
                Debug.Log($"[Lava Rush Installer] Pending: {output}");
            else
                Debug.Log($"[Lava Rush Installer] Completed: {output}");
        }

        private static bool ReadBoolField(Type type, object owner, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(owner) is bool value && value;
        }

        private static string ReadStringField(Type type, object owner, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(owner) as string ?? "";
        }

        private static bool IsDevelopmentEmbedded()
        {
            try
            {
                UnityEditor.PackageManager.PackageInfo package =
                    UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(LavaRushInstallerBootstrap).Assembly);
                return package != null && package.source == UnityEditor.PackageManager.PackageSource.Embedded;
            }
            catch
            {
                return File.Exists(Path.GetFullPath($"Packages/{InstallerPackageId}/package.json"));
            }
        }

        private static string ReadEmbeddedPackageVersion(string packageId)
        {
            string path = Path.GetFullPath($"Packages/{packageId}/package.json");
            if (!File.Exists(path)) return "";
            Match name = Regex.Match(File.ReadAllText(path), "\\\"name\\\"\\s*:\\s*\\\"(?<value>[^\\\"]+)\\\"");
            if (!name.Success || !string.Equals(name.Groups["value"].Value, packageId, StringComparison.Ordinal)) return "";
            Match version = Regex.Match(File.ReadAllText(path), "\\\"version\\\"\\s*:\\s*\\\"(?<value>[^\\\"]+)\\\"");
            return version.Success ? version.Groups["value"].Value : "";
        }

        private static string ReadManifestDependency(string packageId)
        {
            string path = Path.GetFullPath(ManifestRelativePath);
            if (!File.Exists(path))
            {
                Debug.LogError($"[Lava Rush Installer] Manifest not found: {ManifestRelativePath}");
                return "";
            }

            string manifest = File.ReadAllText(path);
            Match match = Regex.Match(
                manifest,
                $"\\\"{Regex.Escape(packageId)}\\\"\\s*:\\s*\\\"(?<value>(?:\\\\.|[^\\\"])*)\\\"");
            return match.Success ? match.Groups["value"].Value : "";
        }

        private static bool TrySplitGitUrl(string value, out string repository, out string revision)
        {
            repository = "";
            revision = "";
            int hash = (value ?? "").LastIndexOf('#');
            if (hash <= 0 || hash >= value.Length - 1) return false;
            repository = value.Substring(0, hash);
            revision = value.Substring(hash + 1);
            return true;
        }

        private static string NormalizeRepository(string repository)
        {
            string normalized = (repository ?? "").Trim().TrimEnd('/');
            return normalized.EndsWith(".git", StringComparison.OrdinalIgnoreCase)
                ? normalized.Substring(0, normalized.Length - 4)
                : normalized;
        }

        private static bool IsVersionTag(string revision)
        {
            return Regex.IsMatch(revision ?? "", "^[0-9]+(?:\\.[0-9]+){1,3}(?:[-+][A-Za-z0-9.-]+)?$");
        }

        private static int CompareVersions(string left, string right)
        {
            int[] leftParts = ParseVersion(left);
            int[] rightParts = ParseVersion(right);
            for (int index = 0; index < Math.Max(leftParts.Length, rightParts.Length); index++)
            {
                int leftValue = index < leftParts.Length ? leftParts[index] : 0;
                int rightValue = index < rightParts.Length ? rightParts[index] : 0;
                int comparison = leftValue.CompareTo(rightValue);
                if (comparison != 0) return comparison;
            }
            return 0;
        }

        private static int[] ParseVersion(string value)
        {
            return (value ?? "").Split('.')
                .Select(part => new string(part.TakeWhile(char.IsDigit).ToArray()))
                .Select(part => int.TryParse(part, out int number) ? number : 0)
                .ToArray();
        }
    }

    internal enum ManagerDependencyDisposition
    {
        Missing,
        Exact,
        UpgradeCanonical,
        PreserveNewerCanonical,
        Conflict,
        EmbeddedCompatible,
        EmbeddedTooOld,
    }
}
#endif
