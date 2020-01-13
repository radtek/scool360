﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WinBase;
using System.Data.Odbc;
using System.Data;
using WebChart;
using System.Drawing;
using System.Collections;

namespace Scool360student
{
    public partial class StudentAttendReportNew : System.Web.UI.Page
    {
        private StudentManagerClass MyStudMang;
        private Attendance MyAttendence;
        private KnowinUser MyUser;
        string Sql;
        private OdbcDataReader MyReader = null;
        private DataSet holidaydataset;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UserObj"] == null)
            {
                Response.Redirect("sectionerr.htm");
            }
            if (Session["StudId"] == null)
            {
                Response.Redirect("SearchStudent.aspx");
            }
            MyUser = (KnowinUser)Session["UserObj"];
            MyStudMang = MyUser.GetStudentObj();
            MyAttendence = MyUser.GetAttendancetObj();
            if (MyAttendence == null)
            {
                Response.Redirect("RoleErr.htm");
                //no rights for this user.
            }
            if (!MyUser.HaveActionRignt(82))
            {
                Response.Redirect("RoleErr.htm");
            }
            else
            {

                if (!IsPostBack)
                {

                   // string _MenuStr;
                 //   _MenuStr = MyStudMang.GetSubStudentMangMenuString(MyUser.UserRoleId, int.Parse(Session["StudType"].ToString())); 
                  //  this.SubStudentMenu.InnerHtml = _MenuStr;
                    Calendar1.VisibleDate = DateTime.Now.Date;
                  //  LoadStudentTopData();
                    LoadYearlyAtendence();
                    LoadDrpMonth();
                    LoadMonthlyAttendanceData();
                    CreateMonthlyAttendancePersentageData();
                    LoadAttendancesCalender();

                    Drp_Select_Month.Enabled = false;
                    Btn_Show.Enabled = false;
                    Btn_Excel.Enabled = false;
                    LoadDateInTextBox();
                    Lbl_batch.Text = MyUser.CurrentBatchName;

                    Btn_ShowRFDetails.Visible = false;
                    if (RFdetailsFound())
                    {
                        Btn_ShowRFDetails.Visible = true;
                    }
                }

                DrawYearlyPieChart();
                DrawMonthlyPieChart();
                DrawMonthlyPersentageChart();
                ChartControl.PerformCleanUp();
            }
        }
        private bool RFdetailsFound()
        {
            bool _valid = false;
            string sql = "SELECT COUNT(tblexternalattencence.ExternalReffid) FROM tblexternalattencence";
            MyReader = MyStudMang.m_MysqlDb.ExecuteQuery(sql);
            if(MyReader.HasRows)
            {
                int _count = 0;
                int.TryParse(MyReader.GetValue(0).ToString(),out _count);
                if (_count > 0)
                {
                    _valid = true;
                }
            }
            return _valid;
        }

        private void LoadAttendancesCalender()
        {
            int Classid = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
            holidaydataset = MyAttendence.MyAssociatedHolidays("class", Classid, MyUser.User_Id);
        }



  


        private void CreateMonthlyAttendancePersentageData()
        {

            if (Drp_Select_Month.SelectedValue != "-1")
            {
                double _average;
                int _classid = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
                int _standard= MyStudMang.GetStandard(MyUser.StudId.ToString());
                if (_classid != -1)
                {
                    DataTable dt;
                    dt = new DataTable();
                    //dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("Month", typeof(string));
                    dt.Columns.Add("Percentage", typeof(int));
                    dt.Columns.Add("Workingdays", typeof(int));
                    dt.Columns.Add("Fulldays", typeof(int));
                    dt.Columns.Add("Halfdays", typeof(int));
                    dt.Columns.Add("Absentdays", typeof(int));
                    dt.Columns.Add("Holidays", typeof(int));

                    DataRow dr;
                    int _no_workingdays, _no_presentdays, _no_absentdays,_no_halfdays,_no_Holidays;
                    for (int i = 0; i < Drp_Select_Month.Items.Count; i++)
                    {

                        _average = MyAttendence.GetStudentMonthly_NewAttendanceDatas(int.Parse(MyUser.StudId.ToString()), _classid, _standard, MyUser.CurrentBatchId, int.Parse(Drp_Select_Month.Items[i].Value), out _no_workingdays, out _no_presentdays, out _no_absentdays, out _no_halfdays, out _no_Holidays);


                        dr = dt.NewRow();
                        //dr["Id"] = int.Parse(Drp_month.Items[i].Value);
                        dr["Month"] = Drp_Select_Month.Items[i].Text;
                        dr["Percentage"] = _average;
                        dr["Workingdays"] = _no_workingdays;
                        dr["Fulldays"] = _no_presentdays;
                        dr["Halfdays"] = _no_halfdays;
                        dr["Absentdays"] = _no_absentdays;
                        dr["Holidays"] = _no_Holidays;
                        dt.Rows.Add(dr);

                    }
                    ViewState["MonthlyPersentData"] = dt;
                }
            }
        }

