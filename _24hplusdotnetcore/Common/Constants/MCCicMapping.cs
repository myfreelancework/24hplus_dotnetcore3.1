using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _24hplusdotnetcore.Common.Constants
{
    public static class MCCicMapping
    {
        public const string CIC_PREFIX = "CIC_{0}";
        public const string DEFAULT = "DEFAULT";
        public static readonly string[] APPROVE_CIC_RESULT_LIST = { "1", "4", "5", "6" };

        /// <summary>
        /// ("{ 'CIC_"; A2;"', '"; E2;"' },")
        /// </summary>
        public static readonly ReadOnlyDictionary<string, string> MC_CHECK_CIC_MESSAGE_MAPPING =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
                { "DEFAULT", "-" },
                { "CIC_1", "Đang có dư nợ, không có nợ xấu hay nợ cần chú ý" },
                { "CIC_2", "Đang có dư nợ, đang có nợ cần chú ý" },
                { "CIC_3", "Đang có dư nợ, đang có nợ xấu" },
                { "CIC_4", "Có thông tin nhưng không có dự nợ" },
                { "CIC_5", "Không có thông tin" },
                { "CIC_6", "CIC lỗi" }
            });
    }
}
