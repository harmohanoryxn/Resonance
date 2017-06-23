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
         <asp:DropDownList ID="ddlreturntype" runat="server" OnSelectedIndexChanged="ddlreturntype_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="ADT">ADT</asp:ListItem>
            <asp:ListItem Value="ORM">ORM</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="ddlfolderselect" runat="server"></asp:DropDownList>
        <asp:DropDownList ID="ddlShoworUpdate" runat="server">
            <asp:ListItem Value="0">Show</asp:ListItem>
            <asp:ListItem Value="1">Insert/update</asp:ListItem>
        </asp:DropDownList>
       
        <asp:Button ID="btnShow" Text="Show Messages"  runat="server" OnClick="btnShow_Click"/>
        <asp:Button ID="btnUpdateORM" runat="server" Text="Update Procedure Time" OnClick="btnUpdateORM_Click" />
        <asp:Button ID="btnUpdateRoom" runat="server" Text="Update Bed & Room" OnClick="btnUpdateRoom_Click" />
        <asp:Button ID="btnUpdateAllergens" runat="server" Text="Update CCI" OnClick="btnUpdateAllergens_Click" />
        <asp:Button ID="updateLocation" runat="server" Text="Update Department Loc" OnClick="updateLocation_Click" />

    </div>
         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true"></asp:GridView>
    </form>
</body>
</html>
