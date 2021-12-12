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

    member this.Target = 
        this.GetNode<Ship>(new NodePath(this.TargetPath)) 

    member this.HealthBarTexture = 
        this.GetNode<TextureProgress>(new NodePath(this.HealthBarPath)) 

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

    
    member this.UpdateHealthBar (currentHp:float) (maxHp:float) =
            this.HealthBarTexture.MaxValue <- maxHp 
            this.HealthBarTexture.Value <- currentHp

            GD.Print("UpdateHealthBar called with current HP = ", currentHp.ToString())
            let yellowRange = this.HealthBarTexture.MaxValue * yellowPercent
            let redRange = this.HealthBarTexture.MaxValue * redPercent

            this.HealthBarTexture.TextureProgress_ <-
                match currentHp with
                | hp when hp < yellowRange -> barYellow
                | hp when hp < redRange -> barRed
                | _ -> this.DefaultBar

    override this._Ready() =
        this.GlobalRotation <- 0f
        this.UpdateHealthBar this.Target.CurrentHp this.Target.MaxHp 
        this.Target.HpProvider.OnDamage <-
            fun (amt) ->
                let max = this.Target.HpProvider.MaxHp
                let current = this.Target.HpProvider.CurrentHp
                this.UpdateHealthBar current max

    override this._Process(delta) = this.GlobalRotation <- 0f
