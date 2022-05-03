using System;

namespace BLD.SettingsPanelHUD.Common
{
    public static class CommonSettingsPanelEvents
    {
        public static event Action OnRefreshAllWidgetsSize;
        public static void RaiseRefreshAllWidgetsSize() { OnRefreshAllWidgetsSize?.Invoke(); }
    }
}