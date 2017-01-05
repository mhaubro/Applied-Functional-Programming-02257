module HeapArrayLoader

open System.Net
open System.Threading

/// Return Array of ints, reprecenting heaps, with numHeaps elements between minSize and maxSize
val loadRandom : int -> int -> int -> int[]

///Return Array of ints, random length, random intervals - still within reason
val loadStandardRandom : unit -> int[]

/// Takes all numbers sepperated from text and puts them into a array parsed as ints
val parseHeapString : string -> Async<int[]>

/// Loads a string from the uri
val loadFromSite : System.Uri -> Async<int[]>

