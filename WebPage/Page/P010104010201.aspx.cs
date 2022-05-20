//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料新增- PCAM一次鍵檔
//*  創建日期：2009/11/05
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;

public partial class P010104010201 : PageBase
{
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            ClearAll();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            //SetFeeDisable();
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        base.strClientMsg += "";
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
         
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));


        //*向資料庫中新增資料

        EntitySet<EntitySHOP_PCAM> eShopPcamSet = null;

        try
        {

            eShopPcamSet = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopPcamSet != null && eShopPcamSet.Count > 0)
        {
            if (eShopPcamSet.GetEntity(0).send3270.Trim() != "Y")
            {
                //* 若查出有一Key資料，則Update
                if (UpdateData())
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01040102_001");
                    ClearAll();
                }
            }
            else
            {
                //*已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040102_003") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            }
        }
        else
        {
            //*若沒有則寫入一筆新的資料
            if (InsertNewData())
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040102_001");
                ClearAll();
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        ClearPage(true);
        if (GetMainframeData())
        {        
            //*根據商店代號查詢資料庫
            EntitySet<EntitySHOP_PCAM> eShopPcamSet = null;
            try
            {
                eShopPcamSet = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "1");
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            CheckShopId();
            if (eShopPcamSet != null && eShopPcamSet.Count > 0)
            {
                if (eShopPcamSet.GetEntity(0).send3270.Trim() != "Y")
                {
                    SetValues(eShopPcamSet.GetEntity(0));
                }
                else
                {
                    //*已上傳主機
                    ClearPage(false);
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040102_003") + "');");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_040");            
                SetFeeDisable();
            }
        }
        else
        {
            ClearPage(false);
            //SetFeeDisable();
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 檢核城市資料事件
    /// </summary>
    protected void btnCheck_Click(object sender, EventArgs e)
    {

            EntitySet<EntitySZIP> eSzipSet = null;
            try
            {
                eSzipSet = BRSZIP.SelectEntitySet(this.txtAddress1.Text.Trim());
                if (eSzipSet.Count > 0)
                {
                    //*網頁中的區域號碼顯示為查詢結果
                    txtZip.Text = eSzipSet.GetEntity(0).zip_code.Trim();
                }
                else
                {
                    txtZip.Text = "";
                    base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01040102_002") + "')) {document.getElementById('txtAddress1').focus();}");
                }
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 設置【費率01】~【費率08】不可用
    /// </summary>
    private void SetFeeDisable()
    {

        string strNum = CommonFunction.GetSubString(this.txtShopId.Text.Trim(), 2, 3);


        if (!(CommonFunction.GetSubString(this.txtShopId.Text.Trim(), 0, 5) == "88320" || strNum == "251" || strNum == "252" || strNum == "253" || strNum == "254" || this.txtShopId.Text.Trim().IndexOf('5') == 2))
        {
            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            this.txtDisRate10.Enabled = false;
            this.txtDisRate11.Enabled = false;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = false;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 向資料庫中新增資料

    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewData()
    {
        #region
            EntitySHOP_PCAM eShopPcam = new EntitySHOP_PCAM();
            eShopPcam.organization = "";//*組織
            eShopPcam.merchant_name = GetMerName().ToUpper();//*帳列名稱
            eShopPcam.corp = this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper();//*【統一編號】(1)+【統一編號】(2)
            eShopPcam.merchdba_name = this.txtEngName.Text.Trim().ToUpper();//*英文名稱
            eShopPcam.merchant_nbr = this.txtShopId.Text.Trim().ToUpper();//*商店代號
            eShopPcam.dba_city = this.txtDbaCity.Text.Trim().ToUpper();//*城市
            eShopPcam.zip = this.txtZip.Text.Trim().ToUpper();//*區域編號

            eShopPcam.address1 = this.txtAddress1.Text.Trim().ToUpper();//*【帳單地址】(1)
            eShopPcam.address2 = this.txtAddress2.Text.Trim().ToUpper();//*【帳單地址】(2)
            eShopPcam.address3 = "";//*【帳單地址】(3)
            eShopPcam.address4 = "";
            eShopPcam.phone = this.txtPhone1.Text.Trim() + "-" + this.txtPhone2.Text.Trim();//*連絡電話
            eShopPcam.owner = this.txtOwner.Text.Trim().ToUpper();//*負責人英文名
            eShopPcam.imp1 = this.txtImp1.Text.Trim().ToUpper();//*IMP1
            eShopPcam.pos1 = this.txtPos1.Text.Trim().ToUpper();//*POS1
            eShopPcam.edc1 = this.txtEdc1.Text.Trim().ToUpper();//*EDC1
            eShopPcam.imp2 = this.txtImp2.Text.Trim().ToUpper();//*IMP2
            eShopPcam.pos2 = this.txtPos2.Text.Trim().ToUpper();//*POS2
            eShopPcam.edc2 = this.txtEdc2.Text.Trim().ToUpper();//*EDC2
            eShopPcam.sales_name = this.txtSalesName.Text.Trim().ToUpper();//*推廣姓名
            eShopPcam.agent_bank = this.txtAgentBank.Text.Trim().ToUpper();//*推廣代碼
            eShopPcam.merchant_type = this.txtType.Text.Trim().ToUpper();//*Type
            eShopPcam.chain_store = this.txtStore.Text.Trim().ToUpper();//*STORE
            eShopPcam.chain_merch = this.txtMerch.Text.Trim().ToUpper();//*MERCH
            eShopPcam.chain_level = this.txtLevel.Text.Trim().ToUpper();//*LEVEL
            eShopPcam.Source = this.txtSource.Text.Trim().ToUpper();//*SOURE
            eShopPcam.mcc_VISA = this.txtMcc.Text.Trim().ToUpper();//*MCC
            //eShopPcam.mcc_MC = this.txtMcc.Text.Trim().ToUpper();
            eShopPcam.mcc_MC = "";
            //eShopPcam.mcc_PL = this.txtMcc.Text.Trim().ToUpper();
            eShopPcam.mcc_PL = "";
            eShopPcam.avg_tkt = this.txtAve.Text.Trim().ToUpper();//*AVE
            eShopPcam.monthly_volume = this.txtMonth.Text.Trim().ToUpper();//*Month
            eShopPcam.branch = this.txtBranch.Text.Trim().ToUpper();//*BRANCH
            eShopPcam.INT = this.txtInt.Text.Trim().ToUpper();//*INT
            eShopPcam.MAIL = this.txtMail.Text.Trim().ToUpper();//*MAIL
            eShopPcam.POS_CA = this.txtPosCa.Text.Trim().ToUpper();//*Pos CA
            eShopPcam.POS_MO = this.txtPosMo.Text.Trim().ToUpper();//*Pos Mo
            eShopPcam.CH = this.txtCH.Text.Trim().ToUpper();//*C/H
            eShopPcam.MC = this.txtMC.Text.Trim().ToUpper();//*M/C
            eShopPcam.FileNo = this.txtFileNo.Text.Trim().ToUpper();//*歸檔編號
            eShopPcam.hold_stmt = this.txtHOLDSTMT.Text.Trim().ToUpper();//*Hold STMT
            eShopPcam.DDA_NO = this.txtDdaNo1.Text.Trim().ToUpper() + " " + this.txtDdaNo2.Text.Trim().ToUpper();//*【帳號】(1)+’ ’+【帳號】(2)
            //*Status-01-15
            eShopPcam.status1 = this.txtStatus1.Text.Trim().ToUpper();
            eShopPcam.status2 = this.txtStatus2.Text.Trim().ToUpper();
            eShopPcam.status3 = this.txtStatus3.Text.Trim().ToUpper();
            eShopPcam.status4 = this.txtStatus4.Text.Trim().ToUpper();
            eShopPcam.status5 = this.txtStatus5.Text.Trim().ToUpper();
            eShopPcam.status6 = this.txtStatus6.Text.Trim().ToUpper();
            eShopPcam.status7 = this.txtStatus7.Text.Trim().ToUpper();
            eShopPcam.status8 = this.txtStatus8.Text.Trim().ToUpper();
            eShopPcam.status9 = this.txtStatus9.Text.Trim().ToUpper();
            eShopPcam.status10 = this.txtStatus10.Text.Trim().ToUpper();
            eShopPcam.status11 = this.txtStatus11.Text.Trim().ToUpper();
            eShopPcam.status12 = this.txtStatus12.Text.Trim().ToUpper();
            eShopPcam.status13 = this.txtStatus13.Text.Trim().ToUpper();
            eShopPcam.status14 = this.txtStatus14.Text.Trim().ToUpper();
            eShopPcam.status15 = this.txtStatus15.Text.Trim().ToUpper();
            
            //*費率01-15
            eShopPcam.dis_rate1 = this.txtDisRate1.Text.Trim();
            eShopPcam.dis_rate2 = this.txtDisRate2.Text.Trim();
            eShopPcam.dis_rate3 = this.txtDisRate3.Text.Trim();
            eShopPcam.dis_rate4 = this.txtDisRate4.Text.Trim();
            eShopPcam.dis_rate5 = this.txtDisRate5.Text.Trim();
            eShopPcam.dis_rate6 = this.txtDisRate6.Text.Trim();
            eShopPcam.dis_rate7 = this.txtDisRate7.Text.Trim();
            eShopPcam.dis_rate8 = this.txtDisRate8.Text.Trim();
            eShopPcam.dis_rate9 = this.txtDisRate9.Text.Trim();
            eShopPcam.dis_rate10 = this.txtDisRate10.Text.Trim();
            eShopPcam.dis_rate11 = this.txtDisRate11.Text.Trim();
            eShopPcam.dis_rate12 = this.txtDisRate12.Text.Trim();
            eShopPcam.dis_rate13 = this.txtDisRate13.Text.Trim();
            eShopPcam.dis_rate14 = this.txtDisRate14.Text.Trim();
            eShopPcam.dis_rate15 = this.txtDisRate15.Text.Trim();

            eShopPcam.keyin_day = DateTime.Now.ToString("yyyyMMdd");
           // eShopPcam.first_user = eAgentInfo.agent_id;
            eShopPcam.first_user = eAgentInfo.agent_name;
            eShopPcam.second_user = "";
            eShopPcam.send3270 = "N";

            //新增keyin_flag 和 keyin_userID 欄位
            eShopPcam.keyin_flag = "1";
            eShopPcam.keyin_userID = eAgentInfo.agent_id;
            eShopPcam.MER_NAME_TYPE = GetMerNameType();//*帳列類型
          
            try
            {
                return BRSHOP_PCAM.AddEntity(eShopPcam);
            }
            catch
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                }
                return false;
            }
        #endregion
    }


    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 更新資料庫資料

    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateData()
    {
        #region
        EntitySHOP_PCAM eShopPcam = new EntitySHOP_PCAM();
        eShopPcam.organization = "";//*組織
        eShopPcam.merchant_name = GetMerName().ToUpper();//*帳列名稱
        eShopPcam.corp = this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper();//*【統一編號】(1)+【統一編號】(2)
        eShopPcam.merchdba_name = this.txtEngName.Text.Trim().ToUpper();//*英文名稱
        eShopPcam.merchant_nbr = this.txtShopId.Text.Trim().ToUpper();//*商店代號
        eShopPcam.dba_city = this.txtDbaCity.Text.Trim().ToUpper();//*城市
        eShopPcam.zip = this.txtZip.Text.Trim().ToUpper();//*區域編號

        eShopPcam.address1 = this.txtAddress1.Text.Trim().ToUpper();//*【帳單地址】(1)
        eShopPcam.address2 = this.txtAddress2.Text.Trim().ToUpper();//*【帳單地址】(2)
        eShopPcam.address3 = "";//*【帳單地址】(3)
        eShopPcam.address4 = "";
        eShopPcam.phone = this.txtPhone1.Text.Trim() + "-" + this.txtPhone2.Text.Trim();//*連絡電話
        eShopPcam.owner = this.txtOwner.Text.Trim().ToUpper();//*負責人英文名
        eShopPcam.imp1 = this.txtImp1.Text.Trim().ToUpper();//*IMP1
        eShopPcam.pos1 = this.txtPos1.Text.Trim().ToUpper();//*POS1
        eShopPcam.edc1 = this.txtEdc1.Text.Trim().ToUpper();//*EDC1
        eShopPcam.imp2 = this.txtImp2.Text.Trim().ToUpper();//*IMP2
        eShopPcam.pos2 = this.txtPos2.Text.Trim().ToUpper();//*POS2
        eShopPcam.edc2 = this.txtEdc2.Text.Trim().ToUpper();//*EDC2
        eShopPcam.sales_name = this.txtSalesName.Text.Trim().ToUpper();//*推廣姓名
        eShopPcam.agent_bank = this.txtAgentBank.Text.Trim().ToUpper();//*推廣代碼
        eShopPcam.merchant_type = this.txtType.Text.Trim().ToUpper();//*Type
        eShopPcam.chain_store = this.txtStore.Text.Trim().ToUpper();//*STORE
        eShopPcam.chain_merch = this.txtMerch.Text.Trim().ToUpper();//*MERCH
        eShopPcam.chain_level = this.txtLevel.Text.Trim().ToUpper();//*LEVEL
        eShopPcam.Source = this.txtSource.Text.Trim().ToUpper();//*SOURE
        eShopPcam.mcc_VISA = this.txtMcc.Text.Trim().ToUpper();//*MCC
        //eShopPcam.mcc_MC = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_MC = "";
        //eShopPcam.mcc_PL = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_PL = "";
        eShopPcam.avg_tkt = this.txtAve.Text.Trim().ToUpper();//*AVE
        eShopPcam.monthly_volume = this.txtMonth.Text.Trim().ToUpper();//*Month
        eShopPcam.branch = this.txtBranch.Text.Trim().ToUpper();//*BRANCH
        eShopPcam.INT = this.txtInt.Text.Trim().ToUpper();//*INT
        eShopPcam.MAIL = this.txtMail.Text.Trim().ToUpper();//*MAIL
        eShopPcam.POS_CA = this.txtPosCa.Text.Trim().ToUpper();//*Pos CA
        eShopPcam.POS_MO = this.txtPosMo.Text.Trim().ToUpper();//*Pos Mo
        eShopPcam.CH = this.txtCH.Text.Trim().ToUpper();//*C/H
        eShopPcam.MC = this.txtMC.Text.Trim().ToUpper();//*M/C
        eShopPcam.FileNo = this.txtFileNo.Text.Trim().ToUpper();//*歸檔編號
        eShopPcam.hold_stmt = this.txtHOLDSTMT.Text.Trim().ToUpper();//*Hold STMT
        eShopPcam.DDA_NO = this.txtDdaNo1.Text.Trim().ToUpper() + " " + this.txtDdaNo2.Text.Trim().ToUpper();//*【帳號】(1)+’ ’+【帳號】(2)
        //*Status-01-15
        eShopPcam.status1 = this.txtStatus1.Text.Trim().ToUpper();
        eShopPcam.status2 = this.txtStatus2.Text.Trim().ToUpper();
        eShopPcam.status3 = this.txtStatus3.Text.Trim().ToUpper();
        eShopPcam.status4 = this.txtStatus4.Text.Trim().ToUpper();
        eShopPcam.status5 = this.txtStatus5.Text.Trim().ToUpper();
        eShopPcam.status6 = this.txtStatus6.Text.Trim().ToUpper();
        eShopPcam.status7 = this.txtStatus7.Text.Trim().ToUpper();
        eShopPcam.status8 = this.txtStatus8.Text.Trim().ToUpper();
        eShopPcam.status9 = this.txtStatus9.Text.Trim().ToUpper();
        eShopPcam.status10 = this.txtStatus10.Text.Trim().ToUpper();
        eShopPcam.status11 = this.txtStatus11.Text.Trim().ToUpper();
        eShopPcam.status12 = this.txtStatus12.Text.Trim().ToUpper();
        eShopPcam.status13 = this.txtStatus13.Text.Trim().ToUpper();
        eShopPcam.status14 = this.txtStatus14.Text.Trim().ToUpper();
        eShopPcam.status15 = this.txtStatus15.Text.Trim().ToUpper();
        //*費率01-15
        eShopPcam.dis_rate1 = this.txtDisRate1.Text.Trim();
        eShopPcam.dis_rate2 = this.txtDisRate2.Text.Trim();
        eShopPcam.dis_rate3 = this.txtDisRate3.Text.Trim();
        eShopPcam.dis_rate4 = this.txtDisRate4.Text.Trim();
        eShopPcam.dis_rate5 = this.txtDisRate5.Text.Trim();
        eShopPcam.dis_rate6 = this.txtDisRate6.Text.Trim();
        eShopPcam.dis_rate7 = this.txtDisRate7.Text.Trim();
        eShopPcam.dis_rate8 = this.txtDisRate8.Text.Trim();
        eShopPcam.dis_rate9 = this.txtDisRate9.Text.Trim();
        eShopPcam.dis_rate10 = this.txtDisRate10.Text.Trim();
        eShopPcam.dis_rate11 = this.txtDisRate11.Text.Trim();
        eShopPcam.dis_rate12 = this.txtDisRate12.Text.Trim();
        eShopPcam.dis_rate13 = this.txtDisRate13.Text.Trim();
        eShopPcam.dis_rate14 = this.txtDisRate14.Text.Trim();
        eShopPcam.dis_rate15 = this.txtDisRate15.Text.Trim();

        eShopPcam.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        // eShopPcam.first_user = eAgentInfo.agent_id;
        eShopPcam.first_user = eAgentInfo.agent_name;
        eShopPcam.second_user = "";
        eShopPcam.send3270 = "N";

        //新增keyin_flag 和 keyin_userID 欄位
        eShopPcam.keyin_flag = "1";
        eShopPcam.keyin_userID = eAgentInfo.agent_id;
        eShopPcam.MER_NAME_TYPE = GetMerNameType();//*帳列類型

        try
        {
            return BRSHOP_PCAM.Update(eShopPcam, this.txtShopId.Text.Trim().ToUpper(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
            return false;
        }

        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 為頁面欄位賦值
    /// </summary>
    /// <param name="eShopPcam">EntitySHOP_PCAM</param>
    private void SetValues(EntitySHOP_PCAM eShopPcam)
    {
        #region
        try
        {
            this.txtMerchantName.Text = eShopPcam.merchant_name;//*帳列名稱

            //*統一編號
            //if (!string.IsNullOrEmpty(eShopPcam.corp))
            //{
            //    if (eShopPcam.corp.Length >= 8)
            //    {
            //        this.txtCardNo1.Text = eShopPcam.corp.Substring(0, 8);
            //        this.txtCardNo2.Text = eShopPcam.corp.Remove(0, 8);
            //    }
            //    else
            //    {
            //        this.txtCardNo1.Text = eShopPcam.corp;
            //        this.txtCardNo2.Text = "";
            //    }
            //}
            //else
            //{
            //    this.txtCardNo1.Text = "";
            //    this.txtCardNo2.Text = "";
            //}
            this.txtEngName.Text = eShopPcam.merchdba_name;//*英文名稱
            this.txtDbaCity.Text = eShopPcam.dba_city;//*城市
            this.txtZip.Text = eShopPcam.zip;//*區域編號

            this.txtAddress1.Text = eShopPcam.address1;//*【帳單地址】(1)
            this.txtAddress2.Text = eShopPcam.address2;//*【帳單地址】(2)
            //this.txtAddress3.Text = eShopPcam.address3;//*【帳單地址】(3)

            //*連絡電話
            if (!string.IsNullOrEmpty(eShopPcam.phone))
            {
                if (eShopPcam.phone.IndexOf('-') >= 0)
                {
                    string[] strPhones = eShopPcam.phone.Split(new char[] { '-' });
                    this.txtPhone1.Text = strPhones[0];
                    this.txtPhone2.Text = strPhones[1];
                }
                else
                {
                    if (eShopPcam.phone.Length >= 3)
                    {
                        this.txtPhone1.Text = eShopPcam.phone.Substring(0, 3);
                        this.txtPhone2.Text = eShopPcam.phone.Substring(3, eShopPcam.phone.Length - 3);
                    }
                    else
                    {
                        this.txtPhone1.Text = eShopPcam.phone;
                        this.txtPhone2.Text = "";
                    }
                }
            }
            else
            {
                this.txtPhone1.Text = "";
                this.txtPhone2.Text = "";
            }

            this.txtOwner.Text = eShopPcam.owner;//*負責人英文名
            this.txtImp1.Text = eShopPcam.imp1;//*IMP1
            this.txtPos1.Text = eShopPcam.pos1;//*POS1
            this.txtEdc1.Text = eShopPcam.edc1;//*EDC1
            this.txtImp2.Text = eShopPcam.imp2;//*IMP2
            this.txtPos2.Text = eShopPcam.pos2;//*POS2
            this.txtEdc2.Text = eShopPcam.edc2;//*EDC2
            this.txtSalesName.Text = eShopPcam.sales_name;//*推廣姓名
            this.txtAgentBank.Text = eShopPcam.agent_bank;//*推廣代碼
            this.txtType.Text = eShopPcam.merchant_type;//*Type
            this.txtStore.Text = eShopPcam.chain_store;//*STORE
            this.txtMerch.Text = eShopPcam.chain_merch;//*MERCH
            this.txtLevel.Text = eShopPcam.chain_level;//*LEVEL
            this.txtSource.Text = eShopPcam.Source;//*SOURE
            this.txtMcc.Text = eShopPcam.mcc_VISA;//*MCC
            this.txtAve.Text = eShopPcam.avg_tkt;//*AVE
            this.txtMonth.Text = eShopPcam.monthly_volume;//*Month
            this.txtBranch.Text = eShopPcam.branch;//*BRANCH
            this.txtInt.Text = eShopPcam.INT;//*INT
            this.txtMail.Text = eShopPcam.MAIL;//*MAIL
            this.txtPosCa.Text = eShopPcam.POS_CA;//*POS_CA
            this.txtPosMo.Text = eShopPcam.POS_MO;//*POS_MO
            this.txtCH.Text = eShopPcam.CH;//*C/H
            this.txtMC.Text = eShopPcam.MC;//*M/C
            this.txtFileNo.Text = eShopPcam.FileNo;//*歸檔編號
            this.txtHOLDSTMT.Text = eShopPcam.hold_stmt;//*Hold STMT

            SelectRadioButton(eShopPcam.MER_NAME_TYPE.Trim());//*帳列類型選擇

            //*帳號
            if (!string.IsNullOrEmpty(eShopPcam.DDA_NO))
            {
                if (eShopPcam.DDA_NO.Length >= 3)
                {
                    this.txtDdaNo1.Text = eShopPcam.DDA_NO.Substring(0, 3);
                    this.txtDdaNo2.Text = eShopPcam.DDA_NO.Substring(3, eShopPcam.DDA_NO.Length - 3).Trim();
                }
                else
                {
                    this.txtDdaNo1.Text = eShopPcam.DDA_NO;
                    this.txtDdaNo2.Text = "";
                }
            }
            else
            {
                this.txtDdaNo1.Text = "";
                this.txtDdaNo2.Text = "";
            }

            //*Status-01-15
            this.txtStatus1.Text = eShopPcam.status1;
            this.txtStatus2.Text = eShopPcam.status2;
            this.txtStatus3.Text = eShopPcam.status3;
            this.txtStatus4.Text = eShopPcam.status4;
            this.txtStatus5.Text = eShopPcam.status5;
            this.txtStatus6.Text = eShopPcam.status6;
            this.txtStatus7.Text = eShopPcam.status7;
            this.txtStatus8.Text = eShopPcam.status8;
            this.txtStatus9.Text = eShopPcam.status9;
            this.txtStatus10.Text = eShopPcam.status10;
            this.txtStatus11.Text = eShopPcam.status11;
            this.txtStatus12.Text = eShopPcam.status12;
            this.txtStatus13.Text = eShopPcam.status13;
            this.txtStatus14.Text = eShopPcam.status14;
            this.txtStatus15.Text = eShopPcam.status15;

            //*費率01-15
            this.txtDisRate1.Text = eShopPcam.dis_rate1;
            this.txtDisRate2.Text = eShopPcam.dis_rate2;
            this.txtDisRate3.Text = eShopPcam.dis_rate3;
            this.txtDisRate4.Text = eShopPcam.dis_rate4;
            this.txtDisRate5.Text = eShopPcam.dis_rate5;
            this.txtDisRate6.Text = eShopPcam.dis_rate6;
            this.txtDisRate7.Text = eShopPcam.dis_rate7;
            this.txtDisRate8.Text = eShopPcam.dis_rate8;
            this.txtDisRate9.Text = eShopPcam.dis_rate9;
            this.txtDisRate10.Text = eShopPcam.dis_rate10;
            this.txtDisRate11.Text = eShopPcam.dis_rate11;
            this.txtDisRate12.Text = eShopPcam.dis_rate12;
            this.txtDisRate13.Text = eShopPcam.dis_rate13;
            this.txtDisRate14.Text = eShopPcam.dis_rate14;
            this.txtDisRate15.Text = eShopPcam.dis_rate15;
        }
        catch(Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
        #endregion

    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 得到資料庫中{費率01}~{費率08}顯示在網頁中的值

    /// </summary>
    /// <param name="strDataValue">資料庫中費率</param>
    /// <param name="txtBox">費率對應的TextBox</param>
    private void GetFeeValue(string strDataValue, CustTextBox txtBox)
    {
        if (!string.IsNullOrEmpty(strDataValue) && int.Parse(strDataValue) != 0)
        {
            float intFeeValue = float.Parse(strDataValue) / 1000;
            txtBox.Text = intFeeValue.ToString();
            txtBox.Enabled = true;
        }
        else
        {
            txtBox.Text = "";
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 得到資料庫中{費率01}~{費率08}顯示在網頁中的值

    /// </summary>
    /// <param name="strDataValue">資料庫中費率</param>
    /// <param name="txtBox">費率對應的TextBox</param>
    /// <param name="strStatus">費率狀態</param>
    private void GetFeeValue(string strDataValue, CustTextBox txtBox,string strStatus)
    {
        if (!string.IsNullOrEmpty(strDataValue) && int.Parse(strDataValue) != 0)
        {
            float intFeeValue = float.Parse(strDataValue) / 1000;
            txtBox.Text = intFeeValue.ToString();
            //txtBox.Enabled = true;
        }
        else
        {
            txtBox.Text = "";
        }

        if (!string.IsNullOrEmpty(strStatus) && int.Parse(strStatus) != 0)
        {
            txtBox.Enabled = true; 
        }
    }

    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData()
    { 
        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());//*統編序號CORP1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統編序號CORP2
        htInput.Add("FORCE_FLAG", "");

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            lblRegNameText.Text = htReturn["REG_NAME"].ToString();
            lblBusinessNameText.Text = htReturn["BUSINESS_NAME"].ToString();
            return true;
        }
        else
        {
            //*異動主機資料失敗
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htReturn["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }
            return false;
        }
    }

    /// <summary>
    /// 得到賬列名稱類型
    /// </summary>
    /// <returns>類型</returns>
    private string GetMerNameType()
    { 
        if(this.radRegName.Checked)
        {
            return "1";
        }

        if (this.radBusinessName.Checked)
        {
            return "2";
        }

        if (this.radMerchantName.Checked)
        {
            return "3";
        }
        return "";
    }

    /// <summary>
    /// 得到賬列名稱
    /// </summary>
    /// <returns>賬列名稱</returns>
    private string GetMerName()
    {
        if (this.radRegName.Checked)
        {
            return lblRegNameText.Text.Trim();
        }

        if (this.radBusinessName.Checked)
        {
            return lblBusinessNameText.Text.Trim();
        }

        if (this.radMerchantName.Checked)
        {
            return txtMerchantName.Text.Trim();
        }
        return "";
    }

    /// <summary>
    /// 根據賬列名稱類型選擇對應RadioButton
    /// </summary>
    /// <param name="strValue">類型</param>
    private void SelectRadioButton(string strValue)
    { 
        if(strValue == "1")
        {
            this.radRegName.Checked = true;
        }

        if (strValue == "2")
        {
            this.radBusinessName.Checked = true;
        }

        if (strValue == "3")
        {
            this.radMerchantName.Checked = true;
        }
    }

    /// <summary>
    /// 清空提交部分畫面
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);
        this.radRegName.Checked = false;
        this.radBusinessName.Checked = false;
        this.radMerchantName.Checked = false;
        this.radRegName.Enabled = blnEnabled;
        this.radBusinessName.Enabled = blnEnabled;
        this.radMerchantName.Enabled = blnEnabled;
        lblRegNameText.Text = "";
        lblBusinessNameText.Text = "";
    }

    /// <summary>
    /// 清空網頁所有內容
    /// </summary>
    private void ClearAll()
    {
        ClearPage(false);
        this.txtShopId.Text = "";
        this.txtShopIdHide.Text = "";
        this.txtCardNo1.Text = "";
        this.txtCardNo1Hide.Text = "";
        this.txtCardNo2.Text = "";
        this.txtCardNo2Hide.Text = "";
    }

    //*檢核商店代號
    private void CheckShopId()
    {
        if (CommonFunction.GetSubString(this.txtShopId.Text.Trim(), 0, 5) == "88320")
        {
            //*Status-01-15
            this.txtStatus1.Text = "0";
            this.txtStatus2.Text = "0";
            this.txtStatus3.Text = "0";
            this.txtStatus4.Text = "0";
            this.txtStatus5.Text = "0";
            this.txtStatus6.Text = "0";
            this.txtStatus7.Text = "0";
            this.txtStatus8.Text = "0";
            this.txtStatus9.Text = "0";
            this.txtStatus10.Text = "0";
            this.txtStatus11.Text = "0";
            this.txtStatus12.Text = "0";
            this.txtStatus13.Text = "0";
            this.txtStatus14.Text = "0";
            this.txtStatus15.Text = "0";

            this.txtStatus1.Enabled = false;
            this.txtStatus2.Enabled = false;
            this.txtStatus3.Enabled = false;
            this.txtStatus4.Enabled = false;
            this.txtStatus5.Enabled = false;
            this.txtStatus6.Enabled = false;
            this.txtStatus7.Enabled = false;
            this.txtStatus8.Enabled = false;
            this.txtStatus9.Enabled = false;
            this.txtStatus10.Enabled = false;
            this.txtStatus11.Enabled = false;
            this.txtStatus12.Enabled = false;
            this.txtStatus13.Enabled = false;
            this.txtStatus14.Enabled = false;
            this.txtStatus15.Enabled = false;

            //*費率01-15
            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            this.txtDisRate10.Enabled = false;
            this.txtDisRate11.Enabled = false;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = false;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }

        string strNum = CommonFunction.GetSubString(this.txtShopId.Text.Trim(), 2, 3);
        if (strNum == "251" || strNum == "252" || strNum == "253" || strNum == "254")
        {
            //*Status-01-15
            this.txtStatus1.Text = "0";
            this.txtStatus2.Text = "0";
            this.txtStatus3.Text = "0";
            this.txtStatus4.Text = "0";
            this.txtStatus5.Text = "0";
            this.txtStatus6.Text = "0";
            this.txtStatus7.Text = "0";
            this.txtStatus8.Text = "0";
            this.txtStatus9.Text = "0";
            this.txtStatus10.Text = "1";
            this.txtStatus11.Text = "1";
            this.txtStatus12.Text = "0";
            this.txtStatus13.Text = "1";
            this.txtStatus14.Text = "0";
            this.txtStatus15.Text = "0";

            this.txtStatus1.Enabled = false;
            this.txtStatus2.Enabled = false;
            this.txtStatus3.Enabled = false;
            this.txtStatus4.Enabled = false;
            this.txtStatus5.Enabled = false;
            this.txtStatus6.Enabled = false;
            this.txtStatus7.Enabled = false;
            this.txtStatus8.Enabled = false;
            this.txtStatus9.Enabled = false;
            this.txtStatus10.Enabled = true;
            this.txtStatus11.Enabled = true;
            this.txtStatus12.Enabled = false;
            this.txtStatus13.Enabled = true;
            this.txtStatus14.Enabled = false;
            this.txtStatus15.Enabled = false;

            //*費率01-15
            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            this.txtDisRate10.Enabled = true;
            this.txtDisRate11.Enabled = true;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = true;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }

        if (this.txtShopId.Text.Trim().IndexOf('5') == 2)
        {
            this.txtDisRate1.Text = "0";
            this.txtDisRate2.Text = "0";
            this.txtDisRate3.Text = "0";
            this.txtDisRate4.Text = "0";
            this.txtDisRate5.Text = "0";
            this.txtDisRate6.Text = "0";
            this.txtDisRate7.Text = "0";
            this.txtDisRate8.Text = "0";
            this.txtDisRate9.Text = "0";
            this.txtDisRate10.Text = "0";
            this.txtDisRate11.Text = "0";
            this.txtDisRate12.Text = "0";
            this.txtDisRate13.Text = "0";
            this.txtDisRate14.Text = "0";
            this.txtDisRate15.Text = "0";
        }
    }
}
