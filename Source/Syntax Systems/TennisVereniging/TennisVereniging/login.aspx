<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="TennisVereniging.login" %>

<%@ Register Src="~/Usercontrols/loginControl.ascx" TagPrefix="uc1" TagName="loginControl" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="loginContainer">
    <div id="divLogin">
        <uc1:loginControl runat="server" id="loginControl" />
        <asp:Button ID="btnLogin" runat="server" Text="Login" />
    </div>
            </div>
    </form>
</body>
</html>
