<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Taksi</title>
    <link rel="stylesheet" type="text/css" href="Style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="SelectFileLabel" runat="server" Text="Pasirinkite duomenų failą:" />
        <br />
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <br />
        <div class="InputControl">
            <asp:Label ID="M1Label" runat="server" Text="Įveskite mašinos amžiaus intervalo vieną galą: " Width="300px"></asp:Label>
            <asp:TextBox ID="M1" runat="server" Width="128px"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="M1Validator" ControlToValidate="M1" ValidationExpression="[0-9]{1,}" ErrorMessage="Amžius turi būti natūralusis skaičius!" EnableClientScript="False" ForeColor="Red"/>
            <asp:RequiredFieldValidator runat="server" ID="M1Validator2" ControlToValidate="M1" ErrorMessage="Amžius negali būti tuščias!" EnableClientScript="false" ForeColor="Red" />
        </div>
        <div class="InputControl">
            <asp:Label ID="M2Label" runat="server" Text="Įveskite mašinos amžiaus intervalo kitą galą: " Width="300px"></asp:Label>
            <asp:TextBox ID="M2" runat="server" Width="128px"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="M2Validator" ControlToValidate="M2" ValidationExpression="[0-9]{1,}" ErrorMessage="Amžius turi būti natūralusis skaičius!" EnableClientScript="False" ForeColor="Red"/>
            <asp:RequiredFieldValidator runat="server" ID="M2Validator2" ControlToValidate="M2" ErrorMessage="Amžius negali būti tuščias!" EnableClientScript="false" ForeColor="Red" />
        </div>
        <asp:Button ID="MakeListButton" runat="server" OnClick="MakeList" Text="Nuskaityti failą ir sudaryti mašinų sąrašą" />
        <br />
        <hr />
        <asp:Table ID="InitialDataTable" runat="server" Visible="false" Width="900px" GridLines="Both" CellPadding="5">
            <asp:TableRow runat="server">
                <asp:TableCell ID="TextInitialDataTable" runat="server" ColumnSpan="5">Padinių duomenų lentelė</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell runat="server">Vairtuotojo(-ų) pavardė(-s)</asp:TableCell>
                <asp:TableCell runat="server">Mašinos markė</asp:TableCell>
                <asp:TableCell runat="server">Mašinos valstybinis numeris</asp:TableCell>
                <asp:TableCell runat="server">Pagaminimo metai</asp:TableCell>
                <asp:TableCell runat="server">Rida (tūkst. km)</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Label ID="ResultsLabel" runat="server" Text=""/>
        <asp:Table ID="ResultsTable" runat="server" Width="900px" GridLines="Both" CellPadding="5" Visible="false">
            <asp:TableRow runat="server">
                <asp:TableCell runat="server">Vairtuotojo(-ų) pavardė(-s)</asp:TableCell>
                <asp:TableCell runat="server">Mašinos markė</asp:TableCell>
                <asp:TableCell runat="server">Mašinos valstybinis numeris</asp:TableCell>
                <asp:TableCell runat="server">Pagaminimo metai</asp:TableCell>
                <asp:TableCell runat="server">Rida (tūkst. km)</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Label ID="MostDrivenCar" runat="server" Text=""/>
    </form>
</body>
</html>