namespace Ello

open Godot

type PlayerCameraFs() =
    inherit Camera2D()

    [<Export>]
    member val Text = "Hello World!" with get, set

    override this._Ready() =
        GD.Print(this.Text)
