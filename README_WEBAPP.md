# BeyondTodoWebApp

## Descripción General

Este proyecto es el frontend de la aplicación BeyondTodoApp, una interfaz de usuario moderna y reactiva diseñada para administrar tareas (Todo items). Permite a los usuarios ver, crear, editar, eliminar y seguir el progreso de sus tareas de una manera intuitiva.

La aplicación está construida como una Single Page Application (SPA) que se comunica con un backend a través de una API REST para la persistencia de los datos.

## Arquitectura y Tecnologías

La aplicación sigue una arquitectura moderna de frontend basada en componentes, aprovechando las siguientes tecnologías clave:

### Framework Principal

- **[Nuxt.js 3](https://nuxt.com/)**: Es un framework de código abierto para Vue.js que facilita la creación de aplicaciones universales, renderizadas en el servidor (SSR) o sitios estáticos (SSG). En este caso, se utiliza para proporcionar una estructura robusta, enrutamiento basado en ficheros y una excelente experiencia de desarrollo.

### Lenguaje

- **[TypeScript](https://www.typescriptlang.org/)**: Todo el código de la aplicación está escrito en TypeScript, lo que añade tipado estático a JavaScript. Esto mejora la calidad del código, facilita el mantenimiento y reduce los errores en tiempo de ejecución.

### Librería de UI

- **[Vuetify 3](https://vuetifyjs.com/)**: Es un framework de componentes de UI para Vue.js que sigue las especificaciones de Material Design de Google. Proporciona una amplia gama de componentes preconstruidos y personalizables que se utilizan para construir la interfaz de usuario de la aplicación.

### Iconos

- **[Material Design Icons (MDI)](https://materialdesignicons.com/)**: Se utiliza el conjunto de iconos `@mdi/font` para proporcionar una iconografía consistente y de alta calidad en toda la aplicación, en línea con los principios de Material Design.

### Comunicación con API

- **[useFetch](https://nuxt.com/docs/api/composables/use-fetch)**: La comunicación con el backend se gestiona a través del composable `useFetch` de Nuxt.js. Este se encarga de realizar las peticiones HTTP a la API para obtener, crear, actualizar y eliminar datos. La URL base de la API se configura en `nuxt.config.ts`.

## Estructura del Proyecto

El proyecto sigue la estructura estándar de una aplicación Nuxt.js:

- **`pages/`**: Contiene las vistas y rutas de la aplicación. `index.vue` es la página principal.
- **`plugins/`**: Se utiliza para registrar plugins de Vue, como `vuetify.ts`, que integra Vuetify en la aplicación.
- **`nuxt.config.ts`**: Fichero de configuración principal de Nuxt.js, donde se definen módulos, plugins, build options y configuración del entorno.
- **`package.json`**: Define los scripts del proyecto y gestiona las dependencias de Node.js.
- **`app.vue`**: Es el componente raíz de la aplicación, donde se define la estructura global y se inyecta el componente `v-app` de Vuetify.

## Componentes Principales (`pages/index.vue`)

La vista principal de la aplicación se encuentra en `pages/index.vue` y es responsable de la mayor parte de la funcionalidad de la UI.

### Lógica del Componente (`<script setup>`)

- **Gestión de Estado**: Utiliza la Composition API de Vue 3 con `<script setup>` para gestionar el estado del componente de forma local (ej. `ref`, `computed`).
- **Peticiones a la API**: Realiza peticiones a la API para:
  - Obtener la lista de todos los `Todo Items`.
  - Crear un nuevo `Todo Item`.
  - Editar la descripción de un `Todo Item` existente.
  - Añadir un nuevo registro de progreso a un `Todo Item`.
  - Eliminar un `Todo Item`.
- **Manejo de Diálogos**: Controla la visibilidad y el estado de los diálogos modales para la creación y edición de tareas.

### UI y Componentes de Vuetify (`<template>`)

La interfaz se construye utilizando una combinación de componentes de Vuetify para ofrecer una experiencia de usuario rica y funcional:

- **`v-data-table-server`**: Componente principal para mostrar la lista de tareas en una tabla paginada y ordenable.
- **`v-toolbar`**: Se usa para mostrar el título de la sección y el botón de "Añadir Item".
- **`v-btn`**, **`v-icon`**: Utilizados para acciones como añadir, editar, eliminar y actualizar el progreso.
- **`v-progress-linear`**: Muestra el progreso actual de cada tarea con un color dinámico que cambia según el porcentaje.
- **`v-dialog`** y **`v-card`**: Se utilizan para crear los modales que permiten al usuario introducir datos para crear o editar tareas.
- **`v-text-field`**, **`v-combobox`**: Campos de formulario para la entrada de texto y selección de categorías.

## Configuración y Scripts

- **Instalación**: `npm install`
- **Desarrollo**: `npm run dev` (inicia un servidor en `http://localhost:3000`)
- **Build**: `npm run build` (compila la aplicación para producción)
- **Preview**: `npm run preview` (previsualiza el build de producción localmente)