        private void DrawMonthlyPersentageChart()
        {
            if (ViewState["MonthlyPersentData"] == null)
            {
                Pnl_yearlyBar.Visible = false;
                Img_excel.Visible = false;
            }
            else
            {
                DataTable dt = (DataTable)ViewState["MonthlyPersentData"];
                if (dt.Rows.Count > 0)
                {
                    ColumnChart chart = new ColumnChart();
                    foreach (DataRow dr_MonyhlyData in dt.Rows)
                    {

                        chart.Data.Add(new ChartPoint(dr_MonyhlyData[0].ToString(), int.Parse(dr_MonyhlyData[1].ToString())));


                    }
                    chart.Shadow.Visible = true;
                    chart.DataLabels.Visible = true;
                    chart.MaxColumnWidth = 20;
                    chartcontrol_Monthlypersent.Charts.Add(chart);
                    chart.Fill.Color = System.Drawing.Color.RoyalBlue;
                    chartcontrol_Monthlypersent.Background.Color = Color.White;
                    chartcontrol_Monthlypersent.RedrawChart();
                }
            }
        }



        private void DrawMonthlyPieChart()
        {
            PieChart MonthPiechart = (PieChart)chartcontrol_montnly.Charts.FindChart("Monthly_Chart");
            MonthPiechart.Colors = new Color[] { Color.LightGreen, Color.Cyan, Color.OrangeRed, Color.Yellow, Color.AntiqueWhite, Color.RosyBrown };

            int _presentdays, _absentdays, _holidays,_halfdays;
            _presentdays = int.Parse(Lbl_presentdays.Text);
            _absentdays = int.Parse(lbl_absentdays.Text);
            _holidays = int.Parse(Lbl_holiday.Text);
            _halfdays = int.Parse(Lbl_halfdays.Text);
            MonthPiechart.Data.Clear();
            MonthPiechart.Data.Add(new ChartPoint("Full Days", _presentdays));
            MonthPiechart.Data.Add(new ChartPoint("Half Days", _halfdays));
            MonthPiechart.Data.Add(new ChartPoint("Absent Days", _absentdays));
            MonthPiechart.Data.Add(new ChartPoint("Holidays", _holidays));
            MonthPiechart.DataLabels.Visible = false;
            if ((_presentdays + _absentdays + _holidays + _halfdays) == 0)
            {
                MonthPiechart.Data.Add(new ChartPoint("NO DATA", 100));

                MonthPiechart.DataLabels.Visible = false;
            }
            chartcontrol_montnly.Background.Color = Color.White;
            chartcontrol_montnly.RedrawChart();

        }

        private void LoadMonthlyAttendanceData()
        {
            int _no_workingdays, _no_presentdays, _no_absentdays, _no_holidays, _month, _no_halfdays;
            double _attendencepersent;
            int _standard = MyStudMang.GetStandard(MyUser.StudId.ToString());
            int _classid = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
            _month = Calendar1.VisibleDate.Month;
            _attendencepersent = MyAttendence.GetStudentMonthly_NewAttendanceDatas(int.Parse(MyUser.StudId.ToString()), _classid, _standard, MyUser.CurrentBatchId, _month, out _no_workingdays, out _no_presentdays, out _no_absentdays, out _no_halfdays, out _no_holidays);
            Lbl_TotalWorkingDay.Text = _no_workingdays.ToString();
            Lbl_presentdays.Text = _no_presentdays.ToString();
            Lbl_halfdays.Text = _no_halfdays.ToString();
            lbl_absentdays.Text = _no_absentdays.ToString();
            Lbl_holiday.Text = _no_holidays.ToString();
            Lbl_attendancepersent.Text = _attendencepersent.ToString("#0.00") + "%";
        }

