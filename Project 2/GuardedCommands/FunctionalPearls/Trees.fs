module Trees
    type 'a tree = Node of 'a * 'a tree list

    let movetree = function
                   | (Node((label, x), subtrees), x') -> Node((label, x+x'), subtrees)
    
    