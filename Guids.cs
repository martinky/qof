// Guids.cs
// MUST match guids.h
using System;

namespace QuickOpenFile
{
    static class GuidList
    {
        public const string guidQuickOpenFileVS2010PkgString = "62fe6edb-f401-497e-a147-3158b7d02467";
        public const string guidQuickOpenFileVS2010CmdSetString = "66d22e6e-c8ab-454d-ad5f-c0985e6fd7fd";
        public const string guidToolWindowPersistanceString = "ea72eed9-3d87-4b35-a79a-eedec76be2a4";

        public static readonly Guid guidQuickOpenFileVS2010CmdSet = new Guid(guidQuickOpenFileVS2010CmdSetString);
    };
}