﻿<%@ Page Title="" Language="C#" MasterPageFile="~/WinerPortalMaster.Master" AutoEventWireup="true" CodeBehind="StudentReport.aspx.cs" Inherits="Winer.Portal.StudentReport" %>
<%@ Register
    Assembly="AjaxControlToolkit"
    Namespace="AjaxControlToolkit"
    TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" />
      <asp:UpdatePanel ID="pnlAjaxUpdaet" runat="server">
        <ContentTemplate>
            <div class="text-success">
                <h3>
                    Student Report</h3>
            </div>
            <div class="panel">
            </div>
           
           
                
                <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Left">
                 <div>
                     
                           
                      <table class="table table-striped table-bordered ">
                        <thead>
		                    <tr>
			                    <th>Sl No</th>
			                    <th>School Name</th>
			                    <th>Male </th>
			                    <th>Female</th>
                                <th>Total</th>
		                    </tr>
	                    </thead>
	                    <tbody id="tbody-grid">
	                    </tbody>
                      </table>
                        <div class="col-lg-12">
                            <div class="col-lg-3 col-lg-offset-4">
                                  <input id="load-more-school" type="button"  class="btn btn-lg btn-default btn-block"  value="Load More" style="visibility: hidden;" />
                                <img src="img/Loading_icon.gif" alt="" id="loader" style="visibility: visible;"/>
                            </div>      
                        </div>                                  
                 </div>
            
            </asp:Panel>
          
        </ContentTemplate>
       
    </asp:UpdatePanel>
    <%--<script type="text/javascript"  src="js/config.js" async="async"></script>--%>
        <script type="text/javascript" src="js/api-module.js"></script>
     <script type="text/javascript" src="js/student-report.js"></script>
   
    <asp:HiddenField ID="HiddenField1" runat="server" />
</asp:Content>
