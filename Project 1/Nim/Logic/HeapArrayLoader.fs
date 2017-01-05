module HeapArrayLoader

open System.Net
open System.Threading
open System.Text.RegularExpressions

/// Return Array of ints, reprecenting heaps, with numHeaps elements between minSize and maxSize
let loadRandom (numHeaps:int) (minSize:int) (maxSize:int) =
    let random = new System.Random()
    Array.init numHeaps (fun i -> random.Next(minSize, maxSize))

///Return Array of ints, random length, random intervals - still within reason
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




