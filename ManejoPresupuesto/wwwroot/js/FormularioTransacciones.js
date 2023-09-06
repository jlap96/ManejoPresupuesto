function inicializarFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await respuesta.json();
        const opciones =
            json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`); //<--Generamos el arreglo de opciones
        $("#CategoriaId").html(opciones);//<-Insertamos el arreglo de opciones en el html de CategoriaId, que es el select de CategoriaId

        /*Creamos la acción en el controlador de Transacciones la cual nos va traer las transacciones */
    })
}