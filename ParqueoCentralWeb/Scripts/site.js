// Funcionalidad general con JavaScript (HU-14 / requerimiento técnico #12):
// convierte automáticamente a mayúsculas los campos de placa y código de espacio
// mientras el usuario escribe, para mantener un formato igual en los datos.
$(function () {
    $(document).on("input", "input[name='Placa'], #txtPlaca, #txtPlacaEntrada, input[name='CodigoEspacio']", function () {
        var campo = this;
        var posicion = campo.selectionStart;
        var valorOriginal = $(campo).val();
        var valorMayus = valorOriginal.toUpperCase();

        if (valorOriginal !== valorMayus) {
            $(campo).val(valorMayus);
            campo.setSelectionRange(posicion, posicion);
        }
    });
});
