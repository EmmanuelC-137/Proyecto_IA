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
el coloreo de mapas de México. Implementa:
- Algoritmo de Vuelta Atrás (Backtracking).
- Heurísticas: Mínimos Valores Restantes (MRV) y Grado Heurístico.

El software ha sido desarrollado en F# utilizando la plataforma .NET moderna
(versión 10.0), lo cual ofrece mayor rendimiento y compatibilidad que las 
versiones antiguas de Mono.

-----------------------------------------------------------------------------
2. REQUERIMIENTOS
-----------------------------------------------------------------------------
- .NET SDK (Versión 8.0, 9.0 o 10.0).
- Compatible con Windows, Linux y macOS.

-----------------------------------------------------------------------------
3. INSTRUCCIONES PARA GENERAR EL EJECUTABLE (.EXE)
-----------------------------------------------------------------------------
Para compilar el código fuente y generar un ejecutable optimizado (Release),
abra una terminal en la carpeta del proyecto y ejecute:

    dotnet publish -c Release

Esto creará el archivo 'ColoreoMapa.exe' en la carpeta:
    /bin/Release/net10.0/publish/

**NOTA: El comando dotnet "publish -c Release" se debe ejecutar en la carpeta donde se encuentra
        el proyecto, en este caso ../ColoreoMapa/

**NOTA 2: Este paso solo es necesario si no se encuentra el
        archivo en la ruta /bin/Release/net10.0/publish/
-----------------------------------------------------------------------------
4. EJECUCIÓN
-----------------------------------------------------------------------------
Existen dos formas de ejecutar el programa:

OPCIÓN A (Recomendada para desarrollo - Compila y ejecuta):
    dotnet run -- <archivo.txt> <modo>

OPCIÓN B (Ejecutar el .exe generado en el paso 3):
    1. Navegue a la carpeta del ejecutable:
       cd bin/Release/net10.0/publish
    2. Copie el archivo de texto (ej. mexico_4_colores.txt) a esta carpeta.
    3. Ejecute:
       ColoreoMapa.exe <archivo.txt> <modo>

**NOTA: Este paso solo es necesario si no se encuentra el/los
        archivo(s) en la ruta /bin/Release/net10.0/publish/

-----------------------------------------------------------------------------
5. PARÁMETROS
-----------------------------------------------------------------------------
El <modo> puede ser:
    - simple        (Para Backtracking estándar)
    - heuristicas   (Para Backtracking con MRV y Grado)

EJEMPLO:
    dotnet run -- mexico_4_colores.txt heuristicas

-----------------------------------------------------------------------------
ARCHIVOS INCLUIDOS
-----------------------------------------------------------------------------
- Código fuente (Program.fs)
- Archivo de proyecto (ColoreoMapa.fsproj)
- Archivos de prueba (mexico_2_colores.txt, mexico_3_colores.txt, etc.)