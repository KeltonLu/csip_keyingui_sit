//******************************************************************
//*  作    者：楊璐
//*  功能說明：他行自扣-記錄查詢
//*  創建日期：2012/10/09
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BaseItem;
using CSIPKeyInGUI.EntityLayer;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRcustomer_log_OtherBankTemp : CSIPCommonModel.BusinessRules.BRBase<Entitycustomer_log_OtherBankTemp>
    {
    }
}
