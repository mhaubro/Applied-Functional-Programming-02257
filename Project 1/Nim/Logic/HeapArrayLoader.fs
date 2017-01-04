module HeapArrayLoader

///<param name="numHeaps">the number of heaps in the array</param>
///<param name="minSize">the minimum number of matches in the heaps</param>
///<param name="maxSize">the maximum number of matches in the heaps</param>
///<returns>Array of ints, reprecenting heaps, with <param name="numHeaps"/> elements between <param name="minSize"/> and <param name="maxSize"/> </returns>
let loadRandom (numHeaps:int) (minSize:int) (maxSize:int) = 
    let random = new System.Random()
    let array = Array.init numHeaps (fun i -> random.Next(minSize, maxSize))
    array


