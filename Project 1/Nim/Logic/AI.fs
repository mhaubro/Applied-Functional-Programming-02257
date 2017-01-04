module AI

let optimalMove heapArray =
    let m = Array.fold (^^^) 0 heapArray
    match m with
    | 0 -> let maxv = Array.max heapArray 
           let maxi = Array.findIndex (fun v -> v=maxv) heapArray
           (maxi, maxv)
    | _ -> let vi = Array.findIndex (fun v -> v ^^^ m < v) heapArray
           (vi, heapArray.[vi] - (heapArray.[vi]^^^m));;

let getAIMove = function | heapArray -> optimalMove heapArray;;