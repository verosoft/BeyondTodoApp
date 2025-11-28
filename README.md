# BeyondTodoApp

Bienvenido a BeyondTodoApp. Este proyecto es una aplicación de lista de tareas (To-Do) desarrollada en .NET/C# con un enfoque en la arquitectura limpia y los principios de Diseño Guiado por el Dominio (DDD). Sirve como un ejemplo práctico para desarrolladores que buscan entender cómo estructurar una aplicación de forma robusta, mantenible y escalable.

## Arquitectura del Proyecto

El proyecto está organizado siguiendo una arquitectura en capas, también conocida como Arquitectura Limpia (Clean Architecture). El objetivo principal es la separación de responsabilidades, aislando la lógica de negocio de los detalles de infraestructura y presentación.

```
+------------------------+
|   BeyondTodoApp (Host) |
+------------------------+
             |
             v
+------------------------+
| BeyondTodoServices     |
+------------------------+
             |
             v
+------------------------+      +--------------------------+
|   BeyondTodoDomain     <------+ BeyondTodoInfrastructure |
+------------------------+      +--------------------------+
```

- **`BeyondTodoDomain`**: Es el núcleo de la aplicación. Contiene toda la lógica de negocio, incluyendo entidades, agregados y las interfaces de los repositorios. Este proyecto no tiene dependencias de ninguna otra capa de la aplicación, garantizando que la lógica de negocio sea pura y agnóstica a la tecnología.

- **`BeyondTodoInfrastructure`**: Implementa los contratos (interfaces) definidos en el dominio. Se encarga de la persistencia de datos (por ejemplo, comunicación con una base de datos) y otros servicios externos. Permite intercambiar la tecnología de base de datos sin afectar la lógica de negocio.

- **`BeyondTodoServices`**: Actúa como una capa de orquestación. Utiliza las abstracciones del dominio para ejecutar casos de uso y lógica de la aplicación. Es el puente entre la capa de presentación y el dominio.

- **`BeyondTodoApp`**: Es el punto de entrada y la capa de presentación. Puede ser una API web, una aplicación de consola o cualquier otro tipo de interfaz de usuario. Su responsabilidad es recibir las solicitudes, pasarlas a los servicios de aplicación y devolver una respuesta.

- **`BeyondTodoApp.Tests`**: Contiene pruebas unitarias y de integración para asegurar la calidad y el correcto funcionamiento del código, especialmente de la lógica de negocio en el dominio.

## Conceptos Clave y Patrones

### Patrón Aggregate (Agregado de DDD)

Este patrón es fundamental en el diseño del dominio. Un agregado es un clúster de objetos de dominio (entidades y objetos de valor) que se pueden tratar como una única unidad.

- **`TodoListAggregate`**: Es la raíz del agregado en este proyecto. Agrupa y gestiona una colección de entidades `TodoItem`.
- **Regla fundamental**: Cualquier referencia a los objetos dentro del agregado debe pasar por la raíz (`TodoListAggregate`). Esto asegura que todas las reglas de negocio e invariantes se cumplan antes de realizar cualquier cambio de estado. Por ejemplo, para añadir un `TodoItem`, se debe llamar al método `AddItem` del agregado, que contiene la lógica de validación necesaria.

### Patrón Result

Para manejar los resultados de las operaciones (éxito o fallo) de una manera explícita y sin depender de excepciones, el proyecto incluye una clase `Result<T>`.

- **Propósito**: Encapsula el resultado de una operación. Una instancia de `Result<T>` contendrá el valor de retorno en caso de éxito (`IsSuccess = true`) o un mensaje de error en caso de fallo (`IsFailure = true`).
- **Ventajas**: Obliga al código cliente a manejar ambos casos (éxito y error), lo que resulta en un software más robusto y predecible. Evita el uso de excepciones para controlar el flujo lógico.
- **Observación**: Aunque el patrón `Result` está disponible en el dominio, el `TodoListAggregate` actual utiliza un enfoque basado en excepciones. Esto podría ser un punto de refactorización para alinear todo el manejo de errores a una única estrategia.

```csharp
// Ejemplo de la clase Result
public class Result<T>
{
    public T? Value { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }

    public static Result<T> Success(T value) => new(value, true, null);
    public static Result<T> Failure(string error) => new(default, false, error);
}
```

## Cómo Empezar

Sigue estos pasos para compilar y ejecutar el proyecto en tu máquina local.

### Prerrequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Compilación

Para compilar todos los proyectos y restaurar los paquetes NuGet, ejecuta el siguiente comando desde la raíz del repositorio:

```bash
dotnet build
```

### Ejecutar Pruebas

Para asegurar que toda la lógica de negocio funciona como se espera, puedes ejecutar el conjunto de pruebas:

```bash
dotnet test
```

### Ejecutar la Aplicación

Para iniciar la aplicación principal, ejecuta el siguiente comando:

```bash
dotnet run --project BeyondTodoApp/BeyondTodoApp.csproj
```
