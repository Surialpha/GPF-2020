function Validar() {
    var correo = $("#correo").val();
    var nombre = $("#nombre").val();
    var descripcion = $("#desc").val();
    var regex = /[\w-\.]{2,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;

    if (correo == "" || nombre == "" || descripcion == "") {

        if (correo == "") {
            $("#errorCorreo").html("Este campo es obligatorio*")
            $("#errorCorreo").attr("hidden", false);
            var interCorreo = setInterval(function () {
                $("#errorCorreo").attr("hidden", true);
                clearInterval(interCorreo);
            }, 10000);
        }
        else if (!(regex.test($('#correo').val().trim()))) {
            $("#errorCorreo").html("Correo no válido*")
            $("#errorCorreo").attr("hidden", false);
            var interCorreo12 = setInterval(function () {
                $("#errorCorreo").attr("hidden", true);
                clearInterval(interCorreo12);
            }, 10000);
        }


        if (nombre == "") {
            $("#errorNombre").html("Este campo es obligatorio*")
            $("#errorNombre").attr("hidden", false);
            var interNombre = setInterval(function () {
                $("#errorNombre").attr("hidden", true);
                clearInterval(interNombre);
            }, 10000);
        }

        if (descripcion == "") {
            $("#errorDescripcion").html("Este campo es obligatorio*")
            $("#errorDescripcion").attr("hidden", false);
            var interDescripcion = setInterval(function () {
                $("#errorDescripcion").attr("hidden", true);
                clearInterval(interDescripcion);
            }, 10000);
        }
    }
    else if (!(regex.test($('#correo').val().trim()))) {
        $("#errorCorreo").html("Correo no válido*")
        $("#errorCorreo").attr("hidden", false);
        var interCorreo1 = setInterval(function () {
            $("#errorCorreo").attr("hidden", true);
            clearInterval(interCorreo1);
        }, 10000);
    } else {
        $("#enviar").trigger("click");
    }



}