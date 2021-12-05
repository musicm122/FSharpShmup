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

    member val PlayerSprite: Sprite = new Sprite() with get, set

    member this.GetInputMovement() : Vector2 = GDUtils.getInputMovement (this.Speed)

    member this.UpdateFacingDirection(movementDelta) =
        if this.PlayerSprite <> null then

            let angle =
                this.PlayerSprite.Position.AngleToPoint(movementDelta)

            GD.Print("this.PlayerSprite.Position.Angle = ", this.PlayerSprite.Position.Angle)

            this.PlayerSprite.Rotation <- angle
            this.GetMuzzle().Rotate(angle)
            //this.Muz
            //let angle =this.Position.AngleToPoint(movementDelta)
            GD.Print("UpdateFacingDirection angle = ", angle)

    //this.Rotate(angle)

    member this.ShootCheck() : bool =
        Input.IsActionJustPressed(InputAction.Shoot)

    member this.ShootInDirection(direction) =
        //let line = new Line2D()
        //todo traw debug line  
        let bulletInstance = this.InstantiateBullet this.BulletPath
        this.AddChild(bulletInstance)

        let muzzle =
            this.GetNode<Position2D>(new NodePath(this.MuzzlePath))

        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Direction <- this.PlayerSprite.Position

    override this._Ready() =
        this.MuzzlePath <- "CollisionShape2D/PlayerSprite/Muzzle"

        this.PlayerSprite <-
            this
                .GetNode<Sprite>(
                    "CollisionShape2D/PlayerSprite"
                )
                .Value

        this.PlayerSprite.Rotation <- this.PlayerSprite.Rotation + 270f
        GD.Print("this.PlayerSprite.Rotation = ", this.PlayerSprite.Rotation)

        if this.PlayerSprite = null then
            GD.Print("PlayerSprite is null")

        let onDeath =
            fun () ->
                GD.Print("OnDeath called for player")
                this.ReloadScene() |> ignore

        this.ShootDirection <- Up
        this.HpProvider.OnDeath <- onDeath

    override this._PhysicsProcess(delta) =
        if this.ShootCheck() then
            this.ShootInDirection(this.PlayerSprite.Position)

        let movement = this.GetInputMovement()
        movement |> this.UpdateFacingDirection
        movement |> this.MoveAndSlide |> ignore
