using UnityEngine;
using System.Collections.Generic;

namespace MondayOFF
{
    public static class EventTracker
    {
        private static List<IEventTracker> _eventTrackers = default;

        public static void TryStage(in int stageNum, in string stageName = "Stage")
        {
            foreach (var tracker in _eventTrackers)
            {
                tracker.TryStage(stageNum, stageName);
            }
        }

        public static void ClearStage(in int stageNum, in string stageName = "Stage")
        {
            foreach (var tracker in _eventTrackers)
            {
                tracker.ClearStage(stageNum, stageName);
            }
        }

        [System.Obsolete("Use LogEvent() instead")]
        public static void LogCustomEvent(in string eventName, in Dictionary<string, string> parameters = null)
        {
            foreach (var tracker in _eventTrackers)
            {
                tracker.LogCustomEvent(eventName, parameters);
            }
        }

        public static void LogEvent(in string eventName, in Dictionary<string, object> parameters = null)
        {
            foreach (var tracker in _eventTrackers)
            {
                tracker.LogEvent(eventName, parameters);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _eventTrackers = new();

#if UNITY_EDITOR
            return;
#endif
            // TODO: Flags for each tracker should be set in the settings

            // Make it mandatory?
            bool useSingularEventTracker = true;
            if (useSingularEventTracker)
            {
                _eventTrackers.Add(new SingularEventTracker());
            }

            bool useFirebaseEventTracker = true;
            if (useFirebaseEventTracker)
            {
#if FIREBASE_ENABLED
                _eventTrackers.Add(new FirebaseEventTracker());
#else
                EverydayLogger.Warn("Firebase Analytics is not enabled. Firebase Event Tracker will not be used");
#endif
            }

            bool useEverydayEventTracker = false;
            if (useEverydayEventTracker)
            {
                // TODO: Not implemented yet
                // _eventTrackers.Add(new EverydayEventTracker());
            }

            foreach (var tracker in _eventTrackers)
            {
                tracker.Initialize();
            }
        }
    }
}