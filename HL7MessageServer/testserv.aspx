﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testserv.aspx.cs" Inherits="HL7MessageServer.testserv" %>

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
        </div>
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true"></asp:GridView>
    </div>
    </form>
</body>
</html>
