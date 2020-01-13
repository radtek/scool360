﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Odbc;
using WinBase;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Data;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using WinEr;
using WebChart;
using System.Drawing;
using System.Text;

namespace WinEr
{
    public partial class StudentCumulativeReport : System.Web.UI.Page
    {
        private StudentManagerClass MyStudMang;
        private ExamManage MyExamMang;
        private KnowinUser MyUser;
        private MysqlClass _Mysqlobj;
        private ClassOrganiser MyClassMngr;
        private SchoolClass objSchool = null;
        private string M_Logo = "";
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserObj"] == null)
                Response.Redirect("Default.aspx");

            MyUser = (KnowinUser)Session["UserObj"];
            MyExamMang = MyUser.GetExamObj();
            MyClassMngr = MyUser.GetClassObj();
            MyStudMang = MyUser.GetStudentObj();
            if (MyExamMang == null)
                Response.Redirect("RoleErr.htm");
            string _ConnectionString = WinerUtlity.SingleSchoolConnectionString;
            if (WinerUtlity.NeedCentrelDB())
            {
                if (Session[WinerConstants.SessionSchool] == null)
                {
                    Response.Redirect("Logout.aspx");
                }
                objSchool = (SchoolClass)Session[WinerConstants.SessionSchool];
                _ConnectionString = objSchool.ConnectionString;
            }

            _Mysqlobj = new MysqlClass(_ConnectionString);
            if (!IsPostBack)
                LoadDetails();
        }

