module MapContent

open FSharpPlus
open type System.Text.Encoding

type map_content = string * string

let Abbr (m: map_content) = fst m
let Phrase (m: map_content) = snd m
let null_terminator: string = "\u0000"

let dump (mc: map_content) : byte seq =
    let getBytes (s: string) : byte array = Unicode.GetBytes s

    seq {
        Abbr mc
        null_terminator
        Phrase mc
        null_terminator
    }
    |> map getBytes
    |> Seq.concat
