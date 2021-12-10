namespace Ello

open Godot
open GDUtils
open System

type HealthDisplayFs() =
    inherit Node2D()

    let yellowPercent = 0.7
    let redPercent = 0.4

    let barGreen =
        ResourceLoader.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_green.png") :?> Texture

    let barYellow =
        ResourceLoader.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_yellow.png") :?> Texture

    let barRed =
        ResourceLoader.Load("res://Art/Space Shooter Pack/PNG/UI/barHorizontal_red.png") :?> Texture

    [<Export>]
    member val HealthBarPath = "CanvasLayer/HBoxContainer/Health_Bar" with get, set

    [<Export>]
    member val TargetPath = "../../../Player" with get, set

    member this.TryGetTarget() : Option<Ship> =
        match this.GetNodeOrNull(new NodePath(this.TargetPath)) with
        | null -> None
        | node -> node :?> Ship |> Some

    member this.TryGetHealthBar() : Option<TextureProgress> =
        match this.GetNodeOrNull(new NodePath(this.HealthBarPath)) with
        | null -> None
        | node -> node :?> TextureProgress |> Some

    [<Export>]
    member val DefaultBar: Texture = barGreen with get, set

    member this.UpdateHealthBar currentHp maxHp =
        GD.PrintT("UpdateHealthBar called with ", currentHp, maxHp)

        match this.TryGetHealthBar() with
        | None ->
            GD.PushWarning("GetHealthBar returned null ")
            GD.PrintErr("Cant find a suitable 'TextureProgress' for the Health Display")
            this.PrintTree()
        | hbOption ->
            let healthBar = hbOption.Value
            healthBar.MaxValue <- maxHp 
            GD.Print("UpdateHealthBar called with current HP = ", currentHp.ToString())
            let yellowRange = healthBar.MaxValue * yellowPercent
            let redRange = healthBar.MaxValue * redPercent

            healthBar.TextureProgress_ <-
                match currentHp with
                | hp when hp < yellowRange -> barYellow
                | hp when hp < redRange -> barRed
                | _ -> this.DefaultBar

            healthBar.Value <- currentHp

    override this._Ready() =
        this.GlobalRotation <- 0f

        match this.TryGetTarget() with
        | None ->
            GD.PrintErr("Cant find a suitable 'Target' of type Ship for the Health Display")
            this.PrintTree()
        | target ->
            target.Value.HpProvider.OnDamage <-
                fun () ->
                    let max = (float) target.Value.HpProvider.MaxHp
                    let current =
                        (float) target.Value.HpProvider.CurrentHp

                    this.UpdateHealthBar current max

    override this._Process(delta) = this.GlobalRotation <- 0f
