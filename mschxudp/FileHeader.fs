module FileHeader

open FSharpPlus
open System
open Utils


let private protocol = "mschxudp"B

let private unknown_bytes: byte array =
    map byte [| 0x02; 0x00; 0x60; 0x00; 0x01; 0x00; 0x00; 0x00; 0x40; 0x00; 0x00; 0x00 |]

assert (protocol.Length + unknown_bytes.Length = 20)

[<Struct>]
type file_header =
    { MagicNumber: uint32
      FileSize: uint32
      KeySize: uint32 }

let private unstable: byte array = Array.replicate 2 0uy
let private fixed_unknown: byte array = map byte [| 0x3f; 0x66 |]
let private padding: byte array = Array.replicate 32 0uy

let size: uint32 =
    protocol.length
    + unknown_bytes.length
    + usizeof<file_header>
    + unstable.length
    + fixed_unknown.length
    + padding.length

assert (size = 68u)

let dump (fh: file_header) : byte seq =
    seq {
        protocol
        unknown_bytes
        BitConverter.GetBytes fh.MagicNumber
        BitConverter.GetBytes fh.FileSize
        BitConverter.GetBytes fh.KeySize
        unstable
        fixed_unknown
        padding
    }
    |> Seq.concat
