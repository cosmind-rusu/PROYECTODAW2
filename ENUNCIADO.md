Propuesta de trabajo 
Se debe implementar una aplicación web/API que gestione información almacenándola y recuperándola de una Base de Datos Relacional. Se pensará en un supuesto real, se redactará y se creará la aplicación web/API para gestionar toda la información.
El supuesto real que se prepare como proyecto deberá contener al menos 2 elementos de los que se almacene información (lo que equivaldría a 2 entidades/clases en el modelo Entidad/Relación o Modelo de clases). A su vez, cada uno de esos 2 elementos/entidades deberá tener, al menos, 5 atributos (de tipos cadena de texto, numérico, fecha y boolean).
Se proponen una serie de Requisitos obligatorios que deben ser implementados y otra serie de funcionalidades de entre las que se puede elegir hasta completar la nota.
Todo el desarrollo del proyecto se llevará a cabo utilizando un repositorio en GitHub bajo metodología gitflow y el gestor de Issues que dispone para registrar y gestionar las diferentes tareas e incidencias.
Además, se realizará un seguimiento exhaustivo de las tareas realizadas por cada componente del grupo utilizando Trello (https://www.trello.com).
Requisitos (1 pto cada uno, obligatorios)
Funcionalidad (API): Diseña e implementa una API con una correcta arquitectura RESTful, métodos HTTP para las operaciones CRUD y códigos de estado HTTP
Funcionalidad (web): La aplicación web debe listar el contenido de, al menos, un par de clases del modelo de datos en dos páginas diferentes. Se deberá crear la aplicación mediante Vue 2 y, deberá utilizarse vue-router para la configuración de las rutas de la aplicación
Despliegue: Despliega la aplicación web en la nube de AWS.
DevOps: Conteneriza la API y la base de datos e integrales en el proyecto local
Diseño: Realiza la maquetación del sitio usando Sass, con al menos un mixin y explica la elección de colores, fuentes y espacios del sitio web
Otras funcionalidades (1 pto cada una)
Funcionalidad I: Añade funcionalidades de búsqueda que permita filtrar y ordenar los resultados por, al menos, un campo.
Funcionalidad II: Amplía la aplicación web para que sea posible registrar información (mediante formularios) y eliminar la existente.
Funcionalidad (API): Aplica buenas prácticas de programación como una arquitectura por capas e inyección de dependencias.
Funcionalidad (web): Utiliza Vuex para la gestión centralizada del estado de la aplicación
Despliegue: Cambia la arquitectura de la base de datos local y usa el servicio AWS RDS de tal forma que la base de datos de tu aplicación esté en el servicio administrado de base de datos de AWS y tu código en la instancia de EC2.
DevOps I: Crea la BBDD en la nube de AWS e intégrala en el proyecto
DevOps II: Ejecuta los contenedores conjuntamente mediante un orquestador de contenedores en tu entorno local: Docker Compose, Docker Swarm, Kubernetes
DevOps III: Configura dos entornos de trabajo en local para trabajar el proyecto simultáneamente: PRE, PRO.
DevOps IV: Despliega el proyecto mediante CI/CD en la nube de AWS. Puedes usar una base de datos en memoria si no se dispone de una BBDD en AWS.
Diseño I: Creación de wireframes con sketchy wireframes (Figma) antes de empezar el desarrollo.
Diseño II: Implementa Google Analytics y GTM. Medir como mínimo PageView, interacción con menú y Scroll
Diseño III: Creación de un test a/b, libre pero justificado
Test: Crea una colección de Postman con peticiones HTTP de ejemplo para probar la aplicación.
Inglés: Traduce todos los textos de la aplicación web para que se muestre completamente en inglés.