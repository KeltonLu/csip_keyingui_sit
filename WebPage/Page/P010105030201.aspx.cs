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
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;
using System.Drawing;
using Framework.WebControls;

public partial class P010105030201 : PageBase
{
    #region Variable
    private EntityAGENT_INFO eAgentInfo;
    #endregion Variable

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        //*查詢資料檔Job_Status
        EntitySet<EntityJOB_STATUS> eJobStatusSet = BRJOB_STATUS.SelectEntitySet("1", "Job_LGAT");

        if (eJobStatusSet != null && eJobStatusSet.Count > 0)
        {
            string strAlertMsg = @"『Award 點數回饋參數設定 批次上傳主機中 , 請稍後再進行二 KEY 作業 , \r\n 批次上傳起始時間 : " +
                            ((DateTime)eJobStatusSet.GetEntity(0).start_time).ToString("HH:mm:ss") +
                            @" \r\n 預估作業時間 : " + ((int)(((int)eJobStatusSet.GetEntity(0).data_count) * 3 / 60)).ToString() + " 分鐘』";

            sbRegScript.Append("alert('" + strAlertMsg + "');history.go(-1);");

            return;
        }

        if (!Page.IsPostBack)
        {
            isEnableAll(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        setEnableForCArea();
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
        EntitySet<EntityAwardSet> esAS_1Key = new EntityAwardSetSet();
        EntitySet<EntityAwardSet_ATypeSet> esAS_ATS = new EntitySet<EntityAwardSet_ATypeSet>();
        try
        {
            esAS_1Key = BRAwardSet.SelectData(txtReceiveNumber.Text.Trim(), "1");
            esAS_ATS = BRAwardSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if (null == esAS_1Key || esAS_1Key.Count < 1)
        {
            //*沒有一Key資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_013") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if (null == esAS_ATS || esAS_ATS.Count < 1)
        {
            //*沒有一Key CardType資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050100_002") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if ("Y" == esAS_1Key.GetEntity(0).SEND3270.ToString().ToUpper())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        //*比較一Key和二Key的User是否為同一人
        if (esAS_1Key.GetEntity(0).USER_ID.ToString().Trim() == eAgentInfo.agent_id.ToString().Trim())
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_014") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        //*查詢二Key資料
        EntitySet<EntityAwardSet> esAS_2Key = new EntityAwardSetSet();
        try
        {
            esAS_2Key = BRAwardSet.SelectData(txtReceiveNumber.Text.Trim(), "2");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            isEnableAll(false);
            return;
        }

        if (esAS_2Key.Count > 0)
        {
            if ("N" == esAS_2Key.GetEntity(0).SEND3270)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_003");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                isEnableAll(false);
                return;
            }

            setValue(esAS_2Key.GetEntity(0), BRAwardSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "2"));
        }
        else
        {
            setDefaultValue();
        }

        isEnableAll(true);
        setEnableForCArea();
        txtORG.Focus();

        ViewState["EntityAwardSet"] = esAS_1Key.GetEntity(0);
        ViewState["EntityAwardSet_ATypeSet"] = esAS_ATS;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int iDiffCoutn = 0;

        EntitySet<EntityAwardSet> esAS_2Key = new EntityAwardSetSet();
        esAS_2Key = BRAwardSet.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (Compare(ref iDiffCoutn))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_004");
            if (esAS_2Key.Count > 0)
            {
                if (BRAwardSet.UpdateKey2(txtReceiveNumber.Text.Trim(), CreateAwardSet("N", iDiffCoutn), CreateAwardSet_ATypeSet(), CreateAwardSet3270()))
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
                if (BRAwardSet.InsertKey2(CreateAwardSet("N", iDiffCoutn), CreateAwardSet_ATypeSet(), CreateAwardSet3270()))
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

            if (esAS_2Key.Count > 0)
            {
                BRAwardSet.UpdateKey2(txtReceiveNumber.Text.Trim(), CreateAwardSet("", iDiffCoutn), CreateAwardSet_ATypeSet(), CreateAwardSet3270());
            }
            else
            {
                BRAwardSet.InsertKey2(CreateAwardSet("", iDiffCoutn), CreateAwardSet_ATypeSet(), CreateAwardSet3270());
            }
        }
    }
    #endregion event

    #region method
    private void isEnableAll(bool bIsEnable)
    {
        if (bIsEnable)
        {
            ViewState["lStartTime"] = System.Environment.TickCount;  // 開始 keyin 時間
        }

        // user control 設定
        atl_AT1.IsEnable = bIsEnable;

        //畫面 control 設定
        txtORG.Enabled = bIsEnable;
        txtPROGRAMNO.Enabled = bIsEnable;
        txtPARTNERNO.Enabled = bIsEnable;
        txtTCCODE_Add.Enabled = bIsEnable;
        txtTCCODE_Minus.Enabled = bIsEnable;
        txtConsumptionArea.Enabled = bIsEnable;
        txtMainCardSetPoint.Enabled = bIsEnable;
        txtSUPPCardSetPoint.Enabled = bIsEnable;
        txtUSEREXITFrom.Enabled = bIsEnable;
        txtUSEREXITTo.Enabled = bIsEnable;
        txtUSEREXITSetPoint.Enabled = bIsEnable;
        txtBIRTHFrom.Enabled = bIsEnable;
        txtBIRTHTo.Enabled = bIsEnable;
        txtBIRTHSetPoint.Enabled = bIsEnable;

        for (int i = 1; i < 11; i++)
        {
            CustTextBox ctbB = (CustTextBox)this.FindControl("txtMMCCODE_" + i.ToString() + "_B");
            if (null != ctbB)
            {
                ctbB.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbB.Text = "";
                }
            }

            CustTextBox ctbE = (CustTextBox)this.FindControl("txtMMCCODE_" + i.ToString() + "_E");
            if (null != ctbE)
            {
                ctbE.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbE.Text = "";
                }
            }

        }

        for (int i = 0; i < 5; i++)
        {
            CustTextBox ctbMC_A = (CustTextBox)this.FindControl("MainCardAMT" + i.ToString());
            if (null != ctbMC_A)
            {
                ctbMC_A.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbMC_A.Text = "";
                }
            }
            CustTextBox ctbMC_R = (CustTextBox)this.FindControl("MainCardRATE" + i.ToString());
            if (null != ctbMC_R)
            {
                ctbMC_R.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbMC_R.Text = "";
                }
            }

