<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorReader.aspx.cs" Inherits="HL7MessageServer.ErrorReader" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin:20px;">
    <asp:DropDownList ID="ddlErrorfolders" runat="server"></asp:DropDownList>
        <asp:Button ID="btnShowerrors" runat="server" Text="Show errors list" OnClick="btnShowerrors_Click" />
        <asp:Button ID="parseagain" runat="server" Text="Parse and generate file again" OnClick="parseagain_Click"  />
    </div>
        <asp:GridView ID="griderror" runat="server"></asp:GridView>
    </form>
</body>
</html>
