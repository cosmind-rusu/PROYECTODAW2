# 🎨 Diseño de la Aplicación de Finanzas Personales

Este documento describe las elecciones de diseño para nuestra aplicación de finanzas personales.

## 🎭 Paleta de Colores

Hemos elegido una paleta minimalista con colores que transmiten claridad y confianza:

- **Principal (`$primary-color`)**: #42b983 - Verde menta, transmite crecimiento y estabilidad financiera
- **Secundario (`$secondary-color`)**: #35495e - Azul oscuro, aporta profesionalidad y seriedad
- **Fondo (`$background-color`)**: #f9f9f9 - Gris muy claro, ofrece contraste suave y limpieza visual
- **Texto (`$text-color`)**: #333333 - Casi negro, proporciona buena legibilidad

Además, usamos colores semánticos para estados específicos:
- **Gastos**: #e74c3c (Rojo)
- **Ingresos**: #2ecc71 (Verde)
- **Inactivo**: #ffebee (Rojo claro)
- **Activo**: #e8f5e9 (Verde claro)

## ✏️ Tipografía

Utilizamos una jerarquía tipográfica sencilla:

- **Familia principal**: "Helvetica Neue", Arial, sans-serif
  - Elegida por su alta legibilidad y aspecto moderno
  - Funciona bien tanto en títulos como en textos largos

- **Tamaños**:
  - Títulos principales (h1, h2): 1.5rem - 2rem
  - Subtítulos (h3): 1.2rem
  - Cuerpo de texto: 1rem (16px)
  - Texto secundario: 0.9rem
  - Texto terciario: 0.8rem

## 📐 Espaciado

Para mantener consistencia, usamos un sistema de espaciado basado en una unidad base:

- **Unidad base (`$spacing-unit`)**: 1rem (16px)
- **Elementos relacionados**: $spacing-unit / 2 (8px)
- **Separación entre secciones**: $spacing-unit * 2 (32px)
- **Padding de tarjetas y contenedores**: $spacing-unit (16px)
- **Gap entre elementos de formulario**: $spacing-unit (16px)

Este sistema modular facilita el mantenimiento y ajuste del diseño.

## 🧰 Mixins

Hemos creado mixins reutilizables para mantener consistencia:

- **center-flex**: Centra elementos horizontal y verticalmente
  ```scss
  @mixin center-flex {
    display: flex;
    justify-content: center;
    align-items: center;
  }
  ```

- **card**: Estiliza contenedores tipo tarjeta
  ```scss
  @mixin card($padding: $spacing-unit, $bg: #fff) {
    padding: $padding;
    background-color: $bg;
    border-radius: 4px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  }
  ```

## 📱 Diseño responsivo

- Utilizamos un diseño flexible con media queries para dispositivos móviles
- Las tarjetas de presupuesto usan CSS Grid con `auto-fill` para adaptarse al espacio disponible
- Los formularios tienen `flex-wrap: wrap` para reorganizarse en pantallas pequeñas
