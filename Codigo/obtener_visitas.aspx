<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="obtener_visitas.aspx.cs" Inherits="PruebaResidencialPalmera.Codigo.obtener_visitas" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Detalles del Visitante</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Detalles del Visitante</h2>
            <div class="mb-3">
                <label class="form-label">Nombre:</label>
                <asp:Label ID="lblNombre" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="mb-3">
                <label class="form-label">Identidad:</label>
                <asp:Label ID="lblIdentidad" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="mb-3">
                <label class="form-label">Placa del Vehículo:</label>
                <asp:Label ID="lblPlacaVehiculo" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="mb-3">
                <label class="form-label">Cantidad de Personas:</label>
                <asp:Label ID="lblCantidadPersonas" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="mb-3">
                <label class="form-label">Comentarios:</label>
                <asp:Label ID="lblComentarios" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="mb-3">
                <label class="form-label">¿Recurrente?</label>
                <asp:Label ID="lblRecurrente" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