        protected void Drp_SelectClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudentsDetailsToDropDown();
        }

        protected void Btn_StudentReport_Click(object sender, EventArgs e)
        {
            ExamReportPdf MyPdf = new ExamReportPdf(MyStudMang.m_MysqlDb, MyUser, objSchool);
            string _ErrorMsg = "";
            string _physicalpath = WinerUtlity.GetAbsoluteFilePath(objSchool, Server.MapPath(""));// +"\\PDF_Files\\Invoice" + _InvoiceID + ".pdf";
            string _PdfName = "";
            int _StudentId = int.Parse(Drp_SelectStudent.SelectedValue);
            if (int.Parse(Drp_SelectClass.SelectedValue.ToString()) != -1)
            {

                int _ClassId = 0;

                int _currentclassId = int.Parse(Drp_SelectClass.SelectedValue.ToString());
                DataSet ExamDetails = GetExamDetails(_ClassId, int.Parse(Drp_SelectStudent.SelectedValue));
                if (ExamDetails != null)
                {
                    foreach (DataRow dr in ExamDetails.Tables[0].Rows)
                    {
                        int examSchdlId = int.Parse(dr["Id"].ToString());
                        string classname = dr["ClassName"].ToString();
                        DataSet SubjectDetails = GetSubjects(_ClassId, _StudentId, examSchdlId);
                        if (SubjectDetails != null && SubjectDetails.Tables[0].Rows.Count > 0)
                        {
                            LoadAllPreviousExamReportToGridNew(dr, SubjectDetails, _ClassId, int.Parse(Drp_SelectStudent.SelectedValue));

                            DataSet NonSubject = GetNonSubjects(_currentclassId, int.Parse(Drp_SelectStudent.SelectedValue));
                            }
                        }
                }




                int _ExamId = 0;
                //  int _ExamId = int.Parse(Drp_Exam.SelectedValue);

                if (MyPdf.CreateIndividualStudentExamReportPdf(_StudentId, _ExamId, MyUser.CurrentBatchId, MyUser.CurrentBatchName, _physicalpath, out _PdfName, out _ErrorMsg))
                {
                    _ErrorMsg = "";

                    ClientScript.RegisterClientScriptBlock(this.GetType(), "keyClientBlock", "window.open(\"OpenPdfPage.aspx?PdfName=" + _PdfName + "\");", true);

                }
                else
                {
                    //WC_MessageBox.ShowMssage(_ErrorMsg);
                }

            }

        }
        //protected void test(object sender, EventArgs e)
        //{
        //    if (int.Parse(Drp_SelectClass.SelectedValue.ToString()) != -1)
        //    {

        //        int _ClassId = 0;

        //        int _currentclassId = int.Parse(Drp_SelectClass.SelectedValue.ToString());
        //        DataSet ExamDetails = GetExamDetails(_ClassId, int.Parse(Drp_SelectStudent.SelectedValue));
        //        if (ExamDetails != null)
        //        {


        //            DataSet SubjectDetails = GetAllSubject(_ClassId, ExamDetails, int.Parse(Drp_SelectStudent.SelectedValue));

        //            if (SubjectDetails != null && SubjectDetails.Tables[0].Rows.Count > 0)
        //            {
        //                LoadAllPreviousExamReportToGridNew(ExamDetails, SubjectDetails, _ClassId, int.Parse(Drp_SelectStudent.SelectedValue));

        //                DataSet NonSubject = GetNonSubjects(_currentclassId, int.Parse(Drp_SelectStudent.SelectedValue));
        //                string CTR = GetExamReportDetailsToExcel(ExamDetails, SubjectDetails, _ClassId, int.Parse(Drp_SelectStudent.SelectedValue));
        //                ViewState["ExcelFormat"] = CTR;
        //                string _WorkSheetName = "ExamDetails";
        //                string FileName = "PreviousBatchExamReport";

        //                string _TableString = (string)ViewState["ExcelFormat"];// GetExamReportDetailsToExcel(); 

        //                if (!WinEr.ExcelUtility.ExportBuiltStringToExcel(FileName, CTR, _WorkSheetName))
        //                {
                            
        //                }



                      
        //            }
        //            else
        //            {
                       
                        
        //                Lbl_Err.Text = "Subject Details does not found";
                        
        //            }
        //        }
        //        else
        //        {
                    
        //            Lbl_Err.Text = "No exam found";
                   
        //        }

        //    }
        //    else
        //    {
              
        //        Lbl_Err.Text = "No class found";
              
        //    }
        //}
    

        #endregion
        private DataSet GetNonSubjects(int _classId, int _studentId)
        {
            //string sql = "SELECT tblclasssubmap.SubjectId ,tblsubjects.sub_Catagory from tblclasssubmap INNER JOIN tblsubjects on tblsubjects.Id = tblclasssubmap.SubjectId where tblclasssubmap.ClassId = " + _classId +"";
            string sql = "SELECT tblclasssubmap.SubjectId ,tblsubjects.sub_Catagory,tbltime_subgroup.Name from tblclasssubmap INNER JOIN tblsubjects on tblsubjects.Id = tblclasssubmap.SubjectId INNER JOIN tbltime_subgroup on tbltime_subgroup.Id = tblsubjects.sub_Catagory where tblclasssubmap.ClassId =" + _classId + " and tbltime_subgroup.IsActive = 0";
            DataSet Dt = MyStudMang.m_MysqlDb.ExecuteQueryReturnDataSet(sql);

            return Dt;
        }
        private DataSet GetExamDetails(int _ClassId,int _studId)
        {


            string sql1 = ""; string sql = "";

            if (int.Parse(Drp_SelectClass.SelectedValue) > 0)
            {
                if (int.Parse(Drp_SelectClass.SelectedValue) == MyUser.CurrentBatchId)
                {
                    sql1 = @"select tblexamschedule.Id, tblexammaster.ExamName, (select tblperiod.Period from tblperiod where tblperiod.Id= tblexamschedule.PeriodId) as Period, 
                        tblclass.ClassName as ClassName, (select tblbatch.BatchName from tblbatch where tblbatch.Id= tblexamschedule.BatchId) as Batch from tblexamschedule 
                        inner join tblclassexam on tblclassexam.Id= tblexamschedule.ClassExamId
                        inner join tblexammaster on tblexammaster.id= tblclassexam.ExamId
                        inner join tblclass on  tblclass.id= tblclassexam.ClassId   ";

                    sql1 = sql1 + " where tblexamschedule.BatchId =" + Drp_SelectClass.SelectedValue + " and tblclass.id=" + _ClassId;

                    sql1 = sql1 + " order by tblexamschedule.Id ";
                    sql = sql1;
                }
                else
                {
                    sql = "select tblexamschedule_history.Id, tblexamschedule_history.ExamName, ( select tblperiod.Period from tblperiod where tblperiod.Id= tblexamschedule_history.PeriodId) as Period, (SELECT tblclass.ClassName from tblclass where tblclass.Id= tblexamschedule_history.ClassId) as ClassName, (select tblbatch.BatchName from tblbatch where tblbatch.Id= tblexamschedule_history.BatchId) as Batch from tblexamschedule_history where";
                    if (_ClassId > 0)
                    {
                        sql = sql + " tblexamschedule_history.BatchId in (select Distinct( tblstudentclassmap_history.BatchId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + "      and   tblexamschedule_history.ClassId =" + _ClassId + " and tblexamschedule_history.BatchId=" + Drp_SelectClass.SelectedValue + ") and tblexamschedule_history.ClassId in (select Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + ")";
                    }
                    else
                    {
                        sql = sql + " tblexamschedule_history.BatchId in (select Distinct( tblstudentclassmap_history.BatchId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + ") and tblexamschedule_history.ClassId in (select Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + ")";
                    }
                    sql = sql + " order by tblexamschedule_history.Id ";
                }

            }
            else
            {
                sql1 = @"select tblexamschedule.Id, tblexammaster.ExamName, (select tblperiod.Period from tblperiod where tblperiod.Id= tblexamschedule.PeriodId) as Period, 
                        tblclass.ClassName as ClassName, (select tblbatch.BatchName from tblbatch where tblbatch.Id= tblexamschedule.BatchId) as Batch from tblexamschedule 
                        inner join tblclassexam on tblclassexam.Id= tblexamschedule.ClassExamId
                        inner join tblexammaster on tblexammaster.id= tblclassexam.ExamId
                        inner join tblclass on  tblclass.id= tblclassexam.ClassId  
                    inner join tblstudentclassmap_history on  tblstudentclassmap_history.ClassId= tblclassexam.ClassId ";

                sql1 = sql1 + " where  tblstudentclassmap_history.StudentId=" + _studId;

                sql1 = sql1 + " order by tblexamschedule.Id ";


                sql = "select tblexamschedule_history.Id, tblexamschedule_history.ExamName, ( select tblperiod.Period from tblperiod where tblperiod.Id= tblexamschedule_history.PeriodId) as Period, (SELECT tblclass.ClassName from tblclass where tblclass.Id= tblexamschedule_history.ClassId) as ClassName, (select tblbatch.BatchName from tblbatch where tblbatch.Id= tblexamschedule_history.BatchId) as Batch from tblexamschedule_history where";
                sql = sql + " tblexamschedule_history.BatchId in (select Distinct( tblstudentclassmap_history.BatchId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + ") and tblexamschedule_history.ClassId in (select Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId;
                sql = sql + " order by tblexamschedule_history.Id )";
                sql = "(" + sql + ") union (" + sql1 + ")";

            }


            DataSet Dt = MyStudMang.m_MysqlDb.ExecuteQueryReturnDataSet(sql);

            return Dt;



        }
        private DataSet GetAllSubject(int _ClassId, DataSet ExamDetails,int _studId)
        {

            string sqlSubject = "", sqlSubjectCurrnet = "";

            if (_ClassId > 0)
            {
                if (_ClassId == MyUser.CurrentBatchId)
                {
                    sqlSubjectCurrnet = @"select distinct( tblexammark .SubjectId), tblsubjects.subject_name as subname, tblexammark.MarkColumn, 
                                    tblclassexamsubmap.MaxMark   ,  tblexamschedule.Id as SchId from tblexammark 
                                    inner join tblsubjects on  tblsubjects.id=tblexammark.SubjectId
                                    inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId 
                                    inner join  tblclassexamsubmap on tblclassexamsubmap.ClassExamId= tblexamschedule.ClassExamId and  tblclassexamsubmap.SubId= tblexammark .SubjectId  where ";
                }
                else
                {
                    sqlSubject = "select distinct(tblexammark_history.SubjectId), tblexammark_history.SubjectName as subname, tblexammark_history.MarkColumn, tblexammark_history.MaxMark   , tblexammark_history.ExamSchId as SchId   from tblexammark_history where tblexammark_history.ExamSchId in ( select tblexamschedule_history.Id from tblexamschedule_history where";
                }
            }
            else
            {
                sqlSubjectCurrnet = @"select distinct( tblexammark .SubjectId), tblsubjects.subject_name as subname, tblexammark.MarkColumn, 
                                    tblclassexamsubmap.MaxMark  ,  tblexamschedule.Id as SchId from tblexammark 
                                    inner join tblsubjects on  tblsubjects.id=tblexammark.SubjectId
                                    inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId 
                                    inner join  tblclassexamsubmap on tblclassexamsubmap.ClassExamId= tblexamschedule.ClassExamId and  tblclassexamsubmap.SubId= tblexammark .SubjectId  where ";

                sqlSubject = "select distinct(tblexammark_history.SubjectId), tblexammark_history.SubjectName as subname, tblexammark_history.MarkColumn, tblexammark_history.MaxMark , tblexammark_history.ExamSchId as SchId from tblexammark_history where tblexammark_history.ExamSchId in ( select tblexamschedule_history.Id from tblexamschedule_history where";
            }




            if (_ClassId > 0)
            {
                if (int.Parse(Drp_SelectClass.SelectedValue) == MyUser.CurrentBatchId)
                {
                    sqlSubjectCurrnet = sqlSubjectCurrnet + @" tblexamschedule.BatchId =" + _ClassId + " and tblexamschedule.ClassExamId   in (select tblclassexam.Id  from tblclassexam where tblclassexam.ClassId in    (select  Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId= " + _studId + "))";

                }
                else
                {
                    sqlSubject = sqlSubject + " tblexamschedule_history.BatchId=" + _ClassId + " and tblexamschedule_history.ClassId=" + _ClassId + ")";
                }

            }
            else
            {
                //                sqlSubjectCurrnet = sqlSubjectCurrnet + "  tblexamschedule.BatchId in     (select distinct(tblstudentclassmap_history.BatchId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + int.Parse(Session["StudId"].ToString()) + ") and    tblexamschedule.ClassExamId   in (select tblclassexam.Id  from tblclassexam where tblclassexam.ClassId in  (select  Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId   =" + int.Parse(Session["StudId"].ToString()) + "))";
                sqlSubjectCurrnet = sqlSubjectCurrnet + "     tblexamschedule.ClassExamId   in (select tblclassexam.Id  from tblclassexam where tblclassexam.ClassId in  (select  Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId   =" + _studId + "))";

                sqlSubject = sqlSubject + " tblexamschedule_history.BatchId in (select distinct( tblstudentclassmap_history.BatchId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + ") and tblexamschedule_history.ClassId in (select Distinct( tblstudentclassmap_history.ClassId) from tblstudentclassmap_history where tblstudentclassmap_history.StudentId=" + _studId + "))";
            }

            if (_ClassId > 0)
            {
                if (_ClassId == MyUser.CurrentBatchId)
                {
                    sqlSubject = sqlSubjectCurrnet + " order by tblexammark.SubjectOrder";
                }
                else
                {
                    sqlSubject = sqlSubject + " order by tblexammark_history.SubjectOrder";
                }
            }
            else
            {
                sqlSubject = "(" + sqlSubject + " ) union ( " + sqlSubjectCurrnet + " order by tblexammark.SubjectOrder )";
            }



            DataSet Dt = MyStudMang.m_MysqlDb.ExecuteQueryReturnDataSet(sqlSubject);
            return Dt;

        }


        private DataSet GetSubjects(int _ClassId, int _studId, int _examSchdlId)
        {

            string sqlSubject = "";

            sqlSubject = "select distinct(tblexammark_history.SubjectId), tblexammark_history.SubjectName as subname, tblexammark_history.MarkColumn, tblexammark_history.MaxMark ,tblexammark_history.ExamSchId as SchId from tblexammark_history where tblexammark_history.ExamSchId = " + _examSchdlId + "";


            DataSet Dt = MyStudMang.m_MysqlDb.ExecuteQueryReturnDataSet(sqlSubject);
            return Dt;

        }



        private string GetExamReportDetailsToExcel(DataSet ExamDetails, DataSet SubjectDetails, int _ClassId, int _studId)
        {
            StringBuilder CTR = new StringBuilder();
            string studname = "";
            string adno = "";
            string classname = "";
            string rollno = "";
            string DOB = "";
            int studid = _studId;
            //string studimg = "Student"
            string Sql = "select tblview_student.StudentName, tblview_student.AdmitionNo, tblview_student.DOB,tblclass.ClassName,tblview_student.RollNo from tblview_student INNER join tblclass on tblview_student.ClassId=tblclass.Id where tblview_student.Id=" + studid;
            OdbcDataReader MyReader = MyStudMang.m_MysqlDb.ExecuteQuery(Sql);
            //MyReader = m_MysqlDb.ExecuteQuery(Sql);
            if (MyReader.HasRows)
            {
                MyReader.Read();
                studname = MyReader.GetValue(0).ToString();
                adno = MyReader.GetValue(1).ToString();
                classname = MyReader.GetValue(3).ToString();
                rollno = MyReader.GetValue(4).ToString();
                DOB = MyReader.GetValue(2).ToString();

            }
           
            CTR.Append("<table runat=\"server\" width=\"100%\" style=\"border: thin solid #000000\">");

            CTR.Append("<tr>");
            CTR.Append("<tr><td><br/></td><td class=\"CellStyle\">Student Name: </td>  <td class=\"CellStyle\">" + studname + "</td>  </tr>");
            CTR.Append("<tr><td><br/></td><td class=\"CellStyle\">Admission No: </td>  <td class=\"CellStyle\">" + adno + "</td>  </tr>");
            CTR.Append("<tr><td><br/></td><td class=\"CellStyle\">DOB: </td>  <td class=\"CellStyle\">" + DOB + "</td>  </tr>");
            CTR.Append("<tr><td><br/></td><td class=\"CellStyle\">Class Name: </td>  <td class=\"CellStyle\">" + classname + "</td>  </tr>");
            CTR.Append("<tr><td><br/></td><td class=\"CellStyle\">Roll No: </td>  <td class=\"CellStyle\">" + rollno + "</td>  </tr>");

            CTR.Append("</tr></table>");

            CTR.Append("<table runat=\"server\" width=\"100%\" style=\"border: thin solid #000000\">");

            CTR.Append("<tr>");
            
            foreach (DataRow dr in ExamDetails.Tables[0].Rows)
            {
                string Colmns = "";

                foreach (DataRow dr1 in SubjectDetails.Tables[0].Rows)
                {
                    if (int.Parse(dr1["SchId"].ToString()) == int.Parse(dr["Id"].ToString()))
                    {
                        if (Colmns != "") Colmns = Colmns + ",";


                        Colmns = Colmns + dr1["MarkColumn"];
                    }
                }
                string sql2 = "", sql3 = "";

                if (int.Parse(Drp_SelectClass.SelectedValue) > 0)
                {
                    if (int.Parse(Drp_SelectClass.SelectedValue) == MyUser.CurrentBatchId)
                    {
                        sql2 = "select " + Colmns + ", tblstudentmark.TotalMax , tblstudentmark.TotalMark , tblstudentmark.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark on tblstudentmark.StudId = tblstudentclassmap_history.StudentId where tblstudentmark.ExamSchId=" + dr["Id"] + " and tblstudentmark.StudId=" + _studId;
                    }
                    else
                    {
                        sql2 = "select " + Colmns + ", tblstudentmark_history.TotalMax , tblstudentmark_history.TotalMark , tblstudentmark_history.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark_history on tblstudentmark_history.StudId = tblstudentclassmap_history.StudentId where tblstudentmark_history.ExamSchId=" + dr["Id"] + " and tblstudentmark_history.StudId=" + _studId;
                    }
                }
                else
                {
                    sql3 = "select " + Colmns + ", tblstudentmark.TotalMax , tblstudentmark.TotalMark , tblstudentmark.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark on tblstudentmark.StudId = tblstudentclassmap_history.StudentId where tblstudentmark.ExamSchId=" + dr["Id"] + " and tblstudentmark.StudId=" + _studId;

                    sql2 = "select " + Colmns + ", tblstudentmark_history.TotalMax , tblstudentmark_history.TotalMark , tblstudentmark_history.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark_history on tblstudentmark_history.StudId = tblstudentclassmap_history.StudentId where tblstudentmark_history.ExamSchId=" + dr["Id"] + " and tblstudentmark_history.StudId=" + _studId;

                    sql2 = "(" + sql2 + " ) union (" + sql3 + ")";
                }
                OdbcDataReader Reader1 = MyStudMang.m_MysqlDb.ExecuteQuery(sql2);
                if (Reader1.HasRows)
                {
                    double val = 0;

                    int classid = getclassid(dr["ClassName"].ToString());
                    string ImageName = getimagename(_studId,classid);                  
                    string imgurl = (MyUser.FilePath + "\\ThumbnailImages\\" + "Student" + _studId.ToString() + "Class" + classid + ImageName);
                    CTR.Append("<td valign=\"top\"><table runat=\"server\" width=\"100%\"><tr rowspan=\"4\"> <td colspan=\"2\" class=\"TableHeaderStyle\"> <img alt=\"\" src=\"" + imgurl + "\" width=\"40px\" height=\"40px\" /></td> </tr><tr><td colspan=\"2\" class=\"TableHeaderStyle\"><b>" + dr["ClassName"] + ":" + dr["Batch"] + "</b></td></tr>  <tr><td colspan=\"2\" class=\"SubHeaderStyle\"><b>" + dr["ExamName"] + " (" + dr["Period"] + ")" + "</b></td></tr>  <tr><td colspan=\"2\"></td></tr>");

                    int j = 0;
                    foreach (DataRow dr1 in SubjectDetails.Tables[0].Rows)
                    {
                        if (int.Parse(dr1["SchId"].ToString()) == int.Parse(dr["Id"].ToString()))
                        {
                            double.TryParse(Reader1[dr1["MarkColumn"].ToString()].ToString(), out val);
                            if (val > 0)
                            {
                                CTR.Append("<tr><td class=\"CellStyle\">" + dr1["subname"] + "</td>  <td class=\"CellStyle\">" + val.ToString(".") + " / " + double.Parse(dr1["MaxMark"].ToString()).ToString(".") + "</td>  </tr>");
                            }
                            else
                            {
                                CTR.Append("<tr><td class=\"CellStyle\">" + dr1["subname"] + "</td>  <td class=\"CellStyle\">" + Reader1[dr1["MarkColumn"].ToString()].ToString() + "/" + double.Parse(dr1["MaxMark"].ToString()).ToString(".") + "</td>  </tr>");
                            }


                        }

                        j++;
                    }


                    CTR.Append("<tr><td class=\"CellStyle\">Total Mark </td>  <td class=\"CellStyle\">" + Reader1["TotalMark"].ToString() + "</td>  </tr>");
                    CTR.Append("<tr><td class=\"CellStyle\">Total Maximum </td>  <td class=\"CellStyle\">" + Reader1["TotalMax"].ToString() + "</td>  </tr>");

                    //CTR.Append("<tr><td class=\"CellStyle\">Average </td>  <td class=\"CellStyle\">" + double.Parse(Reader1["Avg"].ToString()).ToString("0.00") + "</td>  </tr>");
                    if (Reader1["Avg"].ToString().Trim() != "")
                        CTR.Append("<tr><td class=\"CellStyle\">Average </td>  <td class=\"CellStyle\">" + double.Parse(Reader1["Avg"].ToString()).ToString("0.00") + "</td>  </tr>");
                    //else
                    //    CTR.Append("<tr><td class=\"CellStyle\">Average </td>  <td class=\"CellStyle\">" + "0.00" + "</td>  </tr>");


                    CTR.Append("<tr><td class=\"CellStyle\">Grade </td>  <td class=\"CellStyle\">" + Reader1["Grade"].ToString() + "</td>  </tr>");
                    CTR.Append("<tr><td class=\"CellStyle\">Result </td>  <td class=\"CellStyle\">" + Reader1["Result"].ToString() + "</td>  </tr>");
                    CTR.Append("<tr><td class=\"CellStyle\">Rank </td>  <td class=\"CellStyle\">" + Reader1["Rank"].ToString() + "</td>  </tr>");

                    CTR.Append("</table>");
                }






            }
            CTR.Append("</td></tr></table>");
            return CTR.ToString();
        }

        #region Report Area
        //protected void Img_Export_Click(object sender, ImageClickEventArgs e)
        //{
        //    string _WorkSheetName = "ExamDetails";
        //    string FileName = "PreviousBatchExamReport";

        //    string _TableString = (string)ViewState["ExcelFormat"];// GetExamReportDetailsToExcel(); 

        //    if (!WinEr.ExcelUtility.ExportBuiltStringToExcel(FileName, _TableString, _WorkSheetName))
        //    {
        //        //WC_MessageBox.ShowMssage("This function need Ms office");
        //    }
        //}
        //private void LoadReport()
        //{
        //    int classId = 0;
        //    int studentId = 0;

        //    if (GatBaseValuese(out classId, out studentId))
        //    {
        //        DataSet TotalStudList;
        //        DataSet totalMarkRatios = null;

        //        ExamNode[] _ExamDetails;
        //        ExamNode[] _SubDetails;
        //        ExamNode[] CC_ExamDetails = null;
        //        ExamNode[] CC_SubDetails = null;

        //        SchoolDetails _SchoolDetails;
        //        List<CBSCFinalReportClass> ClsObj = new List<CBSCFinalReportClass>();
        //        CBSCFinalReportClass _ReportObj;

        //        GetSchoolDetails(out _SchoolDetails);
        //        GetExamDetails(int.Parse(Drp_SelectClass.SelectedValue), "CBSE MAIN REPORT", out _ExamDetails);
        //        GetSubDetails(int.Parse(Drp_SelectClass.SelectedValue), "CBSE MAIN REPORT", out _SubDetails);

        //        if (_ExamDetails != null && _SubDetails != null && _ExamDetails.Length > 0 && _SubDetails.Length > 0)
        //        {
        //            TotalStudList = GetAllStudentListFromClass(studentId, classId);

        //            GetExamDetails(int.Parse(Drp_SelectClass.SelectedValue), "CBSE ACTIVITY REPORT", out CC_ExamDetails);
        //            GetCC_SubDetails(int.Parse(Drp_SelectClass.SelectedValue), "CBSE ACTIVITY REPORT", out CC_SubDetails);

        //            totalMarkRatios = GetMarkRatios(Drp_SelectClass.SelectedValue);


        //            foreach (DataRow Dr in TotalStudList.Tables[0].Rows)
        //            {
        //                _ReportObj = new CBSCFinalReportClass(_Mysqlobj, int.Parse(Dr[0].ToString()));
        //                _ReportObj.ExamDetails(_ExamDetails, _SubDetails, Dr, "Main");
        //                _ReportObj.ExamDetails(CC_ExamDetails, CC_SubDetails, Dr, "CC");

        //                ClsObj.Add(_ReportObj);
        //            }


        //            CreateReport(ClsObj, _ExamDetails, _SubDetails, CC_ExamDetails, CC_SubDetails, _SchoolDetails, studentId, totalMarkRatios);
        //        }
        //        else
        //        {
        //            Lbl_Err.Text = "Exam details not found";
        //        }



        //    }

        //}



        #region Create Records and Write to File

        //private string GtSchoolLogo()
        //{

        //    String ImageUrl = "";


        //    ImageUploaderClass imgobj = new ImageUploaderClass(objSchool);
        //    byte[] img_bytes = imgobj.getImageBytes(objSchool.SchoolId, "Logo");
        //    M_Logo = MyUser.FilePath + "/ThumbnailImages/" + objSchool.SchoolId + "_" + System.DateTime.Now.Millisecond + ".jpg";


        //    File.WriteAllBytes(M_Logo, img_bytes);
        //    ImageUrl = M_Logo;
        //    return ImageUrl;



        //}

        //private string GtSchoolCode()
        //{

        //    String SchoolCode = "";

        //    string sql = "select tblschooldetails.SchoolCode from tblschooldetails";
        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);

        //    if (MyReader.HasRows)
        //    {
        //        SchoolCode = MyReader.GetValue(0).ToString();

        //    }

        //    return SchoolCode;



        //}

        //private string GtdiesCode()
        //{

        //    String DIESCode = "";

        //    string sql = "select tblschooldetails.DIESCode from tblschooldetails";
        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);

        //    if (MyReader.HasRows)
        //    {
        //        DIESCode = MyReader.GetValue(0).ToString();

        //    }

        //    return DIESCode;



        //}



        //private Document LoadPDFPage(List<CBSCFinalReportClass> ClsObj, ExamNode[] _ExamDetails, ExamNode[] _SubDetails, SchoolDetails _SchoolDetails, ExamNode[] CC_ExamDetails, ExamNode[] CC_SubDetails, DataSet totalMarkRatios)
        //{
        //    string path = GtSchoolLogo();
        //    string schoolcode = GtSchoolCode();
        //    string diescode = GtdiesCode();
        //    Document document = new Document();

        //    for (int i = 0; i < ClsObj.Count; i++)
        //    {

        //        PageSetup PgSt = document.DefaultPageSetup;
        //        PgSt.PageFormat = MigraDoc.DocumentObjectModel.PageFormat.A4;
        //        PgSt.LeftMargin = 0;
        //        PgSt.RightMargin = 0;
        //        PgSt.TopMargin = 30;
        //        PgSt.BottomMargin = 0;

        //        PgSt.HeaderDistance = 0;
        //        PgSt.FooterDistance = 0;
        //        // Add a section to the document

        //        Section section = document.AddSection();

        //        //// Add a paragraph to the section
        //        //Paragraph paragraph = section.AddParagraph();

        //        //paragraph.AddLineBreak();
        //        //MigraDoc.DocumentObjectModel.Font font = new MigraDoc.DocumentObjectModel.Font("Calibri", 16);
        //        //paragraph.Format.Font = font;
        //        //paragraph.Format.Alignment = ParagraphAlignment.Center;
        //        //paragraph.AddFormattedText(MyUser.SchoolName, TextFormat.Bold);

        //        //paragraph = section.AddParagraph();
        //        //paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //        //paragraph.Format.Alignment = ParagraphAlignment.Center;
        //        //paragraph.AddFormattedText(_SchoolDetails.Address, TextFormat.NotBold);
        //        //paragraph.AddFormattedText(" ");

        //        //paragraph = section.AddParagraph();
        //        ////paragraph.AddLineBreak();
        //        //paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 14);
        //        //paragraph.Format.Alignment = ParagraphAlignment.Center;
        //        //paragraph.AddFormattedText(Txt_ReportName.Text.ToUpper(), TextFormat.Bold);


        //        //MigraDoc.DocumentObjectModel.Tables.Table table = section.AddTable();

        //        ////PgSt.PageHeight = 100;
        //        ////PgSt.PageWidth = 100; 

        //        //table.Borders.Width = 2;
        //        ////table.Borders.Left.Width = 0.5;
        //        ////table.Borders.Right.Width = 0.5;
        //        //table.Rows.LeftIndent = 25;

        //        //MigraDoc.DocumentObjectModel.Tables.Column Col = table.AddColumn(PgSt.PageWidth - 50);
        //        //MigraDoc.DocumentObjectModel.Tables.Row row = table.AddRow();
        //        //row.Height = PgSt.PageHeight - 100;
        //        //Col.Borders.Visible = true;

        //        //MigraDoc.DocumentObjectModel.Tables.Cell cel = row.Cells[0];

        //        //paragraph = section.AddParagraph();

        //        MigraDoc.DocumentObjectModel.Tables.Table table = section.AddTable();



        //        table.Borders.Width = 2;
        //        //table.Borders.Left.Width = 0.5;
        //        //table.Borders.Right.Width = 0.5;
        //        table.Rows.LeftIndent = 25;
        //        MigraDoc.DocumentObjectModel.Tables.Column Col = table.AddColumn(PgSt.PageWidth - 50);
        //        MigraDoc.DocumentObjectModel.Tables.Row row = table.AddRow();
        //        row.Height = PgSt.PageHeight - 100;
        //        Col.Borders.Visible = true;

        //        MigraDoc.DocumentObjectModel.Tables.Cell cel = row.Cells[0];
        //        // Add a paragraph to the section
        //        Paragraph paragraph = section.AddParagraph();
        //        paragraph = cel.AddParagraph();
        //        paragraph.AddLineBreak();


        //        MigraDoc.DocumentObjectModel.Tables.Table tb = new MigraDoc.DocumentObjectModel.Tables.Table();


        //        tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //        tb.AddColumn((PgSt.PageWidth - 80) / 7);
        //        tb.AddColumn((PgSt.PageWidth - 80) - ((PgSt.PageWidth - 80) / 7));

        //        row = tb.AddRow();
        //        row.Cells[0].AddImage(path);
        //        row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
        //        row.Cells[0].MergeDown = 1;


        //        row.Cells[1].Format.Font.Size = 20;
        //        row.Cells[1].Format.Font.Bold = true;
        //        row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //        row.Cells[1].AddParagraph(MyUser.SchoolName.ToUpperInvariant());

        //        row = tb.AddRow();
        //        row.Cells[1].Format.Font.Size = 14;
        //        row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //        row.Cells[1].AddParagraph(_SchoolDetails.Address);

        //        if (schoolcode != "" && diescode != "")
        //        {
        //            row = tb.AddRow();
        //            row.Cells[1].Format.Font.Size = 14;
        //            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //            row.Cells[1].AddParagraph("School Code:" + schoolcode + ",DISE Code:" + diescode);
        //        }

        //        row = tb.AddRow();
        //        row.Cells[1].Format.Font.Size = 18;
        //        row.Cells[1].Format.Font.Bold = true;
        //        row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //        row.Cells[1].AddParagraph(Txt_ReportName.Text.ToUpper());

        //        row = tb.AddRow();
        //        row.Cells[1].Format.Font.Size = 16;
        //        row.Cells[1].Format.Font.Bold = true;
        //        row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //        row.Cells[1].AddParagraph("SESSION:" + MyUser.CurrentBatchName);




        //        cel.Elements.Add(tb);

        //        paragraph = cel.AddParagraph();
        //        paragraph.Format.Alignment = ParagraphAlignment.Left;
        //        paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //        paragraph.AddFormattedText("STUDENT INFORMATIONS", TextFormat.Underline | TextFormat.Bold);
        //        paragraph.AddLineBreak();

        //      //  MigraDoc.DocumentObjectModel.Tables.Table tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //        tb = new MigraDoc.DocumentObjectModel.Tables.Table();
        //        tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //        tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 3);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 3);

        //        row = tb.AddRow();

        //        row.Cells[0].AddParagraph("Name");
        //        row.Cells[1].AddParagraph(":" + ClsObj[i].m_StudDetails.Name);
        //        row.Cells[2].AddParagraph("Father's Name");
        //        row.Cells[3].AddParagraph(":" + ClsObj[i].m_StudDetails.FatherName);

        //        row = tb.AddRow();

        //        row.Cells[0].AddParagraph("Date of Birth");
        //        row.Cells[1].AddParagraph(":" + ClsObj[i].m_StudDetails.DOB);
        //        row.Cells[2].AddParagraph("Mother's Name");
        //        row.Cells[3].AddParagraph(":" + ClsObj[i].m_StudDetails.MotherName);

        //        row = tb.AddRow();

        //        row.Cells[0].AddParagraph("Admission No");
        //        row.Cells[1].AddParagraph(":" + ClsObj[i].m_StudDetails.AdmissionNum);
        //        row.Cells[2].AddParagraph("Telephone No");
        //        row.Cells[3].AddParagraph(":" + ClsObj[i].m_StudDetails.Tel);

        //        row = tb.AddRow();

        //        row.Cells[0].AddParagraph("Class & Roll No");
        //        row.Cells[1].AddParagraph(":" + ClsObj[i].m_StudDetails.Class + ", " + ClsObj[i].m_StudDetails.RollNum);
        //        row.Cells[2].AddParagraph("Address");
        //        row.Cells[3].AddParagraph(":" + ClsObj[i].m_StudDetails.Add);

        //        tb.Borders.Visible = false;
        //        cel.Elements.Add(tb);

        //        if (Chk_HealthInfo.Checked)
        //        {
        //            paragraph = cel.AddParagraph();
        //            paragraph.Format.Alignment = ParagraphAlignment.Left;
        //            paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //            paragraph.AddLineBreak();
        //            paragraph.AddFormattedText("HEALTH INFORMATIONS", TextFormat.Underline | TextFormat.Bold);
        //            paragraph.AddLineBreak();

        //            tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //            tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //            tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 3);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 3);

        //            row = tb.AddRow();
        //            row.Cells[0].AddParagraph("Height");
        //            row.Cells[1].AddParagraph(":");
        //            row.Cells[2].AddParagraph("Weight");
        //            row.Cells[3].AddParagraph(":");
        //            row = tb.AddRow();
        //            row.Cells[0].AddParagraph("Blood Group");
        //            row.Cells[1].AddParagraph(":");
        //            row.Cells[2].AddParagraph("Vision");
        //            row.Cells[3].AddParagraph(":(L)                    (R)");
        //            row = tb.AddRow();
        //            row.Cells[0].AddParagraph("Dental Hygiene");
        //            row.Cells[1].AddParagraph(":");


        //            tb.Borders.Visible = false;
        //            cel.Elements.Add(tb);


        //        }

        //        if (Chk_Self_Awareness.Checked)
        //        {


        //            // Self awarness
        //            paragraph = cel.AddParagraph();

        //            paragraph.Format.Alignment = ParagraphAlignment.Left;
        //            paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //            paragraph.AddLineBreak();
        //            paragraph.AddFormattedText("SELF AWARENESS", TextFormat.Underline | TextFormat.Bold);
        //            paragraph.AddLineBreak();

        //            tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //            tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //            tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 3);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 6);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 3);

        //            row = tb.AddRow();

        //            row.Cells[0].AddParagraph("Goals");
        //            row.Cells[1].AddParagraph(":");
        //            row.Cells[2].AddParagraph("Interests/Hobbies");
        //            row.Cells[3].AddParagraph(":");

        //            row = tb.AddRow();

        //            row.Cells[0].AddParagraph("Strengths");
        //            row.Cells[1].AddParagraph(":");
        //            row.Cells[2].AddParagraph("Responsibilities Discharged / Achievements ");
        //            row.Cells[3].AddParagraph(":");

        //            tb.Borders.Visible = false;
        //            cel.Elements.Add(tb);
        //        }

        //        // main exam area

        //        paragraph = cel.AddParagraph();
        //        paragraph.Format.Alignment = ParagraphAlignment.Left;
        //        paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //        paragraph.AddLineBreak();
        //        paragraph.AddFormattedText("Part 1: Scholastic Area", TextFormat.Underline | TextFormat.Bold);
        //        paragraph.AddLineBreak();
        //        tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //        tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //        tb.AddColumn((PgSt.PageWidth - 80) * 4 / 15);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 15.5);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 14);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 14);
        //        tb.AddColumn((PgSt.PageWidth - 80) / 14);

        //        row = tb.AddRow();
        //        row.Cells[0].AddParagraph().AddFormattedText("1A: Academic Performance", TextFormat.Bold);


        //        if (_ExamDetails.Length > 0)
        //        {
        //            int TempVal = 0, k = 0;
        //            for (int j = 0; j < _ExamDetails.Length; j++)
        //            {
        //                if (TempVal != 0 && j == 3)
        //                {
        //                    TempVal = 0;
        //                    k = k + 4;
        //                }
        //                string TypeName = "";
        //                if (j == 0)
        //                {
        //                    TypeName = "TERM I";
        //                    row.Cells[k + 1].AddParagraph().AddFormattedText(TypeName, TextFormat.Bold); //Exam Name
        //                    row.Cells[k + 1].MergeRight = 3;
        //                    row.Cells[k + 1].Format.Alignment = ParagraphAlignment.Center;
        //                }

        //                else if (j == 3)
        //                {
        //                    TypeName = "TERM II";
        //                    row.Cells[k + 1].AddParagraph().AddFormattedText(TypeName, TextFormat.Bold); //Exam Name
        //                    row.Cells[k + 1].MergeRight = 3;
        //                    row.Cells[k + 1].Format.Alignment = ParagraphAlignment.Center;
        //                }

        //                TempVal = 1;

        //            }

        //            row.Cells[9].AddParagraph().AddFormattedText("TERM I & TERM II", TextFormat.Bold);
        //            row.Cells[9].MergeRight = 2;
        //            row = tb.AddRow();

        //            TempVal = 0;
        //            k = 0;
        //            string tempName = "";
        //            row[0].Column[0].MergeDown = 1;


        //            for (int j = 0; j < _ExamDetails.Length; j++)
        //            {
        //                k++;
        //                row.Cells[k].AddParagraph().AddFormattedText(_ExamDetails[j].Name, TextFormat.Bold);

        //                tempName = tempName + _ExamDetails[j].Name;

        //                if (j != 2 && j != 5)
        //                    tempName = tempName + " + ";

        //                if (j == 2 || j == 5)
        //                {

        //                    row.Cells[k + 1].AddParagraph().AddFormattedText(tempName, TextFormat.Bold);
        //                    tempName = "";
        //                    k++;
        //                }

        //            }
        //            row.Cells[9].AddParagraph().AddFormattedText("FA1 + FA2 + FA3 + FA4", TextFormat.Bold);

        //            row.Cells[10].AddParagraph().AddFormattedText("SA1 + SA2", TextFormat.Bold);

        //            row.Cells[11].AddParagraph().AddFormattedText("OVER ALL GRADE", TextFormat.Bold);
        //            row.Cells[11].Format.Alignment = ParagraphAlignment.Center;
        //        }

        //        //row = tb.AddRow();


        //        // calculate and enter marks 

        //        DataSet Grade = GetGradeDataSet(_ExamDetails[_ExamDetails.Length - 1].GradeMasterId);
        //        // double[] rowWiseMarkSum = new double[_ExamDetails.Length]; // this is used for save marks added with ratios

        //        //   double[] rowWiseMaxSum = new double[_ExamDetails.Length];

        //        double[] colWiseTotalMarkSum = new double[_ExamDetails.Length];//  this is used for save subject wise mark

        //        double[] colWiseTotalMaxSum = new double[_ExamDetails.Length];
        //        double[] colWiseTotalMarkSum1 = new double[_ExamDetails.Length];//  this is used for save subject wise mark

        //        double[] colWiseTotalMaxSum1 = new double[_ExamDetails.Length];

        //        double rowWiseSubjectMark = 0;//this two variable using  for FA1+FA2+SA1 and FA3+FA4+SA2
        //        double rowWiseSubjectMax = 0;

        //        double rowWiseFAexamMark = 0;// this two variable using for FA1+FA2+FA3+FA4
        //        double rowWiseFAexamMax = 0;

        //        double rowWiseSAExamMark = 0;//this two variable using for SA1+SA2
        //        double rowWiseSAExamMax = 0;

        //        double Ratio = 0;
        //        double MainRatio = 0;
        //        double subRatio = 0;
        //        double FARatio = 0;
        //        double SARatio = 0;
        //        double Term1Ratio = 0;
        //        double Term2Ratio = 0;


        //        for (int Subjct = 0; Subjct < _SubDetails.Length; Subjct++)
        //        {
        //            row = tb.AddRow();
        //            row.Cells[0].AddParagraph().AddFormattedText(_SubDetails[Subjct].Name); //Subject Name
        //            int k = 0;

        //            for (int ExamCount = 0; ExamCount < _ExamDetails.Length; ExamCount++)
        //            {
        //                double r_Mark = 0;
        //                double r_Max = 0;
        //                k++;

        //                //grade
        //                string _Grade = "";
        //                double MaxMark = 1;

        //                string DefaultName = "";

        //                if (ClsObj[i].m_ExamMarks[ExamCount, Subjct].MaxMark.ToString() != "-1")
        //                {
        //                    MaxMark = MyExamMang.GetMaxMarkFromExamScheduleIdandSubjectId(_ExamDetails[ExamCount].Id, _SubDetails[Subjct].Id);
        //                    string sql = "SELECT tblcbseexamratiomap.RatioColumName from tblcbseexamratiomap where tblcbseexamratiomap.ExamName='" + _ExamDetails[ExamCount].Name + "'";
        //                    OdbcDataReader MyReader = MyClassMngr.m_MysqlDb.ExecuteQuery(sql);

        //                    if (MyReader.HasRows)
        //                        DefaultName = MyReader.GetValue(0).ToString();

        //                    _Grade = GetGradeFromMarks(Grade, totalMarkRatios, DefaultName, double.Parse(ClsObj[i].m_ExamMarks[ExamCount, Subjct].MaxMark.ToString()), MaxMark, out r_Mark, out  r_Max);
        //                }

        //                double mark = (double.Parse(ClsObj[i].m_ExamMarks[ExamCount, Subjct].MaxMark.ToString()) / MaxMark) * 100;

        //                // rowWiseMarkSum[ExamCount] = rowWiseMarkSum[ExamCount] + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, mark);


        //                // rowWiseMaxSum[ExamCount] = rowWiseMaxSum[ExamCount] + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, 100);

        //                colWiseTotalMarkSum[ExamCount] = colWiseTotalMarkSum[ExamCount] + double.Parse(ClsObj[i].m_ExamMarks[ExamCount, Subjct].MaxMark.ToString());
        //                colWiseTotalMaxSum[ExamCount] = colWiseTotalMaxSum[ExamCount] + MaxMark;



        //                row.Cells[k].AddParagraph().AddFormattedText(_Grade);

        //                //rowWiseMarkSum[ExamCount] = r_Mark;
        //                // rowWiseMaxSum[ExamCount] = r_Max;

        //                rowWiseSubjectMark = rowWiseSubjectMark + GetMarkRations(totalMarkRatios, DefaultName, mark, out Ratio);
        //                rowWiseSubjectMax = rowWiseSubjectMax + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, 100, out Ratio);
        //                if (Subjct == 0)
        //                {
        //                    MainRatio = MainRatio + Ratio;

        //                }
        //                subRatio = subRatio + Ratio;

        //                if (DefaultName != "SA1" && DefaultName != "SA2")
        //                {
        //                    rowWiseFAexamMark = rowWiseFAexamMark + GetMarkRations(totalMarkRatios, DefaultName, mark, out Ratio);
        //                    rowWiseFAexamMax = rowWiseFAexamMax + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, 100, out Ratio);
        //                    if (Subjct == 0)
        //                    {
        //                        FARatio = FARatio + Ratio;

        //                    }
        //                }
        //                else if (DefaultName == "SA1" || DefaultName == "SA2")
        //                {
        //                    rowWiseSAExamMark = rowWiseSAExamMark + GetMarkRations(totalMarkRatios, DefaultName, mark, out Ratio);
        //                    rowWiseSAExamMax = rowWiseSAExamMax + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, 100, out Ratio);
        //                    if (Subjct == 0)
        //                    {
        //                        SARatio = SARatio + Ratio;

        //                    }

        //                }

        //                colWiseTotalMarkSum1[ExamCount] = colWiseTotalMarkSum1[ExamCount] + GetMarkRations(totalMarkRatios, DefaultName, mark, out Ratio);
        //                colWiseTotalMaxSum1[ExamCount] = colWiseTotalMaxSum1[ExamCount] + GetMarkRations(totalMarkRatios, _ExamDetails[ExamCount].Name, 100, out Ratio);

        //                if (ExamCount == 2 || ExamCount == 5)
        //                {

        //                    if (ExamCount == 2 && Subjct == 0)
        //                    {
        //                        Term1Ratio = subRatio;

        //                    }
        //                    else if (ExamCount == 5 && Subjct == 0)
        //                    {
        //                        Term2Ratio = subRatio;

        //                    }

        //                    double tempMark = 0;

        //                    if (subRatio > 1)
        //                    {
        //                        tempMark = (rowWiseSubjectMark / 100);
        //                        tempMark = (tempMark / subRatio) * 100;
        //                    }
        //                    else
        //                        tempMark = (rowWiseSubjectMark / rowWiseSubjectMax) * 100;

        //                    _Grade = GetGradeFromAvg(Grade, tempMark);
        //                    // _Grade = GetGradeFromMarks(Grade, null, "", rowWiseSubjectMark, rowWiseSubjectMax, out r_Mark, out  r_Max);

        //                    k = k + 1;

        //                    row.Cells[k].AddParagraph().AddFormattedText(_Grade);

        //                    rowWiseSubjectMark = 0;
        //                    rowWiseSubjectMax = 0;
        //                    subRatio = 0;
        //                }

        //            }

        //            string last_Grade = "";
        //            double t_mark = 0, t_Max = 0, Marks = 0;
        //            if (_ExamDetails.Length == 6)
        //            {


        //                //  Marks = (rowWiseFAexamMark / rowWiseFAexamMax) * 100;

        //                if (FARatio > 1)
        //                {
        //                    Marks = (rowWiseFAexamMark / 100);
        //                    Marks = (Marks / FARatio) * 100;
        //                }
        //                else
        //                    Marks = (rowWiseFAexamMark / rowWiseFAexamMax) * 100;


        //                last_Grade = GetGradeFromAvg(Grade, Marks);
        //                //last_Grade = GetGradeFromMarks(Grade, null, "", rowWiseFAexamMark, rowWiseFAexamMax, out t_mark, out  t_Max);
        //                row.Cells[9].AddParagraph().AddFormattedText(last_Grade);

        //                // Marks = (rowWiseSAExamMark / rowWiseSAExamMax) * (100);

        //                if (SARatio > 1)
        //                {
        //                    Marks = (rowWiseSAExamMark / 100);
        //                    Marks = (Marks / SARatio) * 100;
        //                }
        //                else
        //                    Marks = (rowWiseSAExamMark / rowWiseSAExamMax) * (100);

        //                last_Grade = GetGradeFromAvg(Grade, Marks);

        //                //last_Grade = GetGradeFromMarks(Grade, null, "", rowWiseSAExamMark, rowWiseSAExamMax, out t_mark, out  t_Max);
        //                row.Cells[10].AddParagraph().AddFormattedText(last_Grade);

        //                double _totalMark = rowWiseFAexamMark + rowWiseSAExamMark;
        //                //  double _totalMax = rowWiseFAexamMax + rowWiseSAExamMax;



        //                if (MainRatio > 1)

        //                    Marks = (_totalMark / MainRatio);
        //                else
        //                {
        //                    Marks = (_totalMark / _ExamDetails.Length * 100) * 100;
        //                }

        //                last_Grade = GetGradeFromAvg(Grade, Marks);

        //                // last_Grade = GetGradeFromMarks(Grade, null, "", _totalMark, _totalMax, out t_mark, out  t_Max);
        //                row.Cells[11].AddParagraph().AddFormattedText(last_Grade);
        //            }

        //            rowWiseFAexamMark = 0;
        //            rowWiseSAExamMark = 0;
        //            rowWiseFAexamMax = 0;
        //            rowWiseSAExamMax = 0;



        //        }

        //        row = tb.AddRow();
        //        row.Cells[0].AddParagraph().AddFormattedText("TOTAL", TextFormat.Bold); //Subject Name
        //        int m = 0;

        //        double TempMark = 0;
        //        for (int ExamCount = 0; ExamCount < _ExamDetails.Length; ExamCount++)
        //        {
        //            m++;
        //            string last_Grade = "";
        //            double t_mark = 0, t_Max = 0;
        //            //colWiseTotalMarkSum
        //            string _ExamDefaultName = "";
        //            string sql = "SELECT tblcbseexamratiomap.RatioColumName from tblcbseexamratiomap where tblcbseexamratiomap.ExamName='" + _ExamDetails[ExamCount].Name + "'";
        //            OdbcDataReader MyReader = MyClassMngr.m_MysqlDb.ExecuteQuery(sql);
        //            if (MyReader.HasRows)
        //                _ExamDefaultName = MyReader.GetValue(0).ToString();

        //            last_Grade = GetGradeFromMarks(Grade, null, "", colWiseTotalMarkSum[ExamCount], colWiseTotalMaxSum[ExamCount], out t_mark, out  t_Max);
        //            row.Cells[m].AddParagraph().AddFormattedText(last_Grade);

        //            // rowWiseMarkSum[ExamCount] =  colWiseTotalMarkSum[ExamCount];
        //            // rowWiseMaxSum[ExamCount] = colWiseTotalMaxSum[ExamCount];

        //            TempMark = (colWiseTotalMarkSum[ExamCount] / colWiseTotalMaxSum[ExamCount]) * 100;
        //            rowWiseSubjectMark = rowWiseSubjectMark + GetMarkRations(totalMarkRatios, _ExamDefaultName, TempMark, out Ratio);
        //            rowWiseSubjectMax = rowWiseSubjectMax + colWiseTotalMaxSum1[ExamCount];

        //            if (_ExamDefaultName != "SA1" && _ExamDefaultName != "SA2")
        //            {
        //                rowWiseFAexamMark = rowWiseFAexamMark + GetMarkRations(totalMarkRatios, _ExamDefaultName, TempMark, out Ratio);


        //            }
        //            else if (_ExamDefaultName == "SA1" || _ExamDefaultName == "SA2")
        //            {
        //                rowWiseSAExamMark = rowWiseSAExamMark + GetMarkRations(totalMarkRatios, _ExamDefaultName, TempMark, out Ratio);

        //            }

        //            if (ExamCount == 2 || ExamCount == 5)
        //            {
        //                double Ratio1 = 0;
        //                if (ExamCount == 2)
        //                    Ratio1 = Term1Ratio;
        //                else if (ExamCount == 5)
        //                    Ratio1 = Term2Ratio;
        //                double Mark = 0;

        //                if (Ratio1 > 1)
        //                {
        //                    Mark = rowWiseSubjectMark / 100;
        //                    Mark = (Mark / Ratio1) * 100;
        //                }
        //                else
        //                    Mark = (rowWiseSubjectMark / 300) * 100;

        //                last_Grade = GetGradeFromAvg(Grade, Mark);

        //                //last_Grade = GetGradeFromMarks(Grade, null, "", rowWiseSubjectMark, rowWiseSubjectMax, out t_mark, out  t_Max);

        //                m = m + 1;

        //                row.Cells[m].AddParagraph().AddFormattedText(last_Grade);

        //                rowWiseSubjectMark = 0; rowWiseSubjectMax = 0;

        //            }


        //        }

        //        string total = "";
        //        if (_ExamDetails.Length == 6)
        //        {
        //            double Marks1 = 0;


        //            if (FARatio > 1)
        //            {
        //                Marks1 = rowWiseFAexamMark / 100;
        //                Marks1 = (Marks1 / FARatio) * 100;

        //            }
        //            else
        //            {
        //                Marks1 = (rowWiseFAexamMark / rowWiseFAexamMax) * 100;
        //            }
        //            total = GetGradeFromAvg(Grade, Marks1);

        //            row.Cells[9].AddParagraph().AddFormattedText(total);



        //            if (SARatio > 1)
        //            {
        //                Marks1 = rowWiseSAExamMark / 100;
        //                Marks1 = (Marks1 / SARatio) * 100;
        //            }
        //            else
        //            {

        //                Marks1 = (rowWiseSAExamMark / rowWiseSAExamMax) * 100;
        //            }

        //            total = GetGradeFromAvg(Grade, Marks1);
        //            row.Cells[10].AddParagraph().AddFormattedText(total);

        //            double total_Marks = rowWiseFAexamMark + rowWiseSAExamMark;
        //            if (MainRatio > 1)
        //            {
        //                total_Marks = (total_Marks / MainRatio);
        //            }
        //            else
        //            {
        //                total_Marks = (total_Marks / (_ExamDetails.Length * 100)) * 100;
        //            }

        //            total = GetGradeFromAvg(Grade, total_Marks);

        //            // total = GetGradeFromMarks(Grade, null, "", toal_Marks, total_MaxMark, out total_mark, out  total_Max);
        //            row.Cells[11].AddParagraph().AddFormattedText(total);



        //        }
        //        tb.Borders.Visible = true;
        //        cel.Elements.Add(tb);

        //        // Co-curricular activiteis



        //        if (CC_ExamDetails.Length > 0)
        //        {
        //            paragraph = cel.AddParagraph();
        //            paragraph.Format.Alignment = ParagraphAlignment.Left;
        //            paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //            paragraph.AddLineBreak();
        //            paragraph.AddFormattedText("Part 2: Co-Scholastic Area", TextFormat.Underline | TextFormat.Bold);


        //            tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //            tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //            tb.AddColumn((PgSt.PageWidth - 80) * 5 / 15);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 15);
        //            tb.AddColumn((PgSt.PageWidth - 80) / 15);
        //            tb.AddColumn((PgSt.PageWidth - 80) * 8 / 15);

        //            string _TgroupName = "";
        //            int Temp = 0, temGp = 0;

        //            DataSet CC_Grade = GetGradeDataSet(CC_ExamDetails[CC_ExamDetails.Length - 1].GradeMasterId);

        //            for (int Subjct = 0; Subjct < CC_SubDetails.Length; Subjct++)
        //            {

        //                if (_TgroupName != CC_SubDetails[Subjct].GroupName)
        //                {
        //                    if (Temp > 0)
        //                    {

        //                        tb.Borders.Visible = true;
        //                        cel.Elements.Add(tb);
        //                        paragraph = cel.AddParagraph();
        //                        paragraph.Format.Alignment = ParagraphAlignment.Left;
        //                        paragraph.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);
        //                        paragraph.AddLineBreak();

        //                        if (CC_SubDetails[Subjct].GroupName.StartsWith("3") && temGp == 0)
        //                        {
        //                            temGp = 1;
        //                            paragraph.AddFormattedText("Part 3: Co-Scholastic Activities", TextFormat.Underline | TextFormat.Bold);

        //                        }


        //                        tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //                        tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //                        tb.AddColumn((PgSt.PageWidth - 80) * 5 / 15);
        //                        tb.AddColumn((PgSt.PageWidth - 80) / 15);
        //                        tb.AddColumn((PgSt.PageWidth - 80) / 15);
        //                        tb.AddColumn((PgSt.PageWidth - 80) * 8 / 15);


        //                    }

        //                    row = tb.AddRow();

        //                    _TgroupName = CC_SubDetails[Subjct].GroupName;

        //                    row.Cells[0].AddParagraph().AddFormattedText(CC_SubDetails[Subjct].GroupName, TextFormat.Bold);

        //                    for (int ExamCount = 0; ExamCount < CC_ExamDetails.Length; ExamCount++)
        //                    {

        //                        string Name = "";
        //                        if (CC_ExamDetails[ExamCount].Name.Length > 8)
        //                            Name = CC_ExamDetails[ExamCount].Name.Substring(0, 7);
        //                        else
        //                            Name = CC_ExamDetails[ExamCount].Name;

        //                        row.Cells[ExamCount + 1].AddParagraph().AddFormattedText(Name, TextFormat.Bold); //Exam Name            
        //                        row.Cells[ExamCount + 1].Format.Alignment = ParagraphAlignment.Center;

        //                    }
        //                    row.Cells[3].AddParagraph().AddFormattedText("Descriptive Indicators", TextFormat.Bold); //Exam Name
        //                    row.Cells[3].Format.Alignment = ParagraphAlignment.Center;


        //                }




        //                row = tb.AddRow();

        //                row.Cells[0].AddParagraph().AddFormattedText(CC_SubDetails[Subjct].Name); //Subject Name

        //                double temp_RatioMark = 0, tempRatioMax = 0;
        //                for (int ExamCount = 0; ExamCount < CC_ExamDetails.Length; ExamCount++)
        //                {
        //                    if (ClsObj[i].m_CC_ExamMarks[ExamCount, Subjct].MaxMark.ToString() != "-1")
        //                    {
        //                        double max_Mark = MyExamMang.GetMaxMarkFromExamScheduleIdandSubjectId(CC_ExamDetails[ExamCount].Id, CC_SubDetails[Subjct].Id);


        //                        string _Grade = GetGradeFromMarks(CC_Grade, null, "", double.Parse(ClsObj[i].m_CC_ExamMarks[ExamCount, Subjct].MaxMark.ToString()), max_Mark, out temp_RatioMark, out tempRatioMax);

        //                        row.Cells[ExamCount + 1].AddParagraph().AddFormattedText(_Grade);
        //                    }
        //                    else
        //                        row.Cells[ExamCount + 1].AddParagraph().AddFormattedText("-");

        //                }

        //                row.Cells[2].AddParagraph().AddFormattedText(CC_SubDetails[Subjct].Desc);//sub description
        //                //row = tb.AddRow();
        //                Temp++;

        //            }
        //            tb.Borders.Visible = true;
        //            cel.Elements.Add(tb);

        //        }



        //        // dominic new 

        //        paragraph = cel.AddParagraph();
        //        paragraph.Format.Alignment = ParagraphAlignment.Left;
        //        paragraph.AddLineBreak();
        //        paragraph.AddLineBreak();


        //        tb = new MigraDoc.DocumentObjectModel.Tables.Table();

        //        tb.Format.Font = new MigraDoc.DocumentObjectModel.Font("Calibri", 10);

        //        tb.AddColumn((PgSt.PageWidth - 80) * 5 / 15);
        //        tb.AddColumn((PgSt.PageWidth - 80) * 5 / 15);
        //        tb.AddColumn((PgSt.PageWidth - 80) * 5 / 15);



        //        row = tb.AddRow();
        //        row.Cells[0].AddParagraph().AddFormattedText("CLASS TEACHER", TextFormat.Bold);
        //        row.Cells[1].AddParagraph().AddFormattedText("PRINCIPAL", TextFormat.Bold); //Exam Name            
        //        row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //        row.Cells[2].AddParagraph().AddFormattedText("PARENT", TextFormat.Bold); //Exam Name
        //        row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

        //        row = tb.AddRow();
        //        tb.Borders.Visible = false;
        //        cel.Elements.Add(tb);



        //    }
        //    return document;
        //}


        #endregion

        //#region Functions used for Create report

        //private double GetMarkRatios(DataSet totalMarkRatios, string ExamName, double Mark)
        //{
        //    double _Mark = 0;

        //    if (totalMarkRatios.Tables[0].Rows.Count > 0)
        //    {

        //        _Mark = Mark * double.Parse(totalMarkRatios.Tables[0].Rows[0][ExamName].ToString());
        //    }
        //    return _Mark;
        //}

        //private double GetMarkRations(DataSet totalMarkRatios, string ExamName, double Mark, out double Ratio)
        //{
        //    double _Mark = 0;
        //    Ratio = 0;
        //    string sql = "SELECT tblcbseexamratiomap.RatioColumName from tblcbseexamratiomap where tblcbseexamratiomap.ExamName='" + ExamName + "'";
        //    OdbcDataReader MyReader = MyClassMngr.m_MysqlDb.ExecuteQuery(sql);

        //    if (MyReader.HasRows)
        //    {
        //        if (totalMarkRatios.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow Dr in totalMarkRatios.Tables[0].Rows)
        //            {

        //                double.TryParse(Dr[MyReader.GetValue(0).ToString()].ToString(), out Ratio);

        //                if (Ratio > 0)
        //                    _Mark = Mark * double.Parse(Dr[MyReader.GetValue(0).ToString()].ToString());
        //                else
        //                    _Mark = Mark;

        //            }
        //        }
        //    }
        //    else
        //    {
        //        return 1;
        //    }

        //    return _Mark;
        //}

        //private string GetGradeFromAvg(DataSet Grade, double _Mark)
        //{



        //    foreach (DataRow Dr in Grade.Tables[0].Rows)
        //    {
        //        if (_Mark >= double.Parse(Dr[1].ToString()))
        //        {
        //            return Dr[0].ToString();
        //        }
        //    }


        //    return "";

        //}


        //private string GetGradeFromMarks(DataSet Grade, DataSet totalMarkRatios, string ExamName, double _Mark, double MaxMark, out double Ratio_Mark, out double Ratio_Max_Mark)
        //{
        //    Ratio_Max_Mark = 0;
        //    Ratio_Mark = 0;
        //    double ratio = 0;
        //    if (ExamName != "")
        //    {
        //        if (totalMarkRatios.Tables[0].Rows.Count > 0)
        //        {
        //            if (double.Parse(totalMarkRatios.Tables[0].Rows[0][ExamName].ToString()) < 1)
        //                ratio = 1;
        //            else
        //                ratio = double.Parse(totalMarkRatios.Tables[0].Rows[0][ExamName].ToString());

        //            _Mark = _Mark * ratio;
        //            MaxMark = MaxMark * ratio;

        //        }
        //        Ratio_Max_Mark = MaxMark;
        //        Ratio_Mark = _Mark;
        //    }
        //    double avg = (_Mark / MaxMark) * 100;

        //    foreach (DataRow Dr in Grade.Tables[0].Rows)
        //    {
        //        if (avg >= double.Parse(Dr[1].ToString()))
        //        {
        //            return Dr[0].ToString();
        //        }
        //    }


        //    return "";

        //}

        //private DataSet GetGradeDataSet(int _GradeMasterId)
        //{
        //    string _sql = "select tblgrade.Grade, tblgrade.LowerLimit from tblgrade where";
        //    if (_GradeMasterId > 0)
        //        _sql = _sql + " tblgrade.GradeMasterId=" + _GradeMasterId + " and ";
        //    _sql = _sql + "  tblgrade.`Status`=1   order by tblgrade.id asc";
        //    DataSet Dt = MyExamMang.m_MysqlDb.ExecuteQueryReturnDataSet(_sql);
        //    return Dt;
        //}

        //#endregion

        //#region Create and Open PDF File

        //private void CreateReport(List<CBSCFinalReportClass> ClsObj, ExamNode[] _ExamDetails, ExamNode[] _SubDetails, ExamNode[] CC_ExamDetails, ExamNode[] CC_SubDetails, SchoolDetails _SchoolDetails, int _StudId, DataSet totalMarkRatios)
        //{
        //    string _PhysicalPath = WinerUtlity.GetAbsoluteFilePath(objSchool, Server.MapPath(""));
        //    //int _StudentId = 0;
        //    Document document = LoadPDFPage(ClsObj, _ExamDetails, _SubDetails, _SchoolDetails, CC_ExamDetails, CC_SubDetails, totalMarkRatios);

        //    const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

        //    // ----------------------------------------------------------------------------------------

        //    const bool unicode = false;

        //    // Create a renderer for the MigraDoc document.
        //    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

        //    // Associate the MigraDoc document with a renderer
        //    pdfRenderer.Document = document;

        //    // Layout and render document to PDF


        //    pdfRenderer.RenderDocument();

        //    // Save the document...
        //    string filename = "", MainName = "";

        //    if (_StudId == 0)
        //        MainName = Drp_SelectClass.SelectedItem.ToString();
        //    else
        //        MainName = Drp_SelectStudent.SelectedItem.ToString();

        //    filename = _PhysicalPath + "\\PDF_Files\\CBSC_Final_" + MainName + ".pdf";

        //    pdfRenderer.PdfDocument.Save(filename);
        //    // ...and start a viewer.
        //    ScriptManager.RegisterClientScriptBlock(this.pnlAjaxUpdaet, this.pnlAjaxUpdaet.GetType(), "null", "window.open(\"OpenPdfPage.aspx?PdfName=CBSC_Final_" + MainName + ".pdf\",'Info','status=1, width=1000, height=700,,resizable=1,scrollbars=1');", true);

        //    //ClientScript.RegisterClientScriptBlock(this.GetType(), "keyClientBlock", "window.open(\"OpenPdfPage.aspx?PdfName=PRC_" + MainName + ".pdf\");", true);
        //    // Response.Redirect("OpenPdfPage.aspx?PdfName=PRC_" + MainName + ".pdf", false);

        //}


        //#endregion

        //#region Read Base Details From DB

        //private DataSet GetMarkRatios(string _selectedClassId)
        //{
        //    string stdId = MyClassMngr.GetStandardIdfromClassId(_selectedClassId);
        //    string sqlRation = "select FA1,FA2,SA1,FA3,FA4,SA2 from tblcbsegraderatio where tblcbsegraderatio.StandardId=" + stdId;
        //    DataSet Dt = MyExamMang.m_MysqlDb.ExecuteQueryReturnDataSet(sqlRation);
        //    return Dt;
        //}

        //private DataSet GetAllStudentListFromClass(int StudentId, int ClassId)
        //{

        //    DataSet _StdentSet = new DataSet();
        //    DataTable dt;
        //    DataRow dr;

        //    _StdentSet.Tables.Add(new DataTable("Student"));

        //    dt = _StdentSet.Tables["Student"];
        //    dt.Columns.Add("Id");
        //    dt.Columns.Add("Name");
        //    dt.Columns.Add("RollNum");

        //    dt.Columns.Add("DOB");
        //    dt.Columns.Add("Class");
        //    dt.Columns.Add("AdmissionNum");
        //    dt.Columns.Add("FatherName");
        //    dt.Columns.Add("MotherName");
        //    dt.Columns.Add("Tel");
        //    dt.Columns.Add("Add");

        //    string sql_Student = "select tblview_student.Id, tblview_student.StudentName, tblview_student.RollNo, tblclass.ClassName,date_Format(tblview_student.DOB,'%d/%m/%Y') as DOB , tblview_student.AdmitionNo,tblview_student.GardianName,tblview_student.MothersName, tblview_student.ResidencePhNo ,tblview_student.Address from tblview_student inner join tblclass on tblclass.Id= tblview_student.ClassId inner join tblstudentclassmap on tblstudentclassmap.StudentId= tblview_student.Id  where tblview_student.ClassId=" + ClassId + " and tblstudentclassmap.BatchId=" + MyUser.CurrentBatchId;

        //    if (StudentId != 0)
        //        sql_Student = sql_Student + " and tblview_student.Id=" + StudentId;



        //    DataSet _studList = MyExamMang.m_MysqlDb.ExecuteQueryReturnDataSet(sql_Student);


        //    foreach (DataRow dr_values in _studList.Tables[0].Rows)
        //    {

        //        dr = _StdentSet.Tables["Student"].NewRow();

        //        dr["Id"] = dr_values["Id"];
        //        dr["Name"] = dr_values["StudentName"];
        //        dr["RollNum"] = dr_values["RollNo"];
        //        dr["DOB"] = dr_values["DOB"];
        //        dr["Class"] = dr_values["ClassName"];
        //        dr["AdmissionNum"] = dr_values["AdmitionNo"];
        //        dr["FatherName"] = dr_values["GardianName"];
        //        dr["MotherName"] = dr_values["MothersName"];
        //        dr["Tel"] = dr_values["ResidencePhNo"];
        //        dr["Add"] = dr_values["Address"];

        //        _StdentSet.Tables["Student"].Rows.Add(dr);

        //    }
        //    return _StdentSet;


        //}

        //private void GetSubDetails(int ClassId, string ExamType, out ExamNode[] _SubDetails)
        //{
        //    //string sql = "SELECT distinct tblsubjects.Id,tblsubjects.subject_name, tblclassexamsubmap.MaxMark from tblsubjects inner JOIN tblexammark on tblexammark.SubjectId=tblsubjects.Id inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId inner join tblclassexam on tblclassexam.Id = tblexamschedule.ClassExamId inner join tblclassexamsubmap on tblclassexamsubmap.ClassExamId=tblexamschedule.ClassExamId and tblclassexamsubmap.SubId= tblsubjects.Id inner join tblexammaster on tblexammaster.Id= tblclassexam.ExamId  where tblexamschedule.BatchId=" + MyUser.CurrentBatchId + " and tblclassexam.ClassId=" + ClassId + " and tblexammaster.Id=" + ExamId;
        //    //string sql = "SELECT distinct tblsubjects.Id,tblsubjects.subject_name, tblclassexamsubmap.MaxMark ,tblsubjects.sub_description from tblsubjects inner JOIN tblexammark on tblexammark.SubjectId=tblsubjects.Id  inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId   inner join tblclassexam on tblclassexam.Id = tblexamschedule.ClassExamId   inner join tblclassexamsubmap on tblclassexamsubmap.ClassExamId=tblexamschedule.ClassExamId   and tblclassexamsubmap.SubId= tblsubjects.Id inner join tblexammaster on tblexammaster.Id= tblclassexam.ExamId     where tblexamschedule.BatchId=" + MyUser.CurrentBatchId + "   and tblexammaster.Id     in (select tblexammaster.id from tblsubject_type  inner join tblexammaster on tblexammaster.ExamTypeId=tblsubject_type.Id  inner join tblclassexam on tblclassexam.ExamId = tblexammaster.Id   inner join tblexamschedule on tblexamschedule.ClassExamId = tblclassexam.id    where tblclassexam.ClassId=" + ClassId + "  and   tblsubject_type.TypeDisc='" + ExamType + "' and tblexamschedule.Status='Completed'  order by tblexamschedule.id )";
        //    string sql = " SELECT distinct tblsubjects.Id,tblsubjects.subject_name,tblsubjects.sub_description from tblsubjects inner JOIN tblexammark on tblexammark.SubjectId=tblsubjects.Id  inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId   inner join tblclassexam on tblclassexam.Id = tblexamschedule.ClassExamId   inner join tblclassexamsubmap on tblclassexamsubmap. ClassExamId=tblexamschedule.ClassExamId   and tblclassexamsubmap.SubId= tblsubjects.Id inner join tblexammaster on tblexammaster.Id=  .tblclassexam.ExamId Inner join tblsubject_type on  tblexammaster.ExamTypeId=tblsubject_type.Id   where tblexamschedule.BatchId=" + MyUser.CurrentBatchId + "  and tblclassexam.ClassId=" + ClassId + "  and     tblsubject_type.TypeDisc='CBSE MAIN REPORT' and tblexamschedule.Status='Completed'   order by tblexammark.SubjectOrder ";

        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);
        //    if (MyReader.HasRows)
        //    {
        //        int i = 0;
        //        int Count = MyReader.RecordsAffected;
        //        _SubDetails = new ExamNode[Count];

        //        while (MyReader.Read())
        //        {
        //            _SubDetails[i].Id = int.Parse(MyReader.GetValue(0).ToString());
        //            _SubDetails[i].Name = MyReader.GetValue(1).ToString();

        //            _SubDetails[i].Desc = MyReader.GetValue(2).ToString();
        //            i++;
        //        }
        //    }
        //    else
        //    {
        //        _SubDetails = new ExamNode[0];
        //    }

        //}

        //private void GetCC_SubDetails(int ClassId, string ExamType, out ExamNode[] CC_SubDetails)
        //{
        //    //string sql = "SELECT distinct tblsubjects.Id,tblsubjects.subject_name, tblclassexamsubmap.MaxMark from tblsubjects inner JOIN tblexammark on tblexammark.SubjectId=tblsubjects.Id inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId inner join tblclassexam on tblclassexam.Id = tblexamschedule.ClassExamId inner join tblclassexamsubmap on tblclassexamsubmap.ClassExamId=tblexamschedule.ClassExamId and tblclassexamsubmap.SubId= tblsubjects.Id inner join tblexammaster on tblexammaster.Id= tblclassexam.ExamId  where tblexamschedule.BatchId=" + MyUser.CurrentBatchId + " and tblclassexam.ClassId=" + ClassId + " and tblexammaster.Id=" + ExamId;
        //    string sql = "SELECT distinct tblsubjects.Id,tblsubjects.subject_name, tblsubjects.sub_description , tblsubjects.sub_Catagory, tbltime_subgroup.Name from tblsubjects inner JOIN tblexammark on tblexammark.SubjectId=tblsubjects.Id  inner join tblexamschedule on tblexamschedule.Id=tblexammark.ExamSchId   inner join tblclassexam on tblclassexam.Id = tblexamschedule.ClassExamId   inner join tblclassexamsubmap on tblclassexamsubmap.ClassExamId=tblexamschedule.ClassExamId   and tblclassexamsubmap.SubId= tblsubjects.Id inner join tblexammaster on tblexammaster.Id= tblclassexam.ExamId   inner join tbltime_subgroup on tbltime_subgroup.Id= tblsubjects.sub_Catagory where tblexamschedule.BatchId=" + MyUser.CurrentBatchId + "   and tblexammaster.Id     in (select tblexammaster.id from tblsubject_type  inner join tblexammaster on tblexammaster.ExamTypeId=tblsubject_type.Id  inner join tblclassexam on tblclassexam.ExamId = tblexammaster.Id   inner join tblexamschedule on tblexamschedule.ClassExamId = tblclassexam.id    where tblclassexam.ClassId=" + ClassId + "  and   tblsubject_type.TypeDisc='" + ExamType + "' and tblexamschedule.Status='Completed'   order by tblexammark.SubjectOrder  )";
        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);
        //    if (MyReader.HasRows)
        //    {
        //        int i = 0;
        //        int Count = MyReader.RecordsAffected;
        //        CC_SubDetails = new ExamNode[Count];

        //        while (MyReader.Read())
        //        {
        //            CC_SubDetails[i].Id = int.Parse(MyReader.GetValue(0).ToString());
        //            CC_SubDetails[i].Name = MyReader.GetValue(1).ToString();

        //            CC_SubDetails[i].Desc = MyReader.GetValue(2).ToString();
        //            CC_SubDetails[i].Group = int.Parse(MyReader.GetValue(3).ToString());
        //            CC_SubDetails[i].GroupName = MyReader.GetValue(4).ToString();

        //            i++;
        //        }
        //    }
        //    else
        //    {
        //        CC_SubDetails = new ExamNode[0];
        //    }

        //}

        //private void GetExamDetails(int ClassId, string ExamType, out ExamNode[] _ExamDetails)
        //{
        //    // string sql = "select tblexamschedule.id, tblexammaster.ExamName , tblperiod.Period from tblexammaster inner join tblclassexam on tblclassexam.ExamId = tblexammaster.Id inner join tblexamschedule on tblexamschedule.ClassExamId = tblclassexam.id inner join tblperiod on tblexamschedule.PeriodId = tblperiod.Id     where tblclassexam.ClassId=" + ClassId + "  and tblexamschedule.Id in (select tblstudentmark.ExamSchId from  tblstudentmark) and tblexammaster.Id=" + ExamId + " and tblexamschedule.Status='Completed'  order by tblexamschedule.id desc";
        //    string sql = "select tblexamschedule.id, tblexammaster.ExamName , tblperiod.Period, tblexamschedule.GradeMasterId from tblsubject_type  inner join tblexammaster on tblexammaster.ExamTypeId=tblsubject_type.Id   inner join tblclassexam on tblclassexam.ExamId = tblexammaster.Id inner join tblexamschedule on tblexamschedule.ClassExamId = tblclassexam.id inner join tblperiod on tblexamschedule.PeriodId = tblperiod.Id     where tblclassexam.ClassId=" + ClassId + "  and tblexamschedule.Id in (select tblstudentmark.ExamSchId from  tblstudentmark) and  tblsubject_type.TypeDisc='" + ExamType + "' and tblexamschedule.Status='Completed' order by tblexammaster.ExamOrder ";


        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);

        //    if (MyReader.HasRows)
        //    {
        //        int i = 0;
        //        int Count = MyReader.RecordsAffected;
        //        _ExamDetails = new ExamNode[Count];

        //        while (MyReader.Read())
        //        {
        //            _ExamDetails[i].Id = int.Parse(MyReader.GetValue(0).ToString());
        //            _ExamDetails[i].Name = MyReader.GetValue(1).ToString();
        //            _ExamDetails[i].GradeMasterId = int.Parse(MyReader.GetValue(3).ToString());
        //            string sql1 = "select Max( tblexammark.ExamDate) from tblexammark where tblexammark.ExamSchId=" + int.Parse(MyReader.GetValue(0).ToString());
        //            OdbcDataReader dr = MyExamMang.m_MysqlDb.ExecuteQuery(sql1);
        //            if (dr.HasRows)
        //            {

        //                _ExamDetails[i].Date = DateTime.Parse(dr.GetValue(0).ToString());
        //            }

        //            i++;
        //        }
        //    }
        //    else
        //    {
        //        _ExamDetails = new ExamNode[0];
        //    }
        //}

        //private void GetSchoolDetails(out SchoolDetails _SchoolDetails)
        //{
        //    _SchoolDetails.SchoolName = "";
        //    _SchoolDetails.Address = "";
        //    _SchoolDetails.LogoURL = "";

        //    string sql = "select tblschooldetails.SchoolName, tblschooldetails.Address, tblschooldetails.LogoUrl from tblschooldetails";
        //    OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);

        //    if (MyReader.HasRows)
        //    {
        //        _SchoolDetails.SchoolName = MyReader.GetValue(0).ToString();
        //        _SchoolDetails.Address = MyReader.GetValue(1).ToString();
        //        _SchoolDetails.LogoURL = MyReader.GetValue(2).ToString();
        //    }

        //}

        #endregion

        #region  Read User Entries

        private bool GatBaseValuese(out int classId, out int studentId)
        {
            classId = 0;
            studentId = 0;


            if (ValidEntries())
            {
                int.TryParse(Drp_SelectClass.SelectedValue, out classId);
                int.TryParse(Drp_SelectStudent.SelectedValue, out studentId);

                return true;
            }
            else
                return false;
        }
        public string getimagename(int studid, int classid)
        {
            string imgname = "";
            string Sql = "select tblfileurl.FilePath from tblfileurl where tblfileurl.UserId='" + studid + "' and  tblfileurl.Class =" + classid + "";
            OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(Sql);

            if (MyReader.HasRows)
            {
                imgname = MyReader.GetValue(0).ToString();

            }

            return imgname;

        }
        public int getclassid(string classname)
        {
            int classid = 0;
            string Sql = "select tblclass.Id from tblclass where tblclass.ClassName='" + classname + "' and  tblclass.Status=1";
            OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(Sql);

            if (MyReader.HasRows)
            {
                classid = int.Parse(MyReader.GetValue(0).ToString());
              
            }

            return classid;
        }

        private bool ValidEntries()
        {

            if (Drp_SelectClass.SelectedValue == "-1")
            {
                Lbl_Err.Text = "No class found.";
                return false;
            }
            //else if (Txt_ReportName.Text == "")
            //{
            //    Lbl_Err.Text = "Enter Report Name.";
            //    return false;
            //}
            else if (Drp_SelectStudent.SelectedValue == "-1")
            {
                Lbl_Err.Text = "No students exist in the class.";
                return false;
            }

            return true;

        }

        #endregion



        #region LoadMethos

        private void LoadDetails()
        {
            LoadClassDetailsToDropDown();

            LoadStudentsDetailsToDropDown();

        }

        private void LoadStudentsDetailsToDropDown()
        {
            if (Drp_SelectClass.SelectedValue != "-1")
            {
                int ClassId = 0;
                int.TryParse(Drp_SelectClass.SelectedValue.ToString(), out ClassId);

                Drp_SelectStudent.Items.Clear();
                string Sql = "select tblview_student.Id, tblview_student.StudentName from tblview_student INNER join tblclass on tblview_student.ClassId=tblclass.Id inner join tblstudentclassmap on tblstudentclassmap.StudentId=tblview_student.Id where tblview_student.ClassId=" + ClassId + " and  tblview_student.LIve=1  and tblview_student.RollNo<>-1  order by  tblview_student.StudentName ASC";
                OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(Sql);

                if (MyReader.HasRows)
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("All Student", "0");
                    Drp_SelectStudent.Items.Add(li);

                    while (MyReader.Read())
                    {
                        System.Web.UI.WebControls.ListItem Li = new System.Web.UI.WebControls.ListItem(MyReader.GetValue(1).ToString(), MyReader.GetValue(0).ToString());
                        Drp_SelectStudent.Items.Add(Li);
                    }
                }
                else
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("Student Not Found", "-1");
                    Drp_SelectStudent.Items.Add(li);
                }
            }
            else
            {

                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("All Student", "0");
                Drp_SelectStudent.Items.Add(li);
            }
        }

        private void LoadClassDetailsToDropDown()
        {
            Drp_SelectClass.Items.Clear();
            string sql = "SELECT tblclass.Id,tblclass.ClassName FROM tblclass where tblclass.Status =1 AND tblclass.Id IN (SELECT tblclass.Id from tblclass where tblclass.ParentGroupID IN (SELECT DISTINCT tblgroup.Id FROM tblgroup INNER JOIN tblgrouprelation ON tblgroup.Id=tblgrouprelation.ChildId INNER JOIN tblgroupusermap ON tblgrouprelation.ParentId = tblgroupusermap.GroupId WHERE  tblgroupusermap.UserId=" + MyUser.UserId + " UNION SELECT DISTINCT tblgroup.Id FROM tblgroup INNER JOIN tblgroupusermap ON tblgroup.Id = tblgroupusermap.GroupId WHERE  tblgroupusermap.UserId=" + MyUser.UserId + ")) ORDER BY tblclass.Standard,tblclass.ClassName";
            OdbcDataReader MyReader = MyExamMang.m_MysqlDb.ExecuteQuery(sql);

            if (MyReader.HasRows)
            {
                while (MyReader.Read())
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem(MyReader.GetValue(1).ToString(), MyReader.GetValue(0).ToString());
                    Drp_SelectClass.Items.Add(li);
                }
            }
            else
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem("No Class Found", "-1");
                Drp_SelectClass.Items.Add(li);
            }
        }



        private void LoadAllPreviousExamReportToGridNew(DataRow ExamDetails, DataSet SubjectDetails, int _ClassId, int _studId)
        {
            int i = 0, examCount = 0;

            ExamArray[,] _ExamArray = new ExamArray[SubjectDetails.Tables[0].Rows.Count, ExamDetails.Table.Rows.Count];

            //foreach (DataRow dr in ExamDetails.Tables[0].Rows)
            //{
                string Colmns = "";

                foreach (DataRow dr1 in SubjectDetails.Tables[0].Rows)
                {
                    if (int.Parse(dr1["SchId"].ToString()) == int.Parse(ExamDetails["Id"].ToString()))
                    {
                        if (Colmns != "") Colmns = Colmns + ",";


                        Colmns = Colmns + dr1["MarkColumn"];
                    }
                }
                string sql2 = "", sql3 = "";

                if (_ClassId > 0)
                {
                    if (_ClassId == MyUser.CurrentBatchId)
                    {
                        sql2 = "select " + Colmns + ", tblstudentmark.TotalMax , tblstudentmark.TotalMark , tblstudentmark.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark on tblstudentmark.StudId = tblstudentclassmap_history.StudentId where tblstudentmark.ExamSchId=" + ExamDetails["Id"] + " and tblstudentmark.StudId=" + _studId;
                    }
                    else
                    {
                        sql2 = "select " + Colmns + ", tblstudentmark_history.TotalMax , tblstudentmark_history.TotalMark , tblstudentmark_history.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark_history on tblstudentmark_history.StudId = tblstudentclassmap_history.StudentId where tblstudentmark_history.ExamSchId=" + ExamDetails["Id"] + " and tblstudentmark_history.StudId=" + _studId;
                    }
                }
                else
                {
                    sql3 = "select " + Colmns + ", tblstudentmark.TotalMax , tblstudentmark.TotalMark , tblstudentmark.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark on tblstudentmark.StudId = tblstudentclassmap_history.StudentId where tblstudentmark.ExamSchId=" + ExamDetails["Id"] + " and tblstudentmark.StudId=" + _studId;

                    sql2 = "select " + Colmns + ", tblstudentmark_history.TotalMax , tblstudentmark_history.TotalMark , tblstudentmark_history.`Avg`, Grade, Result, Rank  from tblstudentclassmap_history inner join tblstudentmark_history on tblstudentmark_history.StudId = tblstudentclassmap_history.StudentId where tblstudentmark_history.ExamSchId=" + ExamDetails["Id"] + " and tblstudentmark_history.StudId=" + _studId;

                    sql2 = "(" + sql2 + " ) union (" + sql3 + ")";
                }
                OdbcDataReader Reader1 = MyExamMang.m_MysqlDb.ExecuteQuery(sql2);
                if (Reader1.HasRows)
                {
                    double val = 0;



                    foreach (DataRow dr1 in SubjectDetails.Tables[0].Rows)
                    {

                        if (int.Parse(dr1["SchId"].ToString()) == int.Parse(ExamDetails["Id"].ToString()))
                        {
                            double.TryParse(Reader1[dr1["MarkColumn"].ToString()].ToString(), out val);
                            if (val > 0)
                            {
                                _ExamArray[i, examCount].Mark = (double.Parse(val.ToString(".")) / (double.Parse(dr1["MaxMark"].ToString())) * 100).ToString(".");
                                _ExamArray[i, examCount].ExamName = ExamDetails["ExamName"].ToString();
                                _ExamArray[i, examCount].ExamId = int.Parse(ExamDetails["Id"].ToString());
                                _ExamArray[i, examCount].SubjectId = int.Parse(dr1["SubjectId"].ToString());
                            }
                            else
                            {
                                _ExamArray[i, examCount].Mark = "";

                            }
                            i = i + 1;

                        }


                    }
                    i = 0;
                }
                examCount = examCount + 1;
            //}

            Session["ExamReportArray"] = _ExamArray;
            // Img_Export.Visible = true;
            // Pnl_ExamGraph.Visible = true;
            //this.MarkListArea.Visible = true;
            //this.MarkListArea1.Visible = true;
            //this.ExamNames.Visible = true;
            //Lbl_indexammsg.Text = "";
            //LoadConditionDropDownWithSubject();
            //LoadPerformanceGraphWithExamData();
        }
        #endregion
    }
}

