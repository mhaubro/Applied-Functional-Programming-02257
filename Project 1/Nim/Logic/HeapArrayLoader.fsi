module HeapArrayLoader

open System.Net
open System.Threading

val loadRandom : int -> int -> int -> Async<int[]>
val loadFromSite : System.Uri -> CancellationTokenSource -> Async<int[]>
