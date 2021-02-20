using Microsoft.Extensions.Logging;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calamus.TaskScheduler.Infrastructure
{
    [PersistJobDataAfterExecution]  // 执行完，更新JobData
    [DisallowConcurrentExecution]   // 单个Job不允许并发 执行
    public class SharedFolderJob : IJob
    {
        private readonly ILogger<SharedFolderJob> _log;

        public SharedFolderJob(ILogger<SharedFolderJob> log)
        {
            _log = log;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var transferType = context.JobDetail.JobDataMap.GetTransferType();
            var srcRootPath = context.JobDetail.JobDataMap.GetSourceRootPath();
            var srcFilePatthern = context.JobDetail.JobDataMap.GetSourceFilePattern();
            var destRootPath = context.JobDetail.JobDataMap.GetDestinationRootPath();
            var destFilePattern = context.JobDetail.JobDataMap.GetDestinationFilePattern();
            var userName = context.JobDetail.JobDataMap.GetUserName();
            var password = context.JobDetail.JobDataMap.GetPassword();
            var deleteOnCopied = context.JobDetail.JobDataMap.GetDeleteOnCopied();

            var jobName = $"{context.JobDetail.Key.Group}-{context.JobDetail.Key.Name}";

            if (transferType == (int)TransferTypeEnum.SharedFolder)
            {
                var credential = new System.Net.NetworkCredential
                {
                    UserName = userName,
                    Password = password
                };

                try
                {
                    using (var ns = new NetworkConnection(srcRootPath, credential))
                    {
                        var fileList = new List<string>();
                        await Task.Run(() =>
                        {
                            try
                            {
                                LogInformation(jobName, $"Start getting source path {srcRootPath} info...");

                                GetFiles(srcRootPath, "*.*", ref fileList);

                                LogInformation(jobName, $"Complete getting source path {srcRootPath} info.");
                            }
                            catch (Exception ex)
                            {
                                LogError(jobName, $"Error while getting source path {srcRootPath} info.");
                                LogError(jobName, ex, ex.Message);
                            }
                        });

                        //^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)
                        var regex = new Regex(srcFilePatthern);
                        foreach (var file in fileList)
                        {
                            var matchResult = regex.Match(file);
                            if (matchResult.Success)
                            {
                                await Task.Run(() =>
                                {
                                    var destFilePath = $"{destRootPath}\\{regex.Replace(file, destFilePattern)}";

                                    try
                                    {
                                        var dir = Path.GetDirectoryName(destFilePath);
                                        Directory.CreateDirectory(dir);

                                        LogInformation(jobName, $"Start copying {file} to {destFilePath}...");

                                        File.Copy(file, destFilePath, true);

                                        LogInformation(jobName, $"Complete copying {file} to {destFilePath}.");

                                        if (deleteOnCopied)
                                        {
                                            LogInformation(jobName, $"Start deleting {file}...");

                                            File.Delete(file);

                                            LogInformation(jobName, $"Complete deleting {file}.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError(jobName, $"Error while copying {file} to {destFilePath}.");
                                        LogError(jobName, ex, ex.Message);
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(jobName, ex, ex.Message);
                }
            }

            List<string> list = context.JobDetail.JobDataMap.GetLogList();
            while (list.Count >= 20)
            {
                list.RemoveAt(list.Count - 1);// 最大保存日志数量 20 条
            }

            string log = $"执行时间：{context.FireTimeUtc.LocalDateTime} - {DateTime.Now}，耗时：{context.JobRunTime.TotalMilliseconds}ms";
            list.Insert(0, log);
            context.JobDetail.JobDataMap[DataKeys.LogList] = list;
        }

        /// <summary>
        /// 获取文件夹下所有文件
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <param name="pattern">文件类型</param>
        /// <param name="list">集合</param>
        void GetFiles(string directory, string pattern, ref List<string> list)
        {
            var directoryInfo = new DirectoryInfo(directory);
            foreach (var info in directoryInfo.GetFiles(pattern))
            {
                list.Add(info.FullName);
            }
            foreach (var info in directoryInfo.GetDirectories())
            {
                GetFiles(info.FullName, pattern, ref list);
            }
        }

        /// <summary>
        /// 截取字符串长度
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        string CutString(string source, int length)
        {
            if (source.Length <= length) return source;
            return source.Substring(0, length);
        }

        void LogInformation(string jobName, string message)
        {
            _log.Log(Microsoft.Extensions.Logging.LogLevel.Information,
             default(EventId),
             new MyLogEvent(message).WithProperty("JobName", jobName),
             (Exception)null,
             MyLogEvent.Formatter);
        }

        void LogError(string jobName, string message)
        {
            _log.Log(Microsoft.Extensions.Logging.LogLevel.Error,
             default(EventId),
             new MyLogEvent(message).WithProperty("JobName", jobName),
             (Exception)null,
             MyLogEvent.Formatter);
        }

        void LogError(string jobName, Exception ex, string message)
        {
            _log.Log(Microsoft.Extensions.Logging.LogLevel.Error,
             default(EventId),
             new MyLogEvent(message).WithProperty("JobName", jobName),
             ex,
             MyLogEvent.Formatter);
        }
    }
}
