using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MondayOFF
{
    internal class SingularEventTracker : IEventTracker
    {
        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;

        public void TryStage(int stageNum, string stageName = "Stage")
        {
            if (!_isInitialized)
            {
                EverydayLogger.Info("Event Tracker is NOT initialized!");
                return;
            }
            EverydayLogger.Info($"[Singular Event Tracker] Trying {stageName} {stageNum}");
            SingularSDK.Event("Try", stageName, stageNum);
        }

        public void ClearStage(int stageNum, string stageName = "Stage")
        {
            // Send event regardless of initialization status
            switch (stageNum)
            {
                case 10:
                case 20:
                case 30:
                    SingularSDK.Event($"Stage{stageNum}");
                    break;
            }

            if (!_isInitialized)
            {
                EverydayLogger.Info("Event Tracker is NOT initialized!");
                return;
            }

            EverydayLogger.Info($"[Singular Event Tracker] Cleared {stageName} {stageNum}");
            SingularSDK.Event("Clear", stageName, stageNum);
        }

        // Stringify prameter values
        [System.Obsolete("Use LogEvent() instead")]
        public void LogCustomEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (!_isInitialized)
            {
                EverydayLogger.Info("Event Tracker is NOT initialized!");
                return;
            }

            if (parameters == null)
            {
                EverydayLogger.Info($"[Singular Event Tracker] {eventName} logged without any parameters");
            }
            else
            {
                string paramString = "\n";
                foreach (var item in parameters)
                {
                    paramString += $"{item.Key} : {item.Value}\n";
                }

                // Not recommended but ok..
                SingularSDK.Event(parameters.ToDictionary(pair => pair.Key, pair => (object)pair.Value), eventName);

                EverydayLogger.Info($"[Singular Event Tracker] {eventName} logged with parameters: {paramString}");
            }
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!_isInitialized)
            {
                EverydayLogger.Info("Event Tracker is NOT initialized!");
                return;
            }

            if (parameters == null)
            {
                EverydayLogger.Info($"[Singular Event Tracker] {eventName} logged without any parameters");
            }
            else
            {
                SingularSDK.Event(parameters, eventName);

                EverydayLogger.Info($"[Singular Event Tracker] {eventName} logged with parameters: {parameters.ToArray()}");
            }
        }

        public void Initialize()
        {
            if (!EveryDay.isInitialized)
            {
                EveryDay.OnEverydayInitialized += Initialize;
                return;
            }

            if (_isInitialized)
            {
                EverydayLogger.Info("Event Tracker is already initialized!");
                return;
            }
            EverydayLogger.Info("Initialize Event Tracker");
            _isInitialized = true;
#if UNITY_EDITOR
            Application.quitting -= OnEditorStop;
            Application.quitting += OnEditorStop;
#endif
        }

#if UNITY_EDITOR
        private void OnEditorStop()
        {
            EverydayLogger.Info("Stop Playmode Event Tracker");
            _isInitialized = false;
        }
#endif
    }
}