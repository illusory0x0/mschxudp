module PairHeader

open FSharpPlus
open System
open Utils

let private fixed_unknown: byte array = map byte [| 0x10; 0x00 |]

[<Struct>]
type pair_header =
    val AbbrStartOffset: uint16
    val AbbrEndOffset: uint16
    val Position: uint8
    val Anchor: byte

    new(s: uint16, e: uint16, p: uint8) =
        { AbbrStartOffset = s
          AbbrEndOffset = e
          Position = p
          Anchor = 0x06uy }


let private unknown: byte array = Array.replicate 4 0uy
let private unstable: byte array = Array.replicate 2 0uy
let private anchor2: byte array = map byte [| 0xd2; 0x2d |]

let size: uint32 =
    fixed_unknown.length
    + usizeof<pair_header>
    + unknown.length
    + unstable.length
    + anchor2.length

assert (size = 16u)

let dump (mh: pair_header) : byte seq =
    seq {
        fixed_unknown
        BitConverter.GetBytes mh.AbbrStartOffset
        BitConverter.GetBytes mh.AbbrEndOffset
        [|mh.Position|]
        [|mh.Anchor|]
        unknown
        unstable
        anchor2
    }
    |> Seq.concat
