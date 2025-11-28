open System
open System.IO
open System.Collections.Generic

// ==========================================
// MÓDULO CSP (Basado en la plantilla del PDF)
// ==========================================
module CSP =
    // Definición de tipos basada en la plantilla [cite: 47-55]
    type Variable = string
    type Valor = string
    
    // El estado es un mapa de Variables a su valor asignado (si existe)
    type Asignacion = Map<Variable, Valor>

    type Restriccion = {
        Var1 : Variable
        Var2 : Variable
    }

    type Csp = {
        Variables : Variable list
        Dominios : Map<Variable, Valor list>
        Restricciones : Restriccion list
        // Mapa de adyacencias para búsqueda rápida (Variable -> Lista de vecinos)
        Vecinos : Map<Variable, Variable list> 
    }

    // Estructura para el resultado de la búsqueda
    type ResultadoBusqueda = {
        Exito : bool
        Asignaciones : int
        Solucion : Asignacion option
        Metodo : string
    }

    // --- Funciones Auxiliares ---

    // Verifica si una asignación es consistente con las restricciones
    let esConsistente (var: Variable) (valAsignado: Valor) (asignacion: Asignacion) (csp: Csp) =
        // Obtenemos los vecinos de la variable actual
        let vecinos = 
            match csp.Vecinos.TryFind var with
            | Some v -> v
            | None -> []
        
        // Verificamos que ningún vecino ya asignado tenga el mismo color
        vecinos |> List.forall (fun vecino ->
            match asignacion.TryFind vecino with
            | Some valVecino -> valVecino <> valAsignado // La restricción es !=
            | None -> true // Si el vecino no tiene color, no hay conflicto aún
        )

    // Construir mapa de vecinos (grafo) a partir de la lista plana de restricciones
    let construirGrafo (vars: Variable list) (rests: Restriccion list) =
        let initMap = vars |> List.map (fun v -> v, []) |> Map.ofList
        rests |> List.fold (fun acc rest ->
            let v1, v2 = rest.Var1, rest.Var2
            let map1 = acc |> Map.add v1 (v2 :: (acc.[v1]))
            let map2 = map1 |> Map.add v2 (v1 :: (map1.[v2]))
            map2
        ) initMap

// ==========================================
// LECTURA DE ARCHIVOS [cite: 66-115]
// ==========================================
// ==========================================
// LECTURA DE ARCHIVOS (CORREGIDO)
// ==========================================
module Parser =
    open CSP

    let leerArchivo (ruta: string) =
        let lineas = File.ReadAllLines(ruta) |> Array.toList
        
        let rec procesar (estado: string) (vars: Variable list) (doms: Map<Variable, Valor list>) (rests: Restriccion list) (resto: string list) =
            match resto with
            | [] -> (vars, doms, rests)
            | x::xs ->
                let linea = x.Trim()
                if linea = "" then procesar estado vars doms rests xs
                else if linea.StartsWith("variables{") then procesar "VARS" vars doms rests xs
                else if linea.StartsWith("dominios{") then procesar "DOMS" vars doms rests xs
                else if linea.StartsWith("restricciones{") then procesar "RESTS" vars doms rests xs
                // --- CORRECCIÓN AQUÍ: Usamos '=' en lugar de 'EndsWith' ---
                else if linea = "}" then procesar "NADA" vars doms rests xs
                // -----------------------------------------------------------
                else
                    match estado with
                    | "VARS" -> 
                        procesar estado (linea :: vars) doms rests xs
                    | "DOMS" ->
                        // Formato: WA={rojo, verde, azul}
                        let partes = linea.Split('=')
                        let var = partes.[0].Trim()
                        let valoresRaw = partes.[1].Replace("{", "").Replace("}", "").Split(',')
                        let valores = valoresRaw |> Array.map (fun s -> s.Trim()) |> Array.toList
                        procesar estado vars (doms.Add(var, valores)) rests xs
                    | "RESTS" ->
                        // Formato: WA!=NT
                        let partes = linea.Split([|"!="|], StringSplitOptions.None)
                        let r = { Var1 = partes.[0].Trim(); Var2 = partes.[1].Trim() }
                        procesar estado vars doms (r :: rests) xs
                    | _ -> procesar estado vars doms rests xs

        let (v, d, r) = procesar "NADA" [] Map.empty [] lineas
        
        // Retornar el objeto CSP completo
        {
            Variables = List.rev v 
            Dominios = d
            Restricciones = r
            Vecinos = construirGrafo (List.rev v) r
        }

