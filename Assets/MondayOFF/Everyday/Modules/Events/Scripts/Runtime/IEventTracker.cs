using System.Collections.Generic;

namespace MondayOFF
{
    public interface IEventTracker
    {
        public bool IsInitialized { get; }
        public void Initialize();
        public void TryStage(int stageNum, string stageName = "Stage");
        public void ClearStage(int stageNum, string stageName = "Stage");
        public void LogCustomEvent(string eventName, Dictionary<string, string> parameters = null);
        public void LogEvent(string eventName, Dictionary<string, object> parameters = null);
    }
}