=============================================================================
PROYECTO 2: PROBLEMA DEL COLOREO DEL MAPA (CSP)
Inteligencia Artificial I - Universidad Politécnica de San Luis Potosí
=============================================================================

AUTOR: Emmanuel Hernández Méndez
MATRÍCULA: 179169
FECHA: 28 de Noviembre de 2025

-----------------------------------------------------------------------------
1. DESCRIPCIÓN
-----------------------------------------------------------------------------
Este programa resuelve el problema de satisfacción de restricciones (PSR) para
el coloreo de mapas. Implementa dos métodos de solución:

1. Vuelta Atrás (Backtracking) simple.
2. Vuelta Atrás con Heurísticas (Mínimos Valores Restantes + Grado Heurístico).

El programa lee un archivo de texto con las variables, dominios y restricciones,
y determina si existe una asignación de colores válida.

-----------------------------------------------------------------------------
2. REQUERIMIENTOS DEL SISTEMA
-----------------------------------------------------------------------------
- .NET SDK (Versión 8.0 o superior, probado en .NET 10.0).
- Sistema Operativo: Windows, Linux o macOS.

-----------------------------------------------------------------------------
3. INSTRUCCIONES PARA GENERAR EL EJECUTABLE
-----------------------------------------------------------------------------
Para compilar el código fuente y generar los binarios, abra una terminal en
la carpeta del proyecto (donde está el archivo .fsproj) y ejecute:

    dotnet build -c Release

El archivo ejecutable (.exe) se generará en la ruta:
    .\bin\Release\net10.0\ColoreoMapa.exe

-----------------------------------------------------------------------------
4. EJECUCIÓN DESDE LÍNEA DE COMANDOS
-----------------------------------------------------------------------------
El programa acepta dos argumentos: el archivo de entrada y el modo de solución.

Sintaxis:
    dotnet run -- <archivo_entrada> <modo>

Donde <modo> puede ser:
    - simple        (Para Backtracking estándar)
    - heuristicas   (Para Backtracking con MRV y Grado)

EJEMPLOS DE USO:

A) Para resolver el mapa de México con 3 colores usando heurísticas:
    dotnet run -- mexico_3_colores.txt heuristicas

B) Para resolver el mapa de México con 4 colores usando búsqueda simple:
    dotnet run -- mexico_4_colores.txt simple

-----------------------------------------------------------------------------
5. ARCHIVOS INCLUIDOS EN LA ENTREGA
-----------------------------------------------------------------------------
1. Program.fs             - Código fuente en F#.
2. ColoreoMapa.fsproj     - Archivo de proyecto .NET.
3. README.txt             - Instrucciones de uso.
4. mexico_2_colores.txt   - Caso de prueba (Sin solución).
5. mexico_3_colores.txt   - Caso de prueba (Sin solución).
6. mexico_4_colores.txt   - Caso de prueba (Con solución).
7. mexico_5_colores.txt   - Caso de prueba (Con solución).

-----------------------------------------------------------------------------
NOTAS SOBRE LA SALIDA
-----------------------------------------------------------------------------
Si la solución existe, el programa imprimirá:
- El método utilizado.
- "Existe solucion Verdadero".
- El número de asignaciones (pasos) realizados.
- La lista de variables con su color asignado.

Si no existe solución:
- "Existe solucion Falso".
- El número de asignaciones realizadas antes de determinar la imposibilidad.