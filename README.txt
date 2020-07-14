

El software para gestión de proyectos de fabrica de software, siendo especificos en el modulo de agendas, requiere de cambios según el servidor donde es desplegado,
dicha modificación unicamente requiere de cambiar pocisiones de arrays, se pueden hacer las modificaciones en la siguiente ruta:


Controllers/
            AgendaController/ 
                             Create
                             
En el metodo post, donde vamos a encontrar una variable llamada fecha la cual contiene un un array de longitud 3, se bebe analizar cual de las posiciones contiene el día
y cual el mes para seguidamente verificar que el if que valida esa información entre con exito. 
