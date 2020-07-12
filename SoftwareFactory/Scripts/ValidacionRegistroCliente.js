$(document).ready(function () {

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
        else if (id.length > 11) {
            $("#errorLenM").attr("hidden", false);
            var intervaloLM = setInterval(function () {

                $("#errorLenM").attr("hidden", true);
                clearInterval(intervaloLM);
            }, 2000);
            return false;
        }
    });

    $("#Password2").keypress(function () {
        var contraseña = $("#Password").val()
        var contraseña2 = $("#Password2").val()

        if (contraseña2 != contraseña) {
            $("#errorContra3").attr("hidden", true);
            $("#errorContra2").attr("hidden", false);
            var intervalo31 = setInterval(function () {

                $("#errorContra2").attr("hidden", true);
                clearInterval(intervalo31);
            }, 10000);
        }
        else {
            $("#errorContra2").attr("hidden", true);
            $("#errorContra3").attr("hidden", false);
            var intervalo4 = setInterval(function () {

                $("#errorContra3").attr("hidden", true);
                clearInterval(intervalo4);
            }, 10000);
		}

    });

    $("#Password2").keydown(function () {
        var contraseña = $("#Password").val()
        var contraseña2 = $("#Password2").val()

        if (contraseña2 != contraseña) {
            $("#errorContra3").attr("hidden", true);
            $("#errorContra2").attr("hidden", false);
            var intervalo321 = setInterval(function () {

                $("#errorContra2").attr("hidden", true);
                clearInterval(intervalo321);
            }, 10000);
        }
        else {
            $("#errorContra2").attr("hidden", true);
            $("#errorContra3").attr("hidden", false);
            var intervalo41 = setInterval(function () {

                $("#errorContra3").attr("hidden", true);
                clearInterval(intervalo41);
            }, 10000);
        }

    });

    $("#Password2").keyup(function () {
        var contraseña = $("#Password").val()
        var contraseña2 = $("#Password2").val()

        if (contraseña2 != contraseña) {
            $("#errorContra3").attr("hidden", true);
            $("#errorContra2").attr("hidden", false);
            var intervalo31 = setInterval(function () {

                $("#errorContra2").attr("hidden", true);
                clearInterval(intervalo31);
            }, 10000);
        }
        else {
            $("#errorContra2").attr("hidden", true);
            $("#errorContra3").attr("hidden", false);
            var intervalo4 = setInterval(function () {

                $("#errorContra3").attr("hidden", true);
                clearInterval(intervalo4);
            }, 10000);
        }

    });

});


function validarCampos() {
    var id = $("#usuario").val()
    var correo = $("#email").val()
    var contraseña = $("#Password").val()
    var contraseña2 = $("#Password2").val()

    var regex = /[\w-\.]{2,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;

    if (id == "" || correo == "" || contraseña == "" || contraseña2 == "") {
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

        if (contraseña2 == "") {
            $("#errorPassword2").attr("hidden", false);
            var intervalo3 = setInterval(function () {

                $("#errorPassword2").attr("hidden", true);
                clearInterval(intervalo3);
            }, 5000);
        }
    } else if (id.length < 5) {
        $("#errorLen").attr("hidden", false);
        var intervaloL = setInterval(function () {

            $("#errorLen").attr("hidden", true);
            clearInterval(intervaloL);
        }, 2000);
    } else if (contraseña.length < 8) {
        $("#errorContra").attr("hidden", false);
        var intervaloC = setInterval(function () {

            $("#errorContra").attr("hidden", true);
            clearInterval(intervaloC);
        }, 2000);
    } else if (contraseña2 != contraseña) {
        $("#errorContra3").attr("hidden", true);
        $("#errorContra2").attr("hidden", false);
        var intervalo31 = setInterval(function () {

            $("#errorContra2").attr("hidden", true);
            clearInterval(intervalo31);
        }, 10000);
    } else if (!(regex.test($('#email').val().trim()))) {
        $("#errorEmailFormato").attr("hidden", false);
        var intervaloEmail = setInterval(function () {

            $("#errorEmailFormato").attr("hidden", true);
            clearInterval(intervaloEmail);
        }, 5000);

    }else{
        $("#enviar").trigger("click");
	}
	


    
}