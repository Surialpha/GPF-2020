


$(document).ready(function () {
    //called when key is pressed in textbox
    $("#usuario").keypress(function (e) {
        var id = $("#usuario").val()


        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#errorSolonumero").attr("hidden", false);
            var intervaloN = setInterval(function () {

                $("#errorSolonumero").attr("hidden", true);
                clearInterval(intervaloN);
            }, 2000);
            return false;
        }
        else if (id.length < 5) {
            $("#errorLen").attr("hidden", false);
            var intervaloL = setInterval(function () {

                $("#errorLen").attr("hidden", true);
                clearInterval(intervaloL);
            }, 2000);
        }
        else if (id.length > 12) {
            $("#errorLenM").attr("hidden", false);
            var intervaloLM = setInterval(function () {

                $("#errorLenM").attr("hidden", true);
                clearInterval(intervaloLM);
            }, 2000);
            return false;
        }
    });
});

function validarCampos() {
    var id = $("#usuario").val()
    var correo = $("#email").val()
    var contraseña = $("#password").val()



    if (id == "" || correo == "" || contraseña == "") {
        if (id == "") {
            $("#errorUsuario").attr("hidden", false);
            var intervalo = setInterval(function () {

                $("#errorUsuario").attr("hidden", true);
                clearInterval(intervalo);
            }, 5000);
        }

        if (correo == "") {
            $("#errorEmail").attr("hidden", false);
            var intervalo1 = setInterval(function () {

                $("#errorEmail").attr("hidden", true);
                clearInterval(intervalo1);
            }, 5000);
        } else {
            var regex = /[\w-\.]{2,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;

            if (!(regex.test($('#email').val().trim()))) {
                $("#errorEmailFormato").attr("hidden", false);
                var intervaloEmail = setInterval(function () {

                    $("#errorEmailFormato").attr("hidden", true);
                    clearInterval(intervaloEmail);
                }, 5000);

            }
        }

        if (contraseña == "") {
            $("#errorPassword").attr("hidden", false);
            var intervalo2 = setInterval(function () {

                $("#errorPassword").attr("hidden", true);
                clearInterval(intervalo2);
            }, 5000);
        } else if (contraseña.length < 8) {
            $("#errorContra").attr("hidden", false);
            var intervaloC = setInterval(function () {

                $("#errorContra").attr("hidden", true);
                clearInterval(intervaloC);
            }, 2000);
        }
    } else if (id.length < 5) {
        $("#errorLen").attr("hidden", false);
        var intervaloL = setInterval(function () {

            $("#errorLen").attr("hidden", true);
            clearInterval(intervaloL);
        }, 2000);
    }
    else if (contraseña.length < 8) {
        $("#errorContra").attr("hidden", false);
        var intervaloC = setInterval(function () {

            $("#errorContra").attr("hidden", true);
            clearInterval(intervaloC);
        }, 2000);
    }
    else {
        $("#enviar").trigger("click");
	}

}