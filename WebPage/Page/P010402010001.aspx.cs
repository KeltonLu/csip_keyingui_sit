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
using Framework.Common.Message;
using Framework.Common.IO;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;

public partial class P010402010001 : PageBase
{
    #region event

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            isEnableAll(false);
        }
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        isEnableAll(false);
        if (!txtReceiveNumber.Enabled)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            return;
        }

        EntitySet<EntityRedeemUpload> esRU = new EntitySet<EntityRedeemUpload>();
        esRU = BRRedeemUpload.SelectData(txtReceiveNumber.Text.Trim(), "1");

        if (esRU.Count > 0)
        {
            EntitySet<EntityRedeemUpload> esRU2 = new EntitySet<EntityRedeemUpload>();
            esRU2 = BRRedeemUpload.SelectData(txtReceiveNumber.Text.Trim(), "2");
            if (esRU2.Count > 0)
            {
                if ("Y" == esRU2.GetEntity(0).SEND3270.ToUpper())
                {
                    // 已上傳主機
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050200_012") + "');");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                    return;
                }

                if ("N" == esRU2.GetEntity(0).SEND3270.ToUpper())
                {
                    // 已比對完成
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01050200_013") + "');");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                    return;
                }
            }

            isEnableAll(true);
            SetValue(esRU);
        }
        else
        {
            isEnableAll(true);
            cFUpload.Focus();
        }
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("01_01050201_005");
            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_01050201_006");
        }

    }

    protected void btnShowData_Click(object sender, EventArgs e)
    {
        ShowData();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        EntitySet<EntityRedeemUpload> esRU = new EntitySet<EntityRedeemUpload>();
        esRU = BRRedeemUpload.SelectData(txtReceiveNumber.Text, "1");

        if (esRU.Count > 0)
        {
            // Update
            if (UpdateRedeemUpload())
            {
                isEnableAll(false);
                base.strClientMsg += MessageHelper.GetMessage("01_01040102_001");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }

        }
        else
        {
            // Add
            if (InsertRedeemUpload())
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

        cFUpload.Enabled = bIsEnable;
        btnShowData.Enabled = bIsEnable;

        txtReceiveNumber.Enabled = !bIsEnable;


        if (!bIsEnable)
        {
            pnList.Visible = bIsEnable;
        }
    }

    private void ShowData()
    {
        string strServerPath = "";  //檔案上傳到server端后的路徑
        string strMsgID = "";

        if (this.cFUpload.PostedFile.FileName.Trim() == "")
        {
            strAlertMsg = MessageHelper.GetMessage("01_01050200_001");
            return;
        }

        if (cFUpload.PostedFile.ContentLength == 0)
        {
            strAlertMsg = MessageHelper.GetMessage("01_01050200_002");
            return;
        }

        strServerPath = FileUpload(cFUpload.PostedFile, ref strMsgID);

        if ("" == strServerPath)
        {
            strAlertMsg = MessageHelper.GetMessage(strMsgID);
            return;
        }

        string[] arryFile = FileTools.Read(strServerPath.Trim());
        string strT = "";   // DataType
        string strD = "";   // 內容
        string strNowT = "";    // 當前列的 DataType

        DataTable dtList = new DataTable();

        dtList.Columns.Add(new DataColumn("DATA_TYPE", typeof(string)));
        dtList.Columns.Add(new DataColumn("UPLOAD_DATA", typeof(string)));

        pnList.Visible = false;

        for (int i = 0; i < arryFile.Length; i++)
        {
            DataRow dr = dtList.NewRow();

            if (!strSplit(arryFile[i], ref strT, ref strD))
            {
                return;
            }

            // 第一列
            if (0 == i)
            {
                if ("S" != strT.ToUpper())
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_003");
                    return;
                }

                if (strD.Length > 12)
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_004");
                    return;
                }
            }

            // 中間內容列
            if (0 != i && i != arryFile.Length - 1)
            {
                if ("T" != strT && "M" != strT)
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_005");
                    return;
                }

                if ("" == strNowT)
                {
                    if ("M" == strT)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_006");
                        return;
                    }

                    if (strD.Length > 5)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_007");
                        return;
                    }

                    strNowT = "T";
                }
                else if ("T" == strNowT)
                {
                    if ("T" == strT)
                    {
                        if (strD.Length > 5)
                        {
                            strAlertMsg = MessageHelper.GetMessage("01_01050200_007");
                            return;
                        }
                    }
                    else
                    {
                        if (strD.Length > 12)
                        {
                            strAlertMsg = MessageHelper.GetMessage("01_01050200_008");
                            return;
                        }
                        strNowT = "M";
                    }
                }
                else
                {
                    if ("T" == strT)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_006");
                        return;
                    }

                    if (strD.Length > 12)
                    {
                        strAlertMsg = MessageHelper.GetMessage("01_01050200_008");
                        return;
                    }
                }

            }

            // 結束列
            if (i == arryFile.Length - 1)
            {
                if ("E" != arryFile[arryFile.Length - 1].Trim().ToUpper())
                {
                    strAlertMsg = MessageHelper.GetMessage("01_01050200_009");
                    return;
                }
            }

            dr["DATA_TYPE"] = strT;
            dr["UPLOAD_DATA"] = strD;
            dtList.Rows.Add(dr);
        }

        pnList.Visible = true;
        gvList.DataSource = dtList;
        gvList.DataBind();
    }

    private bool strSplit(string str, ref string strT, ref string strD)
    {
        str = str.Trim().Trim('\t');

        str = str + "\t";

        strT = str.Split('\t')[0].Trim();
        strD = str.Split('\t')[1].Trim();

        if ("" == strD)
        {
            if (strT != "E")
            {
                strAlertMsg = strT + MessageHelper.GetMessage("01_01050200_010");
                return false;
            }
        }
        else
        {
            if ("E" == strT)
            {
                strAlertMsg = MessageHelper.GetMessage("01_01050200_011");
                return false;
            }
        }

        return true;
    }

    private EntitySet<EntityRedeemUpload> CreateES()
    {
        EntitySet<EntityRedeemUpload> esRU = new EntitySet<EntityRedeemUpload>();

        for (int i = 0; i < gvList.Rows.Count; i++)
        {
            EntityRedeemUpload eRU = new EntityRedeemUpload();

            eRU.RECEIVE_NUMBER = txtReceiveNumber.Text.Trim().ToUpper();
            eRU.USER_ID = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
            eRU.DATA_TYPE = gvList.Rows[i].Cells[0].Text.Trim().ToUpper().Replace("&nbsp;","");
            eRU.UPLOAD_DATA = gvList.Rows[i].Cells[1].Text.Trim().Replace("&nbsp;", "");
            eRU.LINE_INDEX = i;
            eRU.KEYIN_DATE = DateTime.Now.ToString("yyyyMMdd");
            eRU.UPDATE_DATE = DateTime.Now.ToString("yyyyMMdd");
            eRU.KEYIN_FLAG = "1";
            eRU.SEND3270 = "";
            eRU.USE_TIME = (System.Environment.TickCount - (int)ViewState["lStartTime"]) / 1000;
            eRU.ISSAME = "";
            eRU.DIFF_NUM = 0;
            eRU.EDIT_USER_ID = "";

            esRU.Add(eRU);
        }

        return esRU;
    }

    private bool InsertRedeemUpload()
    {
        try
        {
            return BRRedeemUpload.AddBatch(CreateES());
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

    private bool UpdateRedeemUpload()
    {
        try
        {
            return BRRedeemUpload.UpdateKey(txtReceiveNumber.Text.Trim(), CreateES());
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

    private void SetValue(EntitySet<EntityRedeemUpload> esRU)
    {
        pnList.Visible = true;
        gvList.DataSource = esRU;
        gvList.DataBind();
    }
    #endregion method


}
