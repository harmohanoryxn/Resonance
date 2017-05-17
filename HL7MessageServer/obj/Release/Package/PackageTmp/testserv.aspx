<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testserv.aspx.cs" Inherits="HL7MessageServer.testserv" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin:10px;">
            <asp:Button ID="btn" runat="server" Text="Show Data" OnClick="btn_Click"/>
            <asp:Button ID="Button1" runat="server" Text="Add PID" OnClick="Button1_Click"/>
            <asp:Button id="btn1" runat="server" Text="Show XML" OnClick="btn1_Click" />
        </div>
         <div style="margin:10px;">
             <asp:DropDownList ID="ddlfolderselect" runat="server">

             </asp:DropDownList>
            <asp:DropDownList ID="ddlval" runat="server">
                <asp:ListItem Text="ORM" Value="ORM"></asp:ListItem>
                <asp:ListItem Text="ADT" Value="ADT"></asp:ListItem>
            </asp:DropDownList>
              <asp:DropDownList ID="ddlFilter" runat="server">
                <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                <asp:ListItem Text="No" Value="No"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnparsemessages" runat="server" Text="Show Messages" OnClick="btnparsemessages_Click"/>
           
        </div>
    <div>
        <div style="margin:10px;">
            <asp:Literal ID="totallit" runat="server"></asp:Literal>
            </div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true"></asp:GridView>
    </div>
    </form>
</body>
</html>
