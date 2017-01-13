module TreeeGenerator

    open Trees
    open System

    /// Creates a tree with a fixed number of children.
    let rec generatePlainTree depth numChilds = if (depth < 0) then failwith "Wrong input: Can't make tree with negative depth."
                                                if (numChilds < 0) then failwith "Wrong input: Can't make tree with negative number of children."
                                                match (depth, numChilds) with
                                                | (n,m) when n > 0 -> Node("node", (List.init m (fun _ -> generatePlainTree (n-1) m)))
                                                | (n,m) when n = 0 -> Node("leaf", [])
                                                | _ -> failwith "Unexpected error."


    /// Hidden function that takes a System.Random object, to avoid creating a new one each time.
    let rec private generateRandomTree' depth minC maxC (random:System.Random) = if (depth < 0) then failwith "Wrong input: Can't make tree with negative depth."
                                                                                 if (maxC < 0) then failwith "Wrong input: max is negative."
                                                                                 if (minC < 0) then failwith "Wrong input: min is negative."
                                                                                 match (depth, maxC, minC) with
                                                                                 | (n,max,min) when n > 0 -> Node("node", (List.init (random.Next(min,max)) (fun _ -> generateRandomTree' (n-1) min max random)))
                                                                                 | (n,max,min) when n = 0 -> Node("leaf", [])
                                                                                 | _ -> failwith "Unexpected error."

    /// Creates a tree with each node having between min and max number of children.
    let generateRandomTree depth minC maxC = generateRandomTree' depth minC maxC (new Random())