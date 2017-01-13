module Extents
    type Extent = (float * float) list

    let moveextent = function (e:Extent, x) -> List.map (fun (a, b) -> (a + x,b + x)) e

    let rec merge a b = match a, b with
                             | [], qs->qs
                             | ps, []->ps
                             | (p,_)::ps, (_,q)::qs->(p,q):: merge ps qs
    let mergelist list =  list |> List.fold (merge) []

    let rmax a b =if a>b then a else b

    let rec fit a b = match a, b with
                             | (p,_)::ps, (_,q)::qs -> rmax (fit ps qs) (p-q+1.0)
                             | _, _ -> 0.0

    let rec fitll acc l = match acc,l with 
                             | _, [] ->[]
                             |acc', e::es -> let v = fit acc [e] in
                                                 v::(fitll (merge acc [moveextent(e,v)] ) es)
    let fitlistl list = fitll [] list
    
    let rec fitlr acc l = match acc,l with 
                             | _, [] ->[]
                             |acc', e::es -> let v = fit [e] acc in
                                                 v::(fitlr (merge acc [moveextent(e,v)] ) es)
    let fitlistr list = fitlr [] list

    let fitllist es = List.map (fun (x, y) -> (x + y)/2.0) (List.zip (fitlistl es) (fitlistr es))


