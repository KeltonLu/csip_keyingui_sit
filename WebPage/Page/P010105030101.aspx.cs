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
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;

public partial class P010105030101 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            isEnableAll(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        setEnableForCArea();
        base.strClientMsg += "";
        base.strHostMsg += "";
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        isEnableAll(false);
        if (!txtReceiveNumber.Enabled)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        EntitySet<EntityAwardSet> esAS = new EntityAwardSetSet();
        esAS = BRAwardSet.SelectData(txtReceiveNumber.Text.Trim(), "1");

        if (esAS.Count > 0)
        {
            if ("Y" == esAS.GetEntity(0).SEND3270.ToUpper())
            {
                // 已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040102_003") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                return;
            }

            EntitySet<EntityAwardSet> esAS2 = new EntityAwardSetSet();
            esAS2 = BRAwardSet.SelectData(txtReceiveNumber.Text.Trim(), "2");
            if (esAS2.Count > 0)
            {
                // 二Key已完成
                base.strClientMsg += MessageHelper.GetMessage("01_01050100_006");
                return;
            }

            isEnableAll(true);
            setValue(esAS.GetEntity(0), BRAwardSet_ATypeSet.SelectData(txtReceiveNumber.Text.Trim(), "1"));
        }
        else
        {
            // 申請人
            lbUSERValue.Text = ((EntityAGENT_INFO)Session["Agent"]).agent_name;

            setDefaultValue();
            isEnableAll(true);
            txtORG.Focus();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        EntitySet<EntityAwardSet> esAS = new EntitySet<EntityAwardSet>();
        esAS = BRAwardSet.SelectData(txtReceiveNumber.Text, "1");

        if (esAS.Count > 0)
        {
            // Update
            if (UpdateAwardSet())
            {
                isEnableAll(false);
                base.strClientMsg += MessageHelper.GetMessage("01_01040102_001");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }

        }
        else
        {
            // Add
            if (InsertAwardSet())
            {
                isEnableAll(false);
                base.strClientMsg += MessageHelper.GetMessage("01_01040102_001");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
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

    private EntityAwardSet CreateAwardSet()
    {
        EntityAwardSet eAS = new EntityAwardSet();

        eAS.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
        eAS.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
        eAS.KEYIN_FLAG = "1";
        eAS.SEND3270 = "";
        eAS.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
        eAS.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
        eAS.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
        eAS.ISSAME = "";
        eAS.DIFF_NUM = 0;
        eAS.EDIT_USER_ID = "";


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
                eAS_ATS.KEYIN_FLAG = "1";
                esAS_ATS.Add(eAS_ATS);
            }
        }

        return esAS_ATS;

    }

    private bool InsertAwardSet()
    {
        try
        {
            return BRAwardSet.InsertKey1(CreateAwardSet(), CreateAwardSet_ATypeSet());
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

    private bool UpdateAwardSet()
    {
        try
        {
            return BRAwardSet.UpdateKey1(txtReceiveNumber.Text.Trim(), CreateAwardSet(), CreateAwardSet_ATypeSet());
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_021")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
            }

            return false;
        }
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
