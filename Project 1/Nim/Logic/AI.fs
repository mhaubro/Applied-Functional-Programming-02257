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
    hl';;

let opponentAI g = match g with
                    | (hl, o, p) -> (heapOp hl, o, p);;

let optimalMove heapArray =
    let m = Array.fold (^^^) 0 heapArray
    match m with
    | 0 -> let maxv = Array.max heapArray 
           let maxi = Array.findIndex (fun v -> v=maxv) heapArray
           (maxi, maxv)
    | _ -> let vi = Array.findIndex (fun v -> v ^^^ m < v) heapArray
           (vi, heapArray.[vi] - (heapArray.[vi]^^^m));;

let getAIMove = function | heapArray -> optimalMove heapArray;;