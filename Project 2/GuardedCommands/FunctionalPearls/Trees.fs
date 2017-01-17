module Trees

    open Extents
    open System

    /// A tree with an arbitrary number of children per node
    type 'a tree =
        /// Represents a node in a tree with an arbitrary number of children per node
        Node of 'a * 'a tree list


    /// Move the root node - we do not need to do anything else because we use relative positions
    let movetree = function (Node((label, x), subtrees), x') -> Node((label, x+x'), subtrees)
    

    // Translated from the SML code in the paper and adapted to allow for variation in label length
    let rec designAugmented' labelLength = function Node(label, subtrees) -> 
                                                    let (trees, extents) = (List.unzip (List.map (designAugmented' labelLength) subtrees)) // Preorder/depth first because we need to know the extends of childre before we continue
                                                    let positions = fitlist extents                                 // calculate relative horizontal shift needed for each child subtree to ensure no overlap between child subtrees
                                                    let ptrees = List.map movetree (List.zip trees positions)       // apply each relative shift the root of each child subtree
                                                    let pextents = List.map moveextent (List.zip extents positions) // and move the corresponding extent similarly
                                                    let halfWit = (labelLength label)/2.0                           // calculates the width of this nodes label in points
                                                    let resultextent = (-halfWit, halfWit) :: mergelist pextents    // find total extend of the tree that has current node as root
                                                    let resulttree = Node((label, 0.0), ptrees)                     // ensure that all children are centered around this tree                                     
                                                    (resulttree, resultextent)
    
    ///Magical function for ensuring that node labels do not overlap in an AST printed with Consolas font, size 10
    let ASTaugmentation = function label ->
                                   (float)(System.Math.Max (label.ToString().Length, 3)) / 7.0
    
    ///reformated to work with augmented design'
    let design tree = fst (designAugmented' (fun _ -> 0.0) tree)

    ///Assign to every node in the supplied tree a float representing 
    /// a desirable horizontal position relative to its parent (if any)
    let designAST tree = fst (designAugmented' ASTaugmentation tree)

    ///Converts a tree with floats assigned to every node to a tree with integers assigned to every node
    let rec intDesign (w:int) = function Node((label, x:float), subtrees) ->
                                         Node((label,(int) (System.Math.Round (x * ((float) w)))), 
                                            List.map (intDesign w) subtrees)

              
    