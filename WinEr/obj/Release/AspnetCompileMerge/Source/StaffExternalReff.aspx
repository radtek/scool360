﻿<%@ Page Title="" Language="C#" MasterPageFile="~/WinErStudentMaster.master" AutoEventWireup="true" CodeBehind="StaffExternalReff.aspx.cs" Inherits="WinEr.StaffExternalReff" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><script type="text/javascript">
        function Calculate() {

            var _Array = new Array();
            var gridViewCtl = document.getElementById('<%=Grd_Staff.ClientID%>');
            for (var i = 1; i < gridViewCtl.rows.length; i++) {

                var Tx_Rff = gridViewCtl.rows[i].cells[3].children[0];
                Tx_Rff.style.backgroundColor = 'White';
                Tx_Rff.title = "Enter Reference Number";
                if (Tx_Rff.value != "") {
                    for (var j = 0; j < _Array.length; j++) {
                        if (_Array[j] == Tx_Rff.value) {
                            Tx_Rff.style.backgroundColor = 'Red';
                            Tx_Rff.title = "Duplicate Reference Number";
                            alert("Please enter different Reference Number");
                            break;
                        }
                    }

                    _Array.push(Tx_Rff.value);
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="contents">
        
        <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server">
            </ajaxToolkit:ToolkitScriptManager>  
       <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="pnlAjaxUpdateattendance">
                <ProgressTemplate>               
                
                        <div id="progressBackgroundFilter">

                        </div>

                        <div id="processMessage">

                        <table style="height:100%;width:100%" >

                        <tr>

                        <td align="center">

                        <b>Please Wait...</b><br />

                        <br />

                        <img src="images/indicator-big.gif" alt=""/></td>

                        </tr>

                        </table>

                        </div>
                                        
                      
                </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="pnlAjaxUpdateattendance" runat="server">
 <ContentTemplate>
 


<div class="container skin1" >
		<table cellpadding="0" cellspacing="0" class="containerTable">
			<tr >
				<td class="no"><asp:Image ID="Image3" runat="server" ImageUrl="~/images/user.png" 
                        Height="28px" Width="29px" /> </td>
				<td class="n" align="left">Staff Reference Number</td>
				<td class="ne"> </td>
			</tr>
			<tr >
				<td class="o"> </td>
				<td class="c" >
				
  
                 <asp:Panel ID="Panel_Staff" runat="server" DefaultButton="Btn_Save">
           
           
             
            
             <br />
             
             <table width="100%">
              <tr>
               <td align="right">
                   <asp:Button ID="Btn_Save" runat="server" Text="Save" class="btn btn-success" 
                       onclick="Btn_Save_Click"  />
                   
                    &nbsp;
                    
                   <asp:Button ID="Btn_RemoveAll" runat="server" Text="Clear All" 
                       class="btn btn-primary" onclick="Btn_RemoveAll_Click" />
                   
                   &nbsp;
                    
                   
                    <asp:Button ID="Btn_Cancel" runat="server" Text="Cancel" class="btn btn-danger" 
                       onclick="Btn_Cancel_Click"  />
               </td>
              </tr>
             </table>
              <center>
                 <asp:Label ID="Lbl_msg" runat="server" Text="" class="control-label" ForeColor="Red"></asp:Label>
             </center>
             
             
           <div style="height:350px;overflow:auto">
           
           <br />
           <asp:GridView ID="Grd_Staff" runat="server" CellPadding="4" ForeColor="Black" 
            GridLines="Vertical" Width="100%" AutoGenerateColumns="False"   
            BackColor="#EBEBEB" BorderColor="#BFBFBF" BorderStyle="None" BorderWidth="1px" 
            onselectedindexchanged="Grd_Staff_SelectedIndexChanged">
            <Columns>
                <asp:BoundField DataField="StaffId" HeaderText="Staff Id" />
                <asp:BoundField DataField="Id" HeaderText="Enrollment Id" />
                <asp:BoundField DataField="SurName" HeaderText="Staff Name" />  
                <asp:BoundField DataField="RoleName" HeaderText="Role Name" />  
                <asp:TemplateField HeaderText="Reference Number">
                   <ItemTemplate>
                       <asp:TextBox ID="Txt_ReferanceNumber" runat="server" Text="" Width="140" class="form-control" MaxLength="30" onkeyup="Calculate()"></asp:TextBox>    
                       <ajaxToolkit:FilteredTextBoxExtender ID="IncedentFilteredTextBoxExtender1"
                       runat="server"   TargetControlID="Txt_ReferanceNumber" FilterType="Custom"
                       FilterMode="InvalidChars" InvalidChars="'\"  />     
                   </ItemTemplate>  
                </asp:TemplateField>
                  <asp:CommandField SelectText="&lt;img src='Pics/DeleteRed.png' width='30px' border=0 title='Remove'&gt;"
                 ShowSelectButton="True" HeaderText="Remove"  ItemStyle-Width="50px"  >
              </asp:CommandField>
             </Columns>  
             
         <PagerSettings NextPageText="&gt;&gt;" PageButtonCount="5" PreviousPageText="&lt;&lt;" />
                  <FooterStyle BackColor="#bfbfbf" ForeColor="Black" />
                  <EditRowStyle Font-Size="Medium" />
                  <SelectedRowStyle BackColor="White" ForeColor="Black" />
                  <PagerStyle BackColor="White" ForeColor="#FF6600" HorizontalAlign="Left" />
                  <HeaderStyle BackColor="#e9e9e9" Font-Bold="True" Font-Size="11px" ForeColor="Black"  HorizontalAlign="Left" />
                  <RowStyle BackColor="White"  BorderColor="Olive" Font-Size="11px" ForeColor="Black"  HorizontalAlign="Left" VerticalAlign="Top" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
             
             
             
            </div>
           
           </asp:Panel>
					
				</td>
				<td class="e"> </td>
			</tr>
			<tr >
				<td class="so"> </td>
				<td class="s"></td>
				<td class="se"> </td>
			</tr>
		</table>
	</div>
	

<asp:Panel ID="Panel1" runat="server">
                       
   <asp:Button runat="server" ID="Button1" class="btn btn-info" style="display:none"/>
   <ajaxToolkit:ModalPopupExtender ID="MPE_SingleDelete"   runat="server" CancelControlID="Btn_No1"     PopupControlID="Panel2" TargetControlID="Button1"  BackgroundCssClass="modalBackground" />
   <asp:Panel ID="Panel2" runat="server" style="display:none;"> <%--style="display:none;"--%>
   <div class="container skin1" style="width:400px; top:400px;left:400px" >
    <table   cellpadding="0" cellspacing="0" class="containerTable">
     <tr >
      <td class="no"><asp:Image ID="Image1" runat="server" ImageUrl="~/Pics/comment.png"  Height="28px" Width="29px" /> </td>
         <td class="n">
         
              <table width="100%">
               <tr>
                <td>
                 <span style="color:Black">Alert</span>
                </td>
                <td align="right">
                    
                </td>
               </tr>
              </table>
               
         </td>
      <td class="ne">&nbsp;</td></tr><tr >
      <td class="o"> </td>
      <td class="c" >
      
          <br/>
         <center>
           
          
                
                <table width="100%">
                
                  <tr>
                   <td>
                       <asp:Label ID="lbl_Id" runat="server" Text="" class="control-label" Visible="false"></asp:Label>
                   <asp:Label ID="lbl1" runat="server" class="control-label" Text="You are about to remove Reference Number of selected staff. Do you want to continue?" ForeColor="Black" ></asp:Label>
                   <br />
                   </td>
                  </tr>
                  <tr>
                   <td>
                   
                       <asp:Button ID="Btn_Yes1" runat="server" Text="Yes" class="btn btn-info" 
                           onclick="Btn_Yes1_Click" />
                       
                        &nbsp;<asp:Button ID="Btn_No1" runat="server" Text="No" class="btn btn-info" />
                   
                   </td>
                  </tr>
                </table>
                </center>     
                 
                 
        </td>
        <td class="e"> </td>
      </tr>
      <tr>
        <td class="so"> </td>
        <td class="s"> </td>
        <td class="se"> </td>
      </tr>
    </table>
    <br />
                   
</div>
       </asp:Panel>                 
                        </asp:Panel>

<asp:Panel ID="Panel3" runat="server">
                       
   <asp:Button runat="server" ID="Button2" class="btn btn-info" style="display:none"/>
   <ajaxToolkit:ModalPopupExtender ID="MPE_DeleteAll"   runat="server" CancelControlID="Btn_No"     PopupControlID="Panel4" TargetControlID="Button2"  BackgroundCssClass="modalBackground" />
   <asp:Panel ID="Panel4" runat="server" style="display:none;"> <%--style="display:none;"--%>
   <div class="container skin1" style="width:400px; top:400px;left:400px" >
    <table   cellpadding="0" cellspacing="0" class="containerTable">
     <tr >
      <td class="no"><asp:Image ID="Image2" runat="server" ImageUrl="~/Pics/comments.png"  Height="28px" Width="29px" /> </td>
         <td class="n">
         
              <table width="100%">
               <tr>
                <td>
                 <span style="color:Black">Alert</span>
                </td>
                <td align="right">
                    
                </td>
               </tr>
              </table>
               
         </td>
      <td class="ne">&nbsp;</td></tr><tr >
      <td class="o"> </td>
      <td class="c" >
      
          <br/>
         <center>
           
          
                
                <table width="100%">
                
                  <tr>
                   <td>
                       
                   <asp:Label ID="Label2" class="control-label" runat="server" Text="You are about to remove Reference Number of all staff. Do you want to continue?" ForeColor="Black" ></asp:Label>
                   <br />
                   </td>
                  </tr>
                  <tr>
                   <td>
                   
                       <asp:Button ID="Btn_YesAll" runat="server" Text="Yes" class="btn btn-info" 
                           onclick="Btn_YesAll_Click"  />
                       
                        &nbsp;<asp:Button ID="Btn_No" runat="server" Text="No" class="btn btn-info" />
                   
                   </td>
                  </tr>
                </table>
                </center>     
                 
                 
        </td>
        <td class="e"> </td>
      </tr>
      <tr>
        <td class="so"> </td>
        <td class="s"> </td>
        <td class="se"> </td>
      </tr>
    </table>
    <br />
                   
</div>
       </asp:Panel>                 
                        </asp:Panel>


 </ContentTemplate>
</asp:UpdatePanel>

        <div class="clear"></div>
    </div>
</asp:Content>
