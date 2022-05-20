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
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;

public partial class Common_Controls_CustATypeList : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CreateTxtBox();
        setEnable(bIsEnable);
    }

    #region 屬性
    private int iTBCount;   //總的Textbox的數量
    private int iRowTBCount = 1;  //每row中TextBox的數量
    public int RTBCount
    {
        get
        {
            return iRowTBCount;
        }
        set
        {
            iRowTBCount = value;
        }
    }

    private string strFirstClass = "";   //第一row的class
    public string FirstRowClass
    {
        get
        {
            return strFirstClass;
        }
        set
        {
            strFirstClass = value;
        }
    }

    private bool bIsAType1 = true;   //如果為ACCUMLATION TYPE 1則折抵上限、點數/金額抵用比率有說明
    public bool IsAType1
    {
        get
        {
            return bIsAType1;
        }
        set
        {
            bIsAType1 = value;
        }
    }

    private string strATypeShowID = "";   //ACCUMLATION TYPE Label的ShowID
    public string ATypeShowID
    {
        get
        {
            return strATypeShowID;
        }
        set
        {
            strATypeShowID = value;
        }
    }

    private bool bIsEnable = true;    //是否允許使用
    public bool IsEnable
    {
        get
        {
            return bIsEnable;
        }
        set
        {
            bIsEnable = value;

            if (IsPostBack)
            {
                setEnable(bIsEnable);
            }
        }
    }

    private bool bIsRedeem = true;      //Redeem使用或Award使用
    public bool IsRedeem
    {
        get
        {
            return bIsRedeem;
        }
        set
        {
            bIsRedeem = value;
        }
    }
    #endregion

    /// <summary>
    /// 創建ACCUMLATION TYPE填寫區塊
    /// </summary>
    private void CreateTxtBox()
    {
        int iTrClass;

        int iRowCount;  //總row數
        int iRowTBCountFor;   //每row中TextBox的數量循環用
        int iTBID;    //textboxID編號

        iTBID = 0;

        //設定第一row的class
        if (strFirstClass.ToUpper() == "TREVEN")
        {
            iTrClass = 1;
        }
        else
        {
            iTrClass = -1;
        }

        #region 畫面最上方選擇ACCUMLATION TYPE
        T_Top.Attributes.Add("width", "100%");
        T_Top.Attributes.Add("style", "");
        T_Top.CellPadding = 0;
        T_Top.CellSpacing = 1;

        TableRow trRow_T = new TableRow();

        TableCell tcTC1 = new TableCell();
        tcTC1.HorizontalAlign = HorizontalAlign.Left;
        tcTC1.Attributes.Add("width", "33%");
        CustLabel clAType = new CustLabel();
        clAType.ShowID = strATypeShowID;
        tcTC1.Controls.Add(clAType);
        trRow_T.Cells.Add(tcTC1);

        TableCell tcTC2 = new TableCell();
        tcTC2.HorizontalAlign = HorizontalAlign.Right;
        tcTC2.Attributes.Add("width", "33%");
        CustLabel clATList = new CustLabel();
        clATList.ShowID = "01_01050101_017";
        tcTC2.Controls.Add(clATList);
        trRow_T.Cells.Add(tcTC2);

        TableCell tcTC3 = new TableCell();
        tcTC3.HorizontalAlign = HorizontalAlign.Left;
        CustDropDownList cddlATList = new CustDropDownList();
        cddlATList.ID = "cddlATList";
        fillDropDownList(cddlATList);
        cddlATList.AutoPostBack = true;
        cddlATList.SelectedIndexChanged += new EventHandler(cddlATList_SelectedIndexChanged);
        tcTC3.Controls.Add(cddlATList);
        trRow_T.Cells.Add(tcTC3);

        SetClass(ref iTrClass, trRow_T);

        T_Top.Rows.Add(trRow_T);
        #endregion

        #region 畫面中部填寫的Grid
        iRowTBCountFor = iRowTBCount;

        T_ArryTxtBox.Attributes.Add("width", "100%");
        T_ArryTxtBox.Attributes.Add("style", "");
        T_ArryTxtBox.CellPadding = 0;
        T_ArryTxtBox.CellSpacing = 1;

        //輸入框個數
        if (IsRedeem)
        {
            // Redeem
            BRCardTypeList_Redeem.Select(1, 1, "", ref iTBCount);
        }
        else
        {
            // Award
            BRCardTypeList_Award.Select(1, 1, "", ref iTBCount);
        }
        //iTBCount = 93;

        //Table的Row數
        iRowCount = iTBCount / iRowTBCount;
        if (iTBCount % iRowTBCount != 0)
        {
            iRowCount = iRowCount + 1;
        }

        for (int i = 0; i < iRowCount; i++)
        {
            TableRow trRow = new TableRow();
            SetClass(ref iTrClass, trRow);

            if (i == iRowCount - 1)
            {

                iRowTBCountFor = 0 == iTBCount % iRowTBCount ? iRowTBCountFor : iTBCount % iRowTBCount;
            }

            for (int j = 0; j < iRowTBCountFor; j++)
            {
                TableCell tcCell = new TableCell();
                tcCell.HorizontalAlign = HorizontalAlign.Center;

                CustTextBox ctbTBox = new CustTextBox();

                ctbTBox.MaxLength = 3;
                ctbTBox.Width = 34;
                ctbTBox.Attributes.Add("checktype", "num");
                ctbTBox.Attributes.Add("onfocus", "allselect(this);");
                //ctbTBox.InputType = CustTextBox.textType.Int;

                ctbTBox.ID = "tb_AType" + iTBID.ToString();
                iTBID++;

                tcCell.Controls.Add(ctbTBox);

                trRow.Cells.Add(tcCell);
            }

            if (i == iRowCount - 1)
            {
                TableCell tcCell = new TableCell();
                tcCell.ColumnSpan = iRowTBCount - (iTBCount % iRowTBCount);

                trRow.Cells.Add(tcCell);
            }

            T_ArryTxtBox.Rows.Add(trRow);

        }
        #endregion

        #region 畫面下方填寫的折抵上限、點數/金額抵用比率
        if (bIsRedeem)
        {
            if (bIsAType1)
            {
                //Title
                TableRow trTRBTitle = new TableRow();
                SetClass(ref iTrClass, trTRBTitle);

                TableCell tcTCBTitle = new TableCell();
                tcTCBTitle.ColumnSpan = iRowTBCount;

                CustLabel clCLBTitle = new CustLabel();
                clCLBTitle.ShowID = "01_01050101_018";
                clCLBTitle.ForeColor = System.Drawing.Color.Tomato;

                tcTCBTitle.Controls.Add(clCLBTitle);
                trTRBTitle.Cells.Add(tcTCBTitle);
                T_ArryTxtBox.Rows.Add(trTRBTitle);

                TableRow trTRB1 = new TableRow();
            }

            //折抵上限
            TableRow trTRBLimit = new TableRow();
            SetClass(ref iTrClass, trTRBLimit);

            TableCell tcTCBLimit = new TableCell();
            tcTCBLimit.ColumnSpan = iRowTBCount;

            CustLabel clCLBLimit1 = new CustLabel();
            clCLBLimit1.ShowID = "01_01050101_019";
            clCLBLimit1.IsColon = true;
            CustTextBox tbTBBLimit_INT = new CustTextBox();
            tbTBBLimit_INT.ID = "tbTBBLimit_INT";
            tbTBBLimit_INT.MaxLength = 3;
            tbTBBLimit_INT.Width = 40;
            tbTBBLimit_INT.Attributes.Add("checktype", "num");
            tbTBBLimit_INT.Attributes.Add("onfocus", "allselect(this);");
            CustLabel clCLBLimit2 = new CustLabel();
            clCLBLimit2.ShowID = "01_01050101_010";
            CustTextBox tbTBBLimit_DEC = new CustTextBox();
            tbTBBLimit_DEC.ID = "tbTBBLimit_DEC";
            tbTBBLimit_DEC.MaxLength = 2;
            tbTBBLimit_DEC.Width = 40;
            tbTBBLimit_DEC.Attributes.Add("checktype", "num");
            tbTBBLimit_DEC.Attributes.Add("onfocus", "allselect(this);");
            CustLabel clCLBLimit3 = new CustLabel();
            clCLBLimit3.ShowID = "01_01050101_011";

            tcTCBLimit.Controls.Add(clCLBLimit1);
            tcTCBLimit.Controls.Add(tbTBBLimit_INT);
            tcTCBLimit.Controls.Add(clCLBLimit2);
            tcTCBLimit.Controls.Add(tbTBBLimit_DEC);
            tcTCBLimit.Controls.Add(clCLBLimit3);

            trTRBLimit.Cells.Add(tcTCBLimit);
            T_ArryTxtBox.Rows.Add(trTRBLimit);

            //點數/金額抵用比率
            TableRow trTRBRate = new TableRow();
            SetClass(ref iTrClass, trTRBRate);

            TableCell tcTCBRate = new TableCell();
            tcTCBRate.ColumnSpan = iRowTBCount;

            CustLabel clCLBRate1 = new CustLabel();
            clCLBRate1.ShowID = "01_01050101_020";
            clCLBRate1.IsColon = true;
            CustTextBox tbTBBPoints = new CustTextBox();
            tbTBBPoints.ID = "tbTBBPoints";
            tbTBBPoints.MaxLength = 9;
            tbTBBPoints.Width = 70;
            tbTBBPoints.Attributes.Add("checktype", "num");
            tbTBBPoints.Attributes.Add("onfocus", "allselect(this);");
            CustLabel clCLBRate2 = new CustLabel();
            clCLBRate2.ShowID = "01_01050101_021";
            CustTextBox tbTBBAmount = new CustTextBox();
            tbTBBAmount.ID = "tbTBBAmount";
            tbTBBAmount.MaxLength = 10;
            tbTBBAmount.Width = 80;
            tbTBBAmount.Attributes.Add("checktype", "num");
            tbTBBAmount.Attributes.Add("onfocus", "allselect(this);");
            CustLabel clCLBRate3 = new CustLabel();
            clCLBRate3.ShowID = "01_01050101_022";

            tcTCBRate.Controls.Add(clCLBRate1);
            tcTCBRate.Controls.Add(tbTBBPoints);
            tcTCBRate.Controls.Add(clCLBRate2);
            tcTCBRate.Controls.Add(tbTBBAmount);
            tcTCBRate.Controls.Add(clCLBRate3);

            trTRBRate.Cells.Add(tcTCBRate);
            T_ArryTxtBox.Rows.Add(trTRBRate);
        }
        #endregion
    }

    protected void cddlATList_SelectedIndexChanged(object sender, EventArgs e)
    {
        CustTextBox ctbTmp;
        EntitySet<EntityACCUMGroupTable_Redeem> esGroupR = new EntitySet<EntityACCUMGroupTable_Redeem>();
        EntitySet<EntityACCUMGroupTable_Award> esGroupA = new EntitySet<EntityACCUMGroupTable_Award>();
        int iCount = 0;

        for (int i = 0; i < iTBCount; i++)
        {
            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tb_AType" + i.ToString());
            ctbTmp.Text = "";
        }

        if (IsRedeem)
        {
            // Redeem
            esGroupR = BRACCUMGroupTable_Redeem.Select(((CustDropDownList)sender).SelectedValue.ToString().Trim());
            iCount = esGroupR.Count;
        }
        else
        {
            // Award
            esGroupA = BRACCUMGroupTable_Award.Select(((CustDropDownList)sender).SelectedValue.ToString().Trim());
            iCount = esGroupA.Count;
        }

        if (iCount > 0)
        {
            for (int i = 0; i < iCount; i++)
            {
                ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tb_AType" + i.ToString());

                if (IsRedeem)
                {
                    ctbTmp.Text = esGroupR.GetEntity(i).Card_CODE.ToString().Trim();
                }
                else
                {
                    ctbTmp.Text = esGroupA.GetEntity(i).Card_CODE.ToString().Trim();
                }
            }
        }

    }

    /// <summary>
    /// 設定TABLEROW的CLASS
    /// </summary>
    /// <param name="iTrClass">class設定</param>
    /// <param name="trTR">TABLEROW</param>
    private void SetClass(ref int iTrClass, TableRow trTR)
    {
        iTrClass = iTrClass * (-1);
        if ((-1) == iTrClass)
        {
            trTR.Attributes.Add("class", "trEven");
        }
        else
        {
            trTR.Attributes.Add("class", "trOdd");
        }
    }

    /// <summary>
    /// 設定是否禁止輸入框使用
    /// </summary>
    /// <param name="bIsEnable">true：可以使用，false：不能使用</param>
    private void setEnable(bool bIsEnable)
    {
        CustTextBox ctbTmp;

        for (int i = 0; i < iTBCount; i++)
        {
            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tb_AType" + i.ToString());
            ctbTmp.Enabled = bIsEnable;

            if (!bIsEnable)
            {
                ctbTmp.Text = "";
            }
        }

        if (bIsRedeem)
        {
            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tbTBBLimit_INT");
            ctbTmp.Enabled = bIsEnable;
            if (!bIsEnable) { ctbTmp.Text = ""; }

            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tbTBBLimit_DEC");
            ctbTmp.Enabled = bIsEnable;
            if (!bIsEnable) { ctbTmp.Text = ""; }

            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tbTBBPoints");
            ctbTmp.Enabled = bIsEnable;
            if (!bIsEnable) { ctbTmp.Text = ""; }

            ctbTmp = (CustTextBox)T_ArryTxtBox.FindControl("tbTBBAmount");
            ctbTmp.Enabled = bIsEnable;
            if (!bIsEnable) { ctbTmp.Text = ""; }
        }

        CustDropDownList cddlTmp = (CustDropDownList)T_Top.FindControl("cddlATList");
        cddlTmp.Enabled = bIsEnable;

        if (!bIsEnable) { cddlTmp.SelectedIndex = 0; }

    }

    private void fillDropDownList(CustDropDownList cddl)
    {
        DataTable dt = new DataTable();

        if (IsRedeem)
        {
            // Redeem
            dt = BRACCUMTYPEList_Redeem.SelectAll().Tables[0];
        }
        else
        {
            // Award
            dt = BRACCUMTYPEList_Award.SelectAll().Tables[0];
        }

        cddl.Items.Clear();

        cddl.Items.Add(new ListItem(BaseHelper.GetShowText("01_00000000_003"), "-1"));

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            cddl.Items.Add(new ListItem(dt.Rows[i]["MEMO"].ToString().Trim(), dt.Rows[i]["CODE"].ToString().Trim()));
        }

    }
}
