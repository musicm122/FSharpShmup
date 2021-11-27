namespace Ello

open Godot

type HealthDisplayFs() =
    inherit Node()

    let barGreen =
        GD.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_green.png")

    let barYellow =
        GD.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_yellow.png")

    let barRed =
        GD.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_red.png")

    let healthBar =
        base.GetNode<TextureProgress>(new NodePath("HealthBar"))


    [<Export>]
    member val Text = "Hello World!" with get, set

    override this._Ready() =
        healthBar.Hide()
        let parent = base.GetParent()

        match (parent, parent.Get("MaxHp")) with
        | (_, null)
        | (null, _) -> ignore ()
        | _ -> healthBar.MaxValue <- base.GetParent().Get("MaxHp") :?> float
