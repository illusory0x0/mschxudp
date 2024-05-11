module MapHeader

open FSharpPlus
open type System.BitConverter
open Utils

let fixed_unknown: byte array = map byte [| 0x10; 0x00 |]

[<Struct>]
type map_header =
    val AbbrStartOffset: uint16
    val AbbrEndOffset: uint16
    val Position: uint8
    val Anchor: byte

    new(s: uint16, e: uint16, p: uint8) =
        { AbbrStartOffset = s
          AbbrEndOffset = e
          Position = p
          Anchor = 0x06uy }


let unknown: byte array = Array.replicate 4 0uy
let unstable: byte array = Array.replicate 2 0uy
let anchor2: byte array = map byte [| 0xd2; 0x2d |]

let size: uint32 =
    fixed_unknown.Length
    + sizeof<map_header>
    + unknown.Length
    + unstable.Length
    + anchor2.Length
    |> to_uint32

assert (size = 16u)

let dump (mh: map_header) : byte seq =
    seq {
        fixed_unknown
        GetBytes mh.AbbrStartOffset
        GetBytes mh.AbbrEndOffset
        [|mh.Position|]
        [|mh.Anchor|]
        unknown
        unstable
        anchor2
    }
    |> Seq.concat
