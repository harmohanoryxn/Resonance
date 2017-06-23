<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JsonExporter.aspx.cs" Inherits="HL7MessageServer.JsonExporter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:Button ID="uploadcontent" runat="server" Text="Room to DB" OnClick="uploadcontent_Click" />
        <asp:Button ID="bedtodb" runat="server" Text="Bed to DB" OnClick="bedtodb_Click" />
    </div>
    </form>
</body>
</html>
