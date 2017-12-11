namespace XcodeProjectEditor
{
    public struct BuildSettingsMod
    {
        public static readonly BuildSettingsMod Empty = new BuildSettingsMod
        {
            OtherLinkerFlags = new string[0],
            GccEnableCppExceptions = string.Empty,
            GccEnableObjcExceptions = string.Empty,
        };

        public string[] OtherLinkerFlags;
        public string GccEnableCppExceptions;
        public string GccEnableObjcExceptions;
    }
}
