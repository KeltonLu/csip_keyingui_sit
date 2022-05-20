using System;
using System.Data;
using CSIPCommonModel.BusinessRules;
using System.Text;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.IO;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer;

/// <summary>
/// PostOfficeService 的摘要描述
/// </summary>
public class PostAddLebelService
{
    private string jobID = string.Empty;

    public PostAddLebelService(string jobID)
    {
        this.jobID = jobID;
    }

    /// <summary>
    /// 算起迄時間
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    public void GetSearchTime(DateTime dateTime, out string startTime, out string endTime)
    {
        startTime = "";
        endTime = "";
        int day = (int)dateTime.DayOfWeek;
        int addDay = 0;
        DateTime operatorTime;

        switch (day)
        {
            case 5:
                addDay = -6;
                break;
            case 6:
                addDay = -7;
                break;
            case 0:
                addDay = -8;
                break;
            case 1:
                addDay = -9;
                break;
            case 2:
                addDay = -10;
                break;
            case 3:
                addDay = -11;
                break;
            case 4:
                addDay = -12;
                break;
        }

        operatorTime = dateTime.AddDays(addDay);
        startTime = operatorTime.ToString("yyyyMMdd");
        endTime = operatorTime.AddDays(6).ToString("yyyyMMdd");
    }

    public void GetReplyTime(DateTime dateTime, out string replyTime)
    {
        replyTime = "";
        int day = (int)dateTime.DayOfWeek;
        int addDay = 0;
        DateTime operatorTime;

        switch (day)
        {
            case 5:
                addDay = -7;
                break;
            case 6:
                addDay = -1;
                break;
            case 0:
                addDay = -2;
                break;
            case 1:
                addDay = -3;
                break;
            case 2:
                addDay = -4;
                break;
            case 3:
                addDay = -5;
                break;
            case 4:
                addDay = -6;
                break;
        }

        operatorTime = dateTime.AddDays(addDay);
        replyTime = operatorTime.ToString("yyyyMMdd");
    }

    /// <summary>
    /// 取郵局核印資料
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public DataTable GetSendToPostOfficeData(string endTime)
    {
        return BRFORM_COLUMN.GetCaseToAMLData(endTime);
    }


    /// <summary>
    /// 取郵局核印資料
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public DataTable GetAddressLebelData(string endTime)
    {
        return BRFORM_COLUMN.GetAddressLebelData(endTime);
    }



    /// <summary>
    /// 更新地址條曾經送過
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool updatelabelsended(string ID)
    {
        return BRFORM_COLUMN.updateLabelSended(ID);
    }


    /// <summary>
    /// 取得有地址為NULL的資料
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public DataTable GetLebelDataNull(string endTime)
    {
        return BRFORM_COLUMN.GetLebelDataNull(endTime);
    }




    /// <summary>
    /// 組 DataTable 資訊
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public DataTable SetLocalFileInfoColumn(string path, string fileName)
    {
        DataTable LocalFileInfo = new DataTable();// 上傳檔案資訊
        LocalFileInfo.Columns.Add("LocalFilePath", typeof(System.String));// 路逕
        LocalFileInfo.Columns.Add("LocalFileName", typeof(System.String));// 檔案名稱

        DataRow row = LocalFileInfo.NewRow();
        row["LocalFilePath"] = path;
        row["LocalFileName"] = fileName;
        LocalFileInfo.Rows.Add(row);
        return LocalFileInfo;
    }

