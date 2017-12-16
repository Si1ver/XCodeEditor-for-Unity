namespace XcodeProjectEditor
{
    public struct XcodeProjectMod
    {
        public string FilesPath;
        public string Group;
        public LibraryMod[] Libraries;
        public string[] Frameworks;
        public string[] Headers;
        public string[] Files;
        public string[] Folders;
        public string[] Excludes;
        public BuildSettingsMod BuildSettings;
    }
}
