<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Messageparser.aspx.cs" Inherits="HL7MessageServer.Messageparser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <asp:TextBox ID="txtMRnumber" runat="server"></asp:TextBox>
        <asp:DropDownList ID="ddlfolderselect" runat="server"></asp:DropDownList>
        <asp:Button ID="btnShow" Text="Show Messages"  runat="server" OnClick="btnShow_Click"/>
        <asp:Button ID="btnUpdateORM" runat="server" Text="Update Procedure Time" OnClick="btnUpdateORM_Click" />
    </div>
         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true"></asp:GridView>
    </form>
</body>
</html>
