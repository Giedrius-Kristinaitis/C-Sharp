<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Krepšinis</title>
    <link rel="stylesheet" href="styles.css" />
</head>
<body>
    <form id="MainForm" runat="server">
        <asp:Label ID="InfoText1" runat="server" CssClass="InfoLabel" Text="Įveskite krepšininkų pozicją:"/>
        <asp:TextBox ID="PositionInput" runat="server" CssClass="InputText" />
        <br />
        <asp:Button ID="ValidatePositionButton" CssClass="Button" OnClick="ValidatePosition" Text="Įvesti poziciją" runat="server" />
        <br />
        <asp:Label ID="InfoText3" CssClass="InfoLabel" runat="server" />
        <br />
        <asp:Label ID="InfoText2" runat="server" CssClass="InfoLabel Invisible" Text="Įveskite naudingiausių žaidėjų sąrašo ilgį:"/>
        <asp:TextBox ID="ListLengthInput" runat="server" CssClass="InputText Invisible" />
        <br />
        
        <asp:Button ID="MakeListButton" Text="Sudaryti naudingiausių žaidėjų sąrašą" OnClick="MakePlayerList" CssClass="Button Invisible" runat="server"/>
        <br />
        <asp:Label ID="ErrorText" runat="server" CssClass="Invisible" />
        <br />
        <asp:Label ID="ErrorCause" runat="server" CssClass="Invisible" />
        <br />
        <asp:Label ID="ErrorFix" runat="server" CssClass="Invisible" />
    </form>
</body>
</html>
