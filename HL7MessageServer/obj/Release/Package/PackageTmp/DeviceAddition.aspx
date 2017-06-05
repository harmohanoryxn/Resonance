<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeviceAddition.aspx.cs" Inherits="HL7MessageServer.DeviceAddition" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Button ID="btn" runat="server" OnClick="btn_Click" Text="insert this device" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Test" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Test" />
        <asp:Literal ID="lbl" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
