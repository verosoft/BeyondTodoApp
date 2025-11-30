# BeyondTodoApi - Arquitectura y Uso

Este documento detalla la arquitectura del proyecto `BeyondTodoApi`, cómo interactúa con el dominio principal (`BeyondTodoDomain`) y cómo puedes probar sus endpoints.

## Arquitectura de la API (`BeyondTodoApi`)

El proyecto `BeyondTodoApi` está construido como una **API Mínima de .NET**, un enfoque moderno y ligero para crear servicios HTTP rápidos. Su principal responsabilidad es exponer la lógica de negocio a través de endpoints RESTful.

La arquitectura sigue un flujo de dependencias claro para mantener la separación de responsabilidades:

```
BeyondTodoApi -> BeyondTodoApiService -> BeyondTodoDomain
```

1.  **Capa de Presentación (`BeyondTodoApi`)**:
    -   Define los endpoints (e.g., `POST /todos`, `GET /todos/{id}`).
    -   Maneja las solicitudes y respuestas HTTP.
    -   Utiliza la inyección de dependencias para consumir la capa de servicios (`ITodoService`).
    -   Mapea los datos de las solicitudes a modelos (`record CreateTodoItemRequest`) y los resultados del servicio a respuestas estandarizadas.

2.  **Modelo de Respuesta Estandarizado (`ApiResponse<T>`)**:
    Para asegurar que todas las respuestas de la API tengan una estructura consistente, se utiliza una clase genérica `ApiResponse<T>`. Esto facilita el consumo del lado del cliente.

    ```csharp
    // BeyondTodoApi/Models/Response/ApiResponse.cs
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
    ```

    Cada endpoint devuelve sus resultados envueltos en esta clase, indicando si la operación fue exitosa (`Success`), un mensaje descriptivo y los datos resultantes.

    ```csharp
    // Ejemplo en un endpoint
    return result.IsSuccess
        ? Results.Ok(ApiResponse<bool>.SuccessResponse(result.Value, "Operación exitosa."))
        : Results.BadRequest(ApiResponse<bool>.ErrorResponse(result.Error));
    ```

## El Dominio Compartido (`BeyondTodoDomain`)

El proyecto `BeyondTodoDomain` es el **corazón de la solución** y representa la capa de dominio en una arquitectura limpia. Contiene toda la lógica y las reglas de negocio, y es completamente independiente de la tecnología de la base de datos o de la interfaz de usuario.

### Uso Común para API y Consola

Una de las mayores ventajas de esta arquitectura es que el dominio se comparte entre múltiples aplicaciones cliente. Tanto `BeyondTodoApi` como la aplicación de consola `BeyondTodoApp` utilizan `BeyondTodoDomain` (a través de `BeyondTodoApiService` y de `BeyondTodoServices` respectivamente) para ejecutar sus operaciones.

-   **Consistencia**: Todas las reglas de negocio, como la validación de un `TodoItem` o la lógica para registrar un progreso, están definidas en un solo lugar. Esto garantiza que la lógica sea consistente sin importar si la acción se origina desde una llamada a la API o desde la aplicación de consola.
-   **Reutilización**: Se evita la duplicación de código. La lógica de negocio no necesita ser reescrita para diferentes interfaces.
-   **Mantenibilidad**: Cualquier cambio en las reglas de negocio se realiza en `BeyondTodoDomain` y se refleja automáticamente en todas las aplicaciones que lo consumen.

### Componentes Clave del Dominio

-   **Entidades**: Como `TodoItem`, que representa un objeto de negocio con una identidad única.
-   **Agregados**: `TodoListAggregate` agrupa las entidades `TodoItem` y actúa como un guardián de la consistencia, asegurando que solo se realicen operaciones válidas.
-   **Patrón Result**: La clase `Result<T>` se utiliza para manejar los resultados de las operaciones de una manera explícita, evitando el uso de excepciones para el control de flujo.

## Uso del Cliente HTTP (`BeyondTodoApi.http`)

Para facilitar las pruebas de la API, el proyecto incluye el archivo `BeyondTodoApi.http`. Este archivo puede ser utilizado con extensiones de VS Code como [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) para enviar solicitudes directamente desde el editor.

### Cómo Usarlo

1.  **Instala la extensión REST Client** en Visual Studio Code.
2.  **Inicia la API**: Ejecuta el proyecto `BeyondTodoApi`. Por defecto, estará disponible en `http://localhost:5209`.
3.  **Abre el archivo `BeyondTodoApi.http`**: Verás que aparece un enlace "Send Request" encima de cada solicitud.
4.  **Envía las solicitudes**: Haz clic en "Send Request" para ejecutar la llamada al endpoint correspondiente. La respuesta aparecerá en un nuevo panel.

### Estructura del Archivo

-   **Variable de Entorno**: La línea `@BeyondTodoApi_HostAddress = http://localhost:5209` define la URL base para todas las solicitudes.
-   **Separador de Solicitudes**: Cada solicitud está separada por `###`.
-   **Ejemplos de Endpoints**: El archivo contiene ejemplos para cada uno de los endpoints de la API:
    -   `POST /todos`: Crear un nuevo To-Do.
    -   `GET /todos`: Obtener la lista de todos los To-Dos.
    -   `PATCH /todos/{id}/progress`: Registrar un progreso en un To-Do.
    -   `PATCH /todos/{id}/description`: Actualizar la descripción.
    -   `DELETE /todos/{id}`: Eliminar un To-Do.

```http
# Ejemplo de una solicitud DELETE
DELETE {{BeyondTodoApi_HostAddress}}/todos/1
Content-Type: application/json
```

**Nota**: Recuerda cambiar los `id` en las rutas de los endpoints (ej. `/todos/1`) a valores que existan en tu lista de To-Dos.
