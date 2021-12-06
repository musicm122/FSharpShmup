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

    member val FacingDirection = Vector2.Right with get, set

    member this.GetInputMovement() : Vector2 = GDUtils.getInputMovement (this.Speed)

    member this.UpdateFacingDirection(movementDelta) =
        if this.PlayerSprite <> null then

            let angle =
                this.PlayerSprite.Position.AngleToPoint(movementDelta)
            this.PlayerSprite.Rotation <- angle
            this.GetMuzzle().Rotate(angle)

    member this.UpdateFacingDirectionAlt (movementDelta: Vector2):unit =
        let aimAngle = Mathf.Atan2(movementDelta.y,movementDelta.x)
        this.Rotation <- aimAngle
        this.FacingDirection<-movementDelta

    member this.ShootCheck() : bool =
        Input.IsActionJustPressed(InputAction.Shoot)

    member this.ShootInFacingDirection() =
        let muzzlePos =
            this.GetNode<Position2D>(new NodePath(this.MuzzlePath))
        let bulletInstance = this.InstantiateBullet this.BulletPath
        this.AddChild(bulletInstance)
        bulletInstance.InitBullet muzzlePos.GlobalPosition this.FacingDirection
        bulletInstance.SetAsToplevel(true)

    override this._Ready() =
        this.FacingDirection<-Vector2.Up
        this.MuzzlePath <- "CollisionShape2D/PlayerSprite/Muzzle"
        this.PlayerSprite <- this.GetNode<Sprite>("CollisionShape2D/PlayerSprite").Value
        if this.PlayerSprite = null then GD.Print("PlayerSprite is null")
        let onDeath =
            fun () ->
                GD.Print("OnDeath called for player")
                this.ReloadScene() |> ignore

        this.HpProvider.OnDeath <- onDeath

    override this._PhysicsProcess(delta) =
        let movement = this.GetInputMovement()
        if this.ShootCheck() then
            this.ShootInFacingDirection()
        match movement with
        | v when movement.Length() > 0f ->
            movement |> this.UpdateFacingDirectionAlt
            movement |> this.MoveAndSlide |> ignore
        |_-> ignore()