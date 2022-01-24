// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker;

public sealed class RegexConstants
{
    public sealed class Email
    {
        public const string Any = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
    }

    public sealed class MobileNumber
    {
        public const string Any = @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$";

        /// <summary>
        /// 澳大利亚
        /// Australia
        /// </summary>
        public const string AU = @"^(\+?61|0)4\d{8}$";

        /// <summary>
        /// 比利时
        /// Belgium (The Kingdom of Belgium)
        /// </summary>
        public const string BE = @"^(\+?32|0)4?\d{8}$";

        /// <summary>
        /// 巴西
        /// Brazil (The Federative Republic of Brazil)
        /// </summary>
        public const string BR = @"^(\+?55|0)\-?[1-9]{2}\-?[2-9]{1}\d{3,4}\-?\d{4}$";

        /// <summary>
        /// 中国
        /// China (The People's Republic of China)
        /// </summary>
        public const string CN = @"^(\+?0?86\-?)?1[345789]\d{9}$";

        /// <summary>
        /// 捷克
        /// Czech Republic (The Czech Republic)
        /// </summary>
        public const string CZ = @"^(\+?420)? ?[1-9][0-9]{2} ?[0-9]{3} ?[0-9]{3}$";

        /// <summary>
        /// 德国
        /// Germany (He Federal Republic of Germany)
        /// </summary>
        public const string DE = @"^(\+?49[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$";

        /// <summary>
        /// 丹麦
        /// Denmark (The Kingdom of Denmark)
        /// </summary>
        public const string DK = @"^(\+?45)?(\d{8})$";

        /// <summary>
        /// 阿尔及利亚
        /// Algeria (The People's Democratic Republic of Algeria)
        /// </summary>
        public const string DZ = @"^(\+?213|0)(5|6|7)\d{8}$";

        /// <summary>
        /// 西班牙
        /// Spain (The Kingdom of Spain)
        /// </summary>
        public const string ES = @"^(\+?34)?(6\d{1}|7[1234])\d{7}$";

        /// <summary>
        /// 芬兰
        /// Finland (The Republic of Finland)
        /// </summary>
        public const string FI = @"^(\+?358|0)\s?(4(0|1|2|4|5)?|50)\s?(\d\s?){4,8}\d$";

        /// <summary>
        /// 法国
        /// France (The French Republic)
        /// </summary>
        public const string FR = @"^(\+?33|0)[67]\d{8}$";

        /// <summary>
        /// 英国
        /// United Kingdom (The United Kingdom of Great Britain and Northern Ireland)
        /// </summary>
        public const string GB = @"^(\+?44|0)7\d{9}$";

        /// <summary>
        /// 希腊
        /// Greece (The Hellenic Republic)
        /// </summary>
        public const string GR = @"^(\+?30)?(69\d{8})$";

        /// <summary>
        /// 中国香港
        /// Hong Kong (The Hong Kong Special Administrative Region of China)
        /// </summary>
        public const string HK = @"^(\+?852\-?)?[569]\d{3}\-?\d{4}$";

        /// <summary>
        /// 匈牙利
        /// Hungary (The Republic of Hungary)
        /// </summary>
        public const string HU = @"^(\+?36)(20|30|70)\d{7}$";

        /// <summary>
        /// 印度
        /// India (The Republic of India)
        /// </summary>
        public const string IN = @"^(\+?91|0)?[789]\d{9}$";

        /// <summary>
        /// 意大利
        /// Italy (The Republic of Italy)
        /// </summary>
        public const string IT = @"^(\+?39)?\s?3\d{2} ?\d{6,7}$";

        /// <summary>
        /// 日本
        /// Japan
        /// </summary>
        public const string JP = @"^(\+?81|0)\d{1,4}[ \-]?\d{1,4}[ \-]?\d{4}$";

        /// <summary>
        /// Malaysia (马来西亚)
        /// </summary>
        public const string MY = @"^(\+?6?01){1}(([145]{1}(\-|\s)?\d{7,8})|([236789]{1}(\s|\-)?\d{7}))$";

        /// <summary>
        /// 挪威
        /// Norway (The Kingdom of Norway)
        /// </summary>
        public const string NO = @"^(\+?47)?[49]\d{7}$";

        /// <summary>
        /// 新西兰
        /// New Zealand
        /// </summary>
        public const string NZ = @"^(\+?64|0)2\d{7,9}$";

        /// <summary>
        /// 波兰
        /// Poland (The Republic of Poland)
        /// </summary>
        public const string PL = @"^(\+?48)? ?[5-8]\d ?\d{3} ?\d{2} ?\d{2}$";

        /// <summary>
        /// 葡萄牙
        /// Portugal (The Portuguese Republic)
        /// </summary>
        public const string PT = @"^(\+?351)?9[1236]\d{7}$";

        /// <summary>
        /// 塞尔维亚
        /// Serbia (The Republic of Serbia)
        /// </summary>
        public const string RS = @"^(\+3816|06)[- \d]{5,9}$";

        /// <summary>
        /// 俄罗斯联邦
        /// Russian Federation (The Russian Federation)
        /// </summary>
        public const string RU = @"^(\+?7|8)?9\d{9}$";

        /// <summary>
        /// 沙特阿拉伯
        /// Saudi Arabia (The Kingdom of Saudi Arabia)
        /// </summary>
        public const string SA = @"^(!?(\+?966)|0)?5\d{8}$";

        /// <summary>
        /// 叙利亚
        /// Syrian Arab Republic (The Syrian Arab Republic)
        /// </summary>
        public const string SY = @"^(!?(\+?963)|0)?9\d{8}$";

        /// <summary>
        /// 土耳其
        /// Turkey (The Republic of Turkey)
        /// </summary>
        public const string TR = @"^(\+?90|0)?5\d{9}$";

        /// <summary>
        /// 中国台湾
        /// Taiwan (Province of China)
        /// </summary>
        public const string TW = @"^(\+?886\-?|0)?9\d{8}$";

        /// <summary>
        /// 美国
        /// United States (The United States of America)
        /// </summary>
        public const string US = @"^(\+?1)?[2-9]\d{2}[2-9](?!11)\d{6}$";

        /// <summary>
        /// 越南
        /// Viet Nam (The Socialist Republic of Viet Nam)
        /// </summary>
        public const string VN = @"^(\+?84|0)?((1(2([0-9])|6([2-9])|88|99))|(9((?!5)[0-9])))([0-9]{7})$";

        /// <summary>
        /// 南非
        /// South Africa (The Republic of South Africa)
        /// </summary>
        public const string ZA = @"^(\+?27|0)\d{9}$";

        /// <summary>
        /// 赞比亚
        /// Zambia (The Republic of Zambia)
        /// </summary>
        public const string ZM = @"^(\+?26)?09[567]\d{7}$";
    }

    public sealed class Ipv4Address
    {
        public const string Any = @"((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})(\.((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})){3}";
    }
}
