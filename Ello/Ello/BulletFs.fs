namespace Ello

open Godot

type BulletFs() as this =
    inherit Area2D()

    let _destroyableData =
        { DestroyableData.Default() with OnDestroy = fun () -> this.QueueFree() }

    let _ammoData =
        { AmmoData.Default() with Destroyable = Destroyable _destroyableData }

    member val Ammo = Ammo(_ammoData)

    member val Velocity = Vector2.Zero with get, set

    member val OnCollisionFunc = ignore with get, set

    [<Export>]
    member this.AttackPower
        with get () = this.Ammo.AttackPower
        and set (value) = this.Ammo.AttackPower <- value

    [<Export>]
    member this.Speed
        with get () = this.Ammo.Speed
        and set (value) = this.Ammo.Speed <- value

    [<Export>]
    member this.TimeToLive
        with get () = this.Ammo.Destroyable.TimeToLive
        and set (value) = this.Ammo.Destroyable.TimeToLive <- value

    member this.OnCollision(body: Node) =
        this.OnCollisionFunc(body, this.AttackPower)
        this.QueueFree()

    member this.InitBullet (pos:Vector2) (dir:Vector2) = 
        this.Position <-pos
        this.Rotation <- dir.Angle()
        this.Velocity<- dir * this.Speed
    
    override this._PhysicsProcess(delta) =
        this.Translate(this.Velocity * this.Speed * delta)
        this.Ammo.Destroyable.AccumulateTime(delta)
