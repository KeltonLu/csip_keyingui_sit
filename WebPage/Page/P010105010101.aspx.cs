//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：Redeem點數折抵參數設定一Key
//*  創建日期：2010/06/25
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
using Framework.WebControls;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;

public partial class P010105010101 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            fill_ddlPROGRAM();
            isEnableAll(false, true);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        isEnableFor_UE_B();
        base.strClientMsg += "";
        base.strHostMsg += "";
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        isEnableAll(false, true);
        if (!txtReceiveNumber.Enabled)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        EntitySet<EntityRedeemSet> esRS = new EntityRedeemSetSet();
        esRS = BRRedeemSet.SelectData(txtReceiveNumber.Text, "1");

        if (esRS.Count > 0)
        {
            if ("Y" == esRS.GetEntity(0).SEND3270.ToUpper())
            {
                // 已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050100_001") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                return;
            }
            /*
                        EntitySet<EntityRedeemSet> esRS2 = new EntityRedeemSetSet();
                        esRS2 = BRRedeemSet.SelectData(txtReceiveNumber.Text, "2");
                        if (esRS2.Count > 0)
                        {
                            // 二Key已完成
                            base.strClientMsg += MessageHelper.GetMessage("01_01050100_006");
                            return;
                        }
            */
            isEnableAll(true, true);
            setValue(esRS.GetEntity(0), BRRedeemSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "1"));


        }
        else
        {
            // 申請人
            lbUSERValue.Text = ((EntityAGENT_INFO)Session["Agent"]).agent_name;

            isEnableFirst(true);
            txtORG.Focus();
            setDefaultValue();
        }
    }

    protected void ddlPROGRAM_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtPROGRAM.Text = "";

        if (ddlPROGRAM.SelectedValue != "-1")
        {
            txtPROGRAM.Text = ddlPROGRAM.SelectedValue.ToString().Trim();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        EntitySet<EntityRedeemSet> esRS = new EntityRedeemSetSet();
        esRS = BRRedeemSet.SelectData(txtReceiveNumber.Text, "1");

        if (esRS.Count > 0)
        {
            // Update
            if (UpdateRedeemSet())
            {
                isEnableAll(false, true);
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_013");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }

        }
        else
        {
            // Add
            if (InsertRedeemSet())
            {
                isEnableAll(false, true);
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_013");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }
        }

    }

    protected void btnSelectMF_Click(object sender, EventArgs e)
    {
        bool bMerNoExist = false;
        int iIndex = 1;

        for (int i = 1; i < 11; i++)
        {
            CustTextBox ctb = (CustTextBox)this.FindControl("txtMERCHANTNO" + i.ToString());
            if (null != ctb)
            {
                if ("" != ctb.Text.Trim())
                {
                    switch (txtORG.Text.Trim())
                    {
                        case "822":
                            bMerNoExist = SelectPCMMP4A(ctb.Text.Trim());
                            break;
                        case "900":
                            bMerNoExist = SelectPCMMP4(ctb.Text.Trim());
                            break;
                        default:
                            bMerNoExist = false;
                            break;
                    }
                }

                if (bMerNoExist == false)
                {
                    iIndex = i;
                    break;
                }
            }
        }

        if (bMerNoExist)
        {
            isEnableAll(true, false);
        }
        else
        {
            isEnableAll(false, false);
            isEnableFirst(true);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtMERCHANTNO" + iIndex.ToString()));
        }

    }
    #endregion

    #region method
    private void isEnableFirst(bool bIsEnable)
    {
        if (bIsEnable)
        {
            ViewState["lStartTime"] = System.Environment.TickCount;  // 開始 keyin 時間
        }

        txtORG.Enabled = bIsEnable;

        for (int i = 1; i < 11; i++)
        {
            CustTextBox ctb = (CustTextBox)this.FindControl("txtMERCHANTNO" + i.ToString());
            if (null != ctb)
            {
                ctb.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctb.Text = "";
                }
            }
        }

        btnSelectMF.Enabled = bIsEnable;
    }

    /// <summary>
    /// 設定所有的輸入框是否可以使用
    /// </summary>
    /// <param name="bIsEnable">true：可以使用，false：不能使用</param>
    private void isEnableAll(bool bIsEnable, bool bIsClear)
    {
        if (bIsEnable)
        {
            ViewState["lStartTime"] = System.Environment.TickCount;  // 開始 keyin 時間
        }
        //user control設定
        atl_AT1.IsEnable = bIsEnable;
        atl_AT2.IsEnable = bIsEnable;
        atl_AT3.IsEnable = bIsEnable;

        atl_USEREXITAT1.IsEnable = bIsEnable;
        atl_USEREXITAT2.IsEnable = bIsEnable;
        atl_USEREXITAT3.IsEnable = bIsEnable;

        atl_BIRTH1.IsEnable = bIsEnable;
        atl_BIRTH2.IsEnable = bIsEnable;
        atl_BIRTH3.IsEnable = bIsEnable;

        //畫面control設定
        txtORG.Enabled = bIsEnable;

        for (int i = 1; i < 11; i++)
        {
            CustTextBox ctb = (CustTextBox)this.FindControl("txtMERCHANTNO" + i.ToString());
            if (null != ctb)
            {
                ctb.Enabled = bIsEnable;

                if (!bIsEnable && bIsClear)
                {
                    ctb.Text = "";
                }
            }
        }

        txtPRODCODE.Enabled = bIsEnable;
        txtPROGRAM.Enabled = bIsEnable;
        txtMERCHANT_INT.Enabled = bIsEnable;
        txtMERCHANT_DEC.Enabled = bIsEnable;
        txtUSEREXIT_Type.Enabled = bIsEnable;
        txtUSEREXIT_DateS.Enabled = bIsEnable;
        txtUSEREXIT_DateE.Enabled = bIsEnable;
        txtUSEREXIT_OAOS.Enabled = bIsEnable;
        txtBIRTH_Type.Enabled = bIsEnable;
        txtBIRTH_DateS.Enabled = bIsEnable;
        txtBIRTH_DateE.Enabled = bIsEnable;

        ddlPROGRAM.Enabled = bIsEnable;

        if (!bIsEnable && bIsClear)
        {
            txtORG.Text = "";
            txtPRODCODE.Text = "";
            txtPROGRAM.Text = "";
            txtMERCHANT_INT.Text = "";
            txtMERCHANT_DEC.Text = "";
            txtUSEREXIT_Type.Text = "";
            txtUSEREXIT_DateS.Text = "";
            txtUSEREXIT_DateE.Text = "";
            txtUSEREXIT_OAOS.Text = "";
            txtBIRTH_Type.Text = "";
            txtBIRTH_DateS.Text = "";
            txtBIRTH_DateE.Text = "";

            ddlPROGRAM.SelectedIndex = 0;
        }

        btnAdd.Enabled = bIsEnable;
        btnSelectMF.Enabled = bIsEnable;

        // 開始填寫資料時鎖定手機編號輸入框
        txtReceiveNumber.Enabled = !bIsEnable;
    }

    private void isEnableFor_UE_B()
    {
        if ("01" == txtUSEREXIT_Type.Text.Trim() || "02" == txtUSEREXIT_Type.Text.Trim() || "03" == txtUSEREXIT_Type.Text.Trim())
        {
            atl_USEREXITAT1.IsEnable = true;
            atl_USEREXITAT2.IsEnable = true;
            atl_USEREXITAT3.IsEnable = true;
            txtUSEREXIT_DateS.Enabled = true;
            txtUSEREXIT_DateE.Enabled = true;
            txtUSEREXIT_OAOS.Enabled = true;
        }
        else
        {
            atl_USEREXITAT1.IsEnable = false;
            atl_USEREXITAT2.IsEnable = false;
            atl_USEREXITAT3.IsEnable = false;
            txtUSEREXIT_DateS.Enabled = false;
            txtUSEREXIT_DateE.Enabled = false;
            txtUSEREXIT_OAOS.Enabled = false;
        }

        if ("01" == txtBIRTH_Type.Text.Trim() || "02" == txtBIRTH_Type.Text.Trim() || "03" == txtBIRTH_Type.Text.Trim())
        {
            atl_BIRTH1.IsEnable = true;
            atl_BIRTH2.IsEnable = true;
            atl_BIRTH3.IsEnable = true;
            txtBIRTH_DateS.Enabled = true;
            txtBIRTH_DateE.Enabled = true;
        }
        else
        {
            atl_BIRTH1.IsEnable = false;
            atl_BIRTH2.IsEnable = false;
            atl_BIRTH3.IsEnable = false;
            txtBIRTH_DateS.Enabled = false;
            txtBIRTH_DateE.Enabled = false;
        }
    }

    private void setDefaultValue()
    {
        txtORG.Text = "822";
        txtPRODCODE.Text = "00";
        txtUSEREXIT_Type.Text = "00";
        //txtUSEREXIT_OAOS.Text = "000000";
        txtBIRTH_Type.Text = "00";
        txtMERCHANT_DEC.Text = "00";

        string strControlName = "tbTBBLimit_DEC";

        ((CustTextBox)atl_AT1.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_AT2.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_AT3.FindControl(strControlName)).Text = "00";

        ((CustTextBox)atl_USEREXITAT1.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_USEREXITAT2.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_USEREXITAT3.FindControl(strControlName)).Text = "00";

        ((CustTextBox)atl_BIRTH1.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_BIRTH2.FindControl(strControlName)).Text = "00";
        ((CustTextBox)atl_BIRTH3.FindControl(strControlName)).Text = "00";
    }

    private void fill_ddlPROGRAM()
    {
        EntitySet<EntityProgramList> esPL = new EntitySet<EntityProgramList>();

        esPL = BRProgramList.Search("");

        ddlPROGRAM.Items.Clear();

        ddlPROGRAM.Items.Add(new ListItem(BaseHelper.GetShowText("01_00000000_003"), "-1"));

        if (esPL.Count > 0)
        {
            for (int i = 0; i < esPL.Count; i++)
            {
                ddlPROGRAM.Items.Add(new ListItem(esPL.GetEntity(i).MEMO.ToString().Trim(), esPL.GetEntity(i).CODE.ToString().Trim()));
            }
        }
    }

    private bool InsertRedeemSet()
    {
        try
        {
            return BRRedeemSet.InsertKey1(entityCreate(), RedeemSet_ATypeSetCreate());
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }

            return false;
        }
    }

    private bool UpdateRedeemSet()
    {
        try
        {
            return BRRedeemSet.UpdateKey1(txtReceiveNumber.Text.Trim(), entityCreate(), RedeemSet_ATypeSetCreate());
        }
        catch (Exception e)
        {
            base.strClientMsg = MessageHelper.GetMessage("01_00000000_021") + e;

            return false;
        }
    }

    private EntitySet<EntityRedeemSet_ATypeSet> RedeemSet_ATypeSetCreate()
    {
        int iCount = 0; //ACCUMLATION TYPE中Card Type輸入框個數
        CustTextBox ctbTmp;

        BRCardTypeList_Redeem.Select(1, 1, "", ref iCount);

        EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS = new EntitySet<EntityRedeemSet_ATypeSet>();

        for (int i = 0; i < iCount; i++)
        {
            EntityRedeemSet_ATypeSet eRS_ATS = new EntityRedeemSet_ATypeSet();
            eRS_ATS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
            eRS_ATS.KEYIN_FLAG = "1";

            // 長期活動

            ctbTmp = (CustTextBox)atl_AT1.FindControl("tb_AType" + i.ToString());

            eRS_ATS.TXT_INDEX = i.ToString();

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "L";
                eRS_ATS.STEP = "1";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }

            ctbTmp = (CustTextBox)atl_AT2.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "L";
                eRS_ATS.STEP = "2";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            ctbTmp = (CustTextBox)atl_AT3.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "L";
                eRS_ATS.STEP = "3";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            // 短期促銷

            ctbTmp = (CustTextBox)atl_USEREXITAT1.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "U";
                eRS_ATS.STEP = "1";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            ctbTmp = (CustTextBox)atl_USEREXITAT2.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "U";
                eRS_ATS.STEP = "2";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            ctbTmp = (CustTextBox)atl_USEREXITAT3.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "U";
                eRS_ATS.STEP = "3";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }

            // 生日活動

            ctbTmp = (CustTextBox)atl_BIRTH1.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "B";
                eRS_ATS.STEP = "1";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            ctbTmp = (CustTextBox)atl_BIRTH2.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "B";
                eRS_ATS.STEP = "2";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }


            ctbTmp = (CustTextBox)atl_BIRTH3.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                eRS_ATS.ACTIVITY = "B";
                eRS_ATS.STEP = "3";
                eRS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                EntityRedeemSet_ATypeSet eRS_ATS_Tmp = new EntityRedeemSet_ATypeSet();
                eRS_ATS_Tmp.Clone(eRS_ATS);
                esRS_ATS.Add(eRS_ATS_Tmp);
            }
        }

        return esRS_ATS;

    }

    private EntityRedeemSet entityCreate()
    {
        EntityRedeemSet eRS = new EntityRedeemSet();

        eRS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
        eRS.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eRS.KEYIN_FLAG = "1";
        eRS.SEND3270 = "N";
        eRS.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
        eRS.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
        eRS.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
        eRS.ISSAME = "";
        eRS.DIFF_NUM = 0;
        eRS.EDIT_USER_ID = "";

        eRS.ORG = txtORG.Text.Trim();
        eRS.MERCHANT_NO1 = txtMERCHANTNO1.Text.Trim();
        eRS.MERCHANT_NO2 = txtMERCHANTNO2.Text.Trim();
        eRS.MERCHANT_NO3 = txtMERCHANTNO3.Text.Trim();
        eRS.MERCHANT_NO4 = txtMERCHANTNO4.Text.Trim();
        eRS.MERCHANT_NO5 = txtMERCHANTNO5.Text.Trim();
        eRS.MERCHANT_NO6 = txtMERCHANTNO6.Text.Trim();
        eRS.MERCHANT_NO7 = txtMERCHANTNO7.Text.Trim();
        eRS.MERCHANT_NO8 = txtMERCHANTNO8.Text.Trim();
        eRS.MERCHANT_NO9 = txtMERCHANTNO9.Text.Trim();
        eRS.MERCHANT_NO10 = txtMERCHANTNO10.Text.Trim();
        eRS.PROD_CODE = txtPRODCODE.Text.Trim();
        eRS.PROGRAM = txtPROGRAM.Text.Trim();
        eRS.MERCHANT = txtMERCHANT_INT.Text.Trim() + txtMERCHANT_DEC.Text.Trim();
        eRS.USEREXIT_TYPE = txtUSEREXIT_Type.Text.Trim();
        eRS.USEREXIT_DateS = txtUSEREXIT_DateS.Text.Trim();
        eRS.USEREXIT_DateE = txtUSEREXIT_DateE.Text.Trim();
        eRS.USEREXIT_OAOS = txtUSEREXIT_OAOS.Text.Trim().ToUpper();
        eRS.BIRTH_TYPE = txtBIRTH_Type.Text.Trim();
        eRS.BIRTH_DateS = txtBIRTH_DateS.Text.Trim();
        eRS.BIRTH_DateE = txtBIRTH_DateE.Text.Trim();

        // 長期活動
        eRS.LONG_LIMIT1 = ((CustTextBox)atl_AT1.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_AT1.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.LONG_Points1 = ((CustTextBox)atl_AT1.FindControl("tbTBBPoints")).Text.Trim();
        eRS.LONG_Amount1 = ((CustTextBox)atl_AT1.FindControl("tbTBBAmount")).Text.Trim();

        eRS.LONG_LIMIT2 = ((CustTextBox)atl_AT2.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_AT2.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.LONG_Points2 = ((CustTextBox)atl_AT2.FindControl("tbTBBPoints")).Text.Trim();
        eRS.LONG_Amount2 = ((CustTextBox)atl_AT2.FindControl("tbTBBAmount")).Text.Trim();

        eRS.LONG_LIMIT3 = ((CustTextBox)atl_AT3.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_AT3.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.LONG_Points3 = ((CustTextBox)atl_AT3.FindControl("tbTBBPoints")).Text.Trim();
        eRS.LONG_Amount3 = ((CustTextBox)atl_AT3.FindControl("tbTBBAmount")).Text.Trim();

        // 短期促銷
        eRS.USEREXIT_LIMIT1 = ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.USEREXIT_Points1 = ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBPoints")).Text.Trim();
        eRS.USEREXIT_Amount1 = ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBAmount")).Text.Trim();

        eRS.USEREXIT_LIMIT2 = ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.USEREXIT_Points2 = ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBPoints")).Text.Trim();
        eRS.USEREXIT_Amount2 = ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBAmount")).Text.Trim();

        eRS.USEREXIT_LIMIT3 = ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.USEREXIT_Points3 = ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBPoints")).Text.Trim();
        eRS.USEREXIT_Amount3 = ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBAmount")).Text.Trim();

        // 生日活動
        eRS.BIRTH_LIMIT1 = ((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.BIRTH_Points1 = ((CustTextBox)atl_BIRTH1.FindControl("tbTBBPoints")).Text.Trim();
        eRS.BIRTH_Amount1 = ((CustTextBox)atl_BIRTH1.FindControl("tbTBBAmount")).Text.Trim();

        eRS.BIRTH_LIMIT2 = ((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.BIRTH_Points2 = ((CustTextBox)atl_BIRTH2.FindControl("tbTBBPoints")).Text.Trim();
        eRS.BIRTH_Amount2 = ((CustTextBox)atl_BIRTH2.FindControl("tbTBBAmount")).Text.Trim();

        eRS.BIRTH_LIMIT3 = ((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_INT")).Text.Trim() + ((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_DEC")).Text.Trim();
        eRS.BIRTH_Points3 = ((CustTextBox)atl_BIRTH3.FindControl("tbTBBPoints")).Text.Trim();
        eRS.BIRTH_Amount3 = ((CustTextBox)atl_BIRTH3.FindControl("tbTBBAmount")).Text.Trim();

        return eRS;
    }

    private void setValue(EntityRedeemSet eRS, EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS)
    {
        #region basic_data
        // 申請人
        lbUSERValue.Text = ((EntityAGENT_INFO)Session["Agent"]).agent_name;

        txtORG.Text = eRS.ORG.Trim();
        txtMERCHANTNO1.Text = eRS.MERCHANT_NO1.Trim();
        txtMERCHANTNO2.Text = eRS.MERCHANT_NO2.Trim();
        txtMERCHANTNO3.Text = eRS.MERCHANT_NO3.Trim();
        txtMERCHANTNO4.Text = eRS.MERCHANT_NO4.Trim();
        txtMERCHANTNO5.Text = eRS.MERCHANT_NO5.Trim();
        txtMERCHANTNO6.Text = eRS.MERCHANT_NO6.Trim();
        txtMERCHANTNO7.Text = eRS.MERCHANT_NO7.Trim();
        txtMERCHANTNO8.Text = eRS.MERCHANT_NO8.Trim();
        txtMERCHANTNO9.Text = eRS.MERCHANT_NO9.Trim();
        txtMERCHANTNO10.Text = eRS.MERCHANT_NO10.Trim();
        txtPRODCODE.Text = eRS.PROD_CODE.Trim();
        txtPROGRAM.Text = eRS.PROGRAM.Trim();

        txtMERCHANT_INT.Text = IntDecSplit(eRS.MERCHANT, "I");
        txtMERCHANT_DEC.Text = IntDecSplit(eRS.MERCHANT, "D");

        txtUSEREXIT_Type.Text = eRS.USEREXIT_TYPE.Trim();
        txtUSEREXIT_DateS.Text = eRS.USEREXIT_DateS.Trim();
        txtUSEREXIT_DateE.Text = eRS.USEREXIT_DateE.Trim();
        txtUSEREXIT_OAOS.Text = eRS.USEREXIT_OAOS.Trim();
        txtBIRTH_Type.Text = eRS.BIRTH_TYPE.Trim();
        txtBIRTH_DateS.Text = eRS.BIRTH_DateS.Trim();
        txtBIRTH_DateE.Text = eRS.BIRTH_DateE.Trim();

        //長期活動
        ((CustTextBox)atl_AT1.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.LONG_LIMIT1, "I");
        ((CustTextBox)atl_AT1.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.LONG_LIMIT1, "D");

        ((CustTextBox)atl_AT1.FindControl("tbTBBPoints")).Text = eRS.LONG_Points1.Trim();
        ((CustTextBox)atl_AT1.FindControl("tbTBBAmount")).Text = eRS.LONG_Amount1.Trim();

        ((CustTextBox)atl_AT2.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.LONG_LIMIT2, "I");
        ((CustTextBox)atl_AT2.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.LONG_LIMIT2, "D");
        ((CustTextBox)atl_AT2.FindControl("tbTBBPoints")).Text = eRS.LONG_Points2.Trim();
        ((CustTextBox)atl_AT2.FindControl("tbTBBAmount")).Text = eRS.LONG_Amount2.Trim();

        ((CustTextBox)atl_AT3.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.LONG_LIMIT3, "I");
        ((CustTextBox)atl_AT3.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.LONG_LIMIT3, "D");
        ((CustTextBox)atl_AT3.FindControl("tbTBBPoints")).Text = eRS.LONG_Points3.Trim();
        ((CustTextBox)atl_AT3.FindControl("tbTBBAmount")).Text = eRS.LONG_Amount3.Trim();

        //短期促銷
        ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.USEREXIT_LIMIT1, "I");
        ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.USEREXIT_LIMIT1, "D");
        ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBPoints")).Text = eRS.USEREXIT_Points1.Trim();
        ((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBAmount")).Text = eRS.USEREXIT_Amount1.Trim();

        ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.USEREXIT_LIMIT2, "I");
        ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.USEREXIT_LIMIT2, "D");
        ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBPoints")).Text = eRS.USEREXIT_Points2.Trim();
        ((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBAmount")).Text = eRS.USEREXIT_Amount2.Trim();

        ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.USEREXIT_LIMIT3, "I");
        ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.USEREXIT_LIMIT3, "D");
        ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBPoints")).Text = eRS.USEREXIT_Points3.Trim();
        ((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBAmount")).Text = eRS.USEREXIT_Amount3.Trim();

        //生日活動
        ((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.BIRTH_LIMIT1, "I");
        ((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.BIRTH_LIMIT1, "D");
        ((CustTextBox)atl_BIRTH1.FindControl("tbTBBPoints")).Text = eRS.BIRTH_Points1.Trim();
        ((CustTextBox)atl_BIRTH1.FindControl("tbTBBAmount")).Text = eRS.BIRTH_Amount1.Trim();

        ((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.BIRTH_LIMIT2, "I");
        ((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.BIRTH_LIMIT2, "D");
        ((CustTextBox)atl_BIRTH2.FindControl("tbTBBPoints")).Text = eRS.BIRTH_Points2.Trim();
        ((CustTextBox)atl_BIRTH2.FindControl("tbTBBAmount")).Text = eRS.BIRTH_Amount2.Trim();

        ((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_INT")).Text = IntDecSplit(eRS.BIRTH_LIMIT3, "I");
        ((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_DEC")).Text = IntDecSplit(eRS.BIRTH_LIMIT3, "D");
        ((CustTextBox)atl_BIRTH3.FindControl("tbTBBPoints")).Text = eRS.BIRTH_Points3.Trim();
        ((CustTextBox)atl_BIRTH3.FindControl("tbTBBAmount")).Text = eRS.BIRTH_Amount3.Trim();
        #endregion basic_data

        #region CardType_data
        for (int i = 0; i < esRS_ATS.Count; i++)
        {
            EntityRedeemSet_ATypeSet eRS_ATS = new EntityRedeemSet_ATypeSet();

            eRS_ATS = esRS_ATS.GetEntity(i);

            // 長期活動
            if ("L" == eRS_ATS.ACTIVITY)
            {
                switch (eRS_ATS.STEP)
                {
                    case "1":
                        ((CustTextBox)atl_AT1.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "2":
                        ((CustTextBox)atl_AT2.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "3":
                        ((CustTextBox)atl_AT3.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    default:
                        break;
                }
            }

            // 短期促銷
            if ("U" == eRS_ATS.ACTIVITY)
            {
                switch (eRS_ATS.STEP)
                {
                    case "1":
                        ((CustTextBox)atl_USEREXITAT1.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "2":
                        ((CustTextBox)atl_USEREXITAT2.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "3":
                        ((CustTextBox)atl_USEREXITAT3.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    default:
                        break;
                }
            }

            // 生日活動
            if ("B" == eRS_ATS.ACTIVITY)
            {
                switch (eRS_ATS.STEP)
                {
                    case "1":
                        ((CustTextBox)atl_BIRTH1.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "2":
                        ((CustTextBox)atl_BIRTH2.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    case "3":
                        ((CustTextBox)atl_BIRTH3.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString().Trim())).Text = eRS_ATS.CARD_TYPE.Trim();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion CardType_data
    }

    private string IntDecSplit(string strBe, string strType)
    {
        strBe = strBe.Trim();
        string strR = "";

        if ("I" == strType.ToUpper())
        {
            if (strBe.Length > 2)
            {
                strR = strBe.Substring(0, strBe.Length - 2);
            }
        }
        else
        {
            if (strBe.Length > 2)
            {
                strR = strBe.Substring(strBe.Length - 2, 2);
            }
            else
            {
                strR = strBe;
            }
        }

        return strR;
    }

    /// <summary>
    /// 查詢主機PCMMP4
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool SelectPCMMP4(string strMERCHANTNO)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", strMERCHANTNO.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "900");

        Hashtable htPcmmP4 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInput, false, "1", eAgentInfo);
        if (!htPcmmP4.Contains("HtgMsg"))
        {
            if ("8" != htPcmmP4["STATUS_FLAG"].ToString())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 查詢主機PCMMP4
    /// </summary>
    /// <returns>
    /// true:商店資料存在,且解約代碼<>8
    /// false:商店資料不存在,或主機交易失敗
    /// </returns>
    /// 
    private bool SelectPCMMP4A(string strMERCHANTNO)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", strMERCHANTNO.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "822");

        Hashtable htPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);

        if (!htPcmmP4A.Contains("HtgMsg"))
        {
            base.strHostMsg = htPcmmP4A["HtgSuccess"].ToString();
            if ("8" != htPcmmP4A["STATUS_FLAG"].ToString())
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01050100_008");
                return true;
            }
            else
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01050100_007");
                return false;
            }
        }
        else //Message-Type <> 0000 OR HTG 交易失敗
        {
            base.strHostMsg = htPcmmP4A["HtgMsg"].ToString();
            if (htPcmmP4A["MESSAGE_TYPE"].ToString() == "S014")
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01050100_009");
            }
            else
            {
                base.strClientMsg = MessageHelper.GetMessage("01_01050100_010");
            }

            return false;
        }
    }
    #endregion

}
