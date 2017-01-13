module Extents
    type Extent = (float * float) list

    let moveextent = function (e:Extent, x) -> List.map (fun (a, b) -> (a + x,b + x)) e

    let rec merge a b = match a, b with
                             | [], qs->qs
                             | ps, []->ps
                             | (p,_)::ps, (_,q)::qs->(p,q):: merge ps qs
    let mergelist list =  List.foldBack (merge) list []

    let rmax a b =if a>b then a else b

    let rec fit a b = match a, b with
                             | (_,p)::ps, (q,_)::qs -> rmax (fit ps qs) (p-q+1.0)
                             | _, _ -> 0.0

    let rec fitl acc l = match l with 
                             | [] ->[]
                             | e::es -> let v = fit acc (e) in
                                        v::(fitl (merge (acc) (moveextent(e,v)) ) es)
    let fitlistl list = fitl [] list
    
    let rec fitr acc l = match l with 
                             | [] ->[]
                             | e::es -> let v = -(fit (e) acc) in
                                        v::(fitr (merge (moveextent(e,v)) (acc) ) es)
    let fitlistr list = List.rev (fitr [] (List.rev list))

    let fitlist es = List.map (fun (x, y) -> (x + y)/2.0) (List.zip (fitlistl es) (fitlistr es))


