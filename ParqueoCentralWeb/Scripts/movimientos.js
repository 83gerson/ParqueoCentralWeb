// Funcionalidades AJAX del módulo de Movimientos:
// - Registro de entrada (HU-06, HU-07): carga dinámica de espacios disponibles
//   y verificación de si la placa ya existe en el sistema.
// - Historial (HU-10): filtrado dinámico sin recargar la página completa.
$(function () {
    // Entrada de vehículo
    var $panelNuevo = $("#panelVehiculoNuevo");
    var $ddlEspacios = $("#ddlEspacios");

    function cargarEspaciosDisponibles(tipo) {
        $.ajax({
            url: "/Espacios/Disponibles",
            type: "GET",
            data: { tipo: tipo || "" },
            success: function (data) {
                $ddlEspacios.empty();

                if (data.length === 0) {
                    $ddlEspacios.append('<option value="">No hay espacios disponibles</option>');
                    return;
                }

                $.each(data, function (i, e) {
                    $ddlEspacios.append('<option value="' + e.IdEspacio + '">' + e.CodigoEspacio + ' - ' + e.Tipo + '</option>');
                });
            }
        });
    }

    if ($ddlEspacios.length) {
        cargarEspaciosDisponibles();
    }

    var $txtPlacaEntrada = $("#txtPlacaEntrada");
    if ($txtPlacaEntrada.length) {
        $txtPlacaEntrada.on("blur", function () {
            var placa = $(this).val();
            if (!placa) {
                return;
            }

            $.ajax({
                url: "/Vehiculos/VerificarPlaca",
                type: "GET",
                data: { placa: placa },
                success: function (resultado) {
                    if (resultado.existe) {
                        $("#infoVehiculo").text("Vehículo encontrado en el sistema.").removeClass("text-danger").addClass("text-success");
                        $panelNuevo.hide();
                    } else {
                        $("#infoVehiculo").text("Vehículo no registrado. Complete los datos para crearlo.").removeClass("text-success").addClass("text-danger");
                        $panelNuevo.show();
                    }
                }
            });
        });
    }

    // Historial de movimientos
    function filtrarMovimientos() {
        $.ajax({
            url: "/Movimientos/Buscar",
            type: "GET",
            data: {
                filtroPlaca: $("#fPlaca").val(),
                filtroEstado: $("#fEstado").val(),
                filtroFecha: $("#fFecha").val()
            },
            success: function (html) {
                $("#contenedorTablaMovimientos").html(html);
            }
        });
    }

    $("#btnFiltrar").on("click", function (e) {
        e.preventDefault();
        filtrarMovimientos();
    });
});
