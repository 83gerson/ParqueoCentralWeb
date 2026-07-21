// Funcionalidad AJAX del módulo de Espacios (HU-05, criterio 4),
// actualiza la disponibilidad de espacios sin recargar la página completa.
$(function () {
    function cargarEspacios() {
        $.ajax({
            url: "/Espacios/Estado",
            type: "GET",
            success: function (html) {
                $("#contenedorTablaEspacios").html(html);
            }
        });
    }

    $("#btnActualizar").on("click", function (e) {
        e.preventDefault();
        cargarEspacios();
    });

    // Actualización automática cada 15 segundos para reflejar cambios recientes.
    setInterval(cargarEspacios, 15000);
});
