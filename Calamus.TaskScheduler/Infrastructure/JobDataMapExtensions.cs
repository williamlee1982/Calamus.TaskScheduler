using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calamus.TaskScheduler.Infrastructure
{
    public static class JobDataMapExtensions
    {
        public static int GetTransferType(this JobDataMap map)
            => map.GetInt(DataKeys.TransferType);
        public static string GetSourceRootPath(this JobDataMap map)
            => map.GetString(DataKeys.SourceRootPath);
        public static string GetSourceFilePattern(this JobDataMap map)
            => map.GetString(DataKeys.SourceFilePattern);
        public static string GetDestinationRootPath(this JobDataMap map)
            => map.GetString(DataKeys.DestionationRootPath);
        public static string GetDestinationFilePattern(this JobDataMap map)
            => map.GetString(DataKeys.DestionationFilePattern);
        public static string GetUserName(this JobDataMap map)
            => map.GetString(DataKeys.UserName);
        public static string GetPassword(this JobDataMap map)
            => map.GetString(DataKeys.Password);
        public static bool GetDeleteOnCopied(this JobDataMap map)
            => map.GetBoolean(DataKeys.DeleteOnCopied);
        public static int GetTriggerType(this JobDataMap map)
            => map.GetInt(DataKeys.TriggerType);
        public static int GetRepeatCount(this JobDataMap map)
            => map.GetInt(DataKeys.RepeatCount);
        public static int GetInterval(this JobDataMap map)
            => map.GetInt(DataKeys.Interval);
        public static int GetIntervalType(this JobDataMap map)
            => map.GetInt(DataKeys.IntervalType);
        public static string GetCron(this JobDataMap map)
            => map.GetString(DataKeys.Cron) ?? string.Empty;
        public static string GetRequestBody(this JobDataMap map)
            => map.GetString(DataKeys.RequestBody) ?? string.Empty;
        public static DateTime GetCreateTime(this JobDataMap map)
            => map.GetDateTime(DataKeys.CreateTime);
        public static DateTime GetStartTime(this JobDataMap map)
            => map.GetDateTime(DataKeys.StartTime);
        public static DateTime? GetEndTime(this JobDataMap map)
            => string.IsNullOrWhiteSpace(map.GetString(DataKeys.EndTime)) ? null : map.GetDateTime(DataKeys.EndTime);
        public static string GetLastException(this JobDataMap map)
            => map.GetString(DataKeys.LastException) ?? string.Empty;
        public static List<string> GetLogList(this JobDataMap map)
            => (map[DataKeys.LogList] as List<string>) ?? new List<string>();
    }
}
