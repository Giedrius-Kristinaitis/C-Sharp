<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Krepšinis</title>
    <link rel="stylesheet" href="styles.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="InfoText1" runat="server" CssClass="InfoLabel" Text="Įveskite krepšininkų pozicją:"/>
        <asp:TextBox ID="PositionInput" runat="server" CssClass="InputText" />
        <br />
        <asp:Label ID="InfoText2" runat="server" CssClass="InfoLabel" Text="Įveskite naudingiausių žaidėjų sąrašo ilgį:"/>
        <asp:TextBox ID="ListLengthInput" runat="server" CssClass="InputText" />
        <br />
        <asp:Button ID="MakeListButton" Text="Sudaryti naudingiausių žaidėjų sąrašą" OnClick="MakePlayerList" CssClass="Button" runat="server"/>
        <br />
        <br />
        <asp:Table ID="InitialDataTable" runat="server"  CssClass="Invisible">
            <asp:TableRow ID="TableRow0" CssClass="Dark" runat="server">
                <asp:TableCell ID="TableCell0" CssClass="Center" ColumnSpan="8" runat="server">Pradiniai duomenys</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" CssClass="Dark">
                <asp:TableCell runat="server">Komanda</asp:TableCell>
                <asp:TableCell runat="server">Žaidėjo pavardė</asp:TableCell>
                <asp:TableCell runat="server">Žaidėjo vardas</asp:TableCell>
                <asp:TableCell runat="server">Pozicija</asp:TableCell>
                <asp:TableCell runat="server">Žaista minučių</asp:TableCell>
                <asp:TableCell runat="server">Pelnyta taškų</asp:TableCell>
                <asp:TableCell runat="server">Padaryta klaidų</asp:TableCell>
                <asp:TableCell runat="server">Rungtynių data</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Table ID="ResultsTable" runat="server"  CssClass="Invisible">
            <asp:TableRow ID="TableRow1" CssClass="Dark" runat="server">
                <asp:TableCell ID="TableCell1" CssClass="Center" ColumnSpan="8" runat="server">Naudingiausių krepšininkų lentelė</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" CssClass="Dark">
                <asp:TableCell runat="server">Komanda</asp:TableCell>
                <asp:TableCell runat="server">Žaidėjo pavardė</asp:TableCell>
                <asp:TableCell runat="server">Žaidėjo vardas</asp:TableCell>
                <asp:TableCell runat="server">Pozicija</asp:TableCell>
                <asp:TableCell runat="server">Žaista minučių</asp:TableCell>
                <asp:TableCell runat="server">Pelnyta taškų</asp:TableCell>
                <asp:TableCell runat="server">Padaryta klaidų</asp:TableCell>
                <asp:TableCell runat="server">Rungtynių data</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Label ID="ErrorText" runat="server" CssClass="Invisible" />
        <br />
        <asp:Label ID="ErrorCause" runat="server" CssClass="Invisible" />
        <br />
        <asp:Label ID="ErrorFix" runat="server" CssClass="Invisible" />
    </div>
    </form>
</body>
</html>