// ==========================================
// ALGORITMOS DE SOLUCIÓN (Vuelta Atrás y Heurísticas)
// ==========================================
module Solver =
    open CSP

    // --- Heurísticas ---

    // MRV: Mínimos Valores Restantes [cite: 45]
    // Elige la variable con menos valores legales en su dominio
    let heuristicaMRV (varsNoAsignadas: Variable list) (asignacion: Asignacion) (csp: Csp) =
        varsNoAsignadas 
        |> List.minBy (fun var -> 
            // Calcular cuántos valores del dominio son válidos dada la asignación actual
            csp.Dominios.[var] 
            |> List.filter (fun valor -> esConsistente var valor asignacion csp)
            |> List.length
        )

    // Grado Heurístico (Degree Heuristic) [cite: 45]
    // Se usa para desempatar MRV: elige la variable involucrada en más restricciones con otras variables NO asignadas
    let heuristicaGrado (vars: Variable list) (asignacion: Asignacion) (csp: Csp) =
        vars 
        |> List.maxBy (fun var ->
            csp.Vecinos.[var]
            |> List.filter (fun vecino -> not (asignacion.ContainsKey vecino))
            |> List.length
        )

    let seleccionarVariableHeuristica (varsNoAsignadas: Variable list) (asignacion: Asignacion) (csp: Csp) =
    // 1. Calcular tamaño de dominios válidos para cada variable
        let mrvMap = 
            varsNoAsignadas 
            |> List.map (fun var -> 
                // --- INICIO DE BLOQUE DE SEGURIDAD ---
                if not (csp.Dominios.ContainsKey var) then
                    printfn "\n[ERROR FATAL EN EL ARCHIVO DE TEXTO]"
                    printfn "La variable '%s' está listada en la sección 'variables{}'..." var
                    printfn "...pero NO se encontró su definición en la sección 'dominios{}'."
                    printfn "Por favor, revisa que esté escrita IDÉNTICAMENTE en ambas secciones.\n"
                    failwith "Deteniendo programa por error en datos de entrada."
                // --- FIN DE BLOQUE DE SEGURIDAD ---

                let count = csp.Dominios.[var] |> List.filter (fun v -> esConsistente var v asignacion csp) |> List.length
                (var, count))
        
        let minCount = mrvMap |> List.map snd |> List.min
        
        // 2. Filtrar las variables que empatan con el mínimo valor (MRV)
        let candidatosMRV = mrvMap |> List.filter (fun (_, c) -> c = minCount) |> List.map fst

        match candidatosMRV with
        | [unico] -> unico
        | varios -> heuristicaGrado varios asignacion csp // 3. Desempatar con Grado
    // --- Motor de Backtracking ---
    
    // Función recursiva de Vuelta Atrás (DFS)
    let rec backtracking (asignacion: Asignacion) (csp: Csp) (contador: int ref) (usarHeuristicas: bool) : Asignacion option =
        // Si la asignación está completa, terminamos
        if asignacion.Count = csp.Variables.Length then
            Some asignacion
        else
            // Determinar qué variable sigue
            let varsNoAsignadas = csp.Variables |> List.filter (fun v -> not (asignacion.ContainsKey v))
            
            let variableAProcesar = 
                if usarHeuristicas then
                    seleccionarVariableHeuristica varsNoAsignadas asignacion csp
                else
                    List.head varsNoAsignadas // Selección simple (primer programa)

            // Iterar sobre los valores del dominio
            let valoresDominio = csp.Dominios.[variableAProcesar]
            
            // Función interna para iterar valores (simulando bucle for)
            let rec probarValores valores =
                match valores with
                | [] -> None // Falla, backtrack
                | valor :: resto ->
                    if esConsistente variableAProcesar valor asignacion csp then
                        contador := !contador + 1 // Contamos nodo procesado/asignación [cite: 132]
                        let nuevaAsignacion = asignacion.Add(variableAProcesar, valor)
                        let resultado = backtracking nuevaAsignacion csp contador usarHeuristicas
                        match resultado with
                        | Some sol -> Some sol
                        | None -> probarValores resto // Si falla más abajo, probamos siguiente color
                    else
                        probarValores resto

            probarValores valoresDominio

    // Función principal de resolución
    let resolver (csp: Csp) (metodo: string) =
        let contador = ref 0
        let usarHeur = (metodo = "heuristicas")
        let solucion = backtracking Map.empty csp contador usarHeur
        
        let nombreMetodo = 
            if usarHeur then "Vuelta atrás con heurísticas" 
            else "Vuelta atrás"

        {
            Exito = solucion.IsSome
            Asignaciones = !contador
            Solucion = solucion
            Metodo = nombreMetodo
        }

// ==========================================
// PROGRAMA PRINCIPAL [cite: 148-154]
// ==========================================
module Main =
    open Parser
    open CSP
    open Solver

    [<EntryPoint>]
    let main args =
        if args.Length < 1 then
            printfn "Uso: programa <archivo_problema.txt> [simple|heuristicas]"
            1
        else
            let rutaArchivo = args.[0]
            // Por defecto usa heurísticas si no se especifica, o según argumento
            let modo = if args.Length > 1 then args.[1] else "heuristicas" 

            if not (File.Exists rutaArchivo) then
                printfn "El archivo no existe."
                1
            else
                let csp = leerArchivo rutaArchivo
                let resultado = resolver csp modo

                // Salida formateada según PDF [cite: 119-144]
                printfn "Metodo de Solucion = %s" resultado.Metodo
                printfn "Existe solucion"
                if resultado.Exito then printfn "Verdadero" else printfn "Falso"
                printfn "Asignaciones = %d" resultado.Asignaciones
                
                match resultado.Solucion with
                | Some asignaciones ->
                    asignaciones 
                    |> Map.toList 
                    |> List.iter (fun (k, v) -> printfn "%s=%s" k v)
                | None -> ()
                
                0