    public bool InsertFileData(string fileName, DateTime dateTime, DataTable postData)
    {
        bool isFileExist = false;
        bool bulklySuccess = false;
        bool trailerOK = false;

        int totalCount = 0;
        DataTable postOfficeDetail = new DataTable();
        DataTable postOfficeTrailer = new DataTable();

        if (postData.Rows.Count == 0)
        {
            return false;
        }

        // 檢查資料是否已存在
        isFileExist = BRFORM_COLUMN.IsFileExist(fileName);

        // 存在資料，不再 Insert
        if (!isFileExist)
        {
            // Insert Master Table 取 MasterID
            int masterID = BRFORM_COLUMN.InsertPostMaster(fileName, dateTime, false, "");

            if (masterID > 0)
            {
                // 組 Detail
                postOfficeDetail = OperatorPostDetail(masterID, postData, dateTime.ToString("yyyyMMdd"), out totalCount);

                // Insert Detail
                bulklySuccess = BulklyInsertPostDetail("PostOffice_Detail", postOfficeDetail);

                if (bulklySuccess)
                {
                    // 組 Trailer 資訊
                    postOfficeTrailer = OperatorPostTrailer(masterID, dateTime.ToString("yyyyMMdd"), totalCount);

                    //  Insert Trailer
                    trailerOK = BRFORM_COLUMN.InsertPostTrailer(postOfficeTrailer);

                    if (trailerOK)
                    {
                        foreach (DataRow row in postData.Rows)
                        {
                            // 更新 PostOffice_Temp.IsSendToPost 狀態
                            BRFORM_COLUMN.UpdatePostOfficeTempIsSendToPost(row);
                        }
                    }
                }
            }

            return trailerOK;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 郵局核印傳送資料組成
    /// </summary>
    /// <param name="postData"></param>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public StringBuilder SetInvContent(string fileName)
    //public DataTable SetInvContent(string fileName)
    {
        DataTable postDetail = BRFORM_COLUMN.GetDetailData(fileName);
        StringBuilder sb = new StringBuilder();
        DataTable postTrailer = BRFORM_COLUMN.GetTrailerData(postDetail.Rows[0]["ID"].ToString());
        DataRow trailerRow = null;
        DataTable dt = new DataTable();
        dt.Columns.Add("Data", typeof(System.String));
        DataRow dr;
       
        string postDataContent = string.Empty;
        string trailerContent = string.Empty;

        foreach (DataRow row in postDetail.Rows)
        {
            postDataContent = row["DataType"].ToString();           // 1. 1 資料別          (固定值為1)
            postDataContent += row["AgencyCode"].ToString();        // 2. 3 委託機構代號    (大寫英數字)
            postDataContent += row["Filler1"].ToString();           // 3. 4 保留欄          (空白)
            postDataContent += row["FileCreateDate"].ToString();    // 4. 8 媒體產生日期    (西元年月日YYYYMMDD)
            postDataContent += row["BatchNo"].ToString();           // 5. 3 批號            (固定值為001)
            postDataContent += row["RowNo"].ToString();             // 6. 6 流水號          (每批自000001序編)
            postDataContent += row["ApplyType"].ToString();         // 7. 1 申請代號        (1：申請 2：終止；(郵局主動)3：郵局終止 4：誤終止-已回復為申請)
            postDataContent += row["AccountType"].ToString();       // 8. 1 帳戶別          (P：存簿 G：劃撥)
            postDataContent += row["AccountNo"].ToString();         // 9.14 儲金帳號        (存簿：局帳號計14碼 劃撥：000000+8碼帳號)
            postDataContent += row["CusID"].ToString();             //10.20 用戶編號        (右靠左補空，大寫英數字，不得填寫中文 由委託機構自行編給其客戶之編號)
            postDataContent += row["AccID"].ToString();             //11.10 身分證統一編號/統一證號 (左靠右補空白)
            postDataContent += row["StatusType"].ToString();        //12. 2 狀況代號        (初始值為空白，回送資料請參閱媒體資料不符代號一覽表)
            postDataContent += row["CheckFlag"].ToString();         //13. 1 核對註記        (初始值為空白，回送資料請參閱媒體資料不符代號一覽表)
            postDataContent += row["Filler2"].ToString();           //14.26 保留欄          (空白)
            postDataContent += "\r\n";

            sb.Append(postDataContent);

            dr = dt.NewRow();
            dr["Data"] = postDataContent;
            dt.Rows.Add(dr);

        }

        trailerRow = postTrailer.Rows[0];
        trailerContent = trailerRow["DataType"].ToString();         // 1. 1 資料別          (固定值為2)
        trailerContent += trailerRow["AgencyCode"].ToString();      // 2. 3 委託機構代號    (同明細)
        trailerContent += trailerRow["Filler1"].ToString();         // 3. 4 保留欄          (空白)
        trailerContent += trailerRow["FileCreateDate"].ToString();  // 4. 8 媒體產生日期    (同明細)
        trailerContent += trailerRow["BatchNo"].ToString();         // 5. 3 批號            (同明細)
        trailerContent += trailerRow["CreateType"].ToString();      // 6. 1 建檔記號        (固定值B：委託機構送件)
        trailerContent += trailerRow["TotalCount"].ToString();      // 7. 6 總筆數          (右靠左補0)
        trailerContent += trailerRow["PostCreateDate"].ToString();  // 8. 8 資料建檔日期    (初始值為空白，回送時使用)
        trailerContent += trailerRow["FailCount"].ToString();       // 9. 6 錯誤筆數        (初始值為0，回送時使用)
        trailerContent += trailerRow["SuccessCount"].ToString();    //10. 6 成功筆數        (初始值為0，回送時使用)
        trailerContent += trailerRow["Filler2"].ToString();         //11.54 保留欄

        sb.Append(trailerContent);

        dr = dt.NewRow();
        dr["Data"] = trailerContent;
        dt.Rows.Add(dr);
        return sb;
        //return dt;
    }

    public bool UploadFileToMFTP(string jobID, DataTable localFileInfo, string localPath, string fileName)
    {
        try
        {
            bool result = true;
            string ftpIP = string.Empty;
            string ftpId = string.Empty;
            string ftpPwd = string.Empty;
            string ftpPath = string.Empty;
            string ZipPwd = string.Empty;

            bool isGet = BRFORM_COLUMN.GetFTPInfo2(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

            string itemName = string.Empty;
            bool isSuccess = false;
            int falseCount = 0;

            // 檔案加密
            //bool isFileZip = ZipFile(jobID, localPath, fileName, ZipPwd);

            FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            DirectoryInfo di = new DirectoryInfo(localPath);
            foreach (FileInfo file in di.GetFiles())
            {
                /*
                if (file.Name.IndexOf(".ZIP") >= 0)
                {
                    itemName = file.Name;
                    // FTP目錄名稱+FTP檔案名稱+Local檔案名稱
                    isSuccess = objFtp.Upload(ftpPath, itemName, localPath + itemName);
                    if (!isSuccess)
                    {
                        result = false;
                        Logging.SaveLog(ELogLayer.Util, this.jobID + itemName + "上傳MFTP失敗", ELogType.Debug);
                        falseCount++;
                    }
                }
                */


                isSuccess = objFtp.Upload(ftpPath, fileName, localPath + fileName);
                if (!isSuccess)
                {
                    result = false;
                    Logging.Log(this.jobID + itemName + "上傳MFTP失敗", LogState.Error, LogLayer.Util);
                    falseCount++;
                }

            }

            if (falseCount == 0)
            {
                DeleteFile(localPath);
                result = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(this.jobID + " 上傳MFTP失敗：" + ex.ToString(), LogState.Error, LogLayer.Util);
            return false;
        }
    }

    /*======================================================================== Reply =====================================================================================*/

    /// <summary>
    /// 取回覆檔資訊
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public DataTable GetReplyInformation(DateTime dateTime)
    {
        return BRFORM_COLUMN.GetReplyInformation(dateTime);
    }


    public string DownloadFile(string jobID, string fileName, out string ZipPwd)
    {
        DataTable dtLocalFile = new DataTable();
        JobHelper JobHelper = new JobHelper();
        JobHelper.strJobID = jobID;
        string localPath = "";
        ZipPwd = string.Empty;
        try
        {

            string folderName = string.Empty;// 本地存放目錄(格式為JobID+yyyyMMddHHmmss)
            JobHelper.CreateFolderName(jobID, ref folderName);

            string ftpFileName = string.Empty;// FTP檔名
            string ftpFileInfo = string.Empty;// FTP完整路徑(路徑+檔名)

            string ftpIP = string.Empty;
            string ftpId = string.Empty;
            string ftpPwd = string.Empty;
            string ftpPath = string.Empty;

            bool isGet = BRFORM_COLUMN.GetFTPInfoDownload(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

            FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", localPath, "Y");

            ftpFileInfo = ftpPath + fileName;

            if (objFtp.isInFolderList(ftpFileInfo))
            {
                // 檔案存在
                JobHelper.Write(jobID, "開始下載檔案！", LogState.Info);

                // 本地路徑
                localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

                // 下載檔案(FTP路徑, FTP檔名, 本地路徑, FTP檔名)
                if (objFtp.Download(ftpPath, fileName, localPath, fileName))
                {
                    JobHelper.Write(jobID, ftpFileName + " FTP 取檔成功", LogState.Info);
                }
                else
                {
                    localPath = "";
                }
            }
            else
            {
                // 檔案不存在
                JobHelper.Write(jobID, "[FAIL] 檔案: " + ftpFileInfo + " 不存在， FTP 取檔失敗");
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL] " + ex.ToString());
        }

        return localPath;
    }

    /// <summary>
    /// 解壓縮
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="zipFileName"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public bool DecompressFile(string jobID, string filePath, string zipFileName, string pwd)
    {
        JobHelper JobHelper = new JobHelper();
        JobHelper.strJobID = jobID;
        bool unZipResult = false;

        try
        {
            int ZipCount = 0;
            unZipResult = JobHelper.ZipExeFile(filePath, filePath + zipFileName, pwd, ref ZipCount);

        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, ex.ToString());
        }

        return unZipResult;
    }

    /// <summary>
    /// 組回覆檔資料
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="masterID"></param>
    /// <returns></returns>
    public DataTable GetReplyFileStream(string filePath, out DataTable activeData)
    {
        DataTable replyData = SetReplyDetailColumn();
        activeData = SetReplyDetailColumn();
        DataRow row;
        StreamReader sr = new StreamReader(filePath, Encoding.Default);

        string line;
        string dataType = "";
        string agencyCode = "";
        string filler1 = "";
        string fileCreateDate = "";
        string batchNo = "";
        string rowNo = "";
        string applyType = "";
        string accountType = "";
        string accountNo = "";
        string cusID = "";
        string accID = "";
        string statusType = "";
        string checkFlag = "";
        string filler2 = "";
        string postCreateDate = "";
        string failCount = "";
        string successCount = "";
        string trailerFiller2 = "";
        string typeFlag = "";

        while ((line = sr.ReadLine()) != null)
        {
            if (line.Trim() != "")
            {
                dataType = line.Substring(0, 1);            // 資料別
                agencyCode = line.Substring(1, 3);          // 委託機構代號
                filler1 = line.Substring(4, 4);             // 保留欄
                fileCreateDate = line.Substring(8, 8);      // 媒體產生日期
                batchNo = line.Substring(16, 3);            // 批號
                rowNo = line.Substring(19, 6);              // 流水號
                applyType = line.Substring(25, 1);          // 申請代號
                accountType = line.Substring(26, 1);        // 帳戶別
                accountNo = line.Substring(27, 14);         // 儲金帳號
                cusID = line.Substring(41, 20);             // 用戶編號
                accID = line.Substring(61, 10);             // 統一證號
                statusType = line.Substring(71, 2);         // 狀況代號
                checkFlag = line.Substring(73, 1);          // 核對註記
                filler2 = line.Substring(74, 26);           // 保留欄
                postCreateDate = line.Substring(26, 8);
                failCount = line.Substring(34, 6);
                successCount = line.Substring(40, 6);
                trailerFiller2 = line.Substring(46, 54);
                typeFlag = line.Substring(19, 1);
                if (applyType == "1" || applyType == "2" || typeFlag == "B")
                {
                    row = replyData.NewRow();
                    row["DataType"] = dataType;

                    if (dataType == "1")
                    {
                        row["RowNo"] = rowNo;
                        row["FileCreateDate"] = fileCreateDate;
                        row["ApplyType"] = applyType;
                        row["AccountType"] = accountType;
                        row["AccountNo"] = accountNo;
                        row["CusID"] = cusID;
                        row["AccID"] = accID;
                        row["StatusType"] = statusType;
                        row["CheckFlag"] = checkFlag;
                        row["Filler2"] = filler2;
                        row["Complete"] = true;
                    }
                    else
                    {
                        row["PostCreateDate"] = postCreateDate;
                        row["FailCount"] = failCount;
                        row["SuccessCount"] = successCount;
                        row["TrailerFiller2"] = trailerFiller2;
                    }

                    replyData.Rows.Add(row);
                }
                else
                {
                    row = activeData.NewRow();
                    row["DataType"] = dataType;

                    if (dataType == "1")
                    {
                        row["RowNo"] = rowNo;
                        row["AgencyCode"] = agencyCode;
                        row["Filler1"] = filler1;
                        row["FileCreateDate"] = fileCreateDate;
                        row["BatchNo"] = batchNo;
                        row["RowNo"] = rowNo;
                        row["ApplyType"] = applyType;
                        row["AccountType"] = accountType;
                        row["AccountNo"] = accountNo;
                        row["CusID"] = cusID;
                        row["AccID"] = accID;
                        row["StatusType"] = statusType;
                        row["CheckFlag"] = checkFlag;
                        row["Filler2"] = filler2;
                        row["Complete"] = true;
                    }
                    else
                    {
                        row["PostCreateDate"] = postCreateDate;
                        row["FailCount"] = failCount;
                        row["SuccessCount"] = successCount;
                        row["TrailerFiller2"] = trailerFiller2;
                    }

                    activeData.Rows.Add(row);
                }
            }
        }

        return replyData;
    }

    /// <summary>
    /// 回寫回覆檔相關 Table
    /// </summary>
    /// <param name="replyData"></param>
    /// <returns></returns>
    public DataTable UpdateReplyRelatedTable(string masterID, DataTable replyData, DataTable activeData, out int successCount, out int failCount, out int postActiveCount)
    {
        DataTable sendHostData = SetHostDataColumn();
        DataTable postOfficeTemp = new DataTable();
        DataRow hostRow;
        // 回寫回覆檔 Master
        bool masterOK = false;
        bool detailOK = false;
        bool trailerOK = false;
        bool postResultCode = false;
        int totalCount = 0;
        successCount = 0;
        failCount = 0;
        postActiveCount = 0;

        using (OMTransactionScope ts = new OMTransactionScope())
        {
            if (masterID != "" && masterID != "0")
            {
                masterOK = BRFORM_COLUMN.UpDateReplyMaster(masterID);

                if (masterOK)
                {
                    foreach (DataRow row in replyData.Rows)
                    {
                        if (row["DataType"].ToString().Trim() == "1")
                        {
                            // 回寫回覆檔 Detail
                            detailOK = BRFORM_COLUMN.UpDateReplyDetail(row, masterID);
                            postOfficeTemp = BRFORM_COLUMN.GetReceiveNumber(row);
                            BRFORM_COLUMN.UpDatePostTemp(postOfficeTemp.Rows[0]["ReceiveNumber"].ToString(), row);

                            if (detailOK)
                            {
                                totalCount = totalCount + 1;
                            }

                            // 判斷郵局回傳資訊
                            postResultCode = CheckPostResultCode(row["StatusType"].ToString().Trim(), row["CheckFlag"].ToString().Trim());

                            if (postResultCode)
                            {

                                hostRow = sendHostData.NewRow();
                                hostRow["CusID"] = postOfficeTemp.Rows[0]["CusID"].ToString().Trim();
                                hostRow["ReceiveNumber"] = postOfficeTemp.Rows[0]["ReceiveNumber"].ToString().Trim();
                                hostRow["ApplyType"] = row["ApplyType"].ToString().Trim();
                                hostRow["CusName"] = postOfficeTemp.Rows[0]["CusName"].ToString().Trim();
                                hostRow["AccNoBank"] = postOfficeTemp.Rows[0]["AccNoBank"].ToString().Trim();
                                hostRow["AccNo"] = postOfficeTemp.Rows[0]["AccNo"].ToString().Trim();
                                hostRow["AccID"] = postOfficeTemp.Rows[0]["AccID"].ToString().Trim();
                                hostRow["ApplyCode"] = postOfficeTemp.Rows[0]["ApplyCode"].ToString().Trim();
                                hostRow["AccType"] = postOfficeTemp.Rows[0]["AccType"].ToString().Trim();
                                hostRow["AccDeposit"] = postOfficeTemp.Rows[0]["AccDeposit"].ToString().Trim();
                                hostRow["CusNo"] = postOfficeTemp.Rows[0]["CusNo"].ToString().Trim();
                                hostRow["AgentID"] = postOfficeTemp.Rows[0]["AgentID"].ToString().Trim();
                                hostRow["ModDate"] = postOfficeTemp.Rows[0]["ModDate"].ToString().Trim();
                                hostRow["IsNeedUpload"] = postOfficeTemp.Rows[0]["IsNeedUpload"].ToString().Trim();
                                hostRow["UploadDate"] = postOfficeTemp.Rows[0]["UploadDate"].ToString().Trim();
                                hostRow["Pay_Way"] = postOfficeTemp.Rows[0]["Pay_Way"].ToString().Trim();
                                hostRow["Bcycle_Code"] = postOfficeTemp.Rows[0]["Bcycle_Code"].ToString().Trim();
                                hostRow["Mobile_Phone"] = postOfficeTemp.Rows[0]["Mobile_Phone"].ToString().Trim();
                                hostRow["E_Mail"] = postOfficeTemp.Rows[0]["E_Mail"].ToString().Trim();
                                hostRow["E_Bill"] = postOfficeTemp.Rows[0]["E_Bill"].ToString().Trim();
                                hostRow["ReturnStatusTypeCode"] = row["StatusType"].ToString().Trim();
                                hostRow["ReturnCheckFlagCode"] = row["CheckFlag"].ToString().Trim();
                                hostRow["ReturnDate"] = DateTime.Now.ToString("yyyyMMdd");

                                sendHostData.Rows.Add(hostRow);

                                successCount = successCount + 1;
                            }
                            else
                            {
                                failCount = failCount + 1;
                            }
                        }
                        else
                        {
                            // 回寫回覆檔 Trailer
                            trailerOK = BRFORM_COLUMN.UpDateReplyTrailer(row);
                        }
                    }

                    postActiveCount = InsertPostActive(masterID, activeData);
                }
            }
            else
            {
                postActiveCount = InsertPostActive(masterID, activeData);
            }

            ts.Complete();
        }

        return sendHostData;
    }

    /// <summary>
    /// 取 masterID
    /// </summary>
    /// <param name="replyData"></param>
    /// <returns></returns>
    public DataTable GetMasterID(DataTable replyData)
    {
        DataTable result = new DataTable();
        string fileCreateDate = replyData.Rows[0]["FileCreateDate"].ToString();

        if (fileCreateDate != "")
        {
            result = BRFORM_COLUMN.GetMasterID(fileCreateDate);
        }

        return result;
    }

    /// <summary>
    /// 上送主機
    /// </summary>
    /// <param name="replyData"></param>
    /// <param name="eAgentInfo"></param>
    public string SendToHost(DataTable replyData, EntityAGENT_INFO eAgentInfo)
    {
        string userID = "";
        string receiveNumber = "";
        string cusName = "";
        string accNoBank = "";
        string accNo = "";
        string payWay = "";
        string accID = "";
        string bcycleCode = "";
        string mobilePhone = "";
        string eMail = "";
        string eBill = "";
        bool sendHost = true;
        string msg = "";
        foreach (DataRow row in replyData.Rows)
        {
            userID = row["CusID"].ToString();
            accNo = row["AccNo"].ToString();
            accID = row["AccID"].ToString();
            receiveNumber = row["ReceiveNumber"].ToString();

            if (row["ApplyType"].ToString() == "1")
            {
                cusName = row["CusName"].ToString();
                accNoBank = row["AccNoBank"].ToString();
                payWay = row["Pay_Way"].ToString();
                bcycleCode = row["Bcycle_Code"].ToString();
                mobilePhone = row["Mobile_Phone"].ToString();
                eMail = row["E_Mail"].ToString();
                eBill = row["E_Bill"].ToString();

                sendHost = new PostToHostAdapter(userID, receiveNumber, cusName, accNoBank, accNo, payWay, accID, bcycleCode, mobilePhone, eMail, eBill, eAgentInfo).SendToHost();

                if (!sendHost)
                {
                    msg += "收件編號：" + receiveNumber + " 打主機異常 \n";
                }
            }
            else
            {
                BRFORM_COLUMN.UpdatePostOfficeTemp(receiveNumber, "S", "0000");

                // 郵局發動終止
                if (receiveNumber == "")
                {
                    BRFORM_COLUMN.InsertAutoPayPopul(userID, accNo, accID);
                }
            }

        }

        return msg;
    }

    // 郵局自動終止
    private int InsertPostActive(string masterID, DataTable activeData)
    {
        bool autoPayPopulOK = false;
        int postActiveCount = 0;
        int id = masterID == "" ? 0 : Convert.ToInt32(masterID);
        string cusID = "";
        string accNO = "";
        string accID = "";
        string rowNo = "";

        foreach (DataRow row in activeData.Rows)
        {
            if (row["DataType"].ToString().Trim() == "1")
            {
                cusID = row["CusID"].ToString().Trim();
                accNO = row["AccountNo"].ToString().Trim();
                accID = row["AccID"].ToString().Trim();
                rowNo = row["RowNo"].ToString().Trim();

                // 寫入 PostDetail
                BRFORM_COLUMN.InsertPostDetail(id, cusID, accNO, accID, rowNo, "3");
                BRFORM_COLUMN.InsertPostOfficeTemp(cusID, accNO, accID);
                autoPayPopulOK = BRFORM_COLUMN.InsertAutoPayPopul(cusID, accNO, accID);

                if (autoPayPopulOK)
                {
                    postActiveCount = postActiveCount + 1;
                }
            }
        }

        return postActiveCount;
    }

    /*======================================================================== Initiative =====================================================================================*/

    /// <summary>
    /// 取郵局主動終止日期
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public bool IsCatchDate(string date)
    {
        // 取郵局主動終止日期
        DataTable dt = BRFORM_COLUMN.GetCatchDate();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i][0].ToString() == date)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 判斷檔案是否已存在
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool IsFileExist(string fileName)
    {
        return BRFORM_COLUMN.IsFileExist(fileName);

    }

    /// <summary>
    /// 組終止檔資料
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public DataTable GetActiveFileStream(string filePath)
    {
        DataTable activeData = SetReplyDetailColumn();
        DataRow row;
        StreamReader sr = new StreamReader(filePath, Encoding.Default);

        string line;
        string dataType = "";
        string rowNo = "";
        string accountType = "";
        string accountNo = "";
        string cusID = "";
        string accID = "";

        while ((line = sr.ReadLine()) != null)
        {
            dataType = line.Substring(0, 1);
            rowNo = line.Substring(19, 6);
            accountType = line.Substring(26, 1);
            accountNo = line.Substring(27, 14);
            cusID = line.Substring(41, 20);
            accID = line.Substring(61, 10);

            if (dataType == "1")
            {
                row = activeData.NewRow();

                row["DataType"] = dataType.Trim();
                row["RowNo"] = rowNo.Trim();
                row["AccountType"] = accountType.Trim();
                row["AccountNo"] = accountNo.Trim();
                row["CusID"] = cusID.Trim();
                row["AccID"] = accID.Trim();

                activeData.Rows.Add(row);
            }
        }

        return activeData;
    }

    /// <summary>
    /// 寫入每天晚上批次 TABLE
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="activeData"></param>
    /// <returns></returns>
    public int UpdateActiveRelatedTable(string fileName, DataTable activeData)
    {
        DateTime now = DateTime.Today;
        int masterID = 0;
        string cusID = "";
        string accNO = "";
        string accID = "";
        string rowNo = "";
        int count = 0;

        using (OMTransactionScope ts = new OMTransactionScope())
        {
            masterID = BRFORM_COLUMN.InsertPostMaster(fileName, now, true, "F");

            foreach (DataRow row in activeData.Rows)
            {
                cusID = row["CusID"].ToString();
                accNO = row["AccountNo"].ToString();
                accID = row["AccID"].ToString();
                rowNo = row["RowNo"].ToString();

                // 寫入 PostOffice_Detail
                BRFORM_COLUMN.InsertPostDetail(masterID, cusID, accNO, accID, rowNo, "2");

                // 寫入 Auto_Pay_Popul 等晚上跑批次
                BRFORM_COLUMN.InsertAutoPayPopul(cusID, accNO, accID);

                count++;
            }

            ts.Complete();
        }

        return count;
    }

    private string FakeSendHost(DataRow row)
    {
        string result = "N";

        if (row["ReceiveNumber"].ToString() == "2018102000008" || row["ReceiveNumber"].ToString() == "2018102000018" || row["ReceiveNumber"].ToString() == "2018102000028" ||
            row["ReceiveNumber"].ToString() == "2018102000038" || row["ReceiveNumber"].ToString() == "2018102000048" || row["ReceiveNumber"].ToString() == "2018102000058")
        {
            result = "Y";
        }

        return result;
    }

    /// <summary>
    /// 寫檔
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    /// <param name="count"></param>
    /// <param name="datetime"></param>
    public void CreateFile(string path, string fileName, string content)
    //public void CreateFile(string path, string fileName, DataTable content)
    {
        MKDir(path);
        FileStream stream1 = new FileStream(path + fileName, FileMode.Append, FileAccess.Write);
        StreamWriter writer1 = new StreamWriter(stream1, Encoding.GetEncoding(950));
        writer1.WriteLine(content);
        writer1.Close();
        stream1.Close();
        //using (TextWriter fileTW = new StreamWriter(path + fileName))
        //{
        //    foreach (DataRow row in content.Rows)
        //    {
        //        fileTW.NewLine = "\n";
        //        fileTW.WriteLine(row["data"].ToString());
        //    }
        //    fileTW.Flush();
        //}
    }

    /// <summary>
    /// 發送 Mail
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="mailTitle"></param>
    /// <param name="mailBody"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public bool SendMail(string jobID, string mailTitle, string mailBody, string status, DateTime startTime)
    {
        string emailMembers = new Com_AutoJob().GetEmailMembers("01", jobID);
        emailMembers = emailMembers.Substring(0, emailMembers.Length - 1);

        string[] mailTos = emailMembers.Split(';');

        System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

        nvc["Name"] = emailMembers.Replace(';', ',');

        nvc["Title"] = mailTitle;

        nvc["StartTime"] = startTime.ToString();

        nvc["EndTime"] = DateTime.Now.ToString();

        nvc["Message"] = mailBody.ToString().Trim();

        nvc["Status"] = status;

        try
        {
            MailService.MailSender(mailTos, 1, nvc, "");
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
        return true;
    }

    // 組 PostOffice_Detail 資料
    private DataTable OperatorPostDetail(int masterID, DataTable postData, string dateTime, out int totalCount)
    {
        DataTable postDetail = SetPostDetailColumn();
        DataRow rowDetail = null;
        int rowNo = 0;
        string accountType = "P";
        string accNo = "";

        if (accNo.Length == 8)
        {
            accountType = "G";
        }

        for (int i = 0; i < postData.Rows.Count; i++)
        {
            accountType = "P";
            accNo = postData.Rows[i]["AccNo"].ToString();

            if (accNo.Length == 8)
            {
                accountType = "G";
            }

            rowNo = rowNo + 1;

            rowDetail = postDetail.NewRow();
            rowDetail["MasterID"] = masterID;                                           //      主檔ID
            rowDetail["ReceiveNumber"] = postData.Rows[i]["ReceiveNumber"].ToString();  //      收件編號
            rowDetail["DataType"] = "1";                                                // 1. 1 資料別           (固定值為1)
            rowDetail["AgencyCode"] = "700";                                            // 2. 3 委託機構代號     (大寫英數字)
            rowDetail["Filler1"] = "".PadLeft(4);                                       // 3. 4 保留欄           (空白)
            rowDetail["FileCreateDate"] = dateTime;                                     // 4. 8 媒體產生日期     (西元年月日YYYYMMDD)
            rowDetail["BatchNo"] = "001";                                               // 5. 3 批號             (固定值為001)
            rowDetail["RowNo"] = rowNo.ToString().PadLeft(6, '0');                      // 6. 6 流水號           (每批自000001序編)
            rowDetail["ApplyType"] = postData.Rows[i]["ApplyCode"].ToString();          // 7. 1 申請代號         (1：申請 2：終止；(郵局主動)3：郵局終止 4：誤終止-已回復為申請)
            rowDetail["AccountType"] = accountType;                                     // 8. 1 帳戶別           (P：存簿 G：劃撥)
            rowDetail["AccountNo"] = accNo.PadLeft(14, '0');                            // 9.14 儲金帳號         (存簿：局帳號計14碼 劃撥：000000+8碼帳號)
            rowDetail["CusID"] = postData.Rows[i]["CusID"].ToString().PadLeft(20);      //10.20 用戶編號         (右靠左補空，大寫英數字，不得填寫中文 由委託機構自行編給其客戶之編號)
            rowDetail["AccID"] = postData.Rows[i]["AccID"].ToString();                  //11.10 身分證統一編號   (左靠右補空白)
            rowDetail["StatusType"] = "".PadLeft(2);                                    //12. 2 狀況代號         (初始值為空白，回送資料請參閱媒體資料不符代號一覽表)
            rowDetail["CheckFlag"] = "".PadLeft(1);                                     //13. 1 核對註記         (初始值為空白，回送資料請參閱媒體資料不符代號一覽表)
            rowDetail["Filler2"] = "".PadLeft(26);                                      //14.26 保留欄           (空白)
            rowDetail["Complete"] = false;                                              //      是否完成

            postDetail.Rows.Add(rowDetail);
        }

        totalCount = rowNo;

        return postDetail;
    }

    // 組 PostOffice_Trailer 資料
    private DataTable OperatorPostTrailer(int masterID, string dateTime, int totalCount)
    {
        DataTable postTrailer = SetPostTrailerColumn();
        DataRow rowTrailer = postTrailer.NewRow();

        rowTrailer = postTrailer.NewRow();
        rowTrailer["MasterID"] = masterID;                                   //      主檔ID
        rowTrailer["DataType"] = "2";                                        // 1. 1 資料別           (固定值為2)
        rowTrailer["AgencyCode"] = "700";                                    // 2. 3 委託機構代號     (同明細)
        rowTrailer["Filler1"] = "".PadLeft(4);                               // 3. 4 保留欄           (空白)
        rowTrailer["FileCreateDate"] = dateTime;                             // 4. 8 媒體產生日期     (同明細)
        rowTrailer["BatchNo"] = "001";                                       // 5. 3 批號             (同明細)
        rowTrailer["CreateType"] = "B";                                      // 6. 1 建檔記號         (固定值B：委託機構送件)
        rowTrailer["TotalCount"] = totalCount.ToString().PadLeft(6, '0');    // 7. 6 總筆數           (右靠左補0)
        rowTrailer["PostCreateDate"] = "".PadLeft(8);                        // 8. 8 資料建檔日期     (初始值為空白，回送時使用)
        rowTrailer["FailCount"] = "".PadLeft(6, '0');                        // 9. 6 錯誤筆數         (初始值為0，回送時使用)
        rowTrailer["SuccessCount"] = "".PadLeft(6, '0');                     //10. 6 成功筆數         (初始值為0，回送時使用)
        rowTrailer["Filler2"] = "".PadLeft(54);                              //11.54 身分保留欄證統一編號

        postTrailer.Rows.Add(rowTrailer);

        return postTrailer;
    }

    // 大量 Insert
    private bool BulklyInsertPostDetail(string tableName, DataTable postOfficeDetail)
    {
        bool result = false;

        if (BRFORM_COLUMN.InsertToInvData(tableName, postOfficeDetail))
        {
            result = true;
        }

        return result;
    }

    // 組 PostOffice_Detail 欄位
    private DataTable SetPostDetailColumn()
    {
        DataTable postDetail = new DataTable();

        postDetail.Columns.Add("ID", typeof(System.Int32));                 // IdentityKey
        postDetail.Columns.Add("MasterID", typeof(System.Int32));           // 主檔ID
        postDetail.Columns.Add("ReceiveNumber", typeof(System.String));     // 收件編號
        postDetail.Columns.Add("DataType", typeof(System.String));          // 資料別
        postDetail.Columns.Add("AgencyCode", typeof(System.String));        // 委託機構代號
        postDetail.Columns.Add("Filler1", typeof(System.String));           // 保留欄
        postDetail.Columns.Add("FileCreateDate", typeof(System.String));    // 媒體產生日期
        postDetail.Columns.Add("BatchNo", typeof(System.String));           // 批號
        postDetail.Columns.Add("RowNo", typeof(System.String));             // 流水號
        postDetail.Columns.Add("ApplyType", typeof(System.String));         // 申請代號
        postDetail.Columns.Add("AccountType", typeof(System.String));       // 帳戶別
        postDetail.Columns.Add("AccountNo", typeof(System.String));         // 儲金帳號
        postDetail.Columns.Add("CusID", typeof(System.String));             // 用戶編號
        postDetail.Columns.Add("AccID", typeof(System.String));             // 身分證統一編號
        postDetail.Columns.Add("StatusType", typeof(System.String));        // 狀況代號
        postDetail.Columns.Add("CheckFlag", typeof(System.String));         // 核對註記
        postDetail.Columns.Add("Filler2", typeof(System.String));           // 保留欄
        postDetail.Columns.Add("Complete", typeof(System.Boolean));         // 是否完成

        return postDetail;
    }

    // 組 PostOffice_Trailer 欄位
    private DataTable SetPostTrailerColumn()
    {
        DataTable postTrailer = new DataTable();

        postTrailer.Columns.Add("ID", typeof(System.Int32));                 // IdentityKey
        postTrailer.Columns.Add("MasterID", typeof(System.Int32));           // 主檔ID
        postTrailer.Columns.Add("DataType", typeof(System.String));          // 資料別
        postTrailer.Columns.Add("AgencyCode", typeof(System.String));        // 委託機構代號
        postTrailer.Columns.Add("Filler1", typeof(System.String));           // 保留欄
        postTrailer.Columns.Add("FileCreateDate", typeof(System.String));    // 媒體產生日期
        postTrailer.Columns.Add("BatchNo", typeof(System.String));           // 批號
        postTrailer.Columns.Add("CreateType", typeof(System.String));        // 建檔記號
        postTrailer.Columns.Add("TotalCount", typeof(System.String));        // 總筆數
        postTrailer.Columns.Add("PostCreateDate", typeof(System.String));    // 資料建檔日期
        postTrailer.Columns.Add("FailCount", typeof(System.String));         // 錯誤筆數
        postTrailer.Columns.Add("SuccessCount", typeof(System.String));      // 成功筆數
        postTrailer.Columns.Add("Filler2", typeof(System.String));           // 保留欄

        return postTrailer;
    }

    // 組 PostOffice_Detail 回覆 欄位
    private DataTable SetReplyDetailColumn()
    {
        DataTable replyData = new DataTable();

        replyData.Columns.Add("MasterID", typeof(System.Int32));        // MasterID
        replyData.Columns.Add("DataType", typeof(System.String));       // 資料別
        replyData.Columns.Add("AgencyCode", typeof(System.String));     // 委託機構代號
        replyData.Columns.Add("Filler1", typeof(System.String));        // 保留欄
        replyData.Columns.Add("FileCreateDate", typeof(System.String)); // 媒體產生日期
        replyData.Columns.Add("BatchNo", typeof(System.String));        // 批號
        replyData.Columns.Add("RowNo", typeof(System.String));          // 流水號
        replyData.Columns.Add("ApplyType", typeof(System.String));      // 申請代號
        replyData.Columns.Add("AccountType", typeof(System.String));    // 帳戶別
        replyData.Columns.Add("AccountNo", typeof(System.String));      // 儲金帳號
        replyData.Columns.Add("CusID", typeof(System.String));          // 用戶編號
        replyData.Columns.Add("AccID", typeof(System.String));          // 統一證號
        replyData.Columns.Add("StatusType", typeof(System.String));     // 狀況代號
        replyData.Columns.Add("CheckFlag", typeof(System.String));      // 核對註記
        replyData.Columns.Add("Filler2", typeof(System.String));        // 保留欄
        replyData.Columns.Add("Complete", typeof(System.Boolean));
        replyData.Columns.Add("PostCreateDate", typeof(System.String)); // 資料建檔日期
        replyData.Columns.Add("FailCount", typeof(System.String));      // 錯誤筆數
        replyData.Columns.Add("SuccessCount", typeof(System.String));   // 成功筆數
        replyData.Columns.Add("TrailerFiller2", typeof(System.String)); // 檔尾保留欄

        return replyData;
    }

    // 組 傳送主機 欄位
    private DataTable SetHostDataColumn()
    {
        DataTable hostData = new DataTable();

        hostData.Columns.Add("CusID", typeof(System.String));
        hostData.Columns.Add("ReceiveNumber", typeof(System.String));
        hostData.Columns.Add("ApplyType", typeof(System.String));
        hostData.Columns.Add("CusName", typeof(System.String));
        hostData.Columns.Add("AccNoBank", typeof(System.String));
        hostData.Columns.Add("AccNo", typeof(System.String));
        hostData.Columns.Add("AccID", typeof(System.String));
        hostData.Columns.Add("ApplyCode", typeof(System.String));
        hostData.Columns.Add("AccType", typeof(System.String));
        hostData.Columns.Add("AccDeposit", typeof(System.String));
        hostData.Columns.Add("CusNo", typeof(System.String));
        hostData.Columns.Add("AgentID", typeof(System.String));
        hostData.Columns.Add("ModDate", typeof(System.String));
        hostData.Columns.Add("IsNeedUpload", typeof(System.String));
        hostData.Columns.Add("UploadDate", typeof(System.String));
        hostData.Columns.Add("Pay_Way", typeof(System.String));
        hostData.Columns.Add("Bcycle_Code", typeof(System.String));
        hostData.Columns.Add("Mobile_Phone", typeof(System.String));
        hostData.Columns.Add("E_Mail", typeof(System.String));
        hostData.Columns.Add("E_Bill", typeof(System.String));
        hostData.Columns.Add("ReturnStatusTypeCode", typeof(System.String));
        hostData.Columns.Add("ReturnCheckFlagCode", typeof(System.String));
        hostData.Columns.Add("ReturnDate", typeof(System.String));

        return hostData;
    }
    // 建立路徑
    private bool MKDir(string FolderPath)
    {
        try
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(this.jobID + " 目錄 " + FolderPath + " 建制失敗：" + ex.ToString(), LogState.Error, LogLayer.Util);
            return false;
        }
    }

    //private bool CheckIsMatchData(DataRow dataRow, string rowNo, string accountNo, string cusID)
    //{
    //    bool isMatch = true;

    //    if (dataRow["RowNo"].ToString() != rowNo)
    //    {
    //        isMatch = false;
    //    }

    //    if (dataRow["AccountNo"].ToString() != accountNo)
    //    {
    //        isMatch = false;
    //    }

    //    if (dataRow["CusID"].ToString() != cusID)
    //    {
    //        isMatch = false;
    //    }

    //    return isMatch;
    //}

    // 判斷郵局回傳資訊
    private bool CheckPostResultCode(string statusType, string checkFlag)
    {
        bool result = true;

        if (statusType == "03"      // 已終止代繳
            || statusType == "06"   // 凍結戶或警示戶
            || statusType == "07"   // 業務支票專戶
            || statusType == "08"   // 帳號錯誤
            || statusType == "09"   // 終止戶
            || statusType == "10"   // 身分證號不符
            || statusType == "11"   // 轉出戶
            || statusType == "12"   // 拒絕往來戶
            || statusType == "13"   // 無此用戶編號
            || statusType == "14"   // 用戶編號已存在
            || statusType == "16"   // 管制帳戶
            || statusType == "17"   // 掛失戶
            || statusType == "18"   // 異常交易帳戶
            || statusType == "19"   // 用戶編號非英數字元
            || statusType == "91"   // 規定期限內未有扣款
            || statusType == "98")  // 其他
        {
            result = false;
        }

        if (checkFlag == "1"        // 局帳號不符
            || checkFlag == "2"     // 戶名不符
            || checkFlag == "3"     // 身分證號不符
            || checkFlag == "4"     // 印鑑不符
            || checkFlag == "9")    // 其他
        {
            result = false;
        }

        return result;
    }

    // 檔案加密
    private bool ZipFile(string jobID, string path, string fileName, string zipPwd)
    {
        JobHelper JobHelper = new JobHelper();
        JobHelper.strJobID = jobID;
        bool result = true;
        string strFile = path + fileName;
        string strZipFile = path + fileName.Split('.')[0] + ".ZIP";
        string strZipName = "";

        string[] arrFileList = new string[1];
        arrFileList[0] = strFile;
        int intResult = JobHelper.Zip(strZipFile, arrFileList, strZipName, zipPwd, CompressToZip.CompressLevel.Level6);

        if (intResult > 0)
        {
            JobHelper.Write(jobID, "檔案: " + fileName + " 加密成功！", LogState.Info);
        }
        //*壓縮失敗
        else
        {
            JobHelper.Write(jobID, "檔案: " + fileName + " 加密失敗！");
            result = false;
        }
        return result;
    }

    // 刪除檔案
    private void DeleteFile(string path)
    {
        Logging.Log(this.jobID + " 刪除 Path：" + path, LogLayer.Util);
        DirectoryInfo di = new DirectoryInfo(path);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
    }
}
