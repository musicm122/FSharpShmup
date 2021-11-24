namespace Ello

open Godot
open Ello.GDUtils

type PlayerFs() =
    inherit KinematicBody2D()

    [<Export>]
    member val RateOfFire = 1f with get, set

    [<Export>]
    member val Speed = 100f with get, set

    [<Export>]
    member val Hp: int = 100 with get, set

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = Constants.BulletPath with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    member this.OnBulletCollision(body: PhysicsBody2D, attackPower) =
        GD.Print("Hit "+body.Name)
        match body.Name with
        | "Player" -> this.TakeDamage(attackPower)
        | "Enemy" ->
            let e = body:?> EnemyFs
            e.TakeDamage(attackPower)
        | _ -> GD.Print("Hit something else")

    member this.InstantiateBullet scenePath =
        let bulletType = GD.Load<PackedScene>(scenePath)
        let instance = bulletType.Instance() :?> BulletFs
        instance.OnCollisionFunc <- this.OnBulletCollision
        instance

    member this.Shoot() =
        let bulletInstance = this.InstantiateBullet this.BulletPath
        this.AddChild(bulletInstance)
        let muzzle = this.GetNode<Position2D>(new NodePath("Muzzle"))
        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Velocity <- Vector2.Up

    member this.GetHealth() = this.Hp

    member this.TakeDamage amt = this.Hp <- this.Hp - amt

    member this.HealDamage amt = this.Hp <- this.Hp + amt

    member this.Die() = GD.Print("You Died")

    member this.GetInputMovemet(currentVelocity: Vector2) : Vector2 =
        let mutable x = 0f
        let mutable y = 0f

        if Input.IsActionPressed(InputAction.Right) then
            x <- currentVelocity.x + this.Speed
        elif Input.IsActionPressed(InputAction.Left) then
            x <- currentVelocity.x - this.Speed

        if Input.IsActionPressed(InputAction.Up) then
            y <- currentVelocity.y - this.Speed
        elif Input.IsActionPressed(InputAction.Left) then
            y <- currentVelocity.y + this.Speed

        new Vector2(x, y)

    member this.ShootCheck() : bool = Input.IsActionJustPressed(InputAction.Shoot)

    override this._PhysicsProcess(delta) =
        let mutable currentVelocity = Vector2.Zero
        currentVelocity <- this.GetInputMovemet(currentVelocity)
        this.MoveAndSlide(currentVelocity) |> ignore
        if this.ShootCheck() then this.Shoot()