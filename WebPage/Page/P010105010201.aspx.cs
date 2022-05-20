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
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.WebControls;
using System.Drawing;
using System.Collections.Generic;

public partial class P010105010201 : PageBase
{
    #region Variable
    private EntityAGENT_INFO eAgentInfo;
    #endregion Variable

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        //*查詢資料檔Job_Status
        EntitySet<EntityJOB_STATUS> eJobStatusSet = BRJOB_STATUS.SelectEntitySet("1", "Job_LGOR");

        if (eJobStatusSet != null && eJobStatusSet.Count > 0)
        {
            string strAlertMsg = @"『Redeem 點數折抵參數設定 批次上傳主機中 , 請稍後再進行二 KEY 作業 , \r\n 批次上傳起始時間 : " +
                            ((DateTime)eJobStatusSet.GetEntity(0).start_time).ToString("HH:mm:ss") +
                            @" \r\n 預估作業時間 : " + ((int)(((int)eJobStatusSet.GetEntity(0).data_count) * 3 / 60)).ToString() + " 分鐘』";

            sbRegScript.Append("alert('" + strAlertMsg + "');history.go(-1);");

            return;
        }

        if (!Page.IsPostBack)
        {
            fill_ddlPROGRAM();
            isEnableAll(false);
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
        if (!txtReceiveNumber.Enabled)
        {
            isEnableAll(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        // 查詢一KEY資料
        EntitySet<EntityRedeemSet> esRS_1Key = new EntitySet<EntityRedeemSet>();
        EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS = new EntitySet<EntityRedeemSet_ATypeSet>();
        try
        {
            esRS_1Key = BRRedeemSet.SelectData(txtReceiveNumber.Text.Trim(), "1");
            esRS_ATS = BRRedeemSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if (null == esRS_1Key || esRS_1Key.Count < 1)
        {
            //*沒有一Key資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050100_011") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if ("Y" == esRS_1Key.GetEntity(0).SEND3270.ToString().ToUpper())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050100_001") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        //*比較一Key和二Key的User是否為同一人
        if (esRS_1Key.GetEntity(0).USER_ID.ToString().Trim() == eAgentInfo.agent_id.ToString().Trim())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050100_012") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        //*查詢二Key資料
        EntitySet<EntityRedeemSet> esRS_2Key = new EntitySet<EntityRedeemSet>();
        esRS_2Key = BRRedeemSet.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (esRS_2Key.Count > 0)
        {
            if ("N" == esRS_2Key.GetEntity(0).SEND3270)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_003");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                isEnableAll(false);
                return;
            }

            setValue(esRS_2Key.GetEntity(0), BRRedeemSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "2"));
        }
        else
        {
            setDefaultValue();
        }

        isEnableAll(true);
        isEnableFor_UE_B();
        txtORG.Focus();

        ViewState["EntityRedeemSet"] = esRS_1Key.GetEntity(0);
        ViewState["EntityRedeemSet_ATypeSet"] = esRS_ATS;
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
        int iDiffCoutn = 0;

        EntitySet<EntityRedeemSet> esRS_2Key = new EntitySet<EntityRedeemSet>();
        esRS_2Key = BRRedeemSet.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (Compare(ref iDiffCoutn))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_004");

            if (esRS_2Key.Count > 0)
            {
                if (BRRedeemSet.UpdateKey2(txtReceiveNumber.Text.Trim(), entityCreate("N", iDiffCoutn), RedeemSet_ATypeSetCreate(), Redeem3270CreateAll(Redeem3270Create())))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_004");
                    isEnableAll(false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_005");
                }
            }
            else
            {
                if (BRRedeemSet.InsertKey2(entityCreate("N", iDiffCoutn), RedeemSet_ATypeSetCreate(), Redeem3270CreateAll(Redeem3270Create())))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_004");
                    isEnableAll(false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01050100_005");
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_003");

            if (esRS_2Key.Count > 0)
            {
                BRRedeemSet.UpdateKey2(txtReceiveNumber.Text.Trim(), entityCreate("", iDiffCoutn), RedeemSet_ATypeSetCreate(), Redeem3270CreateAll(Redeem3270Create()));
            }
            else
            {
                BRRedeemSet.InsertKey2(entityCreate("", iDiffCoutn), RedeemSet_ATypeSetCreate(), Redeem3270CreateAll(Redeem3270Create()));
            }
        }
    }
    #endregion event

    #region method
    /// <summary>
    /// 設定所有的輸入框是否可以使用
    /// </summary>
    /// <param name="bIsEnable">true：可以使用，false：不能使用</param>
    private void isEnableAll(bool bIsEnable)
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

                if (!bIsEnable)
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

        if (!bIsEnable)
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

    private bool Compare(ref int intDiffCount)
    {
        bool blnSame = true;
        int intCount = 0;//*記錄不相同的數量
        int iCTCount = 0;// CardType輸入框個數

        if (null != ViewState["EntityRedeemSet"] && null != ViewState["EntityRedeemSet_ATypeSet"])
        {
            #region 一般資料
            EntityRedeemSet eRS = (EntityRedeemSet)ViewState["EntityRedeemSet"];

            GetCompareResult(txtORG, eRS.ORG, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO1, eRS.MERCHANT_NO1, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO2, eRS.MERCHANT_NO2, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO3, eRS.MERCHANT_NO3, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO4, eRS.MERCHANT_NO4, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO5, eRS.MERCHANT_NO5, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO6, eRS.MERCHANT_NO6, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO7, eRS.MERCHANT_NO7, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO8, eRS.MERCHANT_NO8, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO9, eRS.MERCHANT_NO9, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANTNO10, eRS.MERCHANT_NO10, ref intCount, ref blnSame);

            GetCompareResult(txtPRODCODE, eRS.PROD_CODE, ref intCount, ref blnSame);

            GetCompareResult(txtPROGRAM, eRS.PROGRAM, ref intCount, ref blnSame);

            GetCompareResult(txtMERCHANT_INT, IntDecSplit(eRS.MERCHANT, "I"), ref intCount, ref blnSame);
            GetCompareResult(txtMERCHANT_DEC, IntDecSplit(eRS.MERCHANT, "D"), ref intCount, ref blnSame);

            #endregion 一般資料

            #region CardType資料
            EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS = (EntitySet<EntityRedeemSet_ATypeSet>)ViewState["EntityRedeemSet_ATypeSet"];

            BRCardTypeList_Redeem.Select(1, 1, "", ref iCTCount);

            // 長期活動
            Compare_CardType(esRS_ATS, atl_AT1, "L", "1", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT1.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.LONG_LIMIT1, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_AT1.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.LONG_LIMIT1, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT1.FindControl("tbTBBPoints"), eRS.LONG_Points1, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT1.FindControl("tbTBBAmount"), eRS.LONG_Amount1, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_AT2, "L", "2", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT2.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.LONG_LIMIT2, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_AT2.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.LONG_LIMIT2, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT2.FindControl("tbTBBPoints"), eRS.LONG_Points2, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT2.FindControl("tbTBBAmount"), eRS.LONG_Amount2, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_AT3, "L", "3", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT3.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.LONG_LIMIT3, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_AT3.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.LONG_LIMIT3, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT3.FindControl("tbTBBPoints"), eRS.LONG_Points3, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_AT3.FindControl("tbTBBAmount"), eRS.LONG_Amount3, ref intCount, ref blnSame);

            // 短期促銷
            GetCompareResult(txtUSEREXIT_Type, eRS.USEREXIT_TYPE, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXIT_DateS, eRS.USEREXIT_DateS, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXIT_DateE, eRS.USEREXIT_DateE, ref intCount, ref blnSame);

            Compare_CardType(esRS_ATS, atl_USEREXITAT1, "U", "1", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.USEREXIT_LIMIT1, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.USEREXIT_LIMIT1, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBPoints"), eRS.USEREXIT_Points1, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT1.FindControl("tbTBBAmount"), eRS.USEREXIT_Amount1, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_USEREXITAT2, "U", "2", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.USEREXIT_LIMIT2, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.USEREXIT_LIMIT2, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBPoints"), eRS.USEREXIT_Points2, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT2.FindControl("tbTBBAmount"), eRS.USEREXIT_Amount2, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_USEREXITAT3, "U", "3", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.USEREXIT_LIMIT3, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.USEREXIT_LIMIT3, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBPoints"), eRS.USEREXIT_Points3, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_USEREXITAT3.FindControl("tbTBBAmount"), eRS.USEREXIT_Amount3, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXIT_OAOS, eRS.USEREXIT_OAOS, ref intCount, ref blnSame);

            // 生日活動
            GetCompareResult(txtBIRTH_Type, eRS.BIRTH_TYPE, ref intCount, ref blnSame);

            GetCompareResult(txtBIRTH_DateS, eRS.BIRTH_DateS, ref intCount, ref blnSame);

            GetCompareResult(txtBIRTH_DateE, eRS.BIRTH_DateE, ref intCount, ref blnSame);

            Compare_CardType(esRS_ATS, atl_BIRTH1, "B", "1", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.BIRTH_LIMIT1, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_BIRTH1.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.BIRTH_LIMIT1, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH1.FindControl("tbTBBPoints"), eRS.BIRTH_Points1, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH1.FindControl("tbTBBAmount"), eRS.BIRTH_Amount1, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_BIRTH2, "B", "2", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.BIRTH_LIMIT2, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_BIRTH2.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.BIRTH_LIMIT2, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH2.FindControl("tbTBBPoints"), eRS.BIRTH_Points2, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH2.FindControl("tbTBBAmount"), eRS.BIRTH_Amount2, ref intCount, ref blnSame);


            Compare_CardType(esRS_ATS, atl_BIRTH3, "B", "3", iCTCount, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_INT"), IntDecSplit(eRS.BIRTH_LIMIT3, "I"), ref intCount, ref blnSame);
            GetCompareResult((CustTextBox)atl_BIRTH3.FindControl("tbTBBLimit_DEC"), IntDecSplit(eRS.BIRTH_LIMIT3, "D"), ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH3.FindControl("tbTBBPoints"), eRS.BIRTH_Points3, ref intCount, ref blnSame);

            GetCompareResult((CustTextBox)atl_BIRTH3.FindControl("tbTBBAmount"), eRS.BIRTH_Amount3, ref intCount, ref blnSame);

            #endregion CardType資料

            intDiffCount = intCount;
            return blnSame;
        }
        else
        {
            return false;
        }
    }

    private bool GetCompareResult(CustTextBox txtBox, string strValue, ref int intCount, ref bool blnSame)
    {
        if (txtBox.Text.ToUpper().Trim() != NullToString(strValue).Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ClientID));
                }
            }
            txtBox.ForeColor = Color.Red;
            blnSame = false;
        }
        else
        {
            txtBox.ForeColor = Color.Black;
        }
        return blnSame;
    }

