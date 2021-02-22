using FluentValidation;
using Quartz;
using System;

namespace Calamus.TaskScheduler.Infrastructure.Dtos
{
    public class JobCreateOrUpdateRequest : RequestBase
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 文件传输方式
        /// </summary>
        public int TransferType { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 源文件根路径
        /// </summary>
        public string SourceRootPath { get; set; }
        /// <summary>
        /// 源文件抓取匹配模式(正则表达式)
        /// </summary>
        public string SourceFilePattern { get; set; }
        /// <summary>
        /// 目标文件根路径
        /// </summary>
        public string DestinationRootPath { get; set; }
        /// <summary>
        /// 目标文件名匹配模式(正则表达式)
        /// </summary>
        public string DestinationFilePattern { get; set; }
        /// <summary>
        /// 复制后是否删除
        /// </summary>
        public bool DeleteOnCopied { get; set; }
        /// <summary>
        /// 触发类型
        /// </summary>
        public int TriggerType { get; set; }
        /// <summary>
        /// 重复次数，0：无限
        /// </summary>
        public int RepeatCount { get; set; }
        /// <summary>
        /// 间隔时间
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 间隔类型
        /// </summary>
        public int IntervalType { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// true：更新，false：新增
        /// </summary>
        public bool IsUpdate { get; set; }
    }

    public class JobCreateOrUpdateValidator : AbstractValidator<JobCreateOrUpdateRequest>
    {
        public JobCreateOrUpdateValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Group).NotEmpty();
            RuleFor(model => model.TransferType).Must(x => TransferTypeEnum.SharedFolder.ToValueList().Contains(x));
            //RuleFor(model => model.SourceRootPath).Matches(@"^((http|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?$").WithMessage("请输入正确的请求地址");
            RuleFor(model => model.StartTime).NotNull();
            RuleFor(model => model.TriggerType).Must(x => TriggerTypeEnum.Simple.ToValueList().Contains(x));
            When(model => model.TriggerType == (int)TriggerTypeEnum.Simple, () =>
            {
                RuleFor(model => model.IntervalType).Must(x => IntervalTypeEnum.Second.ToValueList().Contains(x));
                RuleFor(model => model.Interval).GreaterThan(0);
            });
            When(model => model.TriggerType == (int)TriggerTypeEnum.Cron, () => RuleFor(model => model.Cron)
                                                                                .NotEmpty()
                                                                                .Must(x => CronExpression.IsValidExpression(x)).WithMessage("不正确的Cron表达式"));
            When(model => model.EndTime.HasValue, () => RuleFor(model => model.EndTime).GreaterThan(DateTime.Now).WithMessage("结束时间必须大于当前时间"));
        }
    }
}