        private void DrawYearlyPieChart()
        {
            PieChart YearlyPiechart = (PieChart)chartcontrol_yearly.Charts.FindChart("yearly_Chart");
            YearlyPiechart.Colors = new Color[] { Color.LightGreen, Color.Cyan, Color.OrangeRed, Color.Yellow, Color.AntiqueWhite, Color.RosyBrown };

            int _presentdays, _absentdays, _holidays,_halfdays;
            _presentdays = int.Parse(Lbl_no_presentdays.Text);
            _absentdays = int.Parse(Lbl_no_absent_day.Text);
            _holidays = int.Parse(Lbl_no_holiday.Text);
            _halfdays = int.Parse(Lbl_no_halfdays.Text);
            YearlyPiechart.Data.Clear();
            YearlyPiechart.Data.Add(new ChartPoint("Full Days", _presentdays));
            YearlyPiechart.Data.Add(new ChartPoint("Half Days", _halfdays));
            YearlyPiechart.Data.Add(new ChartPoint("Absent Days", _absentdays));
            YearlyPiechart.Data.Add(new ChartPoint("Holidays", _holidays));
            YearlyPiechart.DataLabels.Visible = false;
            if ((_presentdays + _absentdays + _holidays + _halfdays) == 0)
            {
                YearlyPiechart.Data.Add(new ChartPoint("NO DATA", 100));

                YearlyPiechart.DataLabels.Visible = false;
            }
            chartcontrol_yearly.Background.Color = Color.White;
            chartcontrol_yearly.RedrawChart();


        }

        private void LoadYearlyAtendence()
        {
            int _no_workingdays, _no_presentdays, _no_absentdays, _no_holidays,_no_halfdays;
            double _attendencepersent;
            MyAttendence.GetCurrentBatchNewattendanceDetails(int.Parse(MyUser.StudId.ToString()), out _no_workingdays, out _no_presentdays, out _no_absentdays, out _no_holidays, out _no_halfdays, out _attendencepersent, MyUser.CurrentBatchId);
            Lbl_no_workingdays.Text = _no_workingdays.ToString();
            Lbl_no_presentdays.Text = _no_presentdays.ToString();
            Lbl_no_absent_day.Text = _no_absentdays.ToString();
            Lbl_no_holiday.Text = _no_holidays.ToString();
            Lbl_no_halfdays.Text = _no_halfdays.ToString();
            Lbl_total_persent.Text = _attendencepersent.ToString("#0.00") + "%";
        }



        //private void LoadStudentTopData()
        //{

        //    string _Studstrip = MyStudMang.ToStripString(int.Parse(MyUser.StudId.ToString()), "Handler/ImageReturnHandler.ashx?id=" + int.Parse(MyUser.StudId.ToString()) + "&type=StudentImage", int.Parse(Session["StudType"].ToString()));
        //    this.StudentTopStrip.InnerHtml = _Studstrip;
        //}


        private void LoadDrpMonth()
        {

      
            Drp_Select_Month.Items.Clear();
            int StandardId = MyStudMang.GetStandard(MyUser.StudId.ToString());
            if (MyAttendence.AttendanceTables_Exits(StandardId.ToString(), MyUser.CurrentBatchId))
            {
                Sql = "select DISTINCT tblmonth.Id , tblmonth.`Month` from tblmonth INNER JOIN tblattdcls_std" + StandardId + "yr" + MyUser.CurrentBatchId + " t1 ON MONTH (t1.`Date`)=tblmonth.Id order by year(t1.`Date`),MONTH (t1.`Date`)";
                MyReader = MyAttendence.m_MysqlDb.ExecuteQuery(Sql);
                if (MyReader.HasRows)
                {
                    while (MyReader.Read())
                    {
                        ListItem li = new ListItem(MyReader.GetValue(1).ToString(), MyReader.GetValue(0).ToString());
                        Drp_Select_Month.Items.Add(li);
                     

                    }
             
                    Drp_Select_Month.SelectedValue = DateTime.Now.Month.ToString();
                }
                else
                {
                    ListItem li = new ListItem("No Month Found", "-1");
               
                    Drp_Select_Month.Items.Add(li);

                }
            }
            else
            {
                ListItem li = new ListItem("No Month Found", "-1");
               
                Drp_Select_Month.Items.Add(li);

            }
           

        }




        private void LoadMonthlyDatas()
        {
            //string _Msg;
            AttendanceDetailsDiv.InnerHtml = "";
            Btn_Excel.Enabled = false;
            //LoadDateFromSlectedDate(out _Msg);
            this.Tabs.ActiveTabIndex = 1;
            LoadMonthlyAttendanceData();
            DrawMonthlyPieChart();
        }