    private bool Compare_CardType(EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS, Common_Controls_CustATypeList ascxAT, string strACTIVITY, string strSTEP, int iCTCount, ref int intCount, ref bool blnSame)
    {
        int iesCount = 0;   // EntitySet中符合條件的資料筆數
        int iTextCount = 0; // 當前UserControl中已填寫值的欄位的數量

        for (int i = 0; i < esRS_ATS.Count; i++)
        {
            EntityRedeemSet_ATypeSet eRS_ATS = esRS_ATS.GetEntity(i);

            if (eRS_ATS.ACTIVITY.ToString().ToUpper() == strACTIVITY && eRS_ATS.STEP.ToString() == strSTEP)
            {
                GetCompareResult((CustTextBox)ascxAT.FindControl("tb_AType" + eRS_ATS.TXT_INDEX.ToString()), eRS_ATS.CARD_TYPE, ref intCount, ref blnSame);
                iesCount++;
            }
        }

        for (int i = 0; i < iCTCount; i++)
        {
            if (((CustTextBox)ascxAT.FindControl("tb_AType" + i.ToString())).Text.Trim() != "")
            {
                iTextCount++;
            }

            if (iTextCount > iesCount)
            {
                GetCompareResult((CustTextBox)ascxAT.FindControl("tb_AType" + i.ToString()), "", ref intCount, ref blnSame);
            }
        }

        //if (iesCount != iTextCount)
        //{
        //    blnSame = false;
        //}

        return blnSame;

    }

