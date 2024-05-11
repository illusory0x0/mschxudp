module Utils

open FSharpPlus

let byteToHex (bytes: byte array) =
    bytes
    |> map (fun (x: byte) -> System.String.Format("0x{0:x2} ", x))
    |> String.concat ""

let to_uint32 (v: int) = System.Convert.ToUInt32 v
let to_uint16 (v: int) = System.Convert.ToUInt16 v
let to_int32 (v: uint32) = System.Convert.ToInt32 v