        protected void Img_excel_Click(object sender, ImageClickEventArgs e)
        {

            if (ViewState["MonthlyPersentData"] != null)
            {
                DataTable dt = (DataTable)ViewState["MonthlyPersentData"];
                if (dt.Rows.Count > 0)
                {
                    DataSet _Attendencedataset = new DataSet();
                    _Attendencedataset.Tables.Add(dt);
                    //if (!WinEr.ExcelUtility.ExportDataSetToExcel(_Attendencedataset, "SutdentAttendence.xls"))
                    //{
                    //    Lbl_msg.Text = "This function needs Ms office";
                    //    this.MPE_MessageBox.Show();
                    //}
                    string FileName = "SutdentAttendence";
                    string _ReportName = "SutdentAttendence";
                    if (!WinEr.ExcelUtility.ExportDataToExcel(_Attendencedataset, _ReportName, FileName, MyUser.ExcelHeader))
                    {
                        Lbl_msg.Text = "This function needs Ms office";
                        this.MPE_MessageBox.Show();
                    }
                }
            }
        }
        protected void Drp_Period_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Tabs.ActiveTabIndex = 2;
            Btn_Excel.Enabled = false;
            LoadDateInTextBox();
        }

        private void LoadDateInTextBox()
        {
            string _sdate = null, _edate = null, _msg = "";

            if (Drp_Select_Month.SelectedItem.ToString() != "NO DATA")
            {

                if (Drp_Period.SelectedItem.Text.ToString() == "Today")
                {
                    Drp_Select_Month.Enabled = false;
                    Btn_Show.Enabled = true;
                    DateTime _date = System.DateTime.Today;
                    //_sdate = _date.ToString("dd/MM/yyyy");
                    _sdate = MyUser.GerFormatedDatVal(_date);

                    Txt_StartDate.Enabled = false;
                    Txt_EndDate.Enabled = false;
                    Txt_StartDate.Text = _sdate;
                    Txt_EndDate.Text = _sdate;
                }
                if (Drp_Period.SelectedItem.Text.ToString() == "Last Week")
                {
                    Drp_Select_Month.Enabled = false;
                    Btn_Show.Enabled = true;
                    DateTime _date = System.DateTime.Now;
                    //_edate = _date.ToString("dd/MM/yyyy");
                    _edate = MyUser.GerFormatedDatVal(_date);

                    DateTime _start = _date.AddDays(-7);
                    //_sdate = _start.Date.ToString("dd/MM/yyyy");
                    _sdate = MyUser.GerFormatedDatVal(_start);

                    Txt_StartDate.Enabled = false;
                    Txt_EndDate.Enabled = false;
                    Txt_StartDate.Text = _sdate;
                    Txt_EndDate.Text = _edate;
                }

                if (Drp_Period.SelectedItem.Text.ToString() == "Month Wise")
                {
                    Drp_Select_Month.Enabled = true;
                    LoadDateFromSlectedDate(out _msg);


                }
                if (Drp_Period.SelectedItem.Text.ToString() == "Manual")
                {
                    Btn_Show.Enabled = true;
                    Drp_Select_Month.Enabled = false;
                    Txt_StartDate.Enabled = true;
                    Txt_EndDate.Enabled = true;
                    Txt_StartDate.Text = "";
                    Txt_EndDate.Text = "";

                }
            }
            else
            {
                Btn_Show.Enabled = false;
                Txt_StartDate.Text = "";
                Txt_EndDate.Text = "";
                Txt_StartDate.Enabled = false;
                Txt_EndDate.Enabled = false;
                _msg = "No attendence reported";
            }

            Lbl_Err.Text = _msg;
        }

        private void LoadDateFromSlectedDate(out string _Msg)
        {

            _Msg = "";

            int selected_month = int.Parse(Drp_Select_Month.SelectedValue.ToString());

            if (selected_month != -1)
            {
                string _sdate = null, _edate = null;
                DateTime _date, _start;
                Btn_Show.Enabled = true;


                if (MyAttendence.CheckSelectedMonthIsCurrentMonthorNot(selected_month))
                {
                    _date = System.DateTime.Now;
                    _edate = MyUser.GerFormatedDatVal(_date);
                    _start = System.DateTime.Now;  
                    _sdate = MyUser.GerFormatedDatVal(MyUser.GetDareFromText("01/" + _start.Date.Month + "/" + _start.Date.Year)).ToString();
                }
                else
                {

                    _start = MyAttendence.GetFirstedDayFromMonth(selected_month);
                    int year = DateTime.Now.Date.Year;
                    if (selected_month > DateTime.Now.Date.Month)
                    {
                        year--;
                    }                                         
                        _sdate = MyUser.GerFormatedDatVal(MyUser.GetDareFromText("01/" + _start.Date.Month + "/" + year)).ToString();
                        _date = MyAttendence.GetLastDateFromMonth(selected_month);   
                        _edate = MyUser.GerFormatedDatVal(MyUser.GetDareFromText(_date.Date.Day + "/" + _date.Date.Month + "/" + year)).ToString();
                   
                   
                }


                Txt_StartDate.Enabled = false;
                Txt_EndDate.Enabled = false;
                Txt_StartDate.Text = _sdate;
                Txt_EndDate.Text = _edate;
            }
            else
            {

                Btn_Show.Enabled = false;

                Txt_StartDate.Text = "";
                Txt_EndDate.Text = "";

                if (Drp_Select_Month.SelectedItem.ToString() != "NO DATA")
                {
                    _Msg = "Select a month ";
                }
                else
                {
                    _Msg = "No attendance reported";
                }
            }
        }

        private int findYearofSelectedMonth(int selected_month)
        {
            int _clsId = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
            int year = 0;
            string sql = "select DISTINCT year(tblmasterdate.`date`) as year from  tblmasterdate inner join tbldate on tbldate.DateId= tblmasterdate.Id where tbldate.Id in(  select  tbldate.Id  from tbldate inner join tblmasterdate on tblmasterdate.Id= tbldate.DateId  WHERE tbldate.`Status`='class' AND tbldate.classId= " + _clsId + " AND MONTH( tblmasterdate.`date`) =" + selected_month + " )";
            MyReader = MyAttendence.m_MysqlDb.ExecuteQuery(sql);
            if (MyReader.HasRows)
            {
                year = int.Parse(MyReader.GetValue(0).ToString());
            }
            return year;
        }

        protected void Btn_Show_Click(object sender, EventArgs e)
        {
            Lbl_Err.Text ="";
            string _Errmsg = "";
            Btn_Excel.Enabled = true;
            AttendanceDetailsDiv.InnerHtml = "";
            int standard = MyStudMang.GetStandard(MyUser.StudId.ToString());

            if (Txt_EndDate.Text != "" && Txt_StartDate.Text != "")
            {

                DataSet _AttendenceReport = new DataSet();

                bool _valueExist;

                int _clsId = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
                int selected_month = int.Parse(Drp_Select_Month.SelectedValue.ToString());

         

                string _startDate = Txt_StartDate.Text.ToString();
                string _EndDate = Txt_EndDate.Text.ToString();

                //below fun is used to check the manual entry date is exist in current month
                if (CheckDatesInExistSelectedMonth(selected_month, Drp_Period.SelectedItem.ToString(), out _Errmsg))
                {
                    string[] Dates = new string[40];
                    string[] AttData = new string[40];

                    // second parameter is used to check the module is sudent manager or class manager
                    _valueExist = MyAttendence.StudentAttendanceReport(standard, int.Parse(MyUser.StudId.ToString()), _clsId, MyUser.CurrentBatchId, _startDate, _EndDate, out _Errmsg, out Dates, out AttData);
                    if (_valueExist)
                    {
                        FillAttendenceGrid(Dates, AttData, out _Errmsg);
                    }
                    else
                    {
                        Lbl_Err.Text = _Errmsg;
                    }


                }
                else
                {
                    Lbl_Err.Text = _Errmsg;
                }
            }
            else
            {
                if (Drp_Period.SelectedItem.Text == "Manual")
                {
                    Lbl_Err.Text = "Enter Date";
                }
                else
                {
                    Lbl_Err.Text = "Select a period ";
                }
            }
        }

        private void FillAttendenceGrid(string[] Dates, string[] AttData, out string _Errmsg)
        {
            _Errmsg="";
            string tstr = "<table width=\"100%\" cellspacing=\"0\"> <tr>  <td align=\"left\" class=\"GridLeftHead\">  Days  </td> <td align=\"left\" class=\"GridRightHead\"> Attendance Status  </td>  </tr> ";
            string tds = "";

            for (int i = 0; i < Dates.Length; i++)
            {
                if (Dates[i] != null)
                {
                    tds = tds + "<tr>  <td align=\"left\" class=\"GridLeft\"> " + Dates[i] + " </td>  <td align=\"left\" class=\"GridRight\">" + AttData[i] + "</td> </tr>";

                }
                else
                {
                    break;
                }
            }
            tstr=tstr+tds+"</table>";
           this.AttendanceDetailsDiv.InnerHtml=tstr;
        }





        private bool CheckDatesInExistSelectedMonth(int selected_month, string _Type, out string _msg)
        {

            bool _exist = false;
            _msg = "";
            if (selected_month != -1)
            {
                DateTime _enddate, _startDate, _s_Text_Date, _e_Text_Date;

                _startDate = MyAttendence.GetFirstedDayFromMonth(selected_month);
                _enddate = MyAttendence.GetLastDateFromMonth(selected_month);

                //_s_Text_Date = DateTime.ParseExact(Txt_StartDate.Text, "dd/MM/yyyy", null);

                //_e_Text_Date = DateTime.ParseExact(Txt_EndDate.Text, "dd/MM/yyyy", null);

                _s_Text_Date = MyUser.GetDareFromText(Txt_StartDate.Text);

                _e_Text_Date = MyUser.GetDareFromText(Txt_EndDate.Text);


                TimeSpan _totalday = _e_Text_Date.Subtract(_s_Text_Date);

                if (_Type == "Manual")
                {
                    if (_s_Text_Date <= _e_Text_Date)
                    {
                        if (_totalday.Days <= 31)
                        {

                            _exist = true;
                        }
                        else
                        {
                            _msg = "Total days is greater than 31 days  ";
                            _exist = false;
                        }
                    }
                    else
                    {
                        _msg = "Start date is greater than end date. ";
                        _exist = false;

                    }
                }
                else
                {
                    _exist = true;
                }
            }

            return _exist;

        }

        protected void Btn_Excel_Click(object sender, EventArgs e)
        {

            if (this.AttendanceDetailsDiv.InnerHtml == "")
            {
                Lbl_Err.Text = "No Data Found For Exporting To Excel";
            }
            else
            {
                string tstr = "<table width=\"100%\" cellspacing=\"0\"> <tr>  <td colspan=\"2\" align=\"center\" class=\"GridLeftHead\"  style=\"font-weight:bold;\">  " + MyUser.getStudName(int.Parse(MyUser.StudId.ToString())) + "  </td> </tr> </table>";
                              
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=AttendanceReport.xls");
                Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
                Response.Write("<head>");
                Response.Write("<META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                Response.Write("<!--[if gte mso 9]><xml>");
                Response.Write("<x:ExcelWorkbook>");
                Response.Write("<x:ExcelWorksheets>");
                Response.Write("<x:ExcelWorksheet>");
                Response.Write("<x:Name>Attendance Report</x:Name>");
                Response.Write("<x:WorksheetOptions>");
                Response.Write("<x:Print>");
                Response.Write("<x:ValidPrinterInfo/>");
                Response.Write("</x:Print>");
                Response.Write("</x:WorksheetOptions>");
                Response.Write("</x:ExcelWorksheet>");
                Response.Write("</x:ExcelWorksheets>");
                Response.Write("</x:ExcelWorkbook>");
                Response.Write("</xml>");
                Response.Write("<![endif]--> ");

                Response.Write(tstr+ this.AttendanceDetailsDiv.InnerHtml);
                Response.Write("</head>");
                Response.Flush();
                Response.End();
            }
        }

        protected void Drp_Select_Month_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _Msg;
            Btn_Excel.Enabled = false;
            LoadDateFromSlectedDate(out _Msg);
            this.Tabs.ActiveTabIndex = 2;
        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (IsDateInBath(e.Day.Date))
            {


                bool IsHoliday = false;
                DateTime nextholidayDate;
                if (holidaydataset != null && holidaydataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in holidaydataset.Tables[0].Rows)
                    {
                        nextholidayDate = (DateTime)dr[0];
                        if (MyAttendence.IsDefaultHoliday(e.Day.Date.DayOfWeek))
                        {
                            //e.Cell.BackColor = HexStringToColor("#ffcc00");//Holiday
                            e.Cell.BackColor =Color.Yellow;
                            AddTextToDayCell(e, "Holiday");
                            IsHoliday = true;
                            break;
                        }
                        else
                        {
                            if (nextholidayDate == e.Day.Date)
                            {

                                //e.Cell.BackColor = HexStringToColor("#ffcc00");//Holiday
                                e.Cell.BackColor = Color.Yellow;
                                AddTextToDayCell(e, "Holiday");
                                IsHoliday = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (MyAttendence.IsDefaultHoliday(e.Day.Date.DayOfWeek))
                    {
                       // e.Cell.BackColor = HexStringToColor("#ffcc00"); //Holiday
                        e.Cell.BackColor = Color.Yellow;
                        AddTextToDayCell(e, "Holiday");
                        IsHoliday = true;
                    }
                }

                if (!IsHoliday)
                {
                    int AttendanceStatus = 0;
                     int Classid = MyStudMang.GetClassId(int.Parse(MyUser.StudId.ToString()), MyUser.CurrentBatchId);
                     int standardId = MyStudMang.GetStandard(MyUser.StudId.ToString());
                     if (MyAttendence.AttendanceTables_Exits(standardId.ToString(), MyUser.CurrentBatchId))
                     {
                         if (MyAttendence.IsNew_attendanceReportedOnSelectedDate(int.Parse(MyUser.StudId.ToString()), standardId, MyUser.CurrentBatchId, e.Day.Date, out AttendanceStatus))
                         {
                             if (AttendanceStatus == 3)
                             {
                                 AddTextToDayCell(e, AttendanceStatus.ToString());
                                 //e.Cell.BackColor = HexStringToColor("#a4d805"); // Full day
                                 e.Cell.BackColor = Color.LightGreen;
                             }
                             else if (AttendanceStatus == 2 || AttendanceStatus == 1)
                             {
                                 // e.Cell.BackColor = HexStringToColor("#66ccff"); //Half day
                                 e.Cell.BackColor = Color.Cyan;
                                 AddTextToDayCell(e, AttendanceStatus.ToString());
                             }
                             else
                             {
                                 // e.Cell.BackColor = HexStringToColor("#ff5137");  //Absent
                                 e.Cell.BackColor = Color.OrangeRed;
                                 AddTextToDayCell(e, AttendanceStatus.ToString());
                             }
                         }
                         else
                         {

                             AddTextToDayCell(e, "Other");
                         }
                     }
                     else
                     {
                         AddTextToDayCell(e, "Other");
                     }


                }
            }
            else
            {
                e.Cell.BackColor = HexStringToColor("#ffc1c1"); // Not batch
                AddTextToDayCell(e, "NotBatch");
            }
            
        }


        private bool IsDateInBath(DateTime SELECTEDDATE)
        {
            int M_Id = 0;
            bool valid = false;
            string sql = "SELECT tblbatch.Id FROM tblbatch WHERE tblbatch.Id=" + MyUser.CurrentBatchId + " AND '" + SELECTEDDATE.Date.ToString("s") + "' BETWEEN tblbatch.StartDate AND tblbatch.EndDate";
            MyReader = MyAttendence.m_MysqlDb.ExecuteQuery(sql);
            if (MyReader.HasRows)
            {
                int.TryParse(MyReader.GetValue(0).ToString(), out M_Id);
                if (M_Id > 0)
                {
                    valid = true;
                }
            }
            return valid;
        }

        private System.Drawing.Color HexStringToColor(string hex)
        {
            hex = hex.Replace("#", "");

            if (hex.Length != 6)
                throw new Exception(hex +
                    " is not a valid 6-place hexadecimal color code.");

            string r, g, b;

            r = hex.Substring(0, 2);
            g = hex.Substring(2, 2);
            b = hex.Substring(4, 2);

            return System.Drawing.Color.FromArgb(HexStringToBase10Int(r), HexStringToBase10Int(g),
                                                HexStringToBase10Int(b));
        }

        private int HexStringToBase10Int(string hex)
        {
            int base10value = 0;

            try { base10value = System.Convert.ToInt32(hex, 16); }
            catch { base10value = 0; }

            return base10value;

        }


        void AddTextToDayCell(DayRenderEventArgs e, string Type)
        {

            string TextColor = "#000000";
            if (e.Day.IsOtherMonth)
            {
                TextColor = "#999999";
            }
            if (Type == "Holiday")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Selected day is holiday')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (Type == "NotBatch")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Selected day is not within batch')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (e.Day.Date > DateTime.Now.Date)
            {

                e.Cell.Text = "<a href=\"javascript:alert('You have selected a future day')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (Type == "3")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Present for full day')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (Type == "2")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Present on afternoon')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (Type == "1")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Present on forenoon')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else if (Type == "0")
            {
                e.Cell.Text = "<a href=\"javascript:alert('Absent')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
            else
            {

                e.Cell.Text = "<a href=\"javascript:alert('Attendance Not marked')\" style=\"color:" + TextColor + "\">" + e.Day.DayNumberText;
            }
        }


        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            Calendar1.SelectedDate = new DateTime().Date;
            LoadAttendancesCalender();
        }

        protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            LoadAttendancesCalender();
            LoadMonthlyDatas();
        }

        //protected void LinkBtn_Yearly_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("StudentAttendHistory.aspx");
        //}

        protected void Btn_ShowRFDetails_Click(object sender, EventArgs e)
        {
            Lbl_Err.Text = "";
            string _Errmsg = "";
            Btn_Excel.Enabled = true;
            AttendanceDetailsDiv.InnerHtml = "";

            if (Txt_EndDate.Text != "" && Txt_StartDate.Text != "")
            {

                DataSet _AttendenceReport = new DataSet();

                int selected_month = int.Parse(Drp_Select_Month.SelectedValue.ToString());
                string _startDate = Txt_StartDate.Text.ToString();
                string _EndDate = Txt_EndDate.Text.ToString();

                //below fun is used to check the manual entry date is exist in current month
                if (CheckDatesInExistSelectedMonth(selected_month, Drp_Period.SelectedItem.ToString(), out _Errmsg))
                {
                    string[] Dates = new string[40];
                    string[] AttData = new string[40];

                        

                    if(!FillRFAttendenceGrid(int.Parse(MyUser.StudId.ToString()), _startDate, _EndDate, out _Errmsg))
                    {
                        Lbl_Err.Text = _Errmsg;
                    }


                }
                else
                {
                    Lbl_Err.Text = _Errmsg;
                }
            }
            else
            {
                if (Drp_Period.SelectedItem.Text == "Manual")
                {
                    Lbl_Err.Text = "Enter Date";
                }
                else
                {
                    Lbl_Err.Text = "Select a period ";
                }
            }
        }

        private bool FillRFAttendenceGrid(int StudentId, string _startDate, string _endDate, out string _Errmsg)
        {
            bool _valid = false;
            _Errmsg = "";
          
            DateTime _StartDate = General.GetDateTimeFromText(_startDate);
            DateTime _EndDate = General.GetDateTimeFromText(_endDate);
            string str = "";
            string _tr = "";
            string sql = "SELECT tblexternalrfid.Location,tblexternalattencence.ActionDate,tblexternalattencence.RFReaderType FROM tblexternalattencence INNER JOIN tblexternalreff ON tblexternalreff.Id=tblexternalattencence.ExternalReffid INNER JOIN tblexternalrfid ON tblexternalrfid.EquipmentId=tblexternalattencence.RFReaderID WHERE tblexternalreff.UserType='STUDENT' AND tblexternalreff.UserId=" + StudentId + " AND tblexternalattencence.ActionDate BETWEEN  '" + _StartDate.Date.ToString("s") + "' AND '" + _EndDate.Date.ToString("s") + "' ";
            MyReader = MyStudMang.m_MysqlDb.ExecuteQuery(sql);
            if (MyReader.HasRows)
            {
                while (MyReader.Read())
                {
                    string _location = MyReader.GetValue(0).ToString();
                    DateTime _ActionDate = DateTime.Parse(MyReader.GetValue(1).ToString());
                    string _ReaderType = MyReader.GetValue(2).ToString();
                    _tr = _tr + "  <tr>   <td align=\"left\" class=\"GridLeft\"> " + General.GerFormatedDatVal(_ActionDate.Date) + " </td>    <td align=\"left\" class=\"GridRight\">" + _location + "</td>   <td align=\"left\" class=\"GridRight\">" + _ReaderType + "</td> <td align=\"left\" class=\"GridRight\">" + _ActionDate.TimeOfDay + "</td> </tr> ";
                }
                str = "<table width=\"100%\" cellspacing=\"0\">  <tr>  <td align=\"left\" class=\"GridLeftHead\">  Date  </td> <td align=\"left\" class=\"GridRightHead\"> RF-Reader  </td> <td align=\"left\" class=\"GridRightHead\"> Type  </td> <td align=\"left\" class=\"GridRightHead\"> Time  </td> </tr>   " + _tr + " </table>";
                AttendanceDetailsDiv.InnerHtml = str;
                _valid = true;
            }
            else
            {
                _Errmsg = "No details found";
            }

            return _valid;
        }
    }
}