    private string NullToString(string strValue)
    {
        if (strValue == null)
        {
            return strValue = "";
        }
        return strValue;
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

    private EntityRedeemSet entityCreate(string strSend3270, int iDiffCount)
    {
        EntityRedeemSet eRS = new EntityRedeemSet();

        eRS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
        eRS.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eRS.KEYIN_FLAG = "2";
        eRS.SEND3270 = strSend3270;
        eRS.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
        eRS.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
        eRS.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
        eRS.ISSAME = 0 == iDiffCount ? "Y" : "N";
        eRS.DIFF_NUM = iDiffCount;
        eRS.EDIT_USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;


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

    //private bool InsertRedeemSet()
    //{
    //    try
    //    {
    //        return BRRedeemSet.InsertKey2(entityCreate(), Redeem3270Create());
    //    }
    //    catch
    //    {
    //        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
    //        {
    //            base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
    //        }

    //        return false;
    //    }
    //}

    private EntitySet<EntityRedeem3270> Redeem3270CreateAll(EntitySet<EntityRedeem3270> esR3270)
    {
        EntitySet<EntityRedeem3270> esR3270Return = new EntitySet<EntityRedeem3270>();

        EntitySet<EntityRedeemSet> esRS = new EntitySet<EntityRedeemSet>();
        EntityRedeemSet eRS = new EntityRedeemSet();
        esRS = BRRedeemSet.Select(txtReceiveNumber.Text.Trim());

        if (!(esRS.Count > 0))
        {
            return esR3270Return;
        }
        else
        {
            eRS = esRS.GetEntity(0);
        }

        string[] arry = { eRS.MERCHANT_NO1, eRS.MERCHANT_NO2, eRS.MERCHANT_NO3, eRS.MERCHANT_NO4, eRS.MERCHANT_NO5, eRS.MERCHANT_NO6, eRS.MERCHANT_NO7, eRS.MERCHANT_NO8, eRS.MERCHANT_NO9, eRS.MERCHANT_NO10 };

        for (int i = 0; i < arry.Length; i++)
        {
            if (arry[i].Trim() != "")
            {
                for (int j = 0; j < esR3270.Count; j++)
                {
                    EntityRedeem3270 eR3270 = new EntityRedeem3270();
                    eR3270.Clone(esR3270.GetEntity(j));

                    eR3270.IN_MERCHANT = arry[i].Trim();

                    esR3270Return.Add(eR3270);
                }
            }
        }

        return esR3270Return;
    }

    private EntitySet<EntityRedeem3270> Redeem3270Create()
    {
        bool bIsAll_L;
        bool bIsAll_U;
        bool bIsAll_B;

        EntitySet<EntityRedeem3270> esR3270 = new EntitySet<EntityRedeem3270>();
        EntityRedeem3270 eR3270 = new EntityRedeem3270();

        EntitySet<EntityRedeemSet> esRS = new EntitySet<EntityRedeemSet>();
        EntityRedeemSet eRS = new EntityRedeemSet();
        esRS = BRRedeemSet.Select(txtReceiveNumber.Text.Trim());

        if (!(esRS.Count > 0))
        {
            return esR3270;
        }
        else
        {
            eRS = esRS.GetEntity(0);
        }

        // 設定電文基本資料
        eR3270.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eR3270.FUNCTION_CODE = "A";
        eR3270.IN_ORG = eRS.ORG;
        eR3270.MSG_SEQ = "";
        eR3270.MSG_ERR = "";
        //eR3270.IN_MERCHANT = eRS.MERCHANT_NO1;
        eR3270.IN_PROD_CODE = eRS.PROD_CODE;
        eR3270.IN_CARD_TYPE = "";
        eR3270.PROGID = eRS.PROGRAM;
        eR3270.MER_RATE = eRS.MERCHANT;

        eR3270.LIMITR = "";
        eR3270.CHPOINT = "";
        eR3270.CHAMT = "";
        eR3270.USER_EXIT = eRS.USEREXIT_TYPE;
        eR3270.CYLCO = "";
        eR3270.STARTU = "";
        eR3270.ENDU = "";
        eR3270.LIMITU = "";
        eR3270.CHPOINTU = "";
        eR3270.CHAMTU = "";
        eR3270.BIRTH = eRS.BIRTH_TYPE;
        eR3270.STARTB = "";
        eR3270.ENDB = "";
        eR3270.LIMITB = "";
        eR3270.CHPOINTB = "";
        eR3270.CHAMTB = "";
        eR3270.SEND_FLAG = "N";


        #region 全卡種Check
        bIsAll_L = CardTypeAllCheck(atl_AT1, ((CustDropDownList)atl_AT1.FindControl("cddlATList")).SelectedValue);
        bIsAll_U = CardTypeAllCheck(atl_USEREXITAT1, ((CustDropDownList)atl_USEREXITAT1.FindControl("cddlATList")).SelectedValue);
        bIsAll_B = CardTypeAllCheck(atl_BIRTH1, ((CustDropDownList)atl_BIRTH1.FindControl("cddlATList")).SelectedValue);
        #endregion 全卡種Check


        // 長期活動
        DataSet dsL = new DataSet();
        if (bIsAll_L)
        {
            dsL = BRRedeemSet_ATypeSet.Select3270_CTAll(txtReceiveNumber.Text.Trim(), "L", "2");
        }
        else
        {
            dsL = BRRedeemSet_ATypeSet.Select3270_CT(txtReceiveNumber.Text.Trim(), "L", "2");
        }

        // 短期促銷
        DataSet dsU = new DataSet();
        if (bIsAll_U)
        {
            dsU = BRRedeemSet_ATypeSet.Select3270_CTAll(txtReceiveNumber.Text.Trim(), "U", "2");
        }
        else
        {
            dsU = BRRedeemSet_ATypeSet.Select3270_CT(txtReceiveNumber.Text.Trim(), "U", "2");
        }


        // 生日活動
        DataSet dsB = new DataSet();
        if (bIsAll_B)
        {
            dsB = BRRedeemSet_ATypeSet.Select3270_CTAll(txtReceiveNumber.Text.Trim(), "B", "2");
        }
        else
        {
            dsB = BRRedeemSet_ATypeSet.Select3270_CT(txtReceiveNumber.Text.Trim(), "B", "2");
        }


        DataTable dtL = dsL.Tables[0];

        DataTable dtU = dsU.Tables[0];
        dtU.Columns.Add(new DataColumn("state", typeof(string)));

        DataTable dtB = dsB.Tables[0];
        dtB.Columns.Add(new DataColumn("state", typeof(string)));

        #region 長期活動處理
        if (dtL.Rows.Count > 0)
        {
            for (int i = 0; i < dtL.Rows.Count; i++)
            {
                EntityRedeem3270 eR3270L = new EntityRedeem3270();
                eR3270L.Clone(eR3270);

                eR3270L.IN_CARD_TYPE = dtL.Rows[i]["CARD_TYPE"].ToString();

                switch (dtL.Rows[i]["STEP"].ToString())
                {
                    case "1":
                        eR3270L.LIMITR = eRS.LONG_LIMIT1;
                        eR3270L.CHPOINT = eRS.LONG_Points1;
                        eR3270L.CHAMT = eRS.LONG_Amount1;
                        break;
                    case "2":
                        eR3270L.LIMITR = eRS.LONG_LIMIT2;
                        eR3270L.CHPOINT = eRS.LONG_Points2;
                        eR3270L.CHAMT = eRS.LONG_Amount2;
                        break;
                    case "3":
                        eR3270L.LIMITR = eRS.LONG_LIMIT3;
                        eR3270L.CHPOINT = eRS.LONG_Points3;
                        eR3270L.CHAMT = eRS.LONG_Amount3;
                        break;
                    default:
                        break;
                }

                // 短期促銷中查找是否有相同的CardType
                if (dtU.Rows.Count > 0)
                {
                    for (int j = 0; j < dtU.Rows.Count; j++)
                    {
                        if (dtL.Rows[i]["CARD_TYPE"].ToString() == dtU.Rows[j]["CARD_TYPE"].ToString())
                        {
                            eR3270L.USER_EXIT = eRS.USEREXIT_TYPE;
                            eR3270L.CYLCO = eRS.USEREXIT_OAOS;
                            eR3270L.STARTU = eRS.USEREXIT_DateS;
                            eR3270L.ENDU = eRS.USEREXIT_DateE;

                            switch (dtU.Rows[j]["STEP"].ToString())
                            {
                                case "1":
                                    eR3270L.LIMITU = eRS.USEREXIT_LIMIT1;
                                    eR3270L.CHPOINTU = eRS.USEREXIT_Points1;
                                    eR3270L.CHAMTU = eRS.USEREXIT_Amount1;
                                    break;
                                case "2":
                                    eR3270L.LIMITU = eRS.USEREXIT_LIMIT2;
                                    eR3270L.CHPOINTU = eRS.USEREXIT_Points2;
                                    eR3270L.CHAMTU = eRS.USEREXIT_Amount2;
                                    break;
                                case "3":
                                    eR3270L.LIMITU = eRS.USEREXIT_LIMIT3;
                                    eR3270L.CHPOINTU = eRS.USEREXIT_Points3;
                                    eR3270L.CHAMTU = eRS.USEREXIT_Amount3;
                                    break;
                                default:
                                    break;
                            }
                            dtU.Rows[j]["state"] = "Y";
                        }
                    }
                }

                // 生日活動中查找是否有相同的CardType
                if (dtB.Rows.Count > 0)
                {
                    for (int j = 0; j < dtB.Rows.Count; j++)
                    {
                        if (dtL.Rows[i]["CARD_TYPE"].ToString() == dtB.Rows[j]["CARD_TYPE"].ToString())
                        {
                            eR3270L.BIRTH = eRS.BIRTH_TYPE;
                            eR3270L.STARTB = eRS.BIRTH_DateS;
                            eR3270L.ENDB = eRS.BIRTH_DateE;

                            switch (dtB.Rows[j]["STEP"].ToString())
                            {
                                case "1":
                                    eR3270L.LIMITB = eRS.BIRTH_LIMIT1;
                                    eR3270L.CHPOINTB = eRS.BIRTH_Points1;
                                    eR3270L.CHAMTB = eRS.BIRTH_Amount1;
                                    break;
                                case "2":
                                    eR3270L.LIMITB = eRS.BIRTH_LIMIT2;
                                    eR3270L.CHPOINTB = eRS.BIRTH_Points2;
                                    eR3270L.CHAMTB = eRS.BIRTH_Amount2;
                                    break;
                                case "3":
                                    eR3270L.LIMITB = eRS.BIRTH_LIMIT3;
                                    eR3270L.CHPOINTB = eRS.BIRTH_Points3;
                                    eR3270L.CHAMTB = eRS.BIRTH_Amount3;
                                    break;
                                default:
                                    break;
                            }
                            dtB.Rows[j]["state"] = "Y";
                        }
                    }
                }

                esR3270.Add(eR3270L);
            }

        }
        #endregion 長期活動處理

        #region 短期促銷處理
        if (dtU.Rows.Count > 0)
        {
            for (int i = 0; i < dtU.Rows.Count; i++)
            {
                if ("Y" != dtU.Rows[i]["state"].ToString())
                {
                    EntityRedeem3270 eR3270U = new EntityRedeem3270();
                    eR3270U.Clone(eR3270);

                    eR3270U.USER_EXIT = eRS.USEREXIT_TYPE;
                    eR3270U.CYLCO = eRS.USEREXIT_OAOS;
                    eR3270U.STARTU = eRS.USEREXIT_DateS;
                    eR3270U.ENDU = eRS.USEREXIT_DateE;

                    switch (dtU.Rows[i]["STEP"].ToString())
                    {
                        case "1":
                            eR3270U.LIMITU = eRS.USEREXIT_LIMIT1;
                            eR3270U.CHPOINTU = eRS.USEREXIT_Points1;
                            eR3270U.CHAMTU = eRS.USEREXIT_Amount1;
                            break;
                        case "2":
                            eR3270U.LIMITU = eRS.USEREXIT_LIMIT2;
                            eR3270U.CHPOINTU = eRS.USEREXIT_Points2;
                            eR3270U.CHAMTU = eRS.USEREXIT_Amount2;
                            break;
                        case "3":
                            eR3270U.LIMITU = eRS.USEREXIT_LIMIT3;
                            eR3270U.CHPOINTU = eRS.USEREXIT_Points3;
                            eR3270U.CHAMTU = eRS.USEREXIT_Amount3;
                            break;
                        default:
                            break;
                    }

                    // 生日活動中查找是否有相同的CardType
                    if (dtB.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtB.Rows.Count; j++)
                        {
                            if (dtU.Rows[i]["CARD_TYPE"].ToString() == dtB.Rows[j]["CARD_TYPE"].ToString())
                            {
                                eR3270U.BIRTH = eRS.BIRTH_TYPE;
                                eR3270U.STARTB = eRS.BIRTH_DateS;
                                eR3270U.ENDB = eRS.BIRTH_DateE;

                                switch (dtB.Rows[j]["STEP"].ToString())
                                {
                                    case "1":
                                        eR3270U.LIMITB = eRS.BIRTH_LIMIT1;
                                        eR3270U.CHPOINTB = eRS.BIRTH_Points1;
                                        eR3270U.CHAMTB = eRS.BIRTH_Amount1;
                                        break;
                                    case "2":
                                        eR3270U.LIMITB = eRS.BIRTH_LIMIT2;
                                        eR3270U.CHPOINTB = eRS.BIRTH_Points2;
                                        eR3270U.CHAMTB = eRS.BIRTH_Amount2;
                                        break;
                                    case "3":
                                        eR3270U.LIMITB = eRS.BIRTH_LIMIT3;
                                        eR3270U.CHPOINTB = eRS.BIRTH_Points3;
                                        eR3270U.CHAMTB = eRS.BIRTH_Amount3;
                                        break;
                                    default:
                                        break;
                                }
                                dtB.Rows[j]["state"] = "Y";
                            }
                        }
                    }
                    esR3270.Add(eR3270U);
                }
            }
        }

        #endregion 短期促銷處理

        #region 生日活動處理
        if (dtB.Rows.Count > 0)
        {
            for (int i = 0; i < dtB.Rows.Count; i++)
            {
                if ("Y" != dtB.Rows[i]["state"].ToString())
                {
                    EntityRedeem3270 eR3270B = new EntityRedeem3270();
                    eR3270B.Clone(eR3270);

                    eR3270B.BIRTH = eRS.BIRTH_TYPE;
                    eR3270B.STARTB = eRS.BIRTH_DateS;
                    eR3270B.ENDB = eRS.BIRTH_DateE;

                    switch (dtB.Rows[i]["STEP"].ToString())
                    {
                        case "1":
                            eR3270B.LIMITB = eRS.BIRTH_LIMIT1;
                            eR3270B.CHPOINTB = eRS.BIRTH_Points1;
                            eR3270B.CHAMTB = eRS.BIRTH_Amount1;
                            break;
                        case "2":
                            eR3270B.LIMITB = eRS.BIRTH_LIMIT2;
                            eR3270B.CHPOINTB = eRS.BIRTH_Points2;
                            eR3270B.CHAMTB = eRS.BIRTH_Amount2;
                            break;
                        case "3":
                            eR3270B.LIMITB = eRS.BIRTH_LIMIT3;
                            eR3270B.CHPOINTB = eRS.BIRTH_Points3;
                            eR3270B.CHAMTB = eRS.BIRTH_Amount3;
                            break;
                        default:
                            break;
                    }
                    esR3270.Add(eR3270B);
                }
            }
        }
        #endregion 生日活動處理

        return esR3270;
    }

    private bool CardTypeAllCheck(Common_Controls_CustATypeList ascxAT, string strACCUCode)
    {
        int iTBCount = 0;   // CardType輸入框個數
        bool bIsCardTypeAll = true; // 是否是全類型

        if ("-1" == strACCUCode)
        {
            return false;
        }

        BRCardTypeList_Redeem.Select(1, 1, "", ref iTBCount);

        EntitySet<EntityACCUMGroupTable_Redeem> esAT_R = new EntitySet<EntityACCUMGroupTable_Redeem>();
        esAT_R = BRACCUMGroupTable_Redeem.Select(strACCUCode);

        for (int i = 0; i < esAT_R.Count; i++)
        {
            bIsCardTypeAll = false;

            for (int j = 0; j < iTBCount; j++)
            {
                if (((CustTextBox)ascxAT.FindControl("tb_AType" + j.ToString())).Text.Trim() == esAT_R.GetEntity(i).Card_CODE.Trim())
                {
                    bIsCardTypeAll = true;
                    break;
                }
            }

            if (!bIsCardTypeAll)
            {
                // 沒有在畫面輸入框中找到對應的CardType，所以不是全類型
                bIsCardTypeAll = false;
                break;
            }
        }

        return bIsCardTypeAll;

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
            eRS_ATS.KEYIN_FLAG = "2";

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
    #endregion method
}
