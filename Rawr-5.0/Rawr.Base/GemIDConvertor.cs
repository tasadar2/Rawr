﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr
{
    public static class GemIDConverter 
    { 
        // this class takes a GemID and returns a Blizzard EnchantID 
        // it used the Rawr.Addon routine GemToEnchants to export data from game
        public static int ConvertGemItemIDToEnchantID(int itemID)
        {
            #region switch statement
            switch (itemID)
            {
                case 41495 : return 3690;
                case 28468 : return 2966;
                case 30546 : return 3045;
                case 30594 : return 3082;
                case 52109 : return 3916;
                case 52141 : return 3968;
                case 52173 : return 3954;
                case 52205 : return 4009;
                case 39961 : return 3423;
                case 52269 : return 4035;
                case 40025 : return 3466;
                case 40057 : return 3496;
                case 40089 : return 3502;
                case 40121 : return 3534;
                case 40153 : return 3560;
                case 34143 : return 3254;
                case 41400 : return 3643;
                case 41432 : return 3649;
                case 41464 : return 3700;
                case 41496 : return 3679;
                case 25895 : return 2830;
                case 32193 : return 3115;
                case 28117 : return 2911;
                case 32225 : return 3147;
                case 24057 : return 2741;
                case 52206 : return 3994;
                case 39962 : return 3424;
                case 32833 : return 3201;
                case 40026 : return 3472;
                case 40058 : return 3497;
                case 40090 : return 3503;
                case 40122 : return 3535;
                case 40154 : return 3561;
                case 28389 : return 2950;
                case 41433 : return 3651;
                case 41465 : return 3714;
                case 41497 : return 3693;
                case 28469 : return 2967;
                case 30547 : return 3046;
                case 30563 : return 3060;
                case 32641 : return 3163;
                case 22459 : return 2948;
                case 36766 : return 3292;
                case 52143 : return 3970;
                case 52175 : return 3956;
                case 52207 : return 3995;
                case 39963 : return 3426;
                case 44087 : return 3803;
                case 40027 : return 3473;
                case 40059 : return 3498;
                case 40091 : return 3504;
                case 40123 : return 3526;
                case 40155 : return 3563;
                case 41434 : return 3644;
                case 41466 : return 3702;
                case 41498 : return 3697;
                case 25896 : return 2831;
                case 35488 : return 3282;
                case 32194 : return 3116;
                case 28118 : return 2912;
                case 32226 : return 3148;
                case 39900 : return 3371;
                case 39932 : return 3390;
                case 39964 : return 3418;
                case 39996 : return 3446;
                case 40028 : return 3476;
                case 23115 : return 2696;
                case 40092 : return 3505;
                case 40124 : return 3527;
                case 40156 : return 3564;
                case 41307 : return 3622;
                case 41339 : return 3625;
                case 68356 : return 4236;
                case 41435 : return 3648;
                case 41467 : return 3701;
                case 41499 : return 3684;
                case 28470 : return 2968;
                case 37503 : return 3318;
                case 35489 : return 3283;
                case 30564 : return 3061;
                case 45879 : return 3862;
                case 22460 : return 2947;
                case 52081 : return 3889;
                case 52113 : return 3920;
                case 52145 : return 3972;
                case 39933 : return 3391;
                case 39965 : return 3419;
                case 39997 : return 3447;
                case 40029 : return 3467;
                case 40125 : return 3528;
                case 40157 : return 3565;
                case 33060 : return 3206;
                case 68358 : return 4238;
                case 41436 : return 3652;
                case 41468 : return 3699;
                case 41500 : return 3694;
                case 25897 : return 2832;
                case 34627 : return 3264;
                case 32195 : return 3117;
                case 32211 : return 3133;
                case 52146 : return 3973;
                case 24059 : return 2761;
                case 39934 : return 3392;
                case 39966 : return 3420;
                case 39998 : return 3448;
                case 40030 : return 3475;
                case 23116 : return 2697;
                case 40094 : return 3510;
                case 40126 : return 3529;
                case 40158 : return 3566;
                case 41437 : return 3647;
                case 41469 : return 3712;
                case 41501 : return 3678;
                case 30549 : return 3048;
                case 30565 : return 3062;
                case 30581 : return 3069;
                case 45881 : return 3864;
                case 52083 : return 3891;
                case 52115 : return 3922;
                case 52147 : return 3974;
                case 49110 : return 3879;
                case 39935 : return 3393;
                case 39967 : return 3421;
                case 39999 : return 3449;
                case 40031 : return 3468;
                case 40095 : return 3506;
                case 40127 : return 3530;
                case 31860 : return 3104;
                case 27864 : return 2917;
                case 41438 : return 3650;
                case 41470 : return 3698;
                case 41502 : return 3691;
                case 25898 : return 2833;
                case 45882 : return 3865;
                case 32196 : return 3118;
                case 28120 : return 2914;
                case 52148 : return 3975;
                case 24060 : return 2742;
                case 39936 : return 3394;
                case 39968 : return 3427;
                case 40000 : return 3450;
                case 23101 : return 2760;
                case 40096 : return 3511;
                case 40128 : return 3531;
                case 40160 : return 3568;
                case 28360 : return 2943;
                case 41375 : return 3632;
                case 41439 : return 3646;
                case 41471 : return 3713;
                case 38498 : return 3333;
                case 30550 : return 3049;
                case 30566 : return 3063;
                case 30582 : return 3070;
                case 30598 : return 3103;
                case 45883 : return 3866;
                case 52085 : return 3893;
                case 52117 : return 3924;
                case 52149 : return 3976;
                case 39905 : return 3374;
                case 39937 : return 3395;
                case 52245 : return 4029;
                case 40001 : return 3451;
                case 40033 : return 3470;
                case 42143 : return 3733;
                case 40129 : return 3536;
                case 31861 : return 3105;
                case 27785 : return 2898;
                case 41376 : return 3633;
                case 41440 : return 3654;
                case 41472 : return 3709;
                case 25899 : return 2834;
                case 32197 : return 3119;
                case 24029 : return 2727;
                case 52150 : return 3977;
                case 24061 : return 2756;
                case 39938 : return 3396;
                case 52246 : return 4007;
                case 40002 : return 3452;
                case 40034 : return 3471;
                case 23118 : return 2698;
                case 40098 : return 3515;
                case 40130 : return 3537;
                case 40162 : return 3570;
                case 28361 : return 2944;
                case 41377 : return 3634;
                case 41441 : return 3655;
                case 41473 : return 3705;
                case 30551 : return 3050;
                case 30583 : return 3071;
                case 52087 : return 3895;
                case 52119 : return 3926;
                case 52151 : return 3978;
                case 39907 : return 3376;
                case 39939 : return 3397;
                case 52247 : return 4003;
                case 40003 : return 3453;
                case 40099 : return 3507;
                case 40131 : return 3544;
                case 31862 : return 3106;
                case 27786 : return 2918;
                case 41378 : return 3635;
                case 41442 : return 3653;
                case 41474 : return 3716;
                case 32198 : return 3120;
                case 28122 : return 2915;
                case 52152 : return 3979;
                case 24062 : return 2743;
                case 39940 : return 3398;
                case 52248 : return 3993;
                case 23103 : return 2762;
                case 23119 : return 2699;
                case 40100 : return 3512;
                case 40132 : return 3538;
                case 40164 : return 3572;
                case 33131 : return 3208;
                case 28362 : return 2945;
                case 41379 : return 3636;
                case 41443 : return 3656;
                case 41475 : return 3703;
                case 28458 : return 2956;
                case 30552 : return 3051;
                case 30584 : return 3072;
                case 30600 : return 3083;
                case 52089 : return 3897;
                case 52121 : return 3928;
                case 52153 : return 3980;
                case 39909 : return 3378;
                case 39941 : return 3399;
                case 52249 : return 4020;
                case 40037 : return 3477;
                case 40101 : return 3516;
                case 40133 : return 3545;
                case 31863 : return 3107;
                case 33132 : return 3209;
                case 41380 : return 3637;
                case 41444 : return 3661;
                case 41476 : return 3708;
                case 25901 : return 2835;
                case 32199 : return 3121;
                case 24031 : return 2729;
                case 24047 : return 2734;
                case 39910 : return 3379;
                case 39942 : return 3400;
                case 39974 : return 3428;
                case 23104 : return 2757;
                case 23120 : return 2700;
                case 40102 : return 3508;
                case 40134 : return 3546;
                case 40166 : return 3574;
                case 41285 : return 3621;
                case 33133 : return 3210;
                case 28363 : return 2946;
                case 41381 : return 3638;
                case 41445 : return 3662;
                case 41477 : return 3710;
                case 28459 : return 2957;
                case 30553 : return 3052;
                case 30585 : return 3073;
                case 30601 : return 3084;
                case 52091 : return 3936;
                case 52123 : return 3930;
                case 52155 : return 3982;
                case 39911 : return 3380;
                case 39943 : return 3401;
                case 39975 : return 3429;
                case 40039 : return 3479;
                case 40103 : return 3513;
                case 40135 : return 3548;
                case 31864 : return 3108;
                case 27820 : return 2923;
                case 33134 : return 3211;
                case 41382 : return 3639;
                case 41446 : return 3659;
                case 41478 : return 3715;
                case 32200 : return 3122;
                case 32216 : return 3138;
                case 24048 : return 2735;
                case 39912 : return 3381;
                case 39944 : return 3402;
                case 39976 : return 3430;
                case 40008 : return 3454;
                case 23105 : return 2706;
                case 23121 : return 2701;
                case 40104 : return 3509;
                case 40136 : return 3539;
                case 40168 : return 3576;
                case 40232 : return 3591;
                case 23233 : return 2686;
                case 41447 : return 3660;
                case 41479 : return 3706;
                case 28460 : return 2958;
                case 38538 : return 3334;
                case 30554 : return 3053;
                case 30586 : return 3074;
                case 28556 : return 2969;
                case 52093 : return 3900;
                case 45987 : return 3867;
                case 52157 : return 3938;
                case 39945 : return 3403;
                case 39977 : return 3431;
                case 40009 : return 3455;
                case 40041 : return 3481;
                case 40105 : return 3514;
                case 40137 : return 3547;
                case 31865 : return 3109;
                case 68357 : return 4237;
                case 63697 : return 4213;
                case 63696 : return 4212;
                case 59496 : return 4173;
                case 59493 : return 4172;
                case 59491 : return 4171;
                case 41448 : return 3657;
                case 41480 : return 3707;
                case 59489 : return 4170;
                case 59480 : return 4169;
                case 59479 : return 4168;
                case 59478 : return 4167;
                case 59477 : return 4166;
                case 54616 : return 4119;
                case 52302 : return 4057;
                case 52301 : return 4056;
                case 52300 : return 4055;
                case 52299 : return 4054;
                case 52298 : return 4053;
                case 32201 : return 3123;
                case 35758 : return 3284;
                case 52158 : return 3939;
                case 39914 : return 3382;
                case 39946 : return 3404;
                case 39978 : return 3432;
                case 40010 : return 3456;
                case 23106 : return 2707;
                case 30588 : return 3076;
                case 40106 : return 3517;
                case 40138 : return 3540;
                case 40170 : return 3578;
                case 52296 : return 4051;
                case 32409 : return 3154;
                case 23108 : return 2708;
                case 33137 : return 3214;
                case 52294 : return 4049;
                case 41385 : return 3640;
                case 34256 : return 3262;
                case 41449 : return 3658;
                case 41481 : return 3704;
                case 52293 : return 4048;
                case 28461 : return 2959;
                case 52292 : return 4047;
                case 52291 : return 4046;
                case 30555 : return 3054;
                case 30571 : return 3065;
                case 30587 : return 3075;
                case 30603 : return 3086;
                case 52289 : return 4045;
                case 52265 : return 4044;
                case 44066 : return 3792;
                case 52095 : return 3902;
                case 35759 : return 3285;
                case 52159 : return 3940;
                case 39915 : return 3383;
                case 39947 : return 3405;
                case 39979 : return 3433;
                case 40011 : return 3457;
                case 40043 : return 3482;
                case 52262 : return 4042;
                case 42153 : return 3745;
                case 40139 : return 3541;
                case 27774 : return 2894;
                case 52261 : return 4041;
                case 52266 : return 4040;
                case 52264 : return 4039;
                case 33138 : return 3215;
                case 52268 : return 4038;
                case 52260 : return 4037;
                case 52267 : return 4036;
                case 41450 : return 3664;
                case 41482 : return 3696;
                case 52259 : return 4034;
                case 52258 : return 4033;
                case 52257 : return 4032;
                case 52255 : return 4031;
                case 52250 : return 4030;
                case 52237 : return 4028;
                case 52233 : return 4027;
                case 52231 : return 4026;
                case 45862 : return 3861;
                case 52228 : return 4025;
                case 52227 : return 4024;
                case 32202 : return 3124;
                case 35760 : return 3286;
                case 24050 : return 2736;
                case 39916 : return 3384;
                case 39948 : return 3411;
                case 39980 : return 3434;
                case 40012 : return 3458;
                case 40044 : return 3483;
                case 52225 : return 4023;
                case 42154 : return 3746;
                case 40140 : return 3543;
                case 40172 : return 3580;
                case 52223 : return 4022;
                case 32410 : return 3155;
                case 52218 : return 4021;
                case 23235 : return 2688;
                case 52240 : return 4019;
                case 52239 : return 4018;
                case 52229 : return 4017;
                case 41451 : return 3670;
                case 41483 : return 3683;
                case 52224 : return 4016;
                case 28462 : return 2960;
                case 52222 : return 4015;
                case 52215 : return 4014;
                case 30556 : return 3055;
                case 30572 : return 3064;
                case 32634 : return 3156;
                case 30604 : return 3087;
                case 52214 : return 4013;
                case 52211 : return 4012;
                case 52209 : return 4011;
                case 52097 : return 3903;
                case 35761 : return 3287;
                case 52161 : return 3942;
                case 39917 : return 3385;
                case 39949 : return 3407;
                case 39981 : return 3435;
                case 40013 : return 3459;
                case 40045 : return 3484;
                case 52208 : return 4010;
                case 42155 : return 3747;
                case 40141 : return 3542;
                case 40173 : return 3581;
                case 30592 : return 3080;
                case 52244 : return 4006;
                case 52242 : return 4005;
                case 33140 : return 3217;
                case 52235 : return 4004;
                case 52241 : return 4002;
                case 52232 : return 4001;
                case 41452 : return 3675;
                case 41484 : return 3686;
                case 52226 : return 4000;
                case 52219 : return 3999;
                case 52230 : return 3998;
                case 52216 : return 3997;
                case 52212 : return 3996;
                case 52243 : return 3992;
                case 52238 : return 3991;
                case 31116 : return 3099;
                case 52236 : return 3990;
                case 52234 : return 3989;
                case 52221 : return 3988;
                case 32203 : return 3125;
                case 24035 : return 2732;
                case 24051 : return 2764;
                case 39918 : return 3386;
                case 39950 : return 3408;
                case 39982 : return 3436;
                case 40014 : return 3460;
                case 40046 : return 3485;
                case 52220 : return 3987;
                case 42156 : return 3742;
                case 40142 : return 3549;
                case 40174 : return 3582;
                case 52217 : return 3986;
                case 52213 : return 3985;
                case 52210 : return 3984;
                case 33141 : return 3218;
                case 52203 : return 3983;
                case 41389 : return 3641;
                case 52154 : return 3981;
                case 35315 : return 3270;
                case 41485 : return 3677;
                case 52144 : return 3971;
                case 28463 : return 2961;
                case 23364 : return 2703;
                case 52142 : return 3969;
                case 52140 : return 3967;
                case 30573 : return 3066;
                case 32635 : return 3157;
                case 30605 : return 3088;
                case 52139 : return 3966;
                case 52138 : return 3965;
                case 24033 : return 2731;
                case 52099 : return 3906;
                case 52131 : return 3958;
                case 52163 : return 3944;
                case 39919 : return 3387;
                case 39951 : return 3409;
                case 39983 : return 3437;
                case 40015 : return 3461;
                case 40047 : return 3486;
                case 52136 : return 3963;
                case 40111 : return 3518;
                case 40143 : return 3550;
                case 31868 : return 3112;
                case 44082 : return 3800;
                case 52134 : return 3961;
                case 52114 : return 3921;
                case 33142 : return 3219;
                case 52132 : return 3959;
                case 52176 : return 3957;
                case 52174 : return 3955;
                case 35316 : return 3271;
                case 41486 : return 3692;
                case 25890 : return 2827;
                case 52172 : return 3953;
                case 40161 : return 3569;
                case 38545 : return 3335;
                case 52170 : return 3951;
                case 42701 : return 3749;
                case 23099 : return 2705;
                case 31117 : return 3100;
                case 52168 : return 3949;
                case 44078 : return 3799;
                case 52166 : return 3947;
                case 32204 : return 3126;
                case 32220 : return 3142;
                case 24052 : return 2737;
                case 33782 : return 3226;
                case 39952 : return 3410;
                case 39984 : return 3438;
                case 40016 : return 3462;
                case 23109 : return 2709;
                case 52112 : return 3919;
                case 40112 : return 3519;
                case 40144 : return 3551;
                case 40176 : return 3584;
                case 52164 : return 3945;
                case 52162 : return 3943;
                case 52160 : return 3941;
                case 33143 : return 3220;
                case 52156 : return 3937;
                case 52126 : return 3933;
                case 52125 : return 3932;
                case 41455 : return 3674;
                case 41487 : return 3680;
                case 52124 : return 3931;
                case 28464 : return 2962;
                case 52122 : return 3929;
                case 38546 : return 3336;
                case 30558 : return 3056;
                case 30574 : return 3067;
                case 30590 : return 3078;
                case 30606 : return 3089;
                case 52120 : return 3927;
                case 52118 : return 3925;
                case 52116 : return 3923;
                case 52101 : return 3908;
                case 52133 : return 3960;
                case 52165 : return 3946;
                case 52111 : return 3918;
                case 39953 : return 3406;
                case 39985 : return 3439;
                case 40017 : return 3463;
                case 40049 : return 3488;
                case 52110 : return 3917;
                case 40113 : return 3520;
                case 40145 : return 3552;
                case 31869 : return 3113;
                case 52108 : return 3915;
                case 27809 : return 2921;
                case 34831 : return 3268;
                case 33144 : return 3221;
                case 52106 : return 3913;
                case 28557 : return 2970;
                case 52104 : return 3911;
                case 35318 : return 3272;
                case 41488 : return 3682;
                case 44081 : return 3801;
                case 52102 : return 3909;
                case 52100 : return 3907;
                case 38547 : return 3337;
                case 52098 : return 3905;
                case 52096 : return 3904;
                case 52094 : return 3901;
                case 31118 : return 3101;
                case 52092 : return 3899;
                case 52090 : return 3898;
                case 52070 : return 3884;
                case 32205 : return 3127;
                case 32221 : return 3143;
                case 24053 : return 2759;
                case 52088 : return 3896;
                case 39954 : return 3412;
                case 39986 : return 3440;
                case 23094 : return 2690;
                case 23110 : return 2710;
                case 52086 : return 3894;
                case 40114 : return 3521;
                case 40146 : return 3553;
                case 40178 : return 3586;
                case 52084 : return 3892;
                case 52082 : return 3890;
                case 52129 : return 3888;
                case 52130 : return 3887;
                case 38292 : return 3321;
                case 52128 : return 3886;
                case 52127 : return 3885;
                case 41457 : return 3673;
                case 41489 : return 3685;
                case 45880 : return 3863;
                case 28465 : return 2963;
                case 23366 : return 2704;
                case 38548 : return 3338;
                case 30559 : return 3057;
                case 30575 : return 3068;
                case 32637 : return 3159;
                case 30607 : return 3090;
                case 44089 : return 3805;
                case 44088 : return 3804;
                case 44084 : return 3802;
                case 52103 : return 3910;
                case 52135 : return 3962;
                case 52167 : return 3948;
                case 44076 : return 3798;
                case 39955 : return 3413;
                case 52263 : return 4043;
                case 52295 : return 4050;
                case 40051 : return 3490;
                case 42702 : return 3750;
                case 40115 : return 3522;
                case 40147 : return 3554;
                case 40179 : return 3587;
                case 42158 : return 3744;
                case 42157 : return 3743;
                case 42152 : return 3741;
                case 42151 : return 3740;
                case 42150 : return 3739;
                case 42149 : return 3738;
                case 42148 : return 3737;
                case 41458 : return 3668;
                case 41490 : return 3695;
                case 37430 : return 3317;
                case 42146 : return 3736;
                case 42145 : return 3735;
                case 38549 : return 3339;
                case 42144 : return 3734;
                case 42142 : return 3732;
                case 24039 : return 2765;
                case 35487 : return 3281;
                case 40175 : return 3583;
                case 23114 : return 2695;
                case 39906 : return 3375;
                case 32206 : return 3128;
                case 32222 : return 3144;
                case 24054 : return 2738;
                case 23098 : return 2752;
                case 39956 : return 3414;
                case 39988 : return 3441;
                case 23095 : return 2691;
                case 23111 : return 2711;
                case 40052 : return 3491;
                case 40116 : return 3523;
                case 40148 : return 3555;
                case 28290 : return 2942;
                case 41453 : return 3669;
                case 24065 : return 2744;
                case 40038 : return 3478;
                case 41456 : return 3665;
                case 41454 : return 3663;
                case 41395 : return 3626;
                case 39920 : return 3388;
                case 41459 : return 3672;
                case 41491 : return 3687;
                case 40048 : return 3487;
                case 28466 : return 2964;
                case 27777 : return 2896;
                case 38550 : return 3340;
                case 30560 : return 3058;
                case 32636 : return 3158;
                case 32638 : return 3160;
                case 30608 : return 3091;
                case 23234 : return 2687;
                case 34220 : return 3261;
                case 40032 : return 3469;
                case 52105 : return 3912;
                case 52137 : return 3964;
                case 52169 : return 3950;
                case 30548 : return 3047;
                case 39957 : return 3415;
                case 39989 : return 3442;
                case 52297 : return 4052;
                case 40053 : return 3492;
                case 40085 : return 3499;
                case 40117 : return 3525;
                case 40149 : return 3556;
                case 40181 : return 3589;
                case 32640 : return 3162;
                case 27811 : return 2922;
                case 23100 : return 2755;
                case 33135 : return 3212;
                case 32209 : return 3131;
                case 41396 : return 3631;
                case 32217 : return 3139;
                case 41460 : return 3667;
                case 41492 : return 3681;
                case 25893 : return 2828;
                case 40050 : return 3489;
                case 31866 : return 3110;
                case 41401 : return 3627;
                case 32215 : return 3137;
                case 40163 : return 3571;
                case 24030 : return 2728;
                case 39908 : return 3377;
                case 35503 : return 3275;
                case 30589 : return 3077;
                case 24037 : return 2733;
                case 32207 : return 3129;
                case 32223 : return 3145;
                case 24055 : return 2739;
                case 32210 : return 3132;
                case 39958 : return 3416;
                case 39990 : return 3443;
                case 23096 : return 2692;
                case 40054 : return 3493;
                case 40086 : return 3500;
                case 40118 : return 3524;
                case 40150 : return 3557;
                case 40182 : return 3590;
                case 32218 : return 3140;
                case 24058 : return 2753;
                case 24066 : return 2763;
                case 41333 : return 3623;
                case 40177 : return 3585;
                case 41397 : return 3642;
                case 41429 : return 3767;
                case 41461 : return 3671;
                case 41493 : return 3688;
                case 35501 : return 3274;
                case 28467 : return 2965;
                case 31867 : return 3111;
                case 40180 : return 3588;
                case 40171 : return 3579;
                case 40169 : return 3577;
                case 32639 : return 3161;
                case 40167 : return 3575;
                case 40165 : return 3573;
                case 28595 : return 2971;
                case 35707 : return 3280;
                case 52107 : return 3914;
                case 32735 : return 3197;
                case 52171 : return 3952;
                case 39927 : return 3389;
                case 39959 : return 3417;
                case 39991 : return 3444;
                case 40023 : return 3465;
                case 40055 : return 3494;
                case 40159 : return 3567;
                case 40119 : return 3532;
                case 40151 : return 3558;
                case 33139 : return 3216;
                case 24027 : return 2725;
                case 27812 : return 2924;
                case 28123 : return 2916;
                case 32219 : return 3141;
                case 24067 : return 2758;
                case 41398 : return 3628;
                case 32836 : return 3202;
                case 41462 : return 3666;
                case 41494 : return 3689;
                case 25894 : return 2829;
                case 40024 : return 3474;
                case 30591 : return 3079;
                case 24036 : return 2754;
                case 32213 : return 3135;
                case 40056 : return 3495;
                case 40022 : return 3464;
                case 40040 : return 3480;
                case 24032 : return 2730;
                case 36767 : return 3293;
                case 30602 : return 3085;
                case 32208 : return 3130;
                case 32224 : return 3146;
                case 24056 : return 2740;
                case 52204 : return 4008;
                case 39960 : return 3422;
                case 39992 : return 3445;
                case 23097 : return 2693;
                case 23113 : return 2694;
                case 40088 : return 3501;
                case 40120 : return 3533;
                case 40152 : return 3559;
                case 32212 : return 3134;
                case 32214 : return 3136;
                case 27679 : return 2891;
                case 34142 : return 3242;
                case 41335 : return 3624;
                case 24028 : return 2726;
                case 28388 : return 2949;
                case 28119 : return 2913;
                case 41463 : return 3711;
                case 30593 : return 3081;
            }
            #endregion
            return 0;
        } 
    }

}
