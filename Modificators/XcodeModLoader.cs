namespace XcodeProjectEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using MiniJSON;

    public static class XcodeModLoader
    {
        private const string GroupFieldName = "group";
        private const string LibrariesFieldName = "libs";
        private const string FrameworksFieldName = "frameworks";
        private const string HeadersFieldName = "headerpaths";
        private const string FilesFieldName = "files";
        private const string FoldersFieldName = "folders";
        private const string ExcludesFieldName = "excludes";
        private const string BuildSettingsFieldName = "buildSettings";

        private const string OtherLinkerFlagsFieldName = "OTHER_LDFLAGS";
        private const string GccEnableCppExceptionsFieldName = "GCC_ENABLE_CPP_EXCEPTIONS";
        private const string GccEnableObjcExceptionsFieldName = "GCC_ENABLE_OBJC_EXCEPTIONS";

        private const char LibraryLinkTypeDelimiter = ':';

        private const string LibraryWeakLinkType = "weak";

        public static XcodeProjectMod LoadFromFile(string fileName)
        {
            FileInfo modFileInfo = new FileInfo(fileName);

            if (!modFileInfo.Exists)
            {
                throw new FileNotFoundException("Mod file is not found.", fileName);
            }

            string filesPath = modFileInfo.DirectoryName;

            string jsonContent;
            using (StreamReader streamReader = modFileInfo.OpenText())
            {
                jsonContent = streamReader.ReadToEnd();
            }

            return LoadFromJson(filesPath, jsonContent);
        }

        public static XcodeProjectMod LoadFromJson(string filesPath, string jsonContent)
        {
            var data = (Dictionary<string, object>)Json.Deserialize(jsonContent);

            return new XcodeProjectMod()
            {
                FilesPath = filesPath,
                Group = LoadElementOrDefault(data, GroupFieldName, string.Empty),
                Libraries = LoadLibraries(data, LibrariesFieldName),
                Frameworks = LoadArrayOrEmpty<string>(data, FrameworksFieldName),
                Headers = LoadArrayOrEmpty<string>(data, HeadersFieldName),
                Files = LoadArrayOrEmpty<string>(data, FilesFieldName),
                Folders = LoadArrayOrEmpty<string>(data, FoldersFieldName),
                Excludes = LoadArrayOrEmpty<string>(data, ExcludesFieldName),
                BuildSettings = LoadBuildSettings(data, BuildSettingsFieldName),
            };
        }

        private static T[] LoadArray<T>(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            List<object> list = (List<object>)data;

            T[] result = new T[list.Count];

            int listItemsCount = list.Count;
            for (int i = 0; i < listItemsCount; ++i)
            {
                result[i] = (T)list[i];
            }

            return result;
        }

        private static T LoadElementOrDefault<T>(
            Dictionary<string, object> data, string key, T defaultValue)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            object element;
            if (!data.TryGetValue(key, out element))
            {
                return defaultValue;
            }

            return (T)element;
        }

        private static T[] LoadArrayOrEmpty<T>(Dictionary<string, object> data, string key)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            object element;
            if (!data.TryGetValue(key, out element))
            {
                return new T[0];
            }

            return LoadArray<T>(element);
        }

        private static LibraryMod[] LoadLibraries(Dictionary<string, object> data, string key)
        {
            string[] libsData = LoadArrayOrEmpty<string>(data, key);
            int libsLength = libsData.Length;

            LibraryMod[] libraries = new LibraryMod[libsLength];
            for (int i = 0; i < libsLength; ++i)
            {
                libraries[i] = ParseLibraryMod(libsData[i]);
            }

            return libraries;
        }

        private static BuildSettingsMod LoadBuildSettings(
            Dictionary<string, object> data, string key)
        {
            object buildSettingsObject;
            if (!data.TryGetValue(key, out buildSettingsObject))
            {
                return BuildSettingsMod.Empty;
            }

            Dictionary<string, object> buildSettingsData =
                (Dictionary<string, object>)buildSettingsObject;

            return new BuildSettingsMod
            {
                OtherLinkerFlags = LoadArrayOrEmpty<string>(buildSettingsData, OtherLinkerFlagsFieldName),

                GccEnableCppExceptions = LoadElementOrDefault(
                    buildSettingsData,
                    GccEnableCppExceptionsFieldName,
                    BuildSettingsMod.Empty.GccEnableCppExceptions),

                GccEnableObjcExceptions = LoadElementOrDefault(
                    buildSettingsData,
                    GccEnableObjcExceptionsFieldName,
                    BuildSettingsMod.Empty.GccEnableObjcExceptions),
            };
        }

        private static LibraryMod ParseLibraryMod(string data)
        {
            bool isWeak = false;
            string filePath;

            int delimiterPosition = data.LastIndexOf(LibraryLinkTypeDelimiter);

            if (delimiterPosition == -1)
            {
                filePath = data;
            }
            else
            {
                filePath = data.Substring(0, delimiterPosition);

                string linkType = data.Substring(delimiterPosition + 1);
                isWeak = linkType.Equals(LibraryWeakLinkType, StringComparison.OrdinalIgnoreCase);
            }

            return new LibraryMod
            {
                FilePath = filePath,
                IsWeak = isWeak,
            };
        }
    }
}
