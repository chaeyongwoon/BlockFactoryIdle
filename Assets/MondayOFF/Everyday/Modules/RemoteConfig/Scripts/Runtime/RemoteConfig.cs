using System;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using UnityEngine;

namespace MondayOFF
{
    public static partial class RemoteConfig
    {
        private enum FetchStatus
        {
            NotStarted,
            Pending,
            Success,
            Failure,
        }

        public static Action OnFetchSuccess = default;
        public static bool IsFetched => _fetchStatus == FetchStatus.Success;

        private static FetchStatus _fetchStatus = FetchStatus.NotStarted;

        public static bool GetBool(in string key)
        {
            if (!IsFetched)
            {
                return GetDefaultValue<bool>(in key);
            }
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            return value.BooleanValue;
        }

        public static int GetInt(in string key)
        {
            if (!IsFetched)
            {
                return GetDefaultValue<int>(in key);
            }
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            return (int)value.LongValue;
        }

        public static float GetFloat(in string key)
        {
            if (!IsFetched)
            {
                return GetDefaultValue<float>(in key);
            }
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            return (float)value.DoubleValue;
        }

        public static string GetString(in string key)
        {
            if (!IsFetched)
            {
                return GetDefaultValue<string>(in key);
            }
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            return value.StringValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void InitializeAsync()
        {
            _fetchStatus = FetchStatus.NotStarted;


            await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Debug.Log("RemoteConfig: Firebase is ready to use");

                }
            });


            // DefaultValues["MutationRateHigh"] = PlayerPrefs.GetInt("MutationRateHigh", 0) == 1;
            _ = FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(DefaultValues).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("RemoteConfig: SetDefaultsAsync completed");
                }
                else
                {
                    Debug.Log("RemoteConfig: SetDefaultsAsync failed");
                }
            });


            _ = FetchDataAsync();
        }

        private static Task FetchDataAsync()
        {
            switch (_fetchStatus)
            {
                case FetchStatus.Pending:
                    Debug.Log("RemoteConfig: FetchDataAsync is already pending");
                    return Task.CompletedTask;
                case FetchStatus.Success:
                    Debug.Log("RemoteConfig: FetchDataAsync is already success");
                    return Task.CompletedTask;
                case FetchStatus.Failure:
                    Debug.Log("RemoteConfig: FetchDataAsync is already failure");
                    return Task.CompletedTask;
            }

            _fetchStatus = FetchStatus.Pending;

            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12));
            // Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        private static void FetchComplete(Task fetchTask)
        {
            if (!fetchTask.IsCompleted)
            {
                Debug.LogWarning("Retrieval hasn't finished.");
                return;
            }

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogWarning($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                return;
            }

            // Fetch successful. Parameter values must be activated to use.
            remoteConfig.ActivateAsync()
              .ContinueWithOnMainThread(task =>
              {
                  Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
                  _fetchStatus = FetchStatus.Success;
                  OnFetchSuccess?.Invoke();
              });
        }

        private static T GetDefaultValue<T>(in string key)
        {
            Debug.LogWarning("RemoteConfig is not fetched yet. Trying to get default value for ${key}");
            if (DefaultValues.TryGetValue(key, out var defaultValue))
            {
                if (defaultValue is T castedValue)
                {
                    return castedValue;
                }
                else
                {
                    Debug.LogError($"You are trying to get a value for ${key} which is not {typeof(T).Name}!");
                    return default;
                }
            }

            Debug.LogError($"You are trying to get a value for ${key} which is not in the default values!");
            return default;
        }
    }
}