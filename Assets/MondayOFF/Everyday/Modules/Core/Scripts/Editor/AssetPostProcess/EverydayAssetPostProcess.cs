using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using AppLovinMax.Scripts.IntegrationManager.Editor;

namespace MondayOFF
{
    public class EverydayAssetPostProcess : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths
#if UNITY_2021_2_OR_NEWER
        , bool didDomainReload
#endif
        )
        {
            if (AppLovinInternalSettings.Instance.ConsentFlowEnabled)
            {
                return;
            }

            AppLovinInternalSettings.Instance.ConsentFlowEnabled = true;
            AppLovinInternalSettings.Instance.ConsentFlowPrivacyPolicyUrl = "https://mondayoff.me/privacyPolicy.html";
            AppLovinInternalSettings.Instance.Save();
        }
    }
}