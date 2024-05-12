open FSharpPlus
open Utils
open FileHeader
open PairHeader
open Pair

open Newtonsoft.Json

open System
open System.Linq
open System.IO

let map_offsets (table: pair seq) : uint32 seq =
    let toOffset (p: pair) : uint32 =
        PairHeader.size
        + usizeof<char> * (p.Key.length + 1u)
        + usizeof<char> * (p.Value.length + 1u)

    map toOffset table


let dump (table: pair seq) : byte array =
    let offsets = map_offsets table

    let file_size =
        FileHeader.size
        + usizeof<uint32> * (offsets.length - 1u) 
        + (sum offsets)

    let fileHeader =
        { MagicNumber = 0x40u + 4u * table.length
          FileSize = file_size
          KeySize = table.length }

    let offsets_buf: byte seq =
        (skip 1 offsets).SkipLast 1
        |> scan (+) (offsets.First())
        |> map (fun (v: uint32) -> System.BitConverter.GetBytes v)
        |> Seq.concat

    let dumpMap (p: pair) : byte seq =
        let start = 0x10us

        let header_end =
            (0x10u + usizeof<char> * (p.Key.length + 1u)) |> System.Convert.ToUInt16

        let header = pair_header (start, header_end, 3uy)

        seq {
            PairHeader.dump header
            Pair.dump p
        }
        |> Seq.concat

    seq {
        FileHeader.dump fileHeader
        offsets_buf
        map dumpMap table |> Seq.concat
    }
    |> Seq.concat
    |> Array.ofSeq



[<EntryPoint>]
let main (args: string array) : int =

    let src = File.ReadAllText args[0]

    let result = JsonConvert.DeserializeObject<pair array>(src)

    let rm_semicolon (s: string) : string = s |> filter (fun c -> c <> ':')
    let legal (ch: char) = Char.IsAsciiLetterLower ch || ch = '-'

    let result =
        result
        |> map (fun p -> Cons (p.Key |> rm_semicolon |> map Char.ToLower) p.Value)
        |> filter (fun p -> forall legal p.Key)


    printfn $"table size: {result.Length}"
    File.WriteAllBytes(args[1], (dump result))
    0
