<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="visitas.aspx.cs" Inherits="PruebaResidencialPalmera.Codigo.visitas" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Registro De Visitas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Autocompletar nombres mientras se escribe
            $('#<%= Nombre.ClientID %>').on('keyup', function () {
                var nombre = $(this).val();
                if (nombre.length >= 2) { // Solo buscar si hay al menos 2 caracteres
                    $.ajax({
                        type: "GET",
                        url: '<%= ResolveUrl("~/VisitorService.asmx/GetVisitorNames") %>',
                        data: { prefixText: nombre },
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var suggestions = response.d;
                            // Crear una lista de sugerencias
                            var suggestionList = $('<ul class="suggestions-list" style="border: 1px solid #ccc; position: absolute; z-index: 1000; background: white;"></ul>');
                            $.each(suggestions, function (index, item) {
                                suggestionList.append('<li data-id="' + item.ID_Visitante + '">' + item.Nombre + '</li>');
                            });

                            // Limpiar las sugerencias anteriores
                            $(".suggestions-list").remove();

                            // Mostrar sugerencias debajo del input
                            if (suggestionList.children().length > 0) {
                                suggestionList.insertAfter('#<%= Nombre.ClientID %>');
                                suggestionList.on('click', 'li', function () {
                                    var selectedName = $(this).text();
                                    var selectedId = $(this).data('id'); // Obtener el ID del visitante
                                    $('#<%= Nombre.ClientID %>').val(selectedName);
                                    $('#<%= ID_Visitante.ClientID %>').val(selectedId); // Llenar el campo ID_Visitante
                                    suggestionList.remove(); // Quitar la lista de sugerencias

                                    // Hacer otra llamada para obtener detalles
                                    $.ajax({
                                        type: "GET",
                                        url: '<%= ResolveUrl("~/VisitorService.asmx/GetVisitorDetails") %>',
                                        data: { idVisitante: selectedId }, // Usar el ID seleccionado
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (response) {
                                            if (response.d) {
                                                rellenarCampos(response.d.Identidad, response.d.Placa_Vehiculo, response.d.Cantidad_Personas, response.d.Comentarios, response.d.Recurrente);
                                            }
                                        },
                                        error: function (xhr, status, error) {
                                            console.error(error);
                                        }
                                    });
                                });
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error(error);
                        }
                    });
                } else {
                    $(".suggestions-list").remove(); // Limpiar sugerencias si hay menos de 2 caracteres
                }
            });

            // Limpiar sugerencias si se hace clic fuera
            $(document).on('click', function (e) {
                if (!$(e.target).closest('#<%= Nombre.ClientID %>').length) {
                    $(".suggestions-list").remove();
                }
            });
        });

        function rellenarCampos(identidad, placa, cantidad, comentarios, recurrente) {
            $("#<%= Placa_Vehiculo.ClientID %>").val(placa);
            $("#<%= Cantidad_Personas.ClientID %>").val(cantidad);
            $("#<%= Comentarios.ClientID %>").val(comentarios);
            $("#<%= Recurrente.ClientID %>").prop("checked", recurrente);
        }

        function compartirCodigoQr() {
            var qrCodeUrl = $("#<%= imgQrCode.ClientID %>").attr("src");
            if (qrCodeUrl) {
                var shareText = "Aquí está mi código QR:";
                var shareLink = `https://api.whatsapp.com/send?text=${encodeURIComponent(shareText)}%0A${encodeURIComponent(qrCodeUrl)}`;
                window.open(shareLink, '_blank');
            } else {
                alert("Por favor, primero genera el código QR.");
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <br />
    <div class="mx-auto" style="width:250px">
        <asp:Label runat="server" CssClass="h2" ID="Label1" Text="Registro De Visitas"></asp:Label>
    </div>
    <form runat="server" class="h-100 d-flex align-items-center justify-content-center" enctype="multipart/form-data">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div>
            <div class="mb-3">
                <label class="form-label">Nombre Del Invitado</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Nombre" autocomplete="off"></asp:TextBox>
            </div>

            <div class="mb-3">
                <label class="form-label">Numero De Identidad De Su Invitado</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="ID_Visitante"></asp:TextBox> <!-- Removido ReadOnly -->
            </div>

            <div class="mb-3">
                <label class="form-label">Placa del Vehículo</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Placa_Vehiculo"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label">¿Cuantos Acompañantes Vienen Con El Invitado?</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Cantidad_Personas" Text="1"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label">¿Será una visita recurrente?</label>
                <asp:CheckBox runat="server" CssClass="form-check-input" ID="Recurrente" AutoPostBack="true" OnCheckedChanged="VisitaRecurrente_CheckedChanged" />
            </div>
            <div class="mb-3" id="divDuracionVisita" runat="server" style="display:none;">
                <label class="form-label">¿Cuántos días será válida la invitación?</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Vigencia_Dias" Enabled="false"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label">¿Desea agregar un comentario?</label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Comentarios" TextMode="MultiLine" MaxLength="50"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label">Agregar una foto de su invitado</label>
                <asp:FileUpload runat="server" CssClass="form-control" ID="Foto" />
            </div>
            <asp:Button runat="server" CssClass="btn btn-primary" ID="btnRegistrar" Text="Registrar" Visible="True" OnClick="BtnRegistrar_Click" />
            <asp:Button runat="server" CssClass="btn btn-secondary" ID="btCancelar" Text="Cancelar" Visible="True" OnClick="BtCancelar_Click" />
            <div class="mt-3">
                <asp:Label runat="server" ID="lblMensaje" CssClass="text-danger"></asp:Label>
            </div>
            <div class="mt-3">
                <asp:Image runat="server" ID="imgQrCode" CssClass="img-fluid" Visible="false" />
            </div>
            <div class="mt-3" id="divBotonCompartir" runat="server" style="display:none;">
                <button type="button" class="btn btn-success" onclick="compartirCodigoQr()">Compartir Código QR</button>
            </div>
        </div>
    </form>
</asp:Content>