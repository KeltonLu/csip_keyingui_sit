//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：枚舉


//*  創建日期：2009/07/25
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;


public enum HtgType
{
    /// <summary>
    /// EXMS 1610
    /// </summary>
    P4_JCIL,

    /// <summary>
    /// EXMS 6015
    /// </summary>
    P4_JCIJ,
    P4A_JCGR,

    /// <summary>
    /// JCDHJCGU019
    /// </summary>
    P4_JCF7,

    /// <summary>
    /// P4_JCF6
    /// </summary>
    P4_JCF6,

    /// <summary>
    /// P4D_JCF6
    /// </summary>
    P4D_JCF6,

    /// <summary>
    /// P4_JCDK
    /// </summary>
    P4_JCDK,

    /// <summary>
    /// PCMC、PCMH
    /// </summary>
    P4_PCTI,
    P4D_PCTI,

    /// <summary>
    /// OASA_P4
    /// </summary>
    P4_JCAX,

    /// <summary>
    /// EXMS_1231_P4
    /// </summary>
    P4_JCAW,

    /// <summary>
    /// EXMS_6063_P4
    /// </summary>
    P4_JCGX,
    P4A_JCGX,

    /// <summary>
    /// EXMS_1255
    /// </summary>
    P4_JCHO,

    /// <summary>
    /// EXMS_6001
    /// </summary>
    P4_JCGQ,
    P4A_JCGQ,

    /// <summary>
    /// P4_JCAA
    /// </summary>
    P4_JCAA,

    /// <summary>
    /// P4_JCAT
    /// </summary>
    P4_JCAT,

    /// <summary>
    /// EXMS_1231_P4
    /// </summary>
    P4_JCEM,

    /// <summary>
    /// PCAM_P4_Submit
    /// </summary>
    P4_JCHQ,

    /// <summary>
    /// PCAM_P4A_Submit
    /// </summary>
    P4A_JCHQ,

    /// <summary>
    /// PCMM、PCIM作業P4
    /// </summary>
    P4_JCHR,

    /// <summary>
    /// PCMM、PCIM作業P4A
    /// </summary>
    P4A_JCHR,

    /// <summary>
    /// P4A_JCG1
    /// </summary>
    P4A_JCG1,

    /// <summary>
    /// P4L_LGOR
    /// </summary>
    P4L_LGOR,

    /// <summary>
    /// P4L_LGAT
    /// </summary>
    P4L_LGAT,

    /// <summary>
    /// Bancs
    /// </summary>
    P8_000401,

    /// <summary>
    /// ReqAppro
    /// </summary>
    P4A_JCPA,

    /// <summary>
    /// 餘額轉置作業
    /// </summary>
    P4_JCFA,
    P4D_JCFA,

    /// <summary>
    /// ETAG代扣作業
    /// </summary>
    P4_JCLB,
    P4_JCLD,
    P4_JCLE,
    P4A_JC66,
    P4A_JC67,

    /// <summary>
    /// 長姓名
    /// </summary>
    P4_JC99,
    P4A_JC68,

    /// <summary>
    ///AML收單分公司維護作業--20190918 by Peggy
    /// </summary>
    P4A_JC69,

    /// <summary>
    /// 卡人資料異動-地址 P4_JCDK改P4_JCBG 20211008 by Ares Stanley
    /// </summary>
    P4_JCBG,

    /// <summary>
    /// 別名 20210713 by Ares Dennis
    /// </summary>
    P4A_JC70
}

public enum SubmitType
{
    /// <summary>
    /// 基本資料異動提交
    /// </summary>
    BasicSubmit,

    /// <summary>
    /// 費率異動提交
    /// </summary>
    FeeSubmit,

    /// <summary>
    /// 帳號異動提交
    /// </summary>
    AccountSubmit,

    /// <summary>
    /// 解約作業提交
    /// </summary>
    CancelTaskSubmit,

    /// <summary>
    /// 機器資料提交
    /// </summary>
    MachineSubmit
}

