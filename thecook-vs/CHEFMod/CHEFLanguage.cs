using RoR2;
using System.Collections.Generic;
using Zio;
using Zio.FileSystems;

namespace ChefMod
{
    public static class CHEFLanguage
    {
        public static FileSystem FileSystem { get; private set; }

        public static void Initialize()
        {
            PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
            FileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Assets.AssemblyDir), true);

            if (FileSystem.DirectoryExists("/Languages/"))
            {
                Language.collectLanguageRootFolders += delegate (List<DirectoryEntry> list)
                {
                    ChefPlugin.logger.LogMessage($"Initializing Language");
                    list.Add(FileSystem.GetDirectoryEntry("/Languages/"));
                };
            }
        }
    }
}
