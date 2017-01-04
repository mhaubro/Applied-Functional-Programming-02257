module HeapArrayLoader

open System.Net
open System.Threading
open System.Text.RegularExpressions

///<param name="numHeaps">the number of heaps in the array</param>
///<param name="minSize">the minimum number of matches in the heaps</param>
///<param name="maxSize">the maximum number of matches in the heaps</param>
///<returns>Array of ints, reprecenting heaps, with <param name="numHeaps"/> elements between <param name="minSize"/> and <param name="maxSize"/> </returns>
let loadRandom (numHeaps:int) (minSize:int) (maxSize:int) = async{
    let random = new System.Random()
    return Array.init numHeaps (fun i -> random.Next(minSize, maxSize))}

let downloadString (uri:System.Uri) = async {
    let webCl = new WebClient()
    let! html = webCl.AsyncDownloadString(uri)
    return html} 

let parseHeapString (s:string) =
    Array.collect (fun s -> [|s |> int|]) (Array.filter (fun s -> Regex.IsMatch(s,"^[0-9]+$")) (s.Split [|' '|]))

let loadFromSite (uri:System.Uri) (ts:CancellationTokenSource) = async{
    let! netString = downloadString uri
    let array = parseHeapString netString
    return array
}

//let uri = System.Uri("http://www2.compute.dtu.dk/~mire/02257/nim1.game");;
//let ts = new CancellationTokenSource();;

//let array = Async.RunSynchronously(loadFromSite uri ts);;


// a bit of test
//let s = "7 1 4 1 42 12 24 3f f4 3f4 gds g";;
//let predicate = (fun x -> Regex.IsMatch(x,"^[0-9]+$"));;

//let array = Array.filter predicate (s.Split [|' '|]);;
   