            CustTextBox ctbSUPP_A = (CustTextBox)this.FindControl("SUPPCardAMT" + i.ToString());
            if (null != ctbSUPP_A)
            {
                ctbSUPP_A.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbSUPP_A.Text = "";
                }
            }
            CustTextBox ctbSUPP_R = (CustTextBox)this.FindControl("SUPPCardRATE" + i.ToString());
            if (null != ctbSUPP_R)
            {
                ctbSUPP_R.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbSUPP_R.Text = "";
                }
            }

            CustTextBox ctbUE_A = (CustTextBox)this.FindControl("USEREXITAMT" + i.ToString());
            if (null != ctbUE_A)
            {
                ctbUE_A.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbUE_A.Text = "";
                }
            }
            CustTextBox ctbUE_R = (CustTextBox)this.FindControl("USEREXITRATE" + i.ToString());
            if (null != ctbUE_R)
            {
                ctbUE_R.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbUE_R.Text = "";
                }
            }

            CustTextBox ctbB_A = (CustTextBox)this.FindControl("BIRTHAMT" + i.ToString());
            if (null != ctbB_A)
            {
                ctbB_A.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbB_A.Text = "";
                }
            }
            CustTextBox ctbB_R = (CustTextBox)this.FindControl("BIRTHRATE" + i.ToString());
            if (null != ctbB_R)
            {
                ctbB_R.Enabled = bIsEnable;

                if (!bIsEnable)
                {
                    ctbB_R.Text = "";
                }
            }
        }

        if (!bIsEnable)
        {
            txtORG.Text = "";
            txtPROGRAMNO.Text = "";
            txtPARTNERNO.Text = "";
            txtTCCODE_Add.Text = "";
            txtTCCODE_Minus.Text = "";
            txtConsumptionArea.Text = "";
            txtMainCardSetPoint.Text = "";
            txtSUPPCardSetPoint.Text = "";
            txtUSEREXITFrom.Text = "";
            txtUSEREXITTo.Text = "";
            txtUSEREXITSetPoint.Text = "";
            txtBIRTHFrom.Text = "";
            txtBIRTHTo.Text = "";
            txtBIRTHSetPoint.Text = "";
        }

        btnAdd.Enabled = bIsEnable;

        // 開始填寫資料時鎖定手機編號輸入框
        txtReceiveNumber.Enabled = !bIsEnable;
    }

    private void setEnableForCArea()
    {
        for (int i = 1; i < 11; i++)
        {
            CustTextBox ctbCode = (CustTextBox)this.FindControl("txtConsumptionAreaCode" + i.ToString());
            if (null != ctbCode)
            {
                if ("X" == txtConsumptionArea.Text.Trim().ToUpper() || "I" == txtConsumptionArea.Text.Trim().ToUpper())
                {
                    ctbCode.Enabled = true;
                }
                else
                {
                    ctbCode.Enabled = false;
                    ctbCode.Text = "";
                }
            }
        }
    }

    private void setDefaultValue()
    {
        txtORG.Text = "822";
        txtTCCODE_Add.Text = "40";
        txtTCCODE_Minus.Text = "41";
        txtConsumptionArea.Text = "N";
        txtUSEREXITFrom.Text = "00000000";
        txtUSEREXITTo.Text = "00000000";
        txtBIRTHFrom.Text = "00000000";
        txtBIRTHTo.Text = "00000000";
        txtMMCCODE_1_B.Text = "0000";
        txtMMCCODE_1_E.Text = "9999";
    }

    private bool Compare(ref int intDiffCount)
    {
        bool blnSame = true;
        int intCount = 0;//*記錄不相同的數量
        int iCTCount = 0;// CardType輸入框個數

        if (null != ViewState["EntityAwardSet"] && null != ViewState["EntityAwardSet_ATypeSet"])
        {
            EntityAwardSet eAS = (EntityAwardSet)ViewState["EntityAwardSet"];

            GetCompareResult(txtORG, eAS.ORG, ref intCount, ref blnSame);

            GetCompareResult(txtPROGRAMNO, eAS.PROGRAMNO, ref intCount, ref blnSame);

            GetCompareResult(txtPARTNERNO, eAS.PARTNERNO, ref intCount, ref blnSame);

            #region UserControl Compear
            EntitySet<EntityAwardSet_ATypeSet> esAs_ATS = (EntitySet<EntityAwardSet_ATypeSet>)ViewState["EntityAwardSet_ATypeSet"];

            BRCardTypeList_Award.Select(1, 1, "", ref iCTCount);

            Compare_CardType(esAs_ATS, atl_AT1, iCTCount, ref intCount, ref blnSame);
            #endregion UserControl Compear

            GetCompareResult(txtTCCODE_Add, eAS.TCCODE_Add, ref intCount, ref blnSame);

            GetCompareResult(txtTCCODE_Minus, eAS.TCCODE_Minus, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_1_B, eAS.MMCCODE_1_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_1_E, eAS.MMCCODE_1_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_2_B, eAS.MMCCODE_2_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_2_E, eAS.MMCCODE_2_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_3_B, eAS.MMCCODE_3_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_3_E, eAS.MMCCODE_3_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_4_B, eAS.MMCCODE_4_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_4_E, eAS.MMCCODE_4_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_5_B, eAS.MMCCODE_5_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_5_E, eAS.MMCCODE_5_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_6_B, eAS.MMCCODE_6_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_6_E, eAS.MMCCODE_6_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_7_B, eAS.MMCCODE_7_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_7_E, eAS.MMCCODE_7_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_8_B, eAS.MMCCODE_8_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_8_E, eAS.MMCCODE_8_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_9_B, eAS.MMCCODE_9_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_9_E, eAS.MMCCODE_9_E, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_10_B, eAS.MMCCODE_10_B, ref intCount, ref blnSame);

            GetCompareResult(txtMMCCODE_10_E, eAS.MMCCODE_10_E, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionArea, eAS.ConsumptionArea, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode1, eAS.ConsumptionAreaCode1, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode2, eAS.ConsumptionAreaCode2, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode3, eAS.ConsumptionAreaCode3, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode4, eAS.ConsumptionAreaCode4, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode5, eAS.ConsumptionAreaCode5, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode6, eAS.ConsumptionAreaCode6, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode7, eAS.ConsumptionAreaCode7, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode8, eAS.ConsumptionAreaCode8, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode9, eAS.ConsumptionAreaCode9, ref intCount, ref blnSame);

            GetCompareResult(txtConsumptionAreaCode10, eAS.ConsumptionAreaCode10, ref intCount, ref blnSame);

            GetCompareResult(txtMainCardSetPoint, eAS.MainCardSetPoint, ref intCount, ref blnSame);

            GetCompareResult(txtSUPPCardSetPoint, eAS.SUPPCardSetPoint, ref intCount, ref blnSame);

            GetCompareResult(MainCardAMT1, eAS.MainCardAMT1, ref intCount, ref blnSame);

            GetCompareResult(MainCardRATE1, eAS.MainCardRATE1, ref intCount, ref blnSame);

            GetCompareResult(MainCardAMT2, eAS.MainCardAMT2, ref intCount, ref blnSame);

            GetCompareResult(MainCardRATE2, eAS.MainCardRATE2, ref intCount, ref blnSame);

            GetCompareResult(MainCardAMT3, eAS.MainCardAMT3, ref intCount, ref blnSame);

            GetCompareResult(MainCardRATE3, eAS.MainCardRATE3, ref intCount, ref blnSame);

            GetCompareResult(MainCardAMT4, eAS.MainCardAMT4, ref intCount, ref blnSame);

            GetCompareResult(MainCardRATE4, eAS.MainCardRATE4, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardAMT1, eAS.SUPPCardAMT1, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardRATE1, eAS.SUPPCardRATE1, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardAMT2, eAS.SUPPCardAMT2, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardRATE2, eAS.SUPPCardRATE2, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardAMT3, eAS.SUPPCardAMT3, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardRATE3, eAS.SUPPCardRATE3, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardAMT4, eAS.SUPPCardAMT4, ref intCount, ref blnSame);

            GetCompareResult(SUPPCardRATE4, eAS.SUPPCardRATE4, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXITFrom, eAS.USEREXITFrom, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXITTo, eAS.USEREXITTo, ref intCount, ref blnSame);

            GetCompareResult(txtUSEREXITSetPoint, eAS.USEREXITSetPoint, ref intCount, ref blnSame);

            GetCompareResult(USEREXITAMT1, eAS.USEREXITAMT1, ref intCount, ref blnSame);

            GetCompareResult(USEREXITRATE1, eAS.USEREXITRATE1, ref intCount, ref blnSame);

            GetCompareResult(USEREXITAMT2, eAS.USEREXITAMT2, ref intCount, ref blnSame);

            GetCompareResult(USEREXITRATE2, eAS.USEREXITRATE2, ref intCount, ref blnSame);

            GetCompareResult(USEREXITAMT3, eAS.USEREXITAMT3, ref intCount, ref blnSame);

            GetCompareResult(USEREXITRATE3, eAS.USEREXITRATE3, ref intCount, ref blnSame);

            GetCompareResult(USEREXITAMT4, eAS.USEREXITAMT4, ref intCount, ref blnSame);

            GetCompareResult(USEREXITRATE4, eAS.USEREXITRATE4, ref intCount, ref blnSame);

            GetCompareResult(txtBIRTHFrom, eAS.BIRTHFrom, ref intCount, ref blnSame);

            GetCompareResult(txtBIRTHTo, eAS.BIRTHTo, ref intCount, ref blnSame);

            GetCompareResult(txtBIRTHSetPoint, eAS.BIRTHSetPoint, ref intCount, ref blnSame);

            GetCompareResult(BIRTHAMT1, eAS.BIRTHAMT1, ref intCount, ref blnSame);

            GetCompareResult(BIRTHRATE1, eAS.BIRTHRATE1, ref intCount, ref blnSame);

            GetCompareResult(BIRTHAMT2, eAS.BIRTHAMT2, ref intCount, ref blnSame);

            GetCompareResult(BIRTHRATE2, eAS.BIRTHRATE2, ref intCount, ref blnSame);

            GetCompareResult(BIRTHAMT3, eAS.BIRTHAMT3, ref intCount, ref blnSame);

            GetCompareResult(BIRTHRATE3, eAS.BIRTHRATE3, ref intCount, ref blnSame);

            GetCompareResult(BIRTHAMT4, eAS.BIRTHAMT4, ref intCount, ref blnSame);

            GetCompareResult(BIRTHRATE4, eAS.BIRTHRATE4, ref intCount, ref blnSame);

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

    private string NullToString(string strValue)
    {
        if (strValue == null)
        {
            return strValue = "";
        }
        return strValue;
    }

    private bool Compare_CardType(EntitySet<EntityAwardSet_ATypeSet> esAS_ATS, Common_Controls_CustATypeList ascxAT, int iCTCount, ref int intCount, ref bool blnSame)
    {
        int iesCount = 0;   // EntitySet中符合條件的資料筆數
        int iTextCount = 0; // 當前UserControl中已填寫值的欄位的數量

        for (int i = 0; i < esAS_ATS.Count; i++)
        {
            EntityAwardSet_ATypeSet eAS_ATS = esAS_ATS.GetEntity(i);

            GetCompareResult((CustTextBox)ascxAT.FindControl("tb_AType" + eAS_ATS.TXT_INDEX.ToString()), eAS_ATS.CARD_TYPE, ref intCount, ref blnSame);
            iesCount++;
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

        return blnSame;

    }

    private EntityAwardSet CreateAwardSet(string strSend3270, int iDiffCount)
    {
        EntityAwardSet eAS = new EntityAwardSet();

        eAS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
        eAS.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eAS.KEYIN_FLAG = "2";
        eAS.SEND3270 = strSend3270;
        eAS.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
        eAS.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
        eAS.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
        eAS.ISSAME = 0 == iDiffCount ? "Y" : "N";
        eAS.DIFF_NUM = iDiffCount;
        eAS.EDIT_USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;


        eAS.ORG = txtORG.Text.Trim();
        eAS.PROGRAMNO = txtPROGRAMNO.Text.Trim();
        eAS.PARTNERNO = txtPARTNERNO.Text.Trim();
        eAS.TCCODE_Add = txtTCCODE_Add.Text.Trim();
        eAS.TCCODE_Minus = txtTCCODE_Minus.Text.Trim();
        eAS.MMCCODE_1_B = txtMMCCODE_1_B.Text.Trim();
        eAS.MMCCODE_1_E = txtMMCCODE_1_E.Text.Trim();
        eAS.MMCCODE_2_B = txtMMCCODE_2_B.Text.Trim();
        eAS.MMCCODE_2_E = txtMMCCODE_2_E.Text.Trim();
        eAS.MMCCODE_3_B = txtMMCCODE_3_B.Text.Trim();
        eAS.MMCCODE_3_E = txtMMCCODE_3_E.Text.Trim();
        eAS.MMCCODE_4_B = txtMMCCODE_4_B.Text.Trim();
        eAS.MMCCODE_4_E = txtMMCCODE_4_E.Text.Trim();
        eAS.MMCCODE_5_B = txtMMCCODE_5_B.Text.Trim();
        eAS.MMCCODE_5_E = txtMMCCODE_5_E.Text.Trim();
        eAS.MMCCODE_6_B = txtMMCCODE_6_B.Text.Trim();
        eAS.MMCCODE_6_E = txtMMCCODE_6_E.Text.Trim();
        eAS.MMCCODE_7_B = txtMMCCODE_7_B.Text.Trim();
        eAS.MMCCODE_7_E = txtMMCCODE_7_E.Text.Trim();
        eAS.MMCCODE_8_B = txtMMCCODE_8_B.Text.Trim();
        eAS.MMCCODE_8_E = txtMMCCODE_8_E.Text.Trim();
        eAS.MMCCODE_9_B = txtMMCCODE_9_B.Text.Trim();
        eAS.MMCCODE_9_E = txtMMCCODE_9_E.Text.Trim();
        eAS.MMCCODE_10_B = txtMMCCODE_10_B.Text.Trim();
        eAS.MMCCODE_10_E = txtMMCCODE_10_E.Text.Trim();
        eAS.ConsumptionArea = txtConsumptionArea.Text.Trim().ToUpper();
        eAS.ConsumptionAreaCode1 = txtConsumptionAreaCode1.Text.Trim();
        eAS.ConsumptionAreaCode2 = txtConsumptionAreaCode2.Text.Trim();
        eAS.ConsumptionAreaCode3 = txtConsumptionAreaCode3.Text.Trim();
        eAS.ConsumptionAreaCode4 = txtConsumptionAreaCode4.Text.Trim();
        eAS.ConsumptionAreaCode5 = txtConsumptionAreaCode5.Text.Trim();
        eAS.ConsumptionAreaCode6 = txtConsumptionAreaCode6.Text.Trim();
        eAS.ConsumptionAreaCode7 = txtConsumptionAreaCode7.Text.Trim();
        eAS.ConsumptionAreaCode8 = txtConsumptionAreaCode8.Text.Trim();
        eAS.ConsumptionAreaCode9 = txtConsumptionAreaCode9.Text.Trim();
        eAS.ConsumptionAreaCode10 = txtConsumptionAreaCode10.Text.Trim();
        eAS.MainCardSetPoint = txtMainCardSetPoint.Text.Trim();
        eAS.SUPPCardSetPoint = txtSUPPCardSetPoint.Text.Trim();
        eAS.MainCardAMT1 = MainCardAMT1.Text.Trim();
        eAS.MainCardRATE1 = MainCardRATE1.Text.Trim();
        eAS.MainCardAMT2 = MainCardAMT2.Text.Trim();
        eAS.MainCardRATE2 = MainCardRATE2.Text.Trim();
        eAS.MainCardAMT3 = MainCardAMT3.Text.Trim();
        eAS.MainCardRATE3 = MainCardRATE3.Text.Trim();
        eAS.MainCardAMT4 = MainCardAMT4.Text.Trim();
        eAS.MainCardRATE4 = MainCardRATE4.Text.Trim();
        eAS.SUPPCardAMT1 = SUPPCardAMT1.Text.Trim();
        eAS.SUPPCardRATE1 = SUPPCardRATE1.Text.Trim();
        eAS.SUPPCardAMT2 = SUPPCardAMT2.Text.Trim();
        eAS.SUPPCardRATE2 = SUPPCardRATE2.Text.Trim();
        eAS.SUPPCardAMT3 = SUPPCardAMT3.Text.Trim();
        eAS.SUPPCardRATE3 = SUPPCardRATE3.Text.Trim();
        eAS.SUPPCardAMT4 = SUPPCardAMT4.Text.Trim();
        eAS.SUPPCardRATE4 = SUPPCardRATE4.Text.Trim();
        eAS.USEREXITFrom = txtUSEREXITFrom.Text.Trim();
        eAS.USEREXITTo = txtUSEREXITTo.Text.Trim();
        eAS.USEREXITSetPoint = txtUSEREXITSetPoint.Text.Trim();
        eAS.USEREXITAMT1 = USEREXITAMT1.Text.Trim();
        eAS.USEREXITRATE1 = USEREXITRATE1.Text.Trim();
        eAS.USEREXITAMT2 = USEREXITAMT2.Text.Trim();
        eAS.USEREXITRATE2 = USEREXITRATE2.Text.Trim();
        eAS.USEREXITAMT3 = USEREXITAMT3.Text.Trim();
        eAS.USEREXITRATE3 = USEREXITRATE3.Text.Trim();
        eAS.USEREXITAMT4 = USEREXITAMT4.Text.Trim();
        eAS.USEREXITRATE4 = USEREXITRATE4.Text.Trim();
        eAS.BIRTHFrom = txtBIRTHFrom.Text.Trim();
        eAS.BIRTHTo = txtBIRTHTo.Text.Trim();
        eAS.BIRTHSetPoint = txtBIRTHSetPoint.Text.Trim();
        eAS.BIRTHAMT1 = BIRTHAMT1.Text.Trim();
        eAS.BIRTHRATE1 = BIRTHRATE1.Text.Trim();
        eAS.BIRTHAMT2 = BIRTHAMT2.Text.Trim();
        eAS.BIRTHRATE2 = BIRTHRATE2.Text.Trim();
        eAS.BIRTHAMT3 = BIRTHAMT3.Text.Trim();
        eAS.BIRTHRATE3 = BIRTHRATE3.Text.Trim();
        eAS.BIRTHAMT4 = BIRTHAMT4.Text.Trim();
        eAS.BIRTHRATE4 = BIRTHRATE4.Text.Trim();

        return eAS;
    }

    private EntitySet<EntityAwardSet3270> CreateAwardSet3270()
    {
        EntitySet<EntityAwardSet3270> esAS3270 = new EntitySet<EntityAwardSet3270>();
        EntityAwardSet3270 eAS3270 = new EntityAwardSet3270();

        EntitySet<EntityAwardSet> esAS = new EntitySet<EntityAwardSet>();
        EntityAwardSet eAS = new EntityAwardSet();
        esAS = BRAwardSet.Select(txtReceiveNumber.Text.Trim());

        if (!(esAS.Count > 0))
        {
            return esAS3270;
        }
        else
        {
            eAS = esAS.GetEntity(0);
        }

        EntitySet<EntityAwardSet_ATypeSet> esAS_ATS = new EntitySet<EntityAwardSet_ATypeSet>();
        esAS_ATS = BRAwardSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "2");

        if (!(esAS_ATS.Count > 0))
        {
            return esAS3270;
        }

        eAS3270.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eAS3270.SEND_FLAG = "N";
        eAS3270.FUNCTION_CODE = "A";
        eAS3270.MSG_SEQ = "";
        eAS3270.MSG_TYPE = "";
        eAS3270.ORG = eAS.ORG;
        eAS3270.PROG_ID = eAS.PROGRAMNO;
        eAS3270.PARTNER_ID = eAS.PARTNERNO;
        eAS3270.COUNTRY_CODE_IND = eAS.ConsumptionArea;

        eAS3270.BASIC_CALC_MTHD = GetCALC(eAS.MainCardSetPoint, "M");
        eAS3270.BASIC_CALC_IND = GetCALC(eAS.MainCardSetPoint, "I");
        eAS3270.SUPP_BASIC_CAL_MTHD = GetCALC(eAS.SUPPCardSetPoint, "M");
        eAS3270.SUPP_BASIC_CAL_IND = GetCALC(eAS.SUPPCardSetPoint, "I");
        eAS3270.PROMO_START_DTE = eAS.USEREXITFrom;
        eAS3270.PROMO_END_DTE = eAS.USEREXITTo;
        eAS3270.PROMO_CALC_MTHD = GetCALC(eAS.USEREXITSetPoint, "M");
        eAS3270.PROMO_CALC_IND = GetCALC(eAS.USEREXITSetPoint, "I");
        eAS3270.BTHDTE_FLAG = "";
        eAS3270.BTHDTE_START = eAS.BIRTHFrom;
        eAS3270.BTHDTE_END = eAS.BIRTHTo;
        eAS3270.BTHDTE_CALC_MTHD = GetCALC(eAS.BIRTHSetPoint, "M");
        eAS3270.BTHDTE_CALC_IND = GetCALC(eAS.BIRTHSetPoint, "I");
        eAS3270.M_SIGNON_NAME = "";

        eAS3270.TC_CODE1 = eAS.TCCODE_Add.Trim();
        eAS3270.TC_CODE2 = eAS.TCCODE_Minus.Trim();
        eAS3270.TC_CODE3 = "";
        eAS3270.TC_CODE4 = "";
        eAS3270.TC_CODE5 = "";
        eAS3270.TC_CODE6 = "";
        eAS3270.TC_CODE7 = "";
        eAS3270.TC_CODE8 = "";
        eAS3270.TC_CODE9 = "";
        eAS3270.TC_CODE10 = "";

        eAS3270.TC_OP1 = "";
        eAS3270.TC_OP2 = "";
        eAS3270.TC_OP3 = "";
        eAS3270.TC_OP4 = "";
        eAS3270.TC_OP5 = "";
        eAS3270.TC_OP6 = "";
        eAS3270.TC_OP7 = "";
        eAS3270.TC_OP8 = "";
        eAS3270.TC_OP9 = "";
        eAS3270.TC_OP10 = "";

        eAS3270.MCC_FROM1 = "" == eAS.MMCCODE_1_B.Trim() ? "0000" : eAS.MMCCODE_1_B;
        eAS3270.MCC_TO1 = "" == eAS.MMCCODE_1_E.Trim() ? "0000" : eAS.MMCCODE_1_E;
        eAS3270.COUNTRY_CODE1 = eAS.ConsumptionAreaCode1.Trim();

        eAS3270.MCC_FROM2 = "" == eAS.MMCCODE_2_B.Trim() ? "0000" : eAS.MMCCODE_2_B;
        eAS3270.MCC_TO2 = "" == eAS.MMCCODE_2_E.Trim() ? "0000" : eAS.MMCCODE_2_E;
        eAS3270.COUNTRY_CODE2 = eAS.ConsumptionAreaCode2.Trim();

        eAS3270.MCC_FROM3 = "" == eAS.MMCCODE_3_B.Trim() ? "0000" : eAS.MMCCODE_3_B;
        eAS3270.MCC_TO3 = "" == eAS.MMCCODE_3_E.Trim() ? "0000" : eAS.MMCCODE_3_E;
        eAS3270.COUNTRY_CODE3 = eAS.ConsumptionAreaCode3.Trim();

        eAS3270.MCC_FROM4 = "" == eAS.MMCCODE_4_B.Trim() ? "0000" : eAS.MMCCODE_4_B;
        eAS3270.MCC_TO4 = "" == eAS.MMCCODE_4_E.Trim() ? "0000" : eAS.MMCCODE_4_E;
        eAS3270.COUNTRY_CODE4 = eAS.ConsumptionAreaCode4.Trim();

        eAS3270.MCC_FROM5 = "" == eAS.MMCCODE_5_B.Trim() ? "0000" : eAS.MMCCODE_5_B;
        eAS3270.MCC_TO5 = "" == eAS.MMCCODE_5_E.Trim() ? "0000" : eAS.MMCCODE_5_E;
        eAS3270.COUNTRY_CODE5 = eAS.ConsumptionAreaCode5.Trim();

        eAS3270.MCC_FROM6 = "" == eAS.MMCCODE_6_B.Trim() ? "0000" : eAS.MMCCODE_6_B;
        eAS3270.MCC_TO6 = "" == eAS.MMCCODE_6_E.Trim() ? "0000" : eAS.MMCCODE_6_E;
        eAS3270.COUNTRY_CODE6 = eAS.ConsumptionAreaCode6.Trim();

        eAS3270.MCC_FROM7 = "" == eAS.MMCCODE_7_B.Trim() ? "0000" : eAS.MMCCODE_7_B;
        eAS3270.MCC_TO7 = "" == eAS.MMCCODE_7_E.Trim() ? "0000" : eAS.MMCCODE_7_E;
        eAS3270.COUNTRY_CODE7 = eAS.ConsumptionAreaCode7.Trim();

        eAS3270.MCC_FROM8 = "" == eAS.MMCCODE_8_B.Trim() ? "0000" : eAS.MMCCODE_8_B;
        eAS3270.MCC_TO8 = "" == eAS.MMCCODE_8_E.Trim() ? "0000" : eAS.MMCCODE_8_E;
        eAS3270.COUNTRY_CODE8 = eAS.ConsumptionAreaCode8.Trim();

        eAS3270.MCC_FROM9 = "" == eAS.MMCCODE_9_B.Trim() ? "0000" : eAS.MMCCODE_9_B;
        eAS3270.MCC_TO9 = "" == eAS.MMCCODE_9_E.Trim() ? "0000" : eAS.MMCCODE_9_E;
        eAS3270.COUNTRY_CODE9 = eAS.ConsumptionAreaCode9.Trim();

        eAS3270.MCC_FROM10 = "" == eAS.MMCCODE_10_B.Trim() ? "0000" : eAS.MMCCODE_10_B;
        eAS3270.MCC_TO10 = "" == eAS.MMCCODE_10_E.Trim() ? "0000" : eAS.MMCCODE_10_E;
        eAS3270.COUNTRY_CODE10 = eAS.ConsumptionAreaCode10.Trim();

        eAS3270.BASIC_TIER_AMT1 = "" == eAS.MainCardAMT1.Trim() ? "000000000" : eAS.MainCardAMT1;
        eAS3270.BASIC_TIER_RATE1 = "" == eAS.MainCardRATE1.Trim() ? "0000" : eAS.MainCardRATE1;
        eAS3270.SUPP_BASIC_TIER_AMT1 = "" == eAS.SUPPCardAMT1.Trim() ? "000000000" : eAS.SUPPCardAMT1;
        eAS3270.SUPP_BASIC_TIER_RATE1 = "" == eAS.SUPPCardRATE1.Trim() ? "0000" : eAS.SUPPCardRATE1;
        eAS3270.PROMO_TIER_AMT1 = "" == eAS.USEREXITAMT1.Trim() ? "000000000" : eAS.USEREXITAMT1;
        eAS3270.PROMO_TIER_RATE1 = "" == eAS.USEREXITRATE1.Trim() ? "0000" : eAS.USEREXITRATE1;
        eAS3270.BTHDTE_TIER_AMT1 = "" == eAS.BIRTHAMT1.Trim() ? "000000000" : eAS.BIRTHAMT1;
        eAS3270.BTHDTE_TIER_RATE1 = "" == eAS.BIRTHRATE1.Trim() ? "0000" : eAS.BIRTHRATE1;

        eAS3270.BASIC_TIER_AMT2 = "" == eAS.MainCardAMT2.Trim() ? "000000000" : eAS.MainCardAMT2;
        eAS3270.BASIC_TIER_RATE2 = "" == eAS.MainCardRATE2.Trim() ? "0000" : eAS.MainCardRATE2;
        eAS3270.SUPP_BASIC_TIER_AMT2 = "" == eAS.SUPPCardAMT2.Trim() ? "000000000" : eAS.SUPPCardAMT2;
        eAS3270.SUPP_BASIC_TIER_RATE2 = "" == eAS.SUPPCardRATE2.Trim() ? "0000" : eAS.SUPPCardRATE2;
        eAS3270.PROMO_TIER_AMT2 = "" == eAS.USEREXITAMT2.Trim() ? "000000000" : eAS.USEREXITAMT2;
        eAS3270.PROMO_TIER_RATE2 = "" == eAS.USEREXITRATE2.Trim() ? "0000" : eAS.USEREXITRATE2;
        eAS3270.BTHDTE_TIER_AMT2 = "" == eAS.BIRTHAMT2.Trim() ? "000000000" : eAS.BIRTHAMT2;
        eAS3270.BTHDTE_TIER_RATE2 = "" == eAS.BIRTHRATE2.Trim() ? "0000" : eAS.BIRTHRATE2;

        eAS3270.BASIC_TIER_AMT3 = "" == eAS.MainCardAMT3.Trim() ? "000000000" : eAS.MainCardAMT3;
        eAS3270.BASIC_TIER_RATE3 = "" == eAS.MainCardRATE3.Trim() ? "0000" : eAS.MainCardRATE3;
        eAS3270.SUPP_BASIC_TIER_AMT3 = "" == eAS.SUPPCardAMT3.Trim() ? "000000000" : eAS.SUPPCardAMT3;
        eAS3270.SUPP_BASIC_TIER_RATE3 = "" == eAS.SUPPCardRATE3.Trim() ? "0000" : eAS.SUPPCardRATE3;
        eAS3270.PROMO_TIER_AMT3 = "" == eAS.USEREXITAMT3.Trim() ? "000000000" : eAS.USEREXITAMT3;
        eAS3270.PROMO_TIER_RATE3 = "" == eAS.USEREXITRATE3.Trim() ? "0000" : eAS.USEREXITRATE3;
        eAS3270.BTHDTE_TIER_AMT3 = "" == eAS.BIRTHAMT3.Trim() ? "000000000" : eAS.BIRTHAMT3;
        eAS3270.BTHDTE_TIER_RATE3 = "" == eAS.BIRTHRATE3.Trim() ? "0000" : eAS.BIRTHRATE3;

        eAS3270.BASIC_TIER_AMT4 = "" == eAS.MainCardAMT4.Trim() ? "000000000" : eAS.MainCardAMT4;
        eAS3270.BASIC_TIER_RATE4 = "" == eAS.MainCardRATE4.Trim() ? "0000" : eAS.MainCardRATE4;
        eAS3270.SUPP_BASIC_TIER_AMT4 = "" == eAS.SUPPCardAMT4.Trim() ? "000000000" : eAS.SUPPCardAMT4;
        eAS3270.SUPP_BASIC_TIER_RATE4 = "" == eAS.SUPPCardRATE4.Trim() ? "0000" : eAS.SUPPCardRATE4;
        eAS3270.PROMO_TIER_AMT4 = "" == eAS.USEREXITAMT4.Trim() ? "000000000" : eAS.USEREXITAMT4;
        eAS3270.PROMO_TIER_RATE4 = "" == eAS.USEREXITRATE4.Trim() ? "0000" : eAS.USEREXITRATE4;
        eAS3270.BTHDTE_TIER_AMT4 = "" == eAS.BIRTHAMT4.Trim() ? "000000000" : eAS.BIRTHAMT4;
        eAS3270.BTHDTE_TIER_RATE4 = "" == eAS.BIRTHRATE4.Trim() ? "0000" : eAS.BIRTHRATE4;

        for (int i = 0; i < esAS_ATS.Count; i++)
        {
            EntityAwardSet3270 eAS3270I = new EntityAwardSet3270();
            eAS3270I.Clone(eAS3270);

            eAS3270I.ACCUMULATION_TYPE = esAS_ATS.GetEntity(i).CARD_TYPE;

            esAS3270.Add(eAS3270I);
        }

        return esAS3270;
    }

    private string GetCALC(string strMETHOD, string strType)
    {
        string strV = "";

        if ("M" == strType.ToUpper().Trim())
        {
            // MTHD
            switch (strMETHOD.Trim())
            {
                case "":
                    strV = "0";
                    break;
                case "0":
                case "1":
                    strV = "1";
                    break;
                case "2":
                    strV = "2";
                    break;
                default:
                    break;
            }
        }
        else
        {
            // IND
            if ("1" == strMETHOD.Trim())
            {
                strV = "1";
            }
            else
            {
                strV = "0";
            }
        }

        return strV;
    }

    private EntitySet<EntityAwardSet_ATypeSet> CreateAwardSet_ATypeSet()
    {
        int iCount = 0; //ACCUMLATION TYPE中Card Type輸入框個數
        CustTextBox ctbTmp;

        BRCardTypeList_Award.Select(1, 1, "", ref iCount);

        EntitySet<EntityAwardSet_ATypeSet> esAS_ATS = new EntitySet<EntityAwardSet_ATypeSet>();

        for (int i = 0; i < iCount; i++)
        {
            ctbTmp = (CustTextBox)atl_AT1.FindControl("tb_AType" + i.ToString());

            if ("" != ctbTmp.Text.Trim())
            {
                EntityAwardSet_ATypeSet eAS_ATS = new EntityAwardSet_ATypeSet();

                eAS_ATS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim();
                eAS_ATS.CARD_TYPE = ctbTmp.Text.Trim();
                eAS_ATS.TXT_INDEX = i.ToString();
                eAS_ATS.KEYIN_FLAG = "2";
                esAS_ATS.Add(eAS_ATS);
            }
        }

        return esAS_ATS;

    }

    private void setValue(EntityAwardSet eAS, EntitySet<EntityAwardSet_ATypeSet> esAS_ATS)
    {
        // 申請人
        lbUSERValue.Text = ((EntityAGENT_INFO)Session["Agent"]).agent_name;

        txtORG.Text = eAS.ORG.Trim();
        txtPROGRAMNO.Text = eAS.PROGRAMNO.Trim();
        txtPARTNERNO.Text = eAS.PARTNERNO.Trim();
        txtTCCODE_Add.Text = eAS.TCCODE_Add.Trim();
        txtTCCODE_Minus.Text = eAS.TCCODE_Minus.Trim();
        txtMMCCODE_1_B.Text = eAS.MMCCODE_1_B.Trim();
        txtMMCCODE_1_E.Text = eAS.MMCCODE_1_E.Trim();
        txtMMCCODE_2_B.Text = eAS.MMCCODE_2_B.Trim();
        txtMMCCODE_2_E.Text = eAS.MMCCODE_2_E.Trim();
        txtMMCCODE_3_B.Text = eAS.MMCCODE_3_B.Trim();
        txtMMCCODE_3_E.Text = eAS.MMCCODE_3_E.Trim();
        txtMMCCODE_4_B.Text = eAS.MMCCODE_4_B.Trim();
        txtMMCCODE_4_E.Text = eAS.MMCCODE_4_E.Trim();
        txtMMCCODE_5_B.Text = eAS.MMCCODE_5_B.Trim();
        txtMMCCODE_5_E.Text = eAS.MMCCODE_5_E.Trim();
        txtMMCCODE_6_B.Text = eAS.MMCCODE_6_B.Trim();
        txtMMCCODE_6_E.Text = eAS.MMCCODE_6_E.Trim();
        txtMMCCODE_7_B.Text = eAS.MMCCODE_7_B.Trim();
        txtMMCCODE_7_E.Text = eAS.MMCCODE_7_E.Trim();
        txtMMCCODE_8_B.Text = eAS.MMCCODE_8_B.Trim();
        txtMMCCODE_8_E.Text = eAS.MMCCODE_8_E.Trim();
        txtMMCCODE_9_B.Text = eAS.MMCCODE_9_B.Trim();
        txtMMCCODE_9_E.Text = eAS.MMCCODE_9_E.Trim();
        txtMMCCODE_10_B.Text = eAS.MMCCODE_10_B.Trim();
        txtMMCCODE_10_E.Text = eAS.MMCCODE_10_E.Trim();
        txtConsumptionArea.Text = eAS.ConsumptionArea.Trim();
        txtConsumptionAreaCode1.Text = eAS.ConsumptionAreaCode1.Trim();
        txtConsumptionAreaCode2.Text = eAS.ConsumptionAreaCode2.Trim();
        txtConsumptionAreaCode3.Text = eAS.ConsumptionAreaCode3.Trim();
        txtConsumptionAreaCode4.Text = eAS.ConsumptionAreaCode4.Trim();
        txtConsumptionAreaCode5.Text = eAS.ConsumptionAreaCode5.Trim();
        txtConsumptionAreaCode6.Text = eAS.ConsumptionAreaCode6.Trim();
        txtConsumptionAreaCode7.Text = eAS.ConsumptionAreaCode7.Trim();
        txtConsumptionAreaCode8.Text = eAS.ConsumptionAreaCode8.Trim();
        txtConsumptionAreaCode9.Text = eAS.ConsumptionAreaCode9.Trim();
        txtConsumptionAreaCode10.Text = eAS.ConsumptionAreaCode10.Trim();
        txtMainCardSetPoint.Text = eAS.MainCardSetPoint.Trim();
        txtSUPPCardSetPoint.Text = eAS.SUPPCardSetPoint.Trim();
        MainCardAMT1.Text = eAS.MainCardAMT1.Trim();
        MainCardRATE1.Text = eAS.MainCardRATE1.Trim();
        MainCardAMT2.Text = eAS.MainCardAMT2.Trim();
        MainCardRATE2.Text = eAS.MainCardRATE2.Trim();
        MainCardAMT3.Text = eAS.MainCardAMT3.Trim();
        MainCardRATE3.Text = eAS.MainCardRATE3.Trim();
        MainCardAMT4.Text = eAS.MainCardAMT4.Trim();
        MainCardRATE4.Text = eAS.MainCardRATE4.Trim();
        SUPPCardAMT1.Text = eAS.SUPPCardAMT1.Trim();
        SUPPCardRATE1.Text = eAS.SUPPCardRATE1.Trim();
        SUPPCardAMT2.Text = eAS.SUPPCardAMT2.Trim();
        SUPPCardRATE2.Text = eAS.SUPPCardRATE2.Trim();
        SUPPCardAMT3.Text = eAS.SUPPCardAMT3.Trim();
        SUPPCardRATE3.Text = eAS.SUPPCardRATE3.Trim();
        SUPPCardAMT4.Text = eAS.SUPPCardAMT4.Trim();
        SUPPCardRATE4.Text = eAS.SUPPCardRATE4.Trim();
        txtUSEREXITFrom.Text = eAS.USEREXITFrom.Trim();
        txtUSEREXITTo.Text = eAS.USEREXITTo.Trim();
        txtUSEREXITSetPoint.Text = eAS.USEREXITSetPoint.Trim();
        USEREXITAMT1.Text = eAS.USEREXITAMT1.Trim();
        USEREXITRATE1.Text = eAS.USEREXITRATE1.Trim();
        USEREXITAMT2.Text = eAS.USEREXITAMT2.Trim();
        USEREXITRATE2.Text = eAS.USEREXITRATE2.Trim();
        USEREXITAMT3.Text = eAS.USEREXITAMT3.Trim();
        USEREXITRATE3.Text = eAS.USEREXITRATE3.Trim();
        USEREXITAMT4.Text = eAS.USEREXITAMT4.Trim();
        USEREXITRATE4.Text = eAS.USEREXITRATE4.Trim();
        txtBIRTHFrom.Text = eAS.BIRTHFrom.Trim();
        txtBIRTHTo.Text = eAS.BIRTHTo.Trim();
        txtBIRTHSetPoint.Text = eAS.BIRTHSetPoint.Trim();
        BIRTHAMT1.Text = eAS.BIRTHAMT1.Trim();
        BIRTHRATE1.Text = eAS.BIRTHRATE1.Trim();
        BIRTHAMT2.Text = eAS.BIRTHAMT2.Trim();
        BIRTHRATE2.Text = eAS.BIRTHRATE2.Trim();
        BIRTHAMT3.Text = eAS.BIRTHAMT3.Trim();
        BIRTHRATE3.Text = eAS.BIRTHRATE3.Trim();
        BIRTHAMT4.Text = eAS.BIRTHAMT4.Trim();
        BIRTHRATE4.Text = eAS.BIRTHRATE4.Trim();

        for (int i = 0; i < esAS_ATS.Count; i++)
        {
            EntityAwardSet_ATypeSet eAS_ATS = new EntityAwardSet_ATypeSet();
            eAS_ATS = esAS_ATS.GetEntity(i);

            ((CustTextBox)atl_AT1.FindControl("tb_AType" + eAS_ATS.TXT_INDEX.ToString().Trim())).Text = eAS_ATS.CARD_TYPE.Trim();
        }

    }
    #endregion method
}
