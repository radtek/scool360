﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WinBase;
using System.IO;
using System.Xml;
using System.Data.Odbc;
using WinEr;
using System.Configuration;



namespace Winer.Portal
{
    public partial class WinerPortalMaster : System.Web.UI.MasterPage
    {
        
       
        public MysqlClass _MysqlObj;
        private KnowinUser MyUser;
        private SchoolClass objSchool = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PortalUserObj"] == null)
            {
                Response.Redirect("sectionerr.htm");
            }
            MyUser = (KnowinUser)Session["PortalUserObj"];

            LoadDetails();
        }
        protected void LinkButton1_Click(object sender, EventArgs e)
        {

        }
        private void LoadDetails()
        {

            string SchoolDetails1 = "";
            // string FilePath = WinerUtlity.GetAbsoluteFilePath(objSchool, Server.MapPath(""));

            try
            {

                OdbcConnection _myOdbcConn = null;
                string m_stConnection = WinerUtlity.GetConnectionString(objSchool);
                _myOdbcConn = new OdbcConnection(m_stConnection);
                _myOdbcConn.Open();
                string sql = "select Name,Address,Logo,Status from tblorganization where Id = " + MyUser.m_orgId +" ";
                OdbcCommand m_cmd = new OdbcCommand(sql, _myOdbcConn);
                OdbcDataReader _myReader = m_cmd.ExecuteReader();

                if (_myReader.HasRows)
                {
                    if (_myReader.GetValue(0).ToString() != "")
                    {
                        //SchoolDetails1 =  _myReader.GetValue(0).ToString() + "<br>";



                        SchoolDetails1 = SchoolDetails1 + "<span style=\"Font-size:30px;color:##ffffff;\">" + _myReader.GetValue(0).ToString() + "</span><br>";


                        orgname.InnerHtml = SchoolDetails1;
                    }
                    else
                    {

                        SchoolDetails1 = ConfigurationSettings.AppSettings["SchoolName"];

                        orgname.InnerHtml = SchoolDetails1;

                    }

                    ImageUploaderClass imgobj = new ImageUploaderClass(objSchool);


                    
                    string Mimag1 = "<img src=\"" + "Handler/ImageReturnHandler.ashx?id=" + _myReader.GetValue(2) + "\" width=\"90%\" alt=\"\" />";
                    

                   //string Mimag = "<img src=\"" + "Handler/ImageReturnHandler.ashx?id=" + _myReader.GetValue(2) + "\" width=\"90%\" alt=\"\" />";

                    string Mimag = "<img src=img/winersmalllogo.png width=\"20%\" alt=\"\" />";

                   // orglogo.InnerHtml = Mimag;


                }
                else
                {
                    SchoolDetails1 = ConfigurationSettings.AppSettings["SchoolName"];

                    orgname.InnerHtml = SchoolDetails1;
                }
            }
            catch
            {
                SchoolDetails1 = ConfigurationSettings.AppSettings["SchoolName"];

                orgname.InnerHtml = SchoolDetails1;
            }

        }
    }
}