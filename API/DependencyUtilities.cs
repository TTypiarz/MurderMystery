namespace MurderMystery.API
{
    internal static class DependencyUtilities
    {
        public static void HandleCedModV3(bool enable)
        {
            if (!DependencyChecker.CheckCedModV3())
                return;

            InternalHandleCedModV3(enable);
        }

        private static void InternalHandleCedModV3(bool enable)
        {
            if (enable)
            {
                CedMod.FriendlyFireAutoban.AdminDisabled = true;
            }
            else
            {
                CedMod.FriendlyFireAutoban.AdminDisabled = false;
            }
        }
    }
}
