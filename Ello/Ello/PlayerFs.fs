namespace Ello

open Godot
open Ello.GDUtils

type PlayerFs() =
    inherit Ship()

    [<Export>]
    member val Acceleration = 10f with get, set

    [<Export>]
    member val Speed = 100f with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    member this.GetInputMovement() : Vector2 =
        let mutable velocity = Vector2()

        if Input.IsActionPressed(InputAction.Right) then
            velocity.x <- velocity.x + 1f

        if Input.IsActionPressed(InputAction.Left) then
            velocity.x <- velocity.x - 1f

        if Input.IsActionPressed(InputAction.Up) then
            velocity.y <- velocity.y - 1f

        if Input.IsActionPressed(InputAction.Down) then
            velocity.y <- velocity.y + 1f

        velocity.Normalized() * this.Speed

    member this.ShootCheck() : bool =
        Input.IsActionJustPressed(InputAction.Shoot)

    override this._Ready() =
        let onDeath =
            fun () ->
                GD.Print("OnDeath called for player")
                this.ReloadScene() |> ignore

        this.ShootDirection <- Up
        this.HpProvider.OnDeath <- onDeath

    override this._PhysicsProcess(delta) =
        if this.ShootCheck() then this.Shoot()
        |> this.GetInputMovement
        |> this.MoveAndSlide
        |> ignore
