using GameCreator.Editor.Installs;
using UnityEditor;

namespace GameCreator.Editor.Localization
{
    public static class UninstallReminstance
    {
        [MenuItem(
            itemName: "Game Creator/Uninstall/Reminstance",
            isValidateFunction: false,
            priority: UninstallManager.PRIORITY
        )]
        
        private static void Uninstall()
        {
            UninstallManager.Uninstall("Reminstance");
        }
    }
}