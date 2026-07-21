// Funcionalidades AJAX del módulo de Vehículos (HU-02 y HU-01, criterio 6).
$(function () {
    // Búsqueda en vivo de vehículos en el listado (Index), sin recargar la página.
    var timerBusqueda;
    $("#txtFiltroPlaca").on("keyup", function () {
        clearTimeout(timerBusqueda);
        var filtro = $(this).val();

        timerBusqueda = setTimeout(function () {
            $.ajax({
                url: "/Vehiculos/Buscar",
                type: "GET",
                data: { filtroPlaca: filtro },
                success: function (html) {
                    $("#contenedorTablaVehiculos").html(html);
                }
            });
        }, 300);
    });

    // Verificación en tiempo real de placas duplicadas al registrar un vehículo.
    $("#txtPlaca").on("blur", function () {
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
                    $("#mensajePlaca").text("Ya existe un vehículo con esta placa.").removeClass("text-success").addClass("text-danger");
                } else {
                    $("#mensajePlaca").text("Placa disponible.").removeClass("text-danger").addClass("text-success");
                }
            }
        });
    });
});
