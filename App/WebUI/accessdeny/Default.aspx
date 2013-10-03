<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AccessDeniedDesign_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <!--
     Web.config settings:
     <System.Web>
        <authentication mode="Windows"/>
        <identity impersonate="false"/>
    </System.Web>
    -->
        <div><p>Hello <asp:Label ID="UserLabel" runat="server" Text="Label"></asp:Label></p></div>
        <div><asp:LinkButton ID="btnAccD" runat="server" Text="Sign in as different user" 
                onclick="btnAccD_Click" /></div>
    </div>
    </form>
</body>
</html>
