namespace Ello

open Godot

type PlayerFs() =
    inherit KinematicBody2D()

    [<Export>]
    member val RateOfFire = 1f with get, set

    [<Export>]
    member val Speed = 100f with get, set

    member this.GetInputMovemet(currentVelocity: Vector2) : Vector2 =
        let mutable x = 0f
        let mutable y = 0f

        if Input.IsActionPressed("right") then
            x <- currentVelocity.x + this.Speed
        elif Input.IsActionPressed("left") then
            x <- currentVelocity.x - this.Speed

        if Input.IsActionPressed("up") then
            y <- currentVelocity.y - this.Speed
        elif Input.IsActionPressed("down") then
            y <- currentVelocity.y + this.Speed

        new Vector2(x, y)

    member this.ShootCheck() : bool = Input.IsActionPressed("shoot")

    override this._PhysicsProcess(delta) =
        let mutable currentVelocity = Vector2.Zero
        currentVelocity <- this.GetInputMovemet(currentVelocity)
        this.MoveAndSlide(currentVelocity) |> ignore
