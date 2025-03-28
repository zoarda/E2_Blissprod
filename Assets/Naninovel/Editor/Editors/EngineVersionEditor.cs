using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    [CustomEditor(typeof(EngineVersion))]
    public class EngineVersionEditor : Editor
    {
        protected static string GitHubProjectPath => PlayerPrefs.GetString(nameof(GitHubProjectPath), string.Empty);

        private const string packageTextTemplate = @"{
    ""name"": ""com.elringus.naninovel"",
    ""version"": ""{PACKAGE_VERSION}"",
    ""displayName"": ""Naninovel"",
    ""description"": ""Writer-friendly visual novel engine.{FINAL_NOTICE}"",
    ""unity"": ""2019.4"",
    ""author"": {
        ""name"": ""Elringus"",
        ""url"": ""https://naninovel.com""
    },
    ""documentationUrl"": ""https://naninovel.com/guide"",
    ""changelogUrl"": ""https://github.com/naninovel/docs/releases/tag/v{GIT_VERSION}"",
    ""dependencies"": {
        ""com.unity.modules.audio"": ""1.0.0"",
        ""com.unity.modules.video"": ""1.0.0"",
        ""com.unity.modules.imgui"": ""1.0.0"",
        ""com.unity.modules.uielements"": ""1.0.0"",
        ""com.unity.modules.particlesystem"": ""1.0.0"",
        ""com.unity.modules.imageconversion"": ""1.0.0"",
        ""com.unity.ugui"": ""1.0.0"",
        ""com.unity.textmeshpro"": ""2.1.6""
    }
}
";

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update", GUIStyles.NavigationButton))
                Update();
        }

        public static void Update ()
        {
            var asset = EngineVersion.LoadFromResources();
            using (var serializedObj = new SerializedObject(asset))
            {
                serializedObj.Update();
                var engineVersionProperty = serializedObj.FindProperty("engineVersion");
                var finalProperty = serializedObj.FindProperty("final");
                var buildDateProperty = serializedObj.FindProperty("buildDate");
                buildDateProperty.stringValue = $"{DateTime.Now:yyyy-MM-dd}";
                serializedObj.ApplyModifiedProperties();

                var gitVersion = engineVersionProperty.stringValue;
                var packageVersion = $"{engineVersionProperty.stringValue}.{buildDateProperty.stringValue.Replace("-", "").Substring(2)}";
                var finalNotice = finalProperty.boolValue ? "\n\nWARNING: This version is outdated and not supported. Register your license via naninovel.com/register to get latest release." : "";
                var packageText = packageTextTemplate
                    .Replace("{GIT_VERSION}", gitVersion)
                    .Replace("{PACKAGE_VERSION}", packageVersion)
                    .Replace("{FINAL_NOTICE}", finalNotice);
                var packagePath = PathUtils.Combine(PackagePath.PackageRootPath, "package.json");
                File.WriteAllText(packagePath, packageText);

                EditorUtility.SetDirty(asset);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        private static DateTime ParseBuildDate (string buildDate)
        {
            var parsed = DateTime.TryParseExact(buildDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return parsed ? result : DateTime.MinValue;
        }
    }
}
