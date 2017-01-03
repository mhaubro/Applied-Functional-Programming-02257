module AI

//Replace the first element of a list of elements that meets a criterion
let rec critListReplace crit rep l = match l with
                                        | [] -> []
                                        | v::l' when crit v -> rep v::l'
                                        | v::l' -> v::critListReplace crit rep l';;

//Perform mathematically optimal move - in the Nim game - on a list of heaps
let heapOp hl =
    let m = List.fold (^^^) 0 hl
    match m with
    | 0 -> let maxv = List.max hl
           critListReplace (fun v -> v=maxv) (fun v -> v-1) hl
    | _ -> critListReplace (fun v -> v ^^^ m < v) (fun v -> v ^^^ m) hl;;

//Perfom optimal move in a game of Nim
let opponentAI g = match g with
                    | (hl, o, p) -> (heapOp hl, o, p)

