//#I "X:/Projects/Godot/FSharpShmup/.mono/assemblies/Debug/GodotSharp.dll"
//#I "X:/Projects/Godot/FSharpShmup/.mono/assemblies/Debug/GodotSharpEditor.dll"
//#I "X:/Projects/Godot/FSharpShmup/Libs/"
#r @"X:\Projects\Godot\FSharpShmup\.mono\assemblies\Debug\GodotSharp.dll"
open Godot

#load "GdUtils.fs"
#load "Interfaces.fs"
#load "AudioManagerFs.fs"
// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.

open Ello
open Ello.Singleton

// Define your library scripting code here

let am = AudioManagerFs()

//AudioManagerFs.Instance.Play(DefaultSoundPaths.PlayerShoot)