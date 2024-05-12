module Pair

open FSharpPlus
open System.Collections.Generic
open System.Text

type pair = KeyValuePair<string,string>


let Cons (k:string) (v:string) = KeyValuePair(k,v)

let private null_terminator: string = "\u0000"

let dump (p: pair) : byte seq =
    let getBytes (s: string) : byte array = Encoding.Unicode.GetBytes s

    seq {
        p.Key
        null_terminator
        p.Value
        null_terminator
    }
    |> map getBytes
    |> Seq.concat
