using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Calamus.TaskScheduler.Infrastructure
{
    public enum TransferTypeEnum
    {
        [Description("SharedFolder")]
        SharedFolder = 1,
        [Description("FTP")]
        FTP = 2
    }

    public enum TriggerTypeEnum
    {
        [Description("Simple")]
        Simple = 1,
        [Description("Cron")]
        Cron = 2
    }
    public enum IntervalTypeEnum
    {
        [Description("秒")]
        Second = 1,
        [Description("分")]
        Minute = 2,
        [Description("时")]
        Hour = 3,
        [Description("天")]
        Day = 4
    }
}
