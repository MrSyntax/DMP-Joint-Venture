<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="loginControl.ascx.cs" Inherits="TennisVereniging.Usercontrols.loginControl" %>
<link href="../StyleSheet1.css" rel="stylesheet" />
<table id="loginTable">
    <tr>
        <td>IMAGE</td>
    </tr>
    <tr>
        <td>Email</td>
        <td>
    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td>Wachtwoord</td>
        <td>
    <asp:TextBox CssClass="Password" ID="txtPassword" runat="server"></asp:TextBox></td>
    </tr>
</table>
