module Trees

    open Extents
    open System

    type 'a tree = Node of 'a * 'a tree list

    let movetree = function (Node((label, x), subtrees), x') -> Node((label, x+x'), subtrees)
    
    let rec design' = function Node(label, subtrees) -> let (trees, extents) = (List.unzip (List.map design' subtrees))
                                                        let positions = fitlist extents
                                                        let ptrees = List.map movetree (List.zip trees positions)
                                                        let pextents = List.map moveextent (List.zip extents positions)
                                                        let halfRootWidth = (float)(System.Math.Max (label.ToString().Length, 5))/ 2.0 / 20.0;
                                                        let resultextent = (-halfRootWidth, halfRootWidth) :: mergelist pextents
                                                        let resulttree = Node((label, 0.0), ptrees)                                                        
                                                        (resulttree, resultextent)
    
    //Translated from the SML code in the paper                                                    
    let design tree = fst (design' tree)

    let rec intDesign (w:int) = function Node((label, x:float), subtrees)
                                        -> Node((label,(int) (System.Math.Round (x * ((float) w)))), List.map (intDesign w) subtrees)

              
    