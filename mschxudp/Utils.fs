module Utils

open FSharpPlus
open System.Linq 

let byteToHex (bytes: byte array) =
    bytes
    |> map (fun (x: byte) -> System.String.Format("0x{0:x2} ", x))
    |> String.concat ""

type System.String with 
    member this.length with get() = System.Convert.ToUInt32 this.Length

type System.Array with
    member this.length with get() = System.Convert.ToUInt32 this.Length

let usizeof<'T> : uint32 =
    System.Convert.ToUInt32 sizeof<'T>

type System.Collections.Generic.IEnumerable<'T> with
    member this.length with get() = System.Convert.ToUInt32 (this.Count())