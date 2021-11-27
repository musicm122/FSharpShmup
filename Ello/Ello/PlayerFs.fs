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
    member val MaxHp: int = 100 with get, set

    [<Export>]
    member val CurrentHp: int = 100 with get, set

    [<Export(PropertyHint.File, "*.tscn")>]
    member val BulletPath: string = Constants.BulletPath with get, set

    [<Export>]
    member val CooldownTime: float32 = 2f with get, set

    member this.OnBulletCollision(body: Node, attackPower) =
        GD.Print("Hit " + body.Name)

        match body.Name with
        | "Player" -> this.TakeDamage(attackPower)
        | "Enemy" ->
            let e = body :?> EnemyFs
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

        let muzzle =
            this.GetNode<Position2D>(new NodePath("Muzzle"))

        bulletInstance.SetAsToplevel(true)
        bulletInstance.GlobalPosition <- muzzle.GlobalPosition
        bulletInstance.Velocity <- Vector2.Up

    member this.GetHealth() = this.CurrentHp

    member this.TakeDamage amt =
        this.CurrentHp <- Mathf.Clamp((this.CurrentHp + amt), 0, this.MaxHp)

    member this.HealDamage amt =
        this.CurrentHp <- Mathf.Clamp((this.CurrentHp + amt), 0, this.MaxHp)

    member this.DeathCheck() = if this.CurrentHp <= 0 then this.Die()

    member this.Die() =
        GD.Print("You Died")
        base.ReloadScene() |> ignore

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

    override this._PhysicsProcess(delta) =
        if this.ShootCheck() then this.Shoot()
        this.DeathCheck()

        ()
        |> this.GetInputMovement
        |> this.MoveAndSlide
        |> ignore
