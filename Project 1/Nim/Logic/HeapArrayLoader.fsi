module HeapArrayLoader

open System.Net
open System.Threading

val loadRandom : int -> int -> int -> int[]
val loadFromSite : System.Uri -> Async<int[]>
val loadStandardRandom : unit -> int[]
val parseHeapString : string -> Async<int[]>
