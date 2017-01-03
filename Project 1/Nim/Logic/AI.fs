module AI

let heapOp hl =
    let m = Array.fold (^^^) 0 hl
    let hl' = Array.copy hl
    match m with
    | 0 -> let maxv = Array.max hl 
           let maxi = Array.findIndex (fun v -> v=maxv) hl
           hl'.[maxi] <- 0;
    | _ -> let vi = Array.findIndex (fun v -> v ^^^ m < v) hl
           hl'.[vi] <- hl.[vi]^^^m
    |> ignore
    hl'
           
let opponentAI g = match g with
                    | (hl, o, p) -> (heapOp hl, o, p)

