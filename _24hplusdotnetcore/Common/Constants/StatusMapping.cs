﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _24hplusdotnetcore.Common.Constants
{
    public static class StatusMapping
    {
        public const string STATUS_PREFIX = "STATUS_{0}";
        public const string DETAIL_STATUS_PREFIX = "STATUS_{0}_DETAIL_{1}";
        public const string DEFAULT = "DEFAULT";

        /// <summary>
        /// ("{ 'STATUS_"; A2;"', '"; B2;"' },")
        /// </summary>
        public static readonly ReadOnlyDictionary<string, string> MA_STATUS_MESSAGE_MAPPING =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
            {
                { "DEFAULT", "UNDEFINED" },
                { "STATUS_1", "NEW LEAD" },
                { "STATUS_2", "NOT ALLOCATED" },
                { "STATUS_3", "ALLOCATED TO DC" },
                { "STATUS_4", "INVALID ROUTE" },
                { "STATUS_5", "SCHEDULING" },
                { "STATUS_6", "CONFIRM APPOINTMENT" },
                { "STATUS_7", "FOLLOWING" },
                { "STATUS_8", "CONSIDERING" },
                { "STATUS_9", "SUBMITTED AT RO/SIP" },
                { "STATUS_10", "QDE-DOCUMENT" },
                { "STATUS_11", "QDE-APPLICATION" },
                { "STATUS_12", "FULL" },
                { "STATUS_13", "QDE-CANT PROVIDE DOCUMENTS" },
                { "STATUS_14", "CANCELLED" },
                { "STATUS_15", "IN PROGRESS SALES ADMIN" },
                { "STATUS_16", "FINISH SALES ADMIN" },
                { "STATUS_17", "DEFER SALES ADMIN" },
                { "STATUS_18", "IN PROGRESS DE" },
                { "STATUS_19", "DEFER SGB" },
                { "STATUS_20", "DEFER UND" },
                { "STATUS_21", "FINISH DE" },
                { "STATUS_22", "BDE" },
                { "STATUS_23", "PDOC" },
                { "STATUS_24", "QDE" },
                { "STATUS_25", "DOV" },
                { "STATUS_26", "POR" },
                { "STATUS_27", "POL" },
                { "STATUS_28", "UND" },
                { "STATUS_29", "CBC" },
                { "STATUS_30", "DUR" },
                { "STATUS_31", "FII" },
                { "STATUS_32", "DII" },
                { "STATUS_33", "DISBDTL" },
                { "STATUS_34", "DUP" },
                { "STATUS_35", "SCR" },
                { "STATUS_36", "REJ" },
                { "STATUS_37", "FINISH" },
                { "STATUS_38", "CAN" },
                { "STATUS_39", "SRR" },
            });

        /// <summary>
        /// ("{ 'STATUS_"; A2;"_DETAIL_";B2;"', '"; C2;"' },")
        /// </summary>
        public static readonly ReadOnlyDictionary<string, string> MA_DETAIL_STATUS_MESSAGE_MAPPING =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
            {
                { "DEFAULT", "UNDEFINED" },
                { "STATUS_1_DETAIL_1", "Hồ sơ khách hàng mới vừa nhận" },
                { "STATUS_2_DETAIL_2", "Hồ sơ chưa được phân bổ" },
                { "STATUS_3_DETAIL_3", "Hồ sơ đã phân bổ cho DC" },
                { "STATUS_4_DETAIL_4", "Sai tuyến ( DC note lại địa điểm nhận hồ sơ)" },
                { "STATUS_5_DETAIL_5", "Thuê bao" },
                { "STATUS_6_DETAIL_5", "Không nghe máy" },
                { "STATUS_7_DETAIL_5", "Khách hàng chưa sắp xếp được lịch hẹn" },
                { "STATUS_8_DETAIL_5", "DC chưa sắp xếp được lịch hẹn" },
                { "STATUS_9_DETAIL_5", "Khách hàng chưa chuẩn bị xong hồ sơ" },
                { "STATUS_10_DETAIL_5", "Lý do khác" },
                { "STATUS_11_DETAIL_6", "Đã đặt lịch hẹn với khách hàng 1 ngày tới" },
                { "STATUS_12_DETAIL_6", "Đã đặt lịch hẹn với khách hàng 2 ngày tới" },
                { "STATUS_13_DETAIL_6", "Đã đặt lịch hẹn với khách hàng 3 ngày tới" },
                { "STATUS_14_DETAIL_7", "DC hẹn gọi lại để chốt lịch sau do hẹn quá 3 ngày" },
                { "STATUS_15_DETAIL_7", "DC theo dõi lý do khác (Nghỉ việc)" },
                { "STATUS_16_DETAIL_8", "Khách hàng đang cân nhắc về khoản vay/ cần tư vấn lại" },
                { "STATUS_17_DETAIL_9", "Khách hàng đã nộp hồ sơ tại RO/SIP" },
                { "STATUS_18_DETAIL_10", "Hồ sơ thiếu CMND" },
                { "STATUS_19_DETAIL_10", "Hồ sơ thiếu Hộ khẩu" },
                { "STATUS_20_DETAIL_10", "Hồ sơ thiếu Chứng từ công việc" },
                { "STATUS_21_DETAIL_10", "Hồ sơ thiếu Chứng từ thu nhập" },
                { "STATUS_22_DETAIL_10", "Hồ sơ thiếu Chứng từ cư trú" },
                { "STATUS_23_DETAIL_10", "Hồ sơ thiếu GP DKKD" },
                { "STATUS_24_DETAIL_10", "Hồ sơ thiếu Chứng từ thuế" },
                { "STATUS_25_DETAIL_10", "Hồ sơ thiếu Hóa đơn điện" },
                { "STATUS_26_DETAIL_10", "Hồ sơ thiếu HD BHNT" },
                { "STATUS_27_DETAIL_10", "Hồ sơ thiếu hóa đơn bảo hiểm NT" },
                { "STATUS_28_DETAIL_10", "Hồ sơ thiếu Chứng từ khác" },
                { "STATUS_29_DETAIL_11", "Không cung cấp được thông tin địa chỉ" },
                { "STATUS_30_DETAIL_11", "Không cung cấp được thông tin công việc" },
                { "STATUS_31_DETAIL_11", "Không cung cấp được thông tin thu nhập" },
                { "STATUS_32_DETAIL_11", "Không cung cấp được thông tin người hôn phối" },
                { "STATUS_33_DETAIL_11", "Không cung cấp được số điện thoại tham chiếu" },
                { "STATUS_34_DETAIL_11", "Không cung cấp được thông tin khác" },
                { "STATUS_35_DETAIL_12", "Hồ sơ đầy đủ Upload send to BPO" },
                { "STATUS_36_DETAIL_12", "DC submit app to SIP/RO" },
                { "STATUS_37_DETAIL_13", "Khách hàng không cung cấp được CMND" },
                { "STATUS_38_DETAIL_13", "Khách hàng không cung cấp được Hộ khẩu" },
                { "STATUS_39_DETAIL_13", "Khách hàng không cung cấp được Chứng từ công việc" },
                { "STATUS_40_DETAIL_13", "Khách hàng không cung cấp được Chứng từ thu nhập" },
                { "STATUS_41_DETAIL_13", "Khách hàng không cung cấp được Chứng từ cư trú" },
                { "STATUS_42_DETAIL_13", "Khách hàng không cung cấp được GP DKKD" },
                { "STATUS_43_DETAIL_13", "Khách hàng không cung cấp được Chứng từ thuế" },
                { "STATUS_44_DETAIL_13", "Khách hàng không cung cấp được Hóa đơn Điện" },
                { "STATUS_45_DETAIL_13", "Khách hàng không cung cấp được HD BHNT" },
                { "STATUS_46_DETAIL_13", "Khách hàng không cung cấp được Hóa đơn bảo hiểm NT" },
                { "STATUS_47_DETAIL_13", "Khách hàng không cung cấp được Chứng từ khác" },
                { "STATUS_48_DETAIL_14", "Khách hàng hủy không vay nữa (sau khi Sale đã thuyết phục)" },
                { "STATUS_49_DETAIL_14", "Lead quá thời hạn xử lý (30 ngày)" },
                { "STATUS_50_DETAIL_14", "Lý do khác" },
                { "STATUS_51_DETAIL_15", "Hồ sơ đã chuyển Sales Admin và đang trong quá trình xử lý" },
                { "STATUS_52_DETAIL_16", "Sales Admin hoàn thành xử lý" },
                { "STATUS_53_DETAIL_18", "Hồ sơ đã chuyển sang SGB nhập liệu và đang trong quá trình xử lý" },
                { "STATUS_54_DETAIL_19", "Hồ sơ bị trả về từ SGB (chưa lên AppID)" },
                { "STATUS_55_DETAIL_20", "Hồ sơ bị trả về từ UND (đã lên AppID)" },
                { "STATUS_56_DETAIL_21", "Hồ sơ đã hoàn thành nhập liệu bởi SGB (bao gồm hoàn thành xử lý hồ sơ mới và hồ sơ Defer)" },
                { "STATUS_57_DETAIL_22", "F1 status" },
                { "STATUS_58_DETAIL_23", "F1 status" },
                { "STATUS_59_DETAIL_24", "F1 status" },
                { "STATUS_60_DETAIL_25", "F1 status" },
                { "STATUS_61_DETAIL_26", "F1 status" },
                { "STATUS_62_DETAIL_27", "F1 status" },
                { "STATUS_63_DETAIL_28", "F1 status" },
                { "STATUS_64_DETAIL_29", "F1 status" },
                { "STATUS_65_DETAIL_30", "F1 status" },
                { "STATUS_66_DETAIL_31", "F1 status" },
                { "STATUS_67_DETAIL_32", "F1 status" },
                { "STATUS_68_DETAIL_33", "F1 status" },
                { "STATUS_69_DETAIL_34", "F1 status" },
                { "STATUS_70_DETAIL_35", "F1 status" },
                { "STATUS_71_DETAIL_36", "F1 status" },
                { "STATUS_72_DETAIL_37", "F1 status" },
                { "STATUS_73_DETAIL_38", "F1 status" },
                { "STATUS_74_DETAIL_39", "F1 status" },
                { "STATUS_75_DETAIL_17", "1.1 - Mờ không rõ thông tin" },
                { "STATUS_76_DETAIL_17", "1.2 - App thiếu/sai thông tin khách hàng vay" },
                { "STATUS_77_DETAIL_17", "1.3 - App thiếu/sai thông tin cư trú và thời gian cư trú" },
                { "STATUS_78_DETAIL_17", "1.4 - App thiếu/sai thông tin khoản vay" },
                { "STATUS_79_DETAIL_17", "1.5 - App thiếu/sai thông tin nghề nghiệp" },
                { "STATUS_80_DETAIL_17", "1.6 - App thiếu/sai thông tin thu nhập" },
                { "STATUS_81_DETAIL_17", "1.7 - App thiếu/sai thông tin vợ/chồng" },
                { "STATUS_82_DETAIL_17", "1.8 - App thiếu/sai thông tin, rule tham chiếu" },
                { "STATUS_83_DETAIL_17", "1.9 - App thiếu chữ ký khách hàng" },
                { "STATUS_84_DETAIL_17", "1.10 - Thiếu/sai code sale, code nhân viên tư vấn" },
                { "STATUS_85_DETAIL_17", "2.1 - CMND/Thẻ CCCD mờ do chất lượng ảnh Sales chụp" },
                { "STATUS_86_DETAIL_17", "2.2 - CMND/Thẻ CCCD mờ do bản thân CMND bị mờ" },
                { "STATUS_87_DETAIL_17", "2.3 - CMND/Thẻ CCCD hết hạn" },
                { "STATUS_88_DETAIL_17", "2.4 - Không cung cấp CMND cũ/chứng từ liên quan khi nộp CCCD mới" },
                { "STATUS_89_DETAIL_17", "2.5 - Không trùng khớp trên các giấy tờ khác nhau" },
                { "STATUS_90_DETAIL_17", "2.6 - Scan thiếu/mất góc" },
                { "STATUS_91_DETAIL_17", "3.1 - Hộ khẩu mờ do chất lượng ảnh Sales chụp" },
                { "STATUS_92_DETAIL_17", "3.2 - Hộ khẩu công chứng hết hạn (6 tháng)" },
                { "STATUS_93_DETAIL_17", "3.3 - Hộ khẩu thiếu trang/không hợp lệ" },
                { "STATUS_94_DETAIL_17", "3.4 - Khác" },
                { "STATUS_95_DETAIL_17", "4.1 - Thiếu KT3/XN tạm trú/Thông báo tạm trú/Thẻ tạm trú" },
                { "STATUS_96_DETAIL_17", "4.2 - KT3/XN tạm trú/TB tạm trú mờ do chất lượng ảnh Sales chụp" },
                { "STATUS_97_DETAIL_17", "4.3 - KT3 công chứng hết hạn (6 tháng)" },
                { "STATUS_98_DETAIL_17", "4.4 - KT3 thiếu trang" },
                { "STATUS_99_DETAIL_17", "4.5 - KT3/XN tạm trú hết hạn" },
                { "STATUS_100_DETAIL_17", "5.1 - Thiếu BHYT" },
                { "STATUS_101_DETAIL_17", "5.2 - Mã không hợp lệ" },
                { "STATUS_102_DETAIL_17", "5.3 - BHYT Mờ" },
                { "STATUS_103_DETAIL_17", "5.4 - BHYT Hết hạn" },
                { "STATUS_104_DETAIL_17", "6.1 - Thiếu Hợp đồng lao động/Phụ lục hợp đồng/Xác nhận công tác" },
                { "STATUS_105_DETAIL_17", "6.2 - Hợp đồng lao động/Phụ lục hợp đồng/Xác nhận công tác mờ do chất lượng Sales chụp" },
                { "STATUS_106_DETAIL_17", "6.3 - Hợp đồng lao động/Xác nhận công tác hết hạn" },
                { "STATUS_107_DETAIL_17", "6.4 - Khác" },
                { "STATUS_108_DETAIL_17", "7.1 - Sao kê lương/Phiếu lương/Xác nhận lương mờ do Sales chụp" },
                { "STATUS_109_DETAIL_17", "7.2 - Sao kê lương/Phiếu lương/Xác nhận lương không đủ 3 tháng lương gần nhất" },
                { "STATUS_110_DETAIL_17", "7.3 - Sao kê lương/Phiếu lương/Xác nhận lương không có mộc ngân hàng/công ty" },
                { "STATUS_111_DETAIL_17", "7.4 - Khác" },
                { "STATUS_112_DETAIL_17", "8.1 - Giấy phép ĐKKD mờ do chất lượng ảnh Sales chụp" },
                { "STATUS_113_DETAIL_17", "8.2 - Giấy phép ĐKKD chưa đủ 6 tháng" },
                { "STATUS_114_DETAIL_17", "9.1 - Thiếu Hóa đơn/Biên nhận dịch vụ hoặc Thông báo có dấu \"Đã thu tiền\"" },
                { "STATUS_115_DETAIL_17", "9.2 - Hóa đơn mờ do chất lượng ảnh Sales chụp" },
                { "STATUS_116_DETAIL_17", "9.3 - Hóa đơn hết hạn (3 tháng)" },
                { "STATUS_117_DETAIL_17", "9.4 - Khách nộp thông báo tiền điện KHÔNG có mộc \"Đã thu tiền\"" },
                { "STATUS_118_DETAIL_17", "9.5 – Khác" },
                { "STATUS_119_DETAIL_17", "10.1 - Thiếu Hợp đồng/Lịch thanh toán,Hóa đơn/ Biên nhận thanh toán khoản vay hiện tại" },
                { "STATUS_120_DETAIL_17", "10.2 - Hợp đồng/Lịch thanh toán,Hóa đơn/ Biên nhận thanh toán khoản vay hiện tại không hợp lệ" },
                { "STATUS_121_DETAIL_17", "11.1 - Không view được file" },
                { "STATUS_122_DETAIL_17", "11.2 - Hồ sơ thiếu file DN" },
                { "STATUS_123_DETAIL_17", "11.3 - Đặt sai cấu trúc folder" },
                { "STATUS_124_DETAIL_17", "11.4 - Up trùng hồ sơ" },
                { "STATUS_125_DETAIL_17", "12.1 - Sai từ 2 code trở lên" },
                { "STATUS_126_DETAIL_17", "12.2 - Khác" },
                { "STATUS_127_DETAIL_14", "Quá số lần defer" },
                { "STATUS_128_DETAIL_14", "Quá số lần defer - Sales Admin" },
                { "STATUS_129_DETAIL_14", "Quá số lần defer - SGB" },
                { "STATUS_130_DETAIL_14", "Hết hạn bổ sung defer Sales Admin" },
                { "STATUS_131_DETAIL_14", "Hết hạn bổ sung defer SGB" },
                { "STATUS_132_DETAIL_14", "Hết hạn bổ sung defer UND" },
            });

        /// <summary>
        /// ("{ 'STATUS_"; A2;"', '"; E2;"' },")
        /// </summary>
        public static readonly ReadOnlyDictionary<string, string> CRM_STATUS_MESSAGE_MAPPING =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
                { "DEFAULT", "UNDEFINED" },
                { "STATUS_1", "Trong quá trình thẩm định" },
                { "STATUS_2", "Trong quá trình thẩm định" },
                { "STATUS_3", "Trong quá trình thẩm định" },
                { "STATUS_4", "Trong quá trình thẩm định" },
                { "STATUS_5", "Trong quá trình thẩm định" },
                { "STATUS_6", "Trong quá trình thẩm định" },
                { "STATUS_7", "Trong quá trình thẩm định" },
                { "STATUS_8", "Trong quá trình thẩm định" },
                { "STATUS_9", "Trong quá trình thẩm định" },
                { "STATUS_10", "Trong quá trình thẩm định" },
                { "STATUS_11", "Trong quá trình thẩm định" },
                { "STATUS_12", "Trong quá trình thẩm định" },
                { "STATUS_13", "Trong quá trình thẩm định" },
                { "STATUS_14", "Hồ sơ bị hủy / từ chối" },
                { "STATUS_15", "Trong quá trình thẩm định" },
                { "STATUS_16", "Trong quá trình thẩm định" },
                { "STATUS_17", "Trong quá trình thẩm định" },
                { "STATUS_18", "Trong quá trình thẩm định" },
                { "STATUS_19", "Trong quá trình thẩm định" },
                { "STATUS_20", "Trong quá trình thẩm định" },
                { "STATUS_21", "Trong quá trình thẩm định" },
                { "STATUS_22", "Trong quá trình thẩm định" },
                { "STATUS_23", "Trong quá trình thẩm định" },
                { "STATUS_24", "Trong quá trình thẩm định" },
                { "STATUS_25", "Trong quá trình thẩm định" },
                { "STATUS_26", "Trong quá trình thẩm định" },
                { "STATUS_27", "Trong quá trình thẩm định" },
                { "STATUS_28", "Trong quá trình thẩm định" },
                { "STATUS_29", "Trong quá trình thẩm định" },
                { "STATUS_30", "Trong quá trình thẩm định" },
                { "STATUS_31", "Trong quá trình thẩm định" },
                { "STATUS_32", "Trong quá trình thẩm định" },
                { "STATUS_33", "Trong quá trình thẩm định" },
                { "STATUS_34", "Trong quá trình thẩm định" },
                { "STATUS_35", "Trong quá trình thẩm định" },
                { "STATUS_36", "Hồ sơ bị hủy / từ chối" },
                { "STATUS_37", "Giải ngân thành công" },
                { "STATUS_38", "Trong quá trình thẩm định" },
                { "STATUS_39", "Trong quá trình thẩm định" },
            });
    }
}