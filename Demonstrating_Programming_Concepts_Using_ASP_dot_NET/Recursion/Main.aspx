<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Miestas</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="InfoLabel1" runat="server" Text="Pradiniai duomenys:" Font-Bold="True" Font-Size="16pt"></asp:Label>
            <br />
            <br />
            <asp:Label ID="InfoLabel2" runat="server" Text="Žemėlapis:"></asp:Label>
            <br />
            <asp:Label ID="MapDisplayLabel" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="MapExplanationLabel1" runat="server" Text="Žemėlapio paaiškinimas: 'G' - gėlių parduotuvė, '0' - galima praeiti, '1' - negalima praeiti, '.' - gatvė"></asp:Label>
            <br />
            <br />
            <asp:Label ID="ManCoordinatesLabel" runat="server" Text=""></asp:Label>
            <br />
            <br />
        </div>
        <asp:Button ID="FindPathButton" runat="server" OnClick="FindPathToFlowerStore" Text="Rasti kelią į gėlių parduotuvę" />
        <br />
        <br />
        <div>
            <asp:Label ID="InfoLabel3" runat="server" Text="Rezultatai:" Font-Bold="True" Font-Size="16pt" Visible="false"></asp:Label>
            <br />
            <br />
            <asp:Label ID="ResultsDisplayLabel" runat="server" Text="" Visible="false" ></asp:Label>
            <br />
            <asp:Label ID="MapExplanationLabel2"  runat="server" Text="K - kelias, kuriuo reikia eiti iki gėlių parduotuvės" Visible="False"></asp:Label>
        </div>
    </form>
</body>
</html>
