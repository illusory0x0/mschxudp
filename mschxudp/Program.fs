open FSharpPlus
open Utils
open FileHeader
open MapHeader
open MapContent
open System.Linq
open type System.IO.File

let map_offsets (table: map_content seq) : uint32 seq =
    let toOffset (mc: map_content) : uint32 =
        MapHeader.size
        + (sizeof<char> * ((Abbr mc |> length) + 1) |> to_uint32)
        + (sizeof<char> * ((Phrase mc |> length) + 1) |> to_uint32)

    map toOffset table


let dump (table: map_content seq) : byte array =
    let offsets = map_offsets table

    let file_size =
        FileHeader.size
        + (sizeof<uint32> * (offsets.Count() - 1) |> to_uint32)
        + (sum offsets)

    let fileHeader =
        { MagicNumber = 0x40u + 4u * to_uint32 (table.Count())
          FileSize = file_size
          KeySize = to_uint32 (table.Count()) }

    let offsets_buf: byte seq =
        (skip 1 offsets).SkipLast 1
        |> scan (+) (offsets.First())
        |> map (fun (v: uint32) -> System.BitConverter.GetBytes v)
        |> Seq.concat

    let dumpMap (m: map_content) : byte seq =
        let start = 0x10us

        let header_end =
            0x10us + (to_uint16 sizeof<char>) * ((Abbr m).Length + 1 |> to_uint16)

        let header = map_header (start, header_end, 1uy)

        seq {
            MapHeader.dump header
            MapContent.dump m
        }
        |> Seq.concat

    seq {
        FileHeader.dump fileHeader
        offsets_buf
        map dumpMap table |> Seq.concat
    }
    |> Seq.concat
    |> Array.ofSeq




let table: map_content array = [| ("a", "a"); ("a", "b"); ("a", "c"); ("a", "d") |]

[<EntryPoint>]
let main (args: string array) : int =
    let buf = dump table
    WriteAllBytes(@"C:\Users\11378\Desktop\test_fs.dat", buf)
    0
