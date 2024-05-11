module FileHeader

open FSharpPlus
open type System.Text.Encoding
open type System.BitConverter
open Utils

let protocol = "mschxudp"

let protocol_bytes = ASCII.GetBytes(protocol)

let unknown_bytes: byte array =
    map byte [| 0x02; 0x00; 0x60; 0x00; 0x01; 0x00; 0x00; 0x00; 0x40; 0x00; 0x00; 0x00 |]

assert (protocol_bytes.Length + unknown_bytes.Length = 20)

[<Struct>]
type file_header =
    { MagicNumber: uint32
      FileSize: uint32
      KeySize: uint32 }

let unstable: byte array = Array.replicate 2 0uy
let fixed_unknown: byte array = map byte [| 0x3f; 0x66 |]
let padding: byte array = Array.replicate 32 0uy

let size: uint32 =
    protocol_bytes.Length
    + unknown_bytes.Length
    + sizeof<file_header>
    + unstable.Length
    + fixed_unknown.Length
    + padding.Length
    |> to_uint32

assert (size = 68u)

let dump (fh: file_header) : byte seq =
    seq {
        protocol_bytes
        unknown_bytes
        GetBytes fh.MagicNumber
        GetBytes fh.FileSize
        GetBytes fh.KeySize
        unstable
        fixed_unknown
        padding
    }
    |> Seq.concat
