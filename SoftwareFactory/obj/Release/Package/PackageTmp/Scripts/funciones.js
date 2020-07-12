$(document).ready(function() { //ACCION CUANDO CARGUE LA PAGINA

    var altura_arr = []; //CREAMOS UN ARREGLO VACIO
    $('.cardservices').each(function() { //RECORREMOS TODOS LOS CONTENEDORES DE LAS IMAGENES, DEBEN TENER LA MISMA CLASE
        var altura = $(this).height(); //LES SACAMOS LA ALTURA
        altura_arr.push(altura); //METEMOS LA ALTURA AL ARREGLO
    });
    altura_arr.sort(function(a, b) { return b - a }); //ACOMODAMOS EL ARREGLO EN ORDEN DESCENDENTE
    $('.cardservices').each(function() { //RECORREMOS DE NUEVO LOS CONTENEDORES
        $(this).css('height', altura_arr[0]); //LES PONEMOS A TODOS LOS CONTENEDORES EL PRIMERO ELEMENTO DE ALTURA DEL ARREGLO, QUE ES EL MAS GRANDE.
    });
});

function ScrollF(ancla){
    $('html,body').animate({
        scrollTop: $('#'+ancla).offset().top - 30
    }, 2000);

}
