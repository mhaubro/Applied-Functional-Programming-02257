module HeapArrayLoader

open System.Net
open System.Threading
open System.Text.RegularExpressions




/// Return Array of ints, reprecenting heaps, with numHeaps elements between minSize and maxSize
let loadRandom (numHeaps:int) (minSize:int) (maxSize:int) =
    let random = new System.Random()
    Array.init numHeaps (fun i -> random.Next(minSize, maxSize))

let loadStandardRandom() =
    let random = new System.Random()
    loadRandom (random.Next (4, 12)) 3 10

/// Returns a Array of int downloaded from the given uri
let downloadString (uri:System.Uri) = async {
    let webCl = new WebClient()
    return! webCl.AsyncDownloadString(uri)} 

/// Takes all numbers sepperated from text and puts them into a array parsed as ints
let parseHeapString (s:string) = async {
    return Array.collect (fun s -> [|s |> int|]) (Array.filter (fun s -> Regex.IsMatch(s,"^[1-9][0-9]*$")) ((s.Trim()).Split [|' ';'\n';'\t';'\r'|]))}

/// Loads a string from the uri using downloadString and then parses it using parseHeapString
let loadFromSite (uri:System.Uri) = async{
    let! netString = downloadString uri
    let! array = parseHeapString netString
    return array
}

let rec getMatchString n = 
    match n with
        | n when n < 37 -> String.init n (fun _ -> "|")
        | n -> (getMatchString 34) + "+" + (n-34).ToString()

//let uri = System.Uri("http://www2.compute.dtu.dk/~mire/02257/nim1.game");;
//let ts = new CancellationTokenSource()
//let s = "3 3 2 532 64 36 43 4 25 4 42 9\n"
//let array1 = Async.RunSynchronously(parseHeapString s)
//let s2 = Async.RunSynchronously(downloadString uri)
//let array2 = Async.RunSynchronously(parseHeapString s2)
//let array3 = Async.RunSynchronously(loadFromSite uri);;



// a bit of test
//let s = "7 1 4 1 42 12 24 3f f4 3f4 gds g";;
//let predicate = (fun x -> Regex.IsMatch(x,"^[0-9]+$"));;

//let array = Array.filter predicate (s.Split [|' '|]);;
   